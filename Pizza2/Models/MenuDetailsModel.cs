namespace Pizza2.Models
{
    public class MenuDetailsModel
    {
        public string MenuName { get; set; }

        public bool IsActive { get; set; }

        public List<string> PizzaNames { get; set; }
    }
}
