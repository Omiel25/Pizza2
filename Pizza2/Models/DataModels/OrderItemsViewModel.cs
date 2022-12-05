using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pizza2.Models
{
    public class OrderItemsViewModel
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("PizzaId")]
        public int PizzaId { get; set; }
        [ForeignKey("OrderId")]
        public int OrderId { get; set; }

    }
}
