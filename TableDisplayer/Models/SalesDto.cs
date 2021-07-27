using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TableDisplayer.Models {
    public class SalesDto {
        public string Name { get; set; }
        public string SalesCount { get; set; }
        public string TransactionsCount { get; set; }
        public string Conversion { get; set; }
        public List<SaleViewModel> Rows { get; set; }
    }

    public class SaleViewModel {

        public bool IsSale { get; set; }

        private string date;
        public string Date {
            get => string.IsNullOrEmpty(date) ? "_" : date;
            set => date = value;
        }

        private string tID;
        public string TID { 
            get => string.IsNullOrEmpty(tID) ? "_" : tID; 
            set => tID = value; 
        }

        private string agent;

        public string Agent { 
            get => string.IsNullOrEmpty(agent) ? "_" : agent; 
            set => agent = value; 
        }

        private string status;

        public string Status { 
            get => string.IsNullOrEmpty(status) ? "_" : status; 
            set => status = value; 
        }

        private string comment;
        public string Comment { 
            get => string.IsNullOrEmpty(comment) ? "_" : comment; 
            set => comment = value; 
        }

        private string vendor_ID;
        public string Vendor_ID { 
            get => string.IsNullOrEmpty(vendor_ID) ? "_" : vendor_ID; 
            set => vendor_ID = value; 
        }

        private string first_Name;
        public string First_Name { 
            get => string.IsNullOrEmpty(first_Name) ? "_" : first_Name; 
            set => first_Name = value; 
        }

        private string last_Name;

        public string Last_Name { 
            get => string.IsNullOrEmpty(last_Name) ? "_" : last_Name; 
            set => last_Name = value; 
        }

        private string phone;
        public string Phone { 
            get => string.IsNullOrEmpty(phone) ? "_" : phone; 
            set => phone = value; 
        }

        private string email;
        public string Email { 
            get => string.IsNullOrEmpty(email) ? "_" : email; 
            set => email = value; 
        }

        public int SortId { get; set; }
    }
}
