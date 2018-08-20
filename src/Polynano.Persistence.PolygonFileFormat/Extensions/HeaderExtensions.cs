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

namespace Polynano.Persistence.PolygonFileFormat.Extensions
{
    public static class HeaderExtensions
    {
        public static PlyReader GetDataReader(this PlyHeader header, BufferedStreamReader streamReader) 
        {
            if (header.Format == PlyFormat.Ascii)
                return new PlyAsciiReader(streamReader, header);
            else if (header.Format == PlyFormat.BinaryBigEndian)
                return new PlyBinaryReader(streamReader, header);
            
            throw new ArgumentException("Unknown header format.");
        }
    }
}