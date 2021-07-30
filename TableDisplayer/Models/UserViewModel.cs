using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TableDisplayer.Models {
    public class UserViewModel {
        [Display(Name = "User name")]
        public string Login { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Suspended")]
        public bool IsSuspended { get; set; }

        [Display(Name = "Lex id")]
        public int LexId { get; set; }

        [Display(Name = "Ext")]
        public int Ext { get; set; }
    }
}
