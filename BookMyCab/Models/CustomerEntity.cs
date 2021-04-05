using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BookMyCab.Models
{
    [Table("Customer")]
    public class CustomerEntity : TableEntity
    {
       [Key]
        public string CustomerName { get; set; }
        public double Balance { get; set; }
        public string Address { get; set; }
        public int Offer { get; set; }

    }
}
