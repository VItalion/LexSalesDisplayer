using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TableDisplayer.Data {
    public class Sale {
        [Required]
        public Guid Id { get; set; }
        public bool IsSale { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string TID { get; set; }
        [Required]
        public string Agent { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public string Vendor_ID { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public Sale() {
            Id = Guid.NewGuid();
        }
    }

    public class LexSale : Sale { }
    public class CreditSale : Sale { }
}
