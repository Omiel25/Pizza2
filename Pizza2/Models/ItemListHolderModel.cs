namespace Pizza2.Models
{
    public class ItemListHolderModel<Y, T>
    {
        public Y ItemA { get; set; }
        public List<T> ItemsB { get; set; }
        public int Id { get; set; }
    }
}
