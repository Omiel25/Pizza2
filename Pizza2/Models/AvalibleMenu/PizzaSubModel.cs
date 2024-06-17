using static System.Net.Mime.MediaTypeNames;

namespace Pizza2.Models
{
    public class PizzaSubModel
    {
        public PizzaViewModel Pizza { get; set; }
        public List<IngridientViewModel> PizzaIngridients { get; set; }

        /// <summary>
        /// Get data required to fill order table
        /// </summary>
        /// <param name="totalOrderPrice"> Total price of all items in order </param>
        /// <returns></returns>
        public string CreatePizzaText()
        {
            string tableText = "";
            if (this.Pizza.IsCustomPizza == false)
            {
                tableText += $"<p>{this.Pizza.PizzaName}</p>";
            }
            else
            {
                int ingridinentCount = this.PizzaIngridients.Count();
                tableText += $"<p class=\"fw-bold\">{this.Pizza.PizzaName} (<span>";
                for (int i = 0; i < ingridinentCount; i++)
                {
                    if (i + 1 == ingridinentCount)
                        tableText += this.PizzaIngridients[ i ].IngridientName;
                    else
                        tableText += $"{this.PizzaIngridients[ i ].IngridientName}, ";
                }
                tableText += $"</span>) </p>";
            }
            return tableText;
        }
    }
}
