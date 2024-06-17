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
        public DbSet<OrderHistoryViewModel> OrdersHistory { get; set; }
        public DbSet<MenuViewModel> Menu { get; set; }
        public DbSet<UserViewModel> User { get; set; }
        public DbSet<CustomPizzaIngridientsViewModel> CustomPizzaIngridients { get; set; }



#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {

        }


    }
}
