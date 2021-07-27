using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace TableDisplayer.Data {
    public class ApplicationDbContext : IdentityDbContext<User> {
        public DbSet<LexSale> LexSales { get; set; }
        public DbSet<CreditSale> CreditSales { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {
            Database.EnsureCreated();
        }
    }
}
