using System.Collections;
using System.Collections.Generic;

namespace WS.Utilities.Csv
{
    public class CsvTokens : IEnumerable<Token<CsvTokenType, string>>
    {
        private readonly IEnumerable<char> _enumerable;

        public char Separator { get; }

        public char QuoteCharacter { get; }

        public char EscapeCharacter { get; }

        public CsvTokens(IEnumerable<char> enumerable, char separator, char quoteCharacter, char escapeCharacter)
        {
            _enumerable = enumerable;
            Separator = separator;
            QuoteCharacter = quoteCharacter;
            EscapeCharacter = escapeCharacter;
        }

        public CsvTokens(IEnumerable<char> enumerable) : this(enumerable, ',', '"', '"')
        {
        }

        public IEnumerator<Token<CsvTokenType, string>> GetEnumerator()
        {
            return new CsvTokenEnumerator(_enumerable, Separator, QuoteCharacter, EscapeCharacter);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
