using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Pizza2.Models
{
    public class EditViewModel<T>
    {
        public List<T> itemList { get; set; }

        public T itemModel { get; set; }

        public EditViewModel()
        {

        }
    }
}
