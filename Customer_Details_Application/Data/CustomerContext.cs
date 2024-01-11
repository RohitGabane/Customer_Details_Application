using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Customer_Details_Application.Models;

namespace Customer_Details_Application.Data
{
    public class CustomerContext : DbContext
    {
        public CustomerContext (DbContextOptions<CustomerContext> options)
            : base(options)
        {
        }

        public DbSet<Customer_Details_Application.Models.Customer> Customer { get; set; } = default!;
    }
}
