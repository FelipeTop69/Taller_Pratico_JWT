using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace Entity.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options)
        {
        }

        public DbSet<User> User => Set<User>();
        public DbSet<Role> Role => Set<Role>();
        public DbSet<UserRole> UserRole => Set<UserRole>();

    }
}
