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
using Polynano.Persistence.PolygonFileFormat.Exceptions;
using System;
using System.Globalization;

namespace Polynano.Persistence.PolygonFileFormat.Common
{
    static internal class PlyTypeConverter
    {
        public static Type ToNative(PlyType dataType)
        {
            Type t;
            switch (dataType)
            {
                case PlyType.Char:
                    t = typeof(sbyte);
                    break;
                case PlyType.Uchar:
                    t = typeof(byte);
                    break;
                case PlyType.Short:
                    t = typeof(short);
                    break;
                case PlyType.Ushort:
                    t = typeof(ushort);
                    break;
                case PlyType.Int:
                    t = typeof(int);
                    break;
                case PlyType.Uint:
                    t = typeof(uint);
                    break;
                case PlyType.Float:
                    t = typeof(float);
                    break;
                case PlyType.Double:
                    t = typeof(double);
                    break;
                default:
                    throw new ArgumentException($"Cannot convert {nameof(PlyType)}. Given type is invalid.", nameof(dataType));
            }
            return t;
        }
        public static string ToStringRepresentation(PlyType dataType)
        {
            if(!Enum.IsDefined(typeof(PlyType), dataType))
                throw new ArgumentException($"Cannot convert {nameof(PlyType)}. Given type is invalid.", nameof(dataType));
           
            return dataType.ToString().ToLowerInvariant();
        }
        public static byte[] ToBytes<T>(T val, PlyType dataType) where T : IConvertible
        {
            byte[] bytes;
            switch (dataType)
            {
                case PlyType.Char:
                    bytes = BitConverter.GetBytes(Convert.ToSByte(val));
                    break;
                case PlyType.Uchar:
                    bytes = BitConverter.GetBytes(Convert.ToByte(val));
                    break;
                case PlyType.Short:
                    bytes = BitConverter.GetBytes(Convert.ToInt16(val));
                    break;
                case PlyType.Ushort:
                    bytes = BitConverter.GetBytes(Convert.ToUInt16(val));
                    break;
                case PlyType.Int:
                    bytes = BitConverter.GetBytes(Convert.ToInt32(val));
                    break;
                case PlyType.Uint:
                    bytes = BitConverter.GetBytes(Convert.ToUInt32(val));
                    break;
                case PlyType.Float:
                    bytes = BitConverter.GetBytes(Convert.ToSingle(val));
                    break;
                case PlyType.Double:
                    bytes = BitConverter.GetBytes(Convert.ToDouble(val));
                    break;
                default:
                    throw new ArgumentException($"Cannot convert {nameof(PlyType)}. Given type is invalid.", nameof(dataType));
            }
            return bytes;
        }

        public static T ParseBytesToNative<T>(byte[] value, PlyType dataType)
        {
            object parsed = null;
            switch (dataType)
            {
                case PlyType.Char:
                    parsed = BitConverter.ToChar(value, 0);
                    break;
                case PlyType.Uchar:
                    parsed = value[0];
                    break;
                case PlyType.Short:
                    parsed = BitConverter.ToInt16(value, 0);
                    break;
                case PlyType.Ushort:
                    parsed = BitConverter.ToUInt16(value, 0);
                    break;
                case PlyType.Int:
                    parsed = BitConverter.ToInt32(value, 0);
                    break;
                case PlyType.Uint:
                    parsed = BitConverter.ToUInt32(value, 0);
                    break;
                case PlyType.Float:
                    parsed = BitConverter.ToSingle(value, 0);
                    break;
                case PlyType.Double:
                    parsed = BitConverter.ToDouble(value, 0);
                    break;
                default:
                    throw new ArgumentException("Invalid data type", nameof(dataType));
            }
            return (T)parsed;
        }

        public static T ParseStringToNativeType<T>(string value, PlyType type)
        {
            object parsed;
            switch (type)
            {
                case PlyType.Float:
                    parsed = float.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case PlyType.Int:
                    parsed = int.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case PlyType.Char:
                    parsed = sbyte.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case PlyType.Uchar:
                    parsed = byte.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case PlyType.Short:
                    parsed = short.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case PlyType.Ushort:
                    parsed = ushort.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case PlyType.Uint:
                    parsed = uint.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case PlyType.Double:
                    parsed = double.Parse(value, CultureInfo.InvariantCulture);
                    break;
                default:
                    throw new PlyConverterException($"Failed to parse value {value} to native type.");
            }
            return (T)parsed;
        }

        public static int ArraySizeInBytesToInt(byte[] array)
        {
            int count;
            if(array.Length == 1)
                count = array[0];
            else if(array.Length == 2)
                count = BitConverter.ToInt16(array, 0);
            else if(array.Length == 4)
                count = BitConverter.ToInt32(array, 0);
            else
                throw new ArgumentException("The given array is too big.", nameof(array));
                
            return count;
        }

        public static int GetTypeSize(PlyType type)
        {   
            int s;
            switch (type)
            {
                case PlyType.Char:
                case PlyType.Uchar:
                    s = 1;
                    break;

                case PlyType.Short:
                case PlyType.Ushort:
                    s = 2;
                    break;
                case PlyType.Int:
                case PlyType.Uint:
                case PlyType.Float:
                    s = 4;
                    break;
                case PlyType.Double:
                    s = 8;
                    break;
                default:
                    throw new PlyConverterException($"Cannot get size of the {nameof(PlyType)}. Given type is not known.");
            }
            return s;
        }

        public static PlyType GetNativeType(string typeName)
        {
            if (typeName == null)
                throw new ArgumentNullException($"Cannot get native type. {nameof(typeName)} is null.", nameof(typeName));

            PlyType t;

            switch (typeName)
            {
                case "char":
                case "int8":
                    t = PlyType.Char;
                    break;
                case "uchar":
                case "uint8":
                    t = PlyType.Uchar;
                    break;
                case "short":
                case "int16":
                    t = PlyType.Short;
                    break;
                case "ushort":
                case "uint16":
                    t = PlyType.Ushort;
                    break;
                case "int":
                case "int32":
                    t = PlyType.Int;
                    break;
                case "uint":
                case "uint32":
                    t = PlyType.Uint;
                    break;
                case "float":
                case "float32":
                    t = PlyType.Float;
                    break;
                case "double":
                case "float64":
                    t = PlyType.Double;
                    break;
                default:
                    throw new PlyConverterException($"Cannot get native type. {nameof(typeName)} is invalid.");
            }
            return t;
        }

        public static string ValueToString<T>(T value) where T : IConvertible, IFormattable
        {
            // avoid scientific notation
            const string doubleFixedPoint = "0.####################################################" +
            "######################################################################################" +
            "######################################################################################" +
            "######################################################################################" +
            "#############################";
            return value.ToString(doubleFixedPoint, CultureInfo.InvariantCulture.NumberFormat);
        }
    }
}
