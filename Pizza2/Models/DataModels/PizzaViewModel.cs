using System.ComponentModel.DataAnnotations;

namespace Pizza2.Models
{
    public class PizzaViewModel
    {
        [Key]
        public int Id { get; set; }
        
        public int IngridientsListId { get; set; }
        public string PizzaName { get; set; }
        public float PizzaPrice { get; set; }
        

        public PizzaViewModel()
        {

        }
    }
}
