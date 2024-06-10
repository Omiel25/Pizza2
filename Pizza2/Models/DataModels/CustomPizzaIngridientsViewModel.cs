using System.ComponentModel.DataAnnotations;

namespace Pizza2.Models
{
    public class CustomPizzaIngridientsViewModel
    {
        [Key]
        public int Id { get; set; }
        public int PizzaID { get; set; }
        public int IngridientID { get; set; }

    }
}
