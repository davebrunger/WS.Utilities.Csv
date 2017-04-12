using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace WS.Utilities.Csv
{
    public class StreamEnumerable : IEnumerable<char>
    {
        private readonly Stream _source;

        public StreamEnumerable(Stream source)
        {
            _source = source;
        }

        public IEnumerator<char> GetEnumerator()
        {
            _source.Seek(0, SeekOrigin.Begin);
            using (var streamReader = new StreamReader(_source))
            {
                var buffer = new char[1];
                while (streamReader.Read(buffer, 0, 1) > 0)
                {
                    yield return buffer[0];
                }

            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
