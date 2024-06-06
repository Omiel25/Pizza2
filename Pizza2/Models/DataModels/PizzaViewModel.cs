using System.ComponentModel.DataAnnotations;

namespace Pizza2.Models
{
    public class PizzaViewModel
    {
        [Key]
        public int Id { get; set; }
        
        public int IngridientsListId { get; set; }
        public string PizzaName { get; set; }
        public float? PizzaPrice { get; set; }
        public bool IsCustomPizza { get; set; }


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public PizzaViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {

        }
    }
}
