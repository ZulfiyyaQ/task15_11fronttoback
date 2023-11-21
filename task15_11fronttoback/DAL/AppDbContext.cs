using Microsoft.EntityFrameworkCore;
using task15_11fronttoback.Models;

namespace task15_11fronttoback.DAL
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext>opt):base(opt)
        {
            
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Slide> Slides { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImg> ProductImages { get; set; }
        public DbSet<ProductTags> ProductTags { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public DbSet<Color> Colors { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }

    }
}
