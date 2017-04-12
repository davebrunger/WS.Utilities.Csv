namespace WS.Utilities.Csv
{
    public class ReadAheadItem<T>
    {
        public T Current { get; }

        public Option<T> Next { get; }

        public ReadAheadItem(T current, Option<T> next)
        {
            Current = current;
            Next = next;
        } 
    }
}
