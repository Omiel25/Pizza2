using Microsoft.EntityFrameworkCore;
using Pizza2.Models;

namespace Pizza2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<IngridientViewModel> Ingridients  { get; set; }
        public DbSet<PizzaIngridientsViewModel> PizzaIngridients { get; set; }
        public DbSet<PizzaViewModel> Pizzas { get; set; }
        public DbSet<OrderItemsViewModel> OrderItems { get; set; }
        public DbSet<OrderViewModel> Orders { get; set; }
        public DbSet<MenuViewModel> Menu { get; set; }
        public DbSet<UserViewModel> User { get; set; }



        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

    }
}
