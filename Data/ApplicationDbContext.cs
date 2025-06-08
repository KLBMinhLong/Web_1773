using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web_1773.Models;
namespace Web_1773.Data;


public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cấu hình mối quan hệ giữa Order và OrderDetail
        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Order)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(od => od.OrderId);

        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Product)
            .WithMany()
            .HasForeignKey(od => od.ProductId);
        // Cấu hình mối quan hệ giữa Product và Category
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId);  
        // Seed Categories
        var categories = new[]
        {
            new Category { Id = 1, Name = "Điện thoại" },
            new Category { Id = 2, Name = "Laptop"},
            new Category { Id = 3, Name = "Máy tính bảng"},
            new Category { Id = 4, Name = "Bàn phím" },
            new Category { Id = 5, Name = "Tai nghe"}
        };
        modelBuilder.Entity<Category>().HasData(categories);        // Seed Products
        var products = new[]
        {
            // Điện thoại
            new Product { Id = 1, Name = "iPhone 15 Pro", Price = 29990000, Description = "iPhone 15 Pro 256GB", CategoryId = 1, ImageUrl = "/images/iphone15pro.jpg"},
            new Product { Id = 2, Name = "Samsung Galaxy S24", Price = 22990000, Description = "Galaxy S24 256GB", CategoryId = 1, ImageUrl = "/images/galaxy-s24.jpg"},
            new Product { Id = 3, Name = "Xiaomi 14", Price = 15990000, Description = "Xiaomi 14 256GB", CategoryId = 1, ImageUrl = "/images/xiaomi14.jpg"},

            // Laptop
            new Product { Id = 6, Name = "MacBook Air M3", Price = 32990000, Description = "MacBook Air M3 13inch 256GB", CategoryId = 2, ImageUrl = "/images/macbook-air-m3.jpg"},
            new Product { Id = 7, Name = "Dell XPS 13", Price = 28990000, Description = "Dell XPS 13 Intel i7 512GB", CategoryId = 2, ImageUrl = "/images/dell-xps13.jpg"},

            // Máy tính bảng
            new Product { Id = 11, Name = "iPad Air", Price = 16990000, Description = "iPad Air 256GB WiFi", CategoryId = 3, ImageUrl = "/images/ipad-air.jpg"},
            new Product { Id = 12, Name = "Samsung Galaxy Tab S9", Price = 18990000, Description = "Galaxy Tab S9 256GB", CategoryId = 3, ImageUrl = "/images/galaxy-tab-s9.jpg"},

            // Bàn phím
            new Product { Id = 16, Name = "Logitech MX Keys", Price = 2990000, Description = "Bàn phím wireless cao cấp", CategoryId = 4, ImageUrl = "/images/logitech-mx-keys.jpg"},
            new Product { Id = 17, Name = "Corsair K95", Price = 4990000, Description = "Bàn phím cơ gaming RGB", CategoryId = 4, ImageUrl = "/images/corsair-k95.jpg"},

            // Tai nghe
            new Product { Id = 21, Name = "Sony WH-1000XM5", Price = 7990000, Description = "Tai nghe chống ồn cao cấp", CategoryId = 5, ImageUrl = "/images/sony-wh1000xm5.jpg"},
            new Product { Id = 22, Name = "Apple AirPods Pro", Price = 6990000, Description = "Tai nghe true wireless", CategoryId = 5, ImageUrl = "/images/airpods-pro.jpg"}
        };
        modelBuilder.Entity<Product>().HasData(products);
    }
}