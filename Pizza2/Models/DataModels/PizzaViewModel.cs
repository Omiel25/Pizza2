using System.ComponentModel.DataAnnotations;

namespace Pizza2.Models
{
    public class PizzaViewModel
    {
        [Key]
        public int Id { get; set; }
        
        public int? IngridientsListId { get; set; }
        public string PizzaName { get; set; }
        public float? PizzaPrice { get; set; }
        public bool IsCustomPizza { get; set; }


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public PizzaViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {

        }

        public void CalculateCustomPrice( List<int> customIngridientsId, List<IngridientViewModel> ingridients)
        {
            if (this.IsCustomPizza || this.PizzaPrice == null)
            {
                float customPrice = 0;
                foreach(int ingridientId in customIngridientsId)
                {
                    float? ingridientPrice = ingridients.
                        Where(i => i.Id == ingridientId).
                        Select(i => i.IngridientPrice).
                        FirstOrDefault();

                    if (ingridientPrice != null)
                        customPrice += ingridientPrice.Value;
                }

                string finalPrice = customPrice.ToString( "0.00" );

                this.PizzaPrice = float.Parse(finalPrice);
            }
        }

        public string CreateIngridientList( List<int> ingridientsId, List<IngridientViewModel> ingridients )
        {

            return null;
        }
    }
}
