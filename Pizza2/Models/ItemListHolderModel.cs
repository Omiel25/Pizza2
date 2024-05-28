namespace Pizza2.Models
{
    public class ItemListHolderModel<Y, T>
    {
        public Y ItemA { get; set; }
        public List<T> ItemsB { get; set; }
        public T? ItemB { get; set; }
        public string customData { get; set; }
        public int Id { get; set; }
    }
}
