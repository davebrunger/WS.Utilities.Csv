using System.Collections;
using System.Collections.Generic;

namespace WS.Utilities.Csv
{
    public class CsvRecords : IEnumerable<IReadOnlyList<string>>
    {
        private readonly IEnumerable<Token<CsvTokenType, string>> _enumerable;

        public CsvRecords(IEnumerable<Token<CsvTokenType, string>> enumerable)
        {
            _enumerable = enumerable;
        }

        public IEnumerator<IReadOnlyList<string>> GetEnumerator()
        {
            return new CsvRecordEnumerator(_enumerable);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
