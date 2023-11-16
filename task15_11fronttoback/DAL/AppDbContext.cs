﻿using Microsoft.EntityFrameworkCore;
using task15_11fronttoback.Models;

namespace task15_11fronttoback.DAL
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext>opt):base(opt)
        {
            
        }

        public DbSet<Product> Products { get; set; }
    }
}