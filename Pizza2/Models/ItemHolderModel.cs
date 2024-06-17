namespace Pizza2.Models
{
    public class ItemHolderModel<Y>
    {
        //Class made to check if we want to add target item or not
        public Y ?heldItem { get; set; }

        public bool? addItem { get; set; }

        public int? itemId { get; set; }
    }
}
