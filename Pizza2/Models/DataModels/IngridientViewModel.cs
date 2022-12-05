using System.ComponentModel.DataAnnotations;

namespace Pizza2.Models
{
    public class IngridientViewModel
    {
        [Key]
        public int Id { get; set; }
        public string IngridientName { get; set; }  
        public float IngridientPrice { get; set; }
        public bool ContainsMeat { get; set; }   
        
        public IngridientViewModel()
        {

        }
    }
}
