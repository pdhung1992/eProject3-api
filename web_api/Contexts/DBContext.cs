using Microsoft.EntityFrameworkCore;
using web_api.Entities;

namespace web_api.Contexts
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions options) : base(options)
        {
            
        }
        
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Combo> Combos { get; set; }
        public DbSet<ComboDetail> ComboDetails { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<FoodTag> FoodTags { get; set; }
        public DbSet<FoodType> FoodTypes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<ServeType> ServeTypes { get; set; }
        public DbSet<User> Users { get; set; }
    }
}

