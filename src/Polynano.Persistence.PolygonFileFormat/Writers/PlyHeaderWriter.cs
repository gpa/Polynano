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
using System;
using System.Text;

namespace Polynano.Persistence.PolygonFileFormat.Writers
{
    public static class PlyHeaderWriter
    {
        public static string GetHeader(PlyHeader header)
        {
            var sb = new StringBuilder();

            sb.AppendLine(PlyKeywords.MagicNumber);

            var formatKeyword = default(string);
            if (header.Format == PlyFormat.Ascii)
                formatKeyword = PlyKeywords.AsciiFormat;
            else if (header.Format == PlyFormat.BinaryBigEndian)
                formatKeyword = PlyKeywords.BinaryBigEndianFormat;
            else if (header.Format == PlyFormat.BinaryLittleEndian)
                formatKeyword = PlyKeywords.BinaryLittleEndianFormat;
            else 
                throw new PlyInvalidHeaderVersionException(formatKeyword);

            sb.AppendLine($"{formatKeyword} {PlyKeywords.FormatVersion}");

            if (header.Comment != null)
            {
                var escapedComment = EscapeNewLines(header.Comment);
                foreach (string comment in escapedComment)
                {
                    var commentDeclaration = $"{PlyKeywords.Comment} {comment}";
                    sb.AppendLine(commentDeclaration);
                }
            }

            if (header.ObjectInfo != null)
            {
                var escapedObjectInfo = EscapeNewLines(header.ObjectInfo);
                foreach (string objectInfo in escapedObjectInfo)
                {
                    var objectInfoDeclaration = $"{PlyKeywords.ObjectInfo} {objectInfo}";
                    sb.AppendLine(objectInfoDeclaration);
                }
            }

            foreach (PlyElement element in header.Elements)
            {
                var elementDeclaration = $"{PlyKeywords.Element} {element.Name} {element.InstanceCount}";
                sb.AppendLine(elementDeclaration);

                foreach (PlyProperty property in element.Properties)
                {
                    var valueType = PlyTypeConverter.ToStringRepresentation(property.ValueType);
                    var propertyDeclaration = $"{PlyKeywords.Property} {valueType} {property.Name}";

                    if (property is PlyArrayProperty arrayProperty)
                    {
                        var arraySizeType = PlyTypeConverter.ToStringRepresentation(arrayProperty.ArraySizeType);
                        propertyDeclaration = $"{PlyKeywords.PropertyList} {arraySizeType} {valueType} {property.Name}";
                    }

                    sb.AppendLine(propertyDeclaration);
                }
            }

            sb.AppendLine(PlyKeywords.HeaderEnd);
            return sb.ToString();
        }
        
        private static string[] EscapeNewLines(string input)
        {
            return input.Split(new[] { "\r\n", "\r", "\n"}, StringSplitOptions.None);
        }
    }
}