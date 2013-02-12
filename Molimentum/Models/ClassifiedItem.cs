namespace Molimentum.Models
{
    public class ClassifiedItem<T>
    {
        public T Item { get; set; }

        public int Class { get; set; }
    }
}