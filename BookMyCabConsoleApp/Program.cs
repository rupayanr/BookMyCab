using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookMyCab;
using BookMyCab.Models;


namespace BookMyCabConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            BookMyCabRepository repo = new BookMyCabRepository();

            
            
            //Console.WriteLine(rep.GenerateNewTransactionId());
            //Console.WriteLine(rep.CountTransactions("1002"));

            //var flag = repo.InsertEntityInCustomerTable("1004", "Jane Doe", 1000, "Sector 21 Panchkula", 5);
            //Console.WriteLine(flag);

            //var flag1 = repo.InsertEntityInCustomerTable("1005", "John Doe", 500, "Chennai", 6);
            //Console.WriteLine(flag1);

            //Console.WriteLine("-----------Before Update----------");

            //rep.UpdateOfferForCustomer("1002", 10);
            var customerList = repo.GetAllCustomers();
            Console.WriteLine("============== Customers ==============");
            foreach (var customer in customerList)
            {
                
                Console.WriteLine("{0}--{1}--{2}--{3}--{4}--{5}",
                    customer.PartitionKey,
                    customer.RowKey, customer.CustomerName,
                    customer.Address, customer.Balance,
                    customer.Offer);
            }
            Console.WriteLine("======================================");
            Console.WriteLine("\n");

            List<DriverDetail> drivers = repo.GetDriverList();
            Console.WriteLine("============== Drivers ==============");
            foreach (DriverDetail driver in drivers) 
            {
                Console.WriteLine(driver.Name);
            }
            Console.WriteLine("======================================");
            Console.WriteLine("\n");

            var transList = repo.GetAllTransactions();
            Console.WriteLine("============== Transactions ==============");
            foreach (var trans in transList)
            {
                Console.WriteLine("{0}--{1}--{2}--{3}--{4}--{5}",
                    trans.PartitionKey,
                    trans.RowKey, trans.TransactionDate,
                    trans.Type, trans.Amount,
                    trans.DriverId);
            }
            Console.WriteLine("======================================");



            //var result = repo.DeleteAllCustomers();
            // Console.WriteLine(result);
            // var customerList1 = repo.GetAllCustomers();
            // foreach (var customer in customerList1)
            // {
            //     Console.WriteLine("{0}--{1}--{2}--{3}--{4}--{5}",
            //         customer.PartitionKey,
            //         customer.RowKey, customer.customerName,
            //         customer.address, customer.balance,
            //         customer.offer);
            // }

        }

    }
}
