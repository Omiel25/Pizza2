namespace Pizza2.Models
{
    public class AvalibleMenuModel
    {
        public List<PizzaSubModel>? Pizzas { get; set; }
        public List<IngridientViewModel>? Ingridients { get; set;}
        public List<IngridientViewModel>? Sauces { get; set; }
        public List<IngridientViewModel>? Pies { get; set; }
        public List<IngridientViewModel>? ClassicIngridients { get; set; }

        public AvalibleMenuModel() 
        { 
            
        }

        public void FillAdditionalIngridients( )
        {
            if (this.Ingridients == null || this.Ingridients.Count <= 0)
                return;

            this.Sauces = this.Ingridients.
                    Where( i => i.IngridientName.Contains( "Sauce", StringComparison.OrdinalIgnoreCase ) )
                    .ToList();

            this.Pies = this.Ingridients.
                Where( p =>
                    p.IngridientName.Contains( "Pie", StringComparison.OrdinalIgnoreCase ) &&
                    p.IngridientName != "Pizza Pie" ).
                    ToList() ?? new List<IngridientViewModel>();

            this.ClassicIngridients = this.Ingridients.
                Where( p =>
                    !p.IngridientName.Contains( "Pie", StringComparison.OrdinalIgnoreCase ) &&
                    !p.IngridientName.Contains( "Sauce", StringComparison.OrdinalIgnoreCase ) ).
                ToList() ?? new List<IngridientViewModel>();
        }
    }
}
