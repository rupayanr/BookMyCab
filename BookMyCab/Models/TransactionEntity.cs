using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace BookMyCab.Models
{
    [Table("Transaction")]
    public class TransactionEntity : TableEntity
    {
      
        public DateTime TransactionDate { get; set; }
        public string Type { get; set; }
        public double Amount { get; set; }
        public string DriverId { get; set; }


    }
}