﻿using HerosProject.Entity;
using Microsoft.EntityFrameworkCore;

namespace HerosProject.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base(options)
        {
            
        }
        public DbSet<User> users { get; set; }

       public DbSet<RefreshToken> refreshtokens { get; set; }
    }
}
