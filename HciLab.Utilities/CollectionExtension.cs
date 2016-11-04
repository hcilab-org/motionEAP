// <copyright file=CollectionExtension.cs
// <copyright>
//  Copyright (c) 2016, University of Stuttgart
//  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the Software),
//  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
//  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//  THE SOFTWARE IS PROVIDED AS IS, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
//  DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
//  OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// <license>MIT License</license>
// <main contributors>
//  Markus Funk, Thomas Kosch, Sven Mayer
// </main contributors>
// <co-contributors>
//  Paul Brombosch, Mai El-Komy, Juana Heusler, 
//  Matthias Hoppe, Robert Konrad, Alexander Martin
// </co-contributors>
// <patent information>
//  We are aware that this software implements patterns and ideas,
//  which might be protected by patents in your country.
//  Example patents in Germany are:
//      Patent reference number: DE 103 20 557.8
//      Patent reference number: DE 10 2013 220 107.9
//  Please make sure when using this software not to violate any existing patents in your country.
// </patent information>
// <date> 11/2/2016 12:25:57 PM</date>

using System;
using System.Collections.ObjectModel;

namespace HciLab.Utilities
{
    public static class CollectionExtension
    {

        /// <summary>
        /// Retrieves index of the (first) object with given Id
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if no object with the given Id was found</exception>
        /// <returns>Index value of the (first) object with given Id.</returns>
        public static int GetIndexByObjId<T>(this Collection<T> collection, int pId) where T : DataBaseClass
        {
            for (int i = 0; i < collection.Count; i++)
            {
                if (collection[i].Id == pId)
                {
                    return i;
                }
            }

            throw new ArgumentException("No object with Id "+ pId+ " found", "Id");
        }

        /// <summary>
        /// Retrieves the (first) object with given Id
        /// </summary>
        /// <returns>The found object.</returns>
        public static T GetByObjId<T>(this Collection<T> collection, int pId) where T : DataBaseClass
        {
            int index = GetIndexByObjId(collection, pId);
            return collection[index];
        }

        public static void ReplaceOrAdd<T>(this Collection<T> collection, T pObject) where T : DataBaseClass
        {
            try
            {
                int index = GetIndexByObjId(collection, pObject.Id);
                collection[index] = pObject;
            }
            catch (ArgumentException) {
                collection.Add(pObject);
            }
        }

        public static void RemoveObjectById<T>(this Collection<T> collection, int pId) where T : DataBaseClass
        {
            int index = GetIndexByObjId(collection, pId);
            collection.RemoveAt(index);
        }

    }
}
