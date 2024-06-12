namespace Pizza2.Models
{
    public class AcceptedOrderModel
    {
        public OrderViewModel OrderData { get; set; }
        public List<PizzaSubModel> OrderedPizzas { get; set; }
    }
}
