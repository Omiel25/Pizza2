using System.ComponentModel.DataAnnotations;

namespace Pizza2.Models
{
    public class MenuViewModel
    {
        [Key]
        public int Id { get; set; }

        public int menuId { get; set; }

        public string menuName { get; set; }

        public int MenuItemPosition { get; set; }   

        public bool IsActive { get; set; }

        public int PizzaId { get; set; }
    }
}
