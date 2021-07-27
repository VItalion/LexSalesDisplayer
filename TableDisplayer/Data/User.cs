using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TableDisplayer.Data {
    public class User : IdentityUser {
        public bool IsSuspended { get; set; }
        public string Name { get; set; }
        public int LexId { get; set; }
        public int Ext { get; set; }
    }
}
