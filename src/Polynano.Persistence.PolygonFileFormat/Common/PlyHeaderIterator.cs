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
using System;

namespace Polynano.Persistence.PolygonFileFormat.Common
{
    /// <summary>
    /// Helper class for enumerating the PLY File Structure.
    /// The PLY Format is not strictly defined,
    /// therefore the Reader and the Writer must 
    /// keep track of what properties have already been read and
    /// how many elements have been written
    /// to determine the values that are coming next. 
    /// </summary>
    public class PlyHeaderIterator
    {
        private int _currentElementInstance;

        private int _currentElementPropertyInstance;

        public PlyHeader Header { get; }

        public bool IsOnFirstProperty 
        {
            get => _currentElementPropertyInstance == 0;
        }

        public bool IsOnFirstElement 
        {
            get => _currentElementPropertyInstance == 0 && CurrentElement == 0 && _currentElementInstance == 0;
        }

        public bool IsIterationDone 
        {
            get => CurrentElement >= Header.Elements.Count;
        }

        public int CurrentElement { get; private set; }

        public PlyHeaderIterator(PlyHeader header)
        {
            Header = header;
        }

        public void MoveNext()
        {
            EnsureNotOutOufBounds();

            _currentElementPropertyInstance++;
            if (_currentElementPropertyInstance >= Header.Elements[CurrentElement].Properties.Count)
            {
                _currentElementPropertyInstance = 0;
                _currentElementInstance++;
            }
            
            if (_currentElementInstance >= Header.Elements[CurrentElement].InstanceCount)
            {
                _currentElementInstance = 0;
                _currentElementPropertyInstance = 0;

                CurrentElement++;
            }
        }

        public void JumpToEnd()
        {
            CurrentElement = Header.Elements.Count;
        }

        public PlyProperty CurrentProperty
        {
            get
            {
                EnsureNotOutOufBounds();
                return Header.Elements[CurrentElement].Properties[_currentElementPropertyInstance];
            }
        }

        private void EnsureNotOutOufBounds()
        {
            if (CurrentElement >= Header.Elements.Count)
            {
                throw new InvalidOperationException("Iteration is done.");
            }
        }
    }
}