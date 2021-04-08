using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project1.Models
{
    public class JobTestDB : DbContext
    {
        public JobTestDB(DbContextOptions<JobTestDB> options) : base(options)
        {

        }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Category> Categories { get; set; }

        
    }
}
