using System.ComponentModel.DataAnnotations;

namespace Pizza2.Models
{
    public class MenuViewModel
    {
        [Key]
        public int Id { get; set; }
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public int MenuItemPosition { get; set; }   
        public bool IsActive { get; set; }
        public int PizzaId { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
