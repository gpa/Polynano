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
using System.Collections.Generic;

namespace Polynano.Persistence.PolygonFileFormat.Extensions
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Find the index of a given element defined by a predicate in a list
        /// </summary>
        /// <param name="items">the list or array</param>
        /// <param name="predicate">the element to find defined as a lambda</param>
        /// <returns>the index of the element or -1 if the element was not found</returns>
        public static int FindIndex<T>(this IList<T> items, Func<T, bool> predicate)
        {
            var index = 0;
            foreach (var item in items)
            {
                if (predicate(item))
                    return index;
                index++;
            }
            
            return -1;
        }
    }
}
