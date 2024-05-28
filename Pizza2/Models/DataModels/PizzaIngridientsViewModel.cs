using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pizza2.Models
{
    public class PizzaIngridientsViewModel
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("IngridentId")]
        public int IngridientId { get; set; }

        [ForeignKey("PizzaIngridientListId")]
        public int PizzaIngridientListId { get; set; }

        public string IngridientListName { get; set; }
    }
}
