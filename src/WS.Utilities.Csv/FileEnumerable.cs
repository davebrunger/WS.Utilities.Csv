using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace WS.Utilities.Csv
{
    public class FileEnumerable : IEnumerable<char>
    {
        public string FileName { get; }

        public FileEnumerable(string fileName)
        {
            FileName = fileName;
        }

        public IEnumerator<char> GetEnumerator()
        {
            using (var fileStream = new FileStream(FileName, FileMode.Open))
            {
                var streamEnumerable = new StreamEnumerable(fileStream);
                foreach (var c in streamEnumerable)
                {
                    yield return c;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
