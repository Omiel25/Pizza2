using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Pizza2.Models
{
    public class AddMultipleModel<T, Y>
    {
        //Class Made to to be able to handle multiple models
        public List<T>? itemOneList { get; set; }

        public List<Y>? itemTwoList { get; set; }

        public T? itemOne { get; set; }

        public Y? itemTwo { get; set; }

        public AddMultipleModel()
        {

        }
    }
}
