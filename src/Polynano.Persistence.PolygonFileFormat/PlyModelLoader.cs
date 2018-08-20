/*
MIT License

Copyright(c) 2018 Gratian Pawliszyn

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using Polynano.Persistence.PolygonFileFormat.Common;
using Polynano.Persistence.PolygonFileFormat.Readers;
using Polynano.Persistence.PolygonFileFormat.Readers.StreamReaders;
using System;
using System.IO;

namespace Polynano.Persistence.PolygonFileFormat
{
    public class PlyModelLoader : IDisposable
    {
        private FileStream _fileStream;
        private BufferedStreamReader _streamReader;
        private string _filePath;

        public PlyHeader ContentHeader { get; private set; }
        public PlyReader ContentReader { get; private set; }

        public PlyModelLoader(string filePath)
        {
            _filePath = filePath;
            Initialize();
        }

        private void Initialize()
        {
            _fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read);
            _streamReader = new BufferedStreamReader(_fileStream);
            var plyHeaderReader = new PlyHeaderReader(_streamReader);
            ContentHeader = plyHeaderReader.Read();

            if (ContentHeader.Format == PlyFormat.Ascii)
                ContentReader = new PlyAsciiReader(_streamReader, ContentHeader);
            else if (ContentHeader.Format == PlyFormat.BinaryBigEndian)
                ContentReader = new PlyBinaryReader(_streamReader, ContentHeader);
            else if (ContentHeader.Format == PlyFormat.BinaryLittleEndian)
                ContentReader = new PlyBinaryReader(_streamReader, ContentHeader);
        }

        public void Dispose()
        {
            _streamReader.Dispose();
            _fileStream.Dispose();
        }
    }
}
