using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using BookMyCab.Models;


namespace BookMyCab
{
    public class BookMyCabRepository
    {
        //Properties 
        
        CloudStorageAccount storageAccount;
        CloudTableClient tableClient;
        CloudTable customerTable;
        CloudTable transactionTable;
        //DriverDbContext driverContext;

        //Constructor 
        public BookMyCabRepository()
        {
            try
            {
                storageAccount = new CloudStorageAccount(new StorageCredentials("<name of your storage accnt>", "<creds of your storage account>"), true);
                tableClient = storageAccount.CreateCloudTableClient();
                transactionTable = tableClient.GetTableReference("Transaction");
                customerTable = tableClient.GetTableReference("Customer");
            }
            catch (Exception e)
            {

                throw e;
            }
            //Initialize variables

            //driverContext = new DriverDBContext();
        }

        //To fetch all customers in Customer table
        public List<CustomerEntity> GetAllCustomers()
        {
            //Create a new list of type customer entity 
            List<CustomerEntity> customerList = new List<CustomerEntity>();
            //Create a query object
            var query = new TableQuery<CustomerEntity>();
            //Fetch result and convert into list
            try
            {
                customerList = customerTable.ExecuteQuery(query).ToList();
            }
            catch (Exception e)
            {

                throw e;
            }
            
            return customerList;
        }

        //To insert a new customer in Customer Table  
        public bool InsertEntityInCustomerTable(string customerId, string customerName, double balance, string address, int offer)
        {
            
            bool status;
            CustomerEntity newCustomer = new CustomerEntity();
            //Can you create a parameterized constructor? ExecuteQuery Line 41 is not working
            try
            {
                newCustomer.PartitionKey = "Customer";
                newCustomer.RowKey = customerId;
                newCustomer.CustomerName = customerName;
                newCustomer.Balance = balance;
                newCustomer.Address = address;
                newCustomer.Offer = offer;

                var operation = TableOperation.Insert(newCustomer);
                customerTable.Execute(operation);
                status = true;
                
            }
            catch (Exception e)
            {
                status = false;
                throw e;
            }
            return status;
        }

        //To Update Offer for given Customer
        public void UpdateOfferForCustomer(string customerId, int offerPercentage) 
        {
            //Retrieve the customer entity you want to update
            var retrieveCust = TableOperation.Retrieve<CustomerEntity>("Customer",customerId);
            var targetCust = customerTable.Execute(retrieveCust);
            var updatetargetCust = (CustomerEntity)targetCust.Result;
            updatetargetCust.Offer = offerPercentage;
            var updateOperation = TableOperation.Replace(updatetargetCust);
            customerTable.Execute(updateOperation);
           
        }

        //To Count no of transactions for given customer
        public int CountTransactions(string customerId) 
        {
            List<TransactionEntity> custTransactions = new List<TransactionEntity>();
            var query = new TableQuery<TransactionEntity>();
            custTransactions = (List<TransactionEntity>)transactionTable.ExecuteQuery(query).ToList();

            var numTransactions = (from transaction in custTransactions
                                   where transaction.PartitionKey == customerId
                                    select transaction).ToArray();
            var count = numTransactions.Length;
            return count;
            
        }

        //To create a customer table
        public bool CreateCustomerTable()
        {
            bool status;
            try
            {
                customerTable.CreateIfNotExists();
                status = true;
            }
            catch (Exception)
            {

                status = false;
            }
            return status;
            
        }

        //To create a transaction table
        public bool CreateTransactionTable() 
        {
            bool status;
            try
            {
                transactionTable.CreateIfNotExists();
                status = true;
            }
            catch (Exception)
            {

                status = false;
            }
            return status;
        }

        //To fetch balance of given customer
        public double FetchBalance(string customerId) 
        {
            var retrieveCustOperation = TableOperation.Retrieve<CustomerEntity>("Customer", customerId);
            var targetCustResult = customerTable.Execute(retrieveCustOperation);

            var targetCust = (CustomerEntity)targetCustResult.Result;
            double balance = targetCust.Balance;
            return balance;
        }

        //Generate new transactionId
        public string GenerateNewTransactionId() 
        {
            List<TransactionEntity> transList = new List<TransactionEntity>();

            var query = new TableQuery<TransactionEntity>();
            transList = transactionTable.ExecuteQuery(query).ToList();

            var numTransactions = (from transaction in transList
                                   orderby transaction.RowKey descending
                                   select transaction.RowKey ).ToArray();

            string oldtransId = numTransactions[0];
            string id = "";
            for (int i = 1; i < oldtransId.Length; i++) 
            {
                 id += oldtransId[i];
            }
            int transid = Int32.Parse(id);
            int newtransid = transid + 1;
            string newid = "T" + newtransid.ToString();

            return newid;

        }

        //Fetch a particular customer
        public CustomerEntity GetCustomer(string customerId)
        {
            var operation = TableOperation.Retrieve<CustomerEntity>("Customer", customerId);
            var targetCust = (CustomerEntity)customerTable.Execute(operation).Result;
            return targetCust;
        }

        //To Update wallet of customer after transaction
        public bool UpdateWallet(string customerId, double amount, string type, string driverid = "0")
        {
            if (type == "Credit")
            {
                if (amount >= 100 && amount <= 5000)
                {
                    try
                    {
                        var retrieveop = TableOperation.Retrieve<CustomerEntity>("Customer", customerId);
                        var targetCust = (CustomerEntity)customerTable.Execute(retrieveop).Result;
                        targetCust.Balance = targetCust.Balance - amount;
                        var updateop = TableOperation.Replace(targetCust);
                        customerTable.Execute(updateop);
                        var transId = this.GenerateNewTransactionId();

                        TransactionEntity newTrans = new TransactionEntity();
                        newTrans.PartitionKey = customerId;
                        newTrans.RowKey = transId;
                        newTrans.TransactionDate = DateTime.Now;
                        newTrans.Type = type;
                        newTrans.Amount = amount;
                        newTrans.DriverId = driverid;


                        var insertop = TableOperation.Insert(newTrans);
                        return true;
                    }
                    catch (Exception e)
                    {
                        return false;
                        throw e;
                    }
                    
                }
                return false;
            }
            else if (type == "Debit")
            {
                try
                {
                    var retrieveop = TableOperation.Retrieve<CustomerEntity>("Customer", customerId);
                    var targetCust = (CustomerEntity)customerTable.Execute(retrieveop).Result;
                    if (amount <= targetCust.Balance)
                    {
                        var offerAmount = (targetCust.Offer * amount) / 100;
                        var actualAmount = amount - offerAmount;
                        targetCust.Balance = targetCust.Balance - actualAmount;
                        var updateop = TableOperation.Replace(targetCust);
                        customerTable.Execute(updateop);
                        var tid = this.GenerateNewTransactionId();

                        TransactionEntity newTrans2 = new TransactionEntity();
                        newTrans2.PartitionKey = customerId;
                        newTrans2.RowKey = tid;
                        newTrans2.TransactionDate = DateTime.Now;
                        newTrans2.Type = type;
                        newTrans2.Amount = actualAmount;
                        newTrans2.DriverId = driverid;
                        var insertop = TableOperation.Insert(newTrans2);
                        return true;
                    }
                }
                catch (Exception e)
                {
                    return false;
                    throw e;
                }
                return false;
               
            }
            return false;
        }

        //To Get all transactions 
        public List<TransactionEntity> GetAllTransactions()
        {
            List<TransactionEntity> trans = new List<TransactionEntity>();

            var query = new TableQuery<TransactionEntity>();

            trans = transactionTable.ExecuteQuery(query).ToList();
            return trans;
            
        }
        //public static void GetDrivers() 
        //{
        //    using (var ctx = new DriverDbContext()) 
        //    {
        //        foreach (DriverDetail driver in ctx.DriverDetails) 
        //        {
        //            Console.WriteLine(driver.Name);
        //        }
        //    }
        //}
       

        // To fetch driver list from DriverDetails Table
        public List<DriverDetail> GetDriverList() 
        {
            var ctx = new DriverDbContext();
            List<DriverDetail> drivers = ctx.DriverDetails.ToList();
            return drivers;
        }

        // To Insert dummy values in customer table 

        public bool DeleteAllCustomers() 
        {
            List<CustomerEntity> customers = this.GetAllCustomers();
            try
            {
                foreach (CustomerEntity customer in customers)
                {
                    TableOperation delete = TableOperation.Delete(customer);
                    TableResult result = customerTable.Execute(delete);
                }
                return true;
            }
            catch (StorageException e)
            {
                return false;
                throw e;
            }

        }


     
        



    }





}


