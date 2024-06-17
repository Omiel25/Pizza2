using Microsoft.EntityFrameworkCore;
using Pizza2.Data;

namespace Pizza2.Models
{
    public class OrderDetailsModel
    {
        public OrderViewModel? OrderData { get; set; }
        public List<PizzaSubModel>? OrderedPizzas { get; set; }

        /// <summary>
        /// Get complete list of current order
        /// </summary>
        /// <param name="context">Databse connection</param>
        /// <param name="accepted">Determines if you get pending or accepted orders</param>
        /// <returns></returns>
        public static List<OrderDetailsModel> GetOrders(ApplicationDbContext context, bool accepted)
        {
            List<OrderDetailsModel> orderList = new List<OrderDetailsModel>();
            List<IngridientViewModel> ingridients = context.Ingridients.ToList();
            List<OrderViewModel> acceptedOrders = context.Orders.Where( p => p.OrderConfirmed == accepted ).ToList();

            foreach (OrderViewModel acceptedOrder in acceptedOrders)
            {
                OrderDetailsModel newOrder = new OrderDetailsModel();
                newOrder.OrderedPizzas = new List<PizzaSubModel>();
                newOrder.OrderData = acceptedOrder;

                List<int> pizzasId = context.OrderItems.Where( i => i.OrderId == acceptedOrder.Id ).Select( i => i.PizzaId ).ToList();
                List<PizzaViewModel> orderPizzas = context.Pizzas.Where( p => pizzasId.Contains( p.Id ) ).ToList();

                //Bind ingridients to target pizzas
                float orderPrice = 0f;
                foreach (PizzaViewModel orderedPizza in orderPizzas)
                {
                    PizzaSubModel pizzaModel = new PizzaSubModel();
                    pizzaModel.Pizza = orderedPizza;

                    if (orderedPizza.IsCustomPizza == true)
                    {
                        List<int> customIngridientsId = context.CustomPizzaIngridients.
                            Where( i => i.PizzaID == orderedPizza.Id ).
                            Select( i => i.IngridientID ).
                            ToList();

                        pizzaModel.PizzaIngridients = context.Ingridients.
                            Where( i => customIngridientsId.Contains( i.Id ) ).
                            OrderBy( i => i.DisplayPriority ).
                            ToList();
                    }
                    //Calculate Automatic pizza price
                    if (pizzaModel.Pizza.PizzaPrice == null || pizzaModel.Pizza.PizzaPrice <= 0f)
                    {
                        List<int> ingridientsId = context.PizzaIngridients.
                            Where( i => i.PizzaIngridientListId == pizzaModel.Pizza.IngridientsListId ).
                            Select( i => i.IngridientId ).
                            ToList();

                        pizzaModel.Pizza.CalculateCustomPrice( ingridientsId, ingridients );
                    }

                    orderPrice += pizzaModel.Pizza.PizzaPrice ?? 0f;
                    newOrder.OrderedPizzas.Add( pizzaModel );
                }

                newOrder.OrderData.OrderPrice = orderPrice;
                orderList.Add( newOrder );
            }

            return orderList;
        }
    
    }
}
