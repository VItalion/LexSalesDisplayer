using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TableDisplayer.Models {
    public class RegisterViewModel {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Is suspended")]
        public bool IsSuspended { get; set; }

        [Display(Name = "Lex id")]
        public int LexId { get; set; }

        [Display(Name = "Ext")]
        public int Ext { get; set; }

        public string ReturnUrl { get; set; }

    }
}
