using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WS.Utilities.Csv
{
    public class DataLoader<T> : IEnumerator<T> where T : new()
    {
        private IEnumerator<IEnumerable<string>> _source;
        private T _current;
        private int? _rowNumber;
        private readonly Func<int, IReadOnlyList<string>, Option<T>> _createInstance;

        public T Current
        {
            get
            {
                if (_rowNumber.HasValue)
                {
                    return _current;
                }
                throw new InvalidOperationException();
            }
        }

        object IEnumerator.Current => Current;

        public DataLoader(IEnumerable<IEnumerable<string>> source, Func<int, IReadOnlyList<string>, Option<T>> createInstance)
        {
            _source = source.GetEnumerator();
            _createInstance = createInstance;
        }

        public bool MoveNext()
        {
            while (_source.MoveNext())
            {
                var newRowNumber = (_rowNumber ?? -1) + 1;
                _rowNumber = newRowNumber;
                var data = _source.Current as IReadOnlyList<string> ?? _source.Current.ToList().AsReadOnly();
                var currentChanged = _createInstance(newRowNumber, data).Match(t =>
                    {
                        _current = t;
                        return true;
                    }, () => false);
                if (currentChanged)
                {
                    return true;
                }
            }
            _current = default(T);
            _rowNumber = null;
            return false;
        }

        public void Reset()
        {
            _current = default(T);
            _rowNumber = null;
            _source.Reset();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_source != null)
                {
                    _source.Dispose();
                    _source = null;
                }
            }
        }

        ~DataLoader()
        {
            Dispose(false);
        }
    }
}
