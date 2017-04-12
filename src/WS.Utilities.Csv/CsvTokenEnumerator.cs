using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace WS.Utilities.Csv
{
    public enum CsvTokenType
    {
        Value,
        Separator,
        EndOfLine
    }

    public class CsvTokenEnumerator : IEnumerator<Token<CsvTokenType, string>>
    {
        private Token<CsvTokenType, string> _current;
        private IEnumerator<ReadAheadItem<char>> _source;

        public char Separator { get; }

        public char QuoteCharacter { get; }

        public char EscapeCharacter { get; }

        public Token<CsvTokenType, string> Current
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

        public CsvTokenEnumerator(IEnumerable<char> source, char separator, char quoteCharacter, char escapeCharacter)
        {
            _source = new ReadAheadEnumerable<char>(source).GetEnumerator();
            Separator = separator;
            QuoteCharacter = quoteCharacter;
            EscapeCharacter = escapeCharacter;
            _current = null;
        }

        public CsvTokenEnumerator(IEnumerable<char> source) : this(source, ',', '"', '"')
        {
        }

        public bool MoveNext()
        {
            if (!_source.MoveNext())
            {
                _current = null;
                return false;
            }
            switch (_source.Current.Current)
            {
                case '\n':
                    _current = new Token<CsvTokenType, string>(CsvTokenType.EndOfLine, _source.Current.Current.ToString());
                    return true;
                case '\r':
                    if (_source.Current.Next == Option<char>.Some('\n'))
                    {
                        _source.MoveNext();
                        _current = new Token<CsvTokenType, string>(CsvTokenType.EndOfLine, "\r\n");
                    }
                    else
                    {
                        _current = new Token<CsvTokenType, string>(CsvTokenType.EndOfLine, _source.Current.Current.ToString());
                    }
                    return true;
                default:
                    if (_source.Current.Current == Separator)
                    {
                        _current = new Token<CsvTokenType, string>(CsvTokenType.Separator, _source.Current.Current.ToString());
                    }
                    else if (_source.Current.Current == QuoteCharacter)
                    {
                        _current = new Token<CsvTokenType, string>(CsvTokenType.Value, ReadQuotedString());
                    }
                    else
                    {
                        _current = new Token<CsvTokenType, string>(CsvTokenType.Value, ReadUnquotedString());
                    }
                    return true;
            }
        }

        private string ReadUnquotedString()
        {
            var result = string.Empty;
            while (true)
            {
                if ((_source.Current.Current == QuoteCharacter) ||
                    (_source.Current.Current == EscapeCharacter))
                {
                    throw new InvalidDataException($"Unexpected quote or escape character: {_source.Current.Current}");
                }
                result += _source.Current.Current;
                if ((_source.Current.Next.IsNone) ||
                    (_source.Current.Next == Option<char>.Some(Separator)) ||
                    (_source.Current.Next == Option<char>.Some('\n')) ||
                    (_source.Current.Next == Option<char>.Some('\r')))
                {
                    return result;
                }
                _source.MoveNext();
            }
        }

        private string ReadQuotedString()
        {
            var result = string.Empty;
            if (!_source.MoveNext())
            {
                throw new InvalidDataException("Unexpected end of file");
            }
            var charactersToProcess = true;
            while (charactersToProcess)
            {
                if (_source.Current.Current == EscapeCharacter)
                {
                    if (_source.Current.Next.IsNone)
                    {
                        if (EscapeCharacter != QuoteCharacter)
                        {
                            throw new InvalidDataException("Unexpected end of file");
                        }
                        return result;
                    }
                    if ((_source.Current.Next != Option<char>.Some(EscapeCharacter)) && (_source.Current.Next != Option<char>.Some(QuoteCharacter)))
                    {
                        if (EscapeCharacter != QuoteCharacter)
                        {
                            throw new InvalidDataException($"Invalid Escape sequence {_source.Current.Current}{_source.Current.Next}");
                        }
                        return result;
                    }
                    charactersToProcess = _source.MoveNext();
                    result += _source.Current.Current;
                }
                else
                {
                    if (_source.Current.Current == QuoteCharacter)
                    {
                       return result;
                    }
                    result += _source.Current.Current;
                    if (!_source.MoveNext())
                    {
                        throw new InvalidDataException("Unexpected end of file");
                    }
                }
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

        ~CsvTokenEnumerator()
        {
            Dispose(false);
        }
    }
}
