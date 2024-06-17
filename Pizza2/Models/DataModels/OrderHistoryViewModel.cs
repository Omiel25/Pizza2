using System.ComponentModel.DataAnnotations;

namespace Pizza2.Models
{
    public class OrderHistoryViewModel
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string OrderMakerName { get; set; }
        public float? OrderPrice { get; set; }
        public bool OrderAccepted { get; set; }
        public string? AcceptingUser { get; set; }
        public DateTime CreatedAt { get; set; }

        public OrderHistoryViewModel()
        {

        }

        
    }
}
