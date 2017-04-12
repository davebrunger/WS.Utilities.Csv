using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace WS.Utilities.Csv
{
    /// <summary>
    /// Reads CsvTokens as lines of data
    /// </summary>
    /// <remarks>
    /// It is expected that each record be terminated by an end of line sequence. The end of line sequence on the
    /// last line is optional unless the last line is empty. As a result if no tokens are read the file is 
    /// considered empty
    /// </remarks>
    public class CsvRecordEnumerator : IEnumerator<IReadOnlyList<string>>
    {
        private IEnumerator<ReadAheadItem<Token<CsvTokenType, string>>> _source;

        private ImmutableList<string> _current;

        public IReadOnlyList<string> Current
        {
            get
            {
                if (_current != null)
                {
                    return _current;
                }
                throw new InvalidOperationException();
            }
        }

        object IEnumerator.Current => Current;

        public CsvRecordEnumerator(IEnumerable<Token<CsvTokenType, string>> source)
        {
            _source = new ReadAheadEnumerable<Token<CsvTokenType, string>>(source).GetEnumerator();
            _current = null;
        }

        /// <summary>
        /// Moves the enumerator to the next record in the CSV token input
        /// </summary>
        /// <returns>
        /// true if a record has been read and is available for processing, otherwise false
        /// </returns>
        /// <remarks>
        /// An empty line will be returned as a record with no items, so the consumer of this class will
        /// need to accomodated that. It is expected that each record be terminated by an end of line sequence. 
        /// The end of line sequence on the last line is optional unless the last line is empty. As a result if 
        /// no tokens are read the file is considered empty.
        /// </remarks>
        public bool MoveNext()
        {
            if (!_source.MoveNext())
            {
                _current = null;
                return false;
            }
            _current = Enumerable.Empty<string>().ToImmutableList();
            var currentValue = string.Empty;
            var firstToken = true;
            var tokensToProcess = true;
            while (tokensToProcess)
            {
                switch (_source.Current.Current.TokenType)
                {
                    case CsvTokenType.Value:
                        var value = _source.Current.Current.Representation;
                        currentValue = value;
                        var shouldReturn = _source.Current.Next.Match(next =>
                        {
                            if (next.TokenType == CsvTokenType.Value)
                            {
                                throw new InvalidDataException(
                                    $"Expected separator or end of line, found {next.Representation}");
                            }
                            return false;
                        }, () =>
                        {
                            _current = _current.Add(value);
                            return true;
                        });
                        if (shouldReturn)
                        {
                            return true;
                        }
                        break;
                    case CsvTokenType.Separator:
                        _current = _current.Add(currentValue);
                        currentValue = string.Empty;
                        if (_source.Current.Next.IsNone)
                        {
                            _current = _current.Add(currentValue);
                            return true;
                        }
                        break;
                    case CsvTokenType.EndOfLine:
                        if (!firstToken)
                        {
                            _current = _current.Add(currentValue);
                        }
                        return true;
                }
                tokensToProcess = _source.MoveNext();
                firstToken = false;
            }
            throw new InvalidDataException("Unexpected end of file");
        }

        public void Reset()
        {
            throw new NotImplementedException();
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

        ~CsvRecordEnumerator()
        {
            Dispose(false);
        }
    }
}
