using System;
using System.Collections.Generic;
using System.IO;

namespace common
{
    public class InputReader : IDisposable
    {
        private StreamReader _reader;

        public InputReader(string filePath)
        {
            _reader = new StreamReader(filePath);
        }

        public void Dispose()
        {
            _reader.Dispose();
        }

        public IEnumerable<string> GetLines()
        {
            while(!_reader.EndOfStream)
            {
                yield return _reader.ReadLine();
            }
        }

        public string GetNextLine()
        {
            if(_reader.EndOfStream)
            {
                return null;
            }

            return _reader.ReadLine();
        }
    }
}
