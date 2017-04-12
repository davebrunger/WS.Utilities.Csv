using System;
using System.Collections;
using System.Collections.Generic;

namespace WS.Utilities.Csv
{
    public class CsvData<T> : IEnumerable<T> where T : new()
    {
        private readonly IEnumerable<IReadOnlyList<string>> _source;
        private readonly Func<int, IReadOnlyList<string>, Option<T>> _createInstance;

        public CsvData(IEnumerable<IReadOnlyList<string>> source, Func<int, IReadOnlyList<string>, Option<T>> createInstance)
        {
            _source = source;
            _createInstance = createInstance;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new DataLoader<T>(_source, _createInstance);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
