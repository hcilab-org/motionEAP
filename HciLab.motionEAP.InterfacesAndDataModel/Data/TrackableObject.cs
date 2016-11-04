// <copyright file=TrackableObject.cs
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
// <date> 11/2/2016 12:25:56 PM</date>

using Emgu.CV;
using Emgu.CV.Structure;
using System;

namespace HciLab.motionEAP.InterfacesAndDataModel.Data
{
    public class TrackableObject
    {
        /// <summary>
        /// id of the tracked object 
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// the ref images which are used to identify the object
        /// private List<Image> refImages = new List<Image>(); 
        /// </summary>
        public virtual string Image { get; set; }

        /// <summary>
        /// name of the tracked object 
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// name of the upper group 
        /// </summary>
        public virtual string Category { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string ImageFullPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Image<Gray, byte> EmguImage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual long LastSeenTimeStamp { get; set; }

        public virtual int LastSeenZoneId { get; set; }
    }
}
