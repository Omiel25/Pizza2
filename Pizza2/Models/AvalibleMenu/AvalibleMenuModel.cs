namespace Pizza2.Models
{
    public class AvalibleMenuModel
    {
        public List<PizzaSubModel> Pizzas { get; set; }
        public List<IngridientViewModel> Ingridients { get; set;}
        public List<string> ShopCartPizzasIds { get; set; }
    }
}
