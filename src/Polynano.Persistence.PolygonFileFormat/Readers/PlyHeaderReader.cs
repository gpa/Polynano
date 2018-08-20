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
using Polynano.Persistence.PolygonFileFormat.Exceptions;
using Polynano.Persistence.PolygonFileFormat.Readers.StreamReaders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Polynano.Persistence.PolygonFileFormat.Readers
{
    public class PlyHeaderReader
    {
        private ITextReader Reader { get; }

        private string CurrentLine { get; set; }

        public PlyHeaderReader(ITextReader reader)
        {
            Reader = reader;
        }

        private void MoveNextLine()
        {
            CurrentLine = Reader.ReadLine();
        }

        public PlyHeader Read()
        {
            ParseMagicNumber();
            var format = ParseFormat();
            var comment = ParseComments();
            var info = ParseObjectInfos();
            var elements = ParseElements();
            ParseHeaderEnd();

            return new PlyHeader(format, comment, info, elements);
        }

        private void ParseMagicNumber()
        {
            MoveNextLine();
            if (CurrentLine == null)
                throw new PlyMagicNotFoundException();

            if (CurrentLine.Trim() != PlyKeywords.MagicNumber)
                throw new PlyUnexpectedTokenException(CurrentLine, PlyKeywords.MagicNumber);
            
            MoveNextLine();
        }

        private PlyFormat ParseFormat()
        {
            string line = CurrentLine.Trim();

            PlyFormat format = default(PlyFormat);
            if (line.StartsWith(PlyKeywords.AsciiFormat))
                format = PlyFormat.Ascii;
            else if(line.StartsWith(PlyKeywords.BinaryBigEndianFormat))
                format = PlyFormat.BinaryBigEndian;
            else if(line.StartsWith(PlyKeywords.BinaryLittleEndianFormat))
                format = PlyFormat.BinaryLittleEndian;
            else 
                throw new PlyInvalidHeaderVersionException(format);

            MoveNextLine();
            return format;
        }

        private string ParseComments()
        {
            var buffer = new StringBuilder();
            var keywordLength = PlyKeywords.Comment.Length + 1;
            while (CurrentLine != null && CurrentLine.StartsWith(PlyKeywords.Comment))
            {
                if (buffer.Length != 0)
                    buffer.AppendLine();

                if (CurrentLine.Length <= keywordLength)
                    throw new PlyMalformedLineException(CurrentLine);

                buffer.Append(CurrentLine, keywordLength, CurrentLine.Length - keywordLength);
                MoveNextLine();
            }
            
            return buffer.ToString();
        }

        private string ParseObjectInfos()
        {
            var buffer = new StringBuilder();
            var keywordLength = PlyKeywords.ObjectInfo.Length + 1;
            while (CurrentLine != null && CurrentLine.StartsWith(PlyKeywords.ObjectInfo))
            {
                if (CurrentLine.Length <= keywordLength)
                    throw new PlyMalformedLineException(CurrentLine);

                if (buffer.Length != 0)
                    buffer.AppendLine();

                buffer.Append(CurrentLine.Substring(keywordLength));
                MoveNextLine();
            }

            return buffer.ToString();
        }

        private IList<PlyElement> ParseElements()
        {
            List<PlyElement> elements = new List<PlyElement>();
            while (CurrentLine != null && CurrentLine.StartsWith(PlyKeywords.Element))
            {
                var element = ParseElement();
                elements.Add(element);
            }

            return elements;
        }

        private PlyElement ParseElement()
        {
            try
            {
                string[] tokens = CurrentLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                MoveNextLine();

                if (tokens.Length != 3)
                    throw new PlyMalformedLineException(CurrentLine);

                var name = tokens[1];
                var count = int.Parse(tokens[2]);
                var properties = ParseProperties();

                return new PlyElement(name, count, properties);
            }
            catch (FormatException)
            {
                throw new PlyMalformedLineException(CurrentLine);
            }
        }
        
        private IList<PlyProperty> ParseProperties()
        {
            List<PlyProperty> properties = new List<PlyProperty>();
            while (CurrentLine != null && CurrentLine.StartsWith(PlyKeywords.Property))
            {
                var property = default(PlyProperty);
                if (CurrentLine.StartsWith(PlyKeywords.PropertyList))
                    property = ParseArrayProperty();
                else if (CurrentLine.StartsWith(PlyKeywords.Property))
                    property = ParseValueProperty();
                else
                    break;
                    
                properties.Add(property);
                MoveNextLine();
            }

            return properties;
        }

        private PlyProperty ParseArrayProperty()
        {
            string[] tokens = CurrentLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length != 5)
                throw new PlyMalformedLineException(CurrentLine);

            try
            {
                var name = tokens[4];
                var arrayCountSizeType = PlyTypeConverter.GetNativeType(tokens[2]);
                var valueType = PlyTypeConverter.GetNativeType(tokens[3]);
                return new PlyArrayProperty(name, arrayCountSizeType, valueType);
            }
            catch (ArgumentException)
            {
                throw new PlyMalformedLineException(CurrentLine);
            }
        }

        private PlyProperty ParseValueProperty()
        {
            string[] tokens = CurrentLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length != 3)
                throw new PlyMalformedLineException(CurrentLine);

            try 
            {
                var name = tokens[2];
                var type = PlyTypeConverter.GetNativeType(tokens[1]);
                return new PlyProperty(name, type);
            } 
            catch(ArgumentException)
            {
                throw new PlyMalformedLineException(CurrentLine);
            }
        }

        private void ParseHeaderEnd()
        {
            if (CurrentLine != PlyKeywords.HeaderEnd)
                throw new PlyUnexpectedTokenException(CurrentLine, PlyKeywords.HeaderEnd);
        }
    }
}

