using System.ComponentModel.DataAnnotations;

namespace Pizza2.Models
{
    public class OrderViewModel
    {
        [Key]
        public int Id { get; set; }
        public bool OrderConfirmed { get; set; }
        public string OrderMakerName { get; set; }
        public float OrderPrice { get; set; }

        public OrderViewModel()
        {

        }
    }
}
