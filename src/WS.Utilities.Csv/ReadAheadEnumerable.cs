using System.Collections;
using System.Collections.Generic;

namespace WS.Utilities.Csv
{
    public class ReadAheadEnumerable<T> : IEnumerable<ReadAheadItem<T>>
    {
        private readonly IEnumerable<T> _enumerable;

        public ReadAheadEnumerable(IEnumerable<T> enumerable)
        {
            _enumerable = enumerable;
        }

        public IEnumerator<ReadAheadItem<T>> GetEnumerator()
        {
            var next = Option<T>.None;
            foreach (var item in _enumerable)
            {
                var first = next.IsNone;
                var current = next.Match(t => t, () => default(T));
                next = Option<T>.Some(item);
                if (first)
                {
                    continue;
                }
                yield return new ReadAheadItem<T>(current, next);
            }
            if (!next.IsNone)
            {
                var current = next.Match(t => t, () => default(T));
                yield return new ReadAheadItem<T>(current, Option<T>.None);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
