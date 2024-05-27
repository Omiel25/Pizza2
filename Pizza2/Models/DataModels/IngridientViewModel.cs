using Microsoft.EntityFrameworkCore.Diagnostics;
using System.ComponentModel.DataAnnotations;

namespace Pizza2.Models
{
    public class IngridientViewModel
    {
        [Key]
        public int Id { get; set; }
        public string IngridientName { get; set; }  
        public float IngridientPrice { get; set; }
        public int DisplayPriority { get; set; }
        public int ImagePriority { get; set; }
        public bool ContainsMeat { get; set; }   
        
        public IngridientViewModel()
        {

        }
    }
}
