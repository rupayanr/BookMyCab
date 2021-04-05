using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookMyCab.Models;
using BookMyCab;

namespace BookMyCab.Controllers
{ 
    public class AdminController : Controller
    {
        public BookMyCabRepository repo = new BookMyCabRepository();

        // GET: Admin/Index
        public string Index() 
        {
            return "This is the bookmycab application";
        }

        // GET: Admin/ViewAllCustomers
        public ViewResult ViewAllCustomers()
        {
            
            List<CustomerEntity> customers = repo.GetAllCustomers();
            return View(customers);
        }

        // GET: Admin/ViewAllDrivers
        public ViewResult ViewAllDrivers()
        {
            
            List<DriverDetail> drivers = repo.GetDriverList();
            return View(drivers);

        }

        // GET: Admin/ViewAllTransactions
        public ViewResult ViewAllTransactions() 
        {
            
            List<TransactionEntity> transList = repo.GetAllTransactions();
            return View(transList);
        }

        
    }
}