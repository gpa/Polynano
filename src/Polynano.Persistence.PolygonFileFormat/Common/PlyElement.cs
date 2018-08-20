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
using Polynano.Persistence.PolygonFileFormat.Extensions;
using System;
using System.Collections.Generic;

namespace Polynano.Persistence.PolygonFileFormat.Common
{
    public class PlyElement
    {
        private string _name;
        public string Name 
        { 
            get => _name;
            set 
            {
                if (value == null)
                    throw new ArgumentNullException("Name must not be null.", nameof(Name));

                if (!value.IsToken())
                    throw new ArgumentException("Header element name must be a token.", nameof(Name));
                
                _name = value;
            }
        }

        private IList<PlyProperty> _properties = new List<PlyProperty>();
        public IList<PlyProperty> Properties 
        {
            get => _properties;
            set 
            {
                if (value == null)
                    throw new ArgumentNullException("Property list may not be null.", nameof(Properties));
                
                if(value.Count == 0)
                    throw new ArgumentException("property list must define at least one element.");

                _properties = value;
            }
        }

        private int _instanceCount;
        public int InstanceCount
        {
            get => _instanceCount;
            set 
            {
                if (value < 0)
                    throw new ArgumentException($"{nameof(InstanceCount)} must not be negative.", nameof(InstanceCount));

                if (value == 0)
                    throw new ArgumentException("Element must define at least one instance.");

                _instanceCount = value;
            }
        }
        
        public PlyElement(string name, int count, IList<PlyProperty> properties)
        {
            Name = name;
            InstanceCount = count;
            Properties = properties;
        }
    }
}