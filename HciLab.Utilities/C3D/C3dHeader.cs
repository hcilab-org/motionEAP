// <copyright file=C3dHeader.cs
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

namespace HciLab.Utilities.C3D
{
    /// <summary>
    /// Class representing C3D file header and exposing information as properties
    /// </summary>
    public class C3dHeader
    {
        private byte [] _data;
        
        internal C3dHeader()
        {
            _data = new byte[512];
            FirstWord = 0x5002;
            NumberOfPoints = 21;
            FirstSampleNumber = 1;
            LastSampleNumber = 1;
            FrameRate = 30;
            AnalogSamplesPerFrame = 0;
            AnalogChannels = 0;
            ScaleFactor = -1f;
            Support4CharEventLabels = true;
        }

        public Int16 FirstWord { get { return BitConverter.ToInt16(_data, 0); }                 set { Array.Copy(BitConverter.GetBytes(value), 0, _data,0, sizeof(Int16)); } }
        public byte FirstParameterBlock { get { return _data[0]; }                              set { _data[0] = value; } }
        public Int16 NumberOfPoints { get { return BitConverter.ToInt16(_data, 2); }            set { Array.Copy(BitConverter.GetBytes(value), 0, _data, 2, sizeof(Int16)); } }
        public Int16 AnalogChannels { get { return BitConverter.ToInt16(_data, 4); }            set { Array.Copy(BitConverter.GetBytes(value), 0, _data, 4, sizeof(Int16)); } }

        public Int16 FirstSampleNumber { get { return BitConverter.ToInt16(_data, 6); }         set { Array.Copy(BitConverter.GetBytes(value), 0, _data, 6, sizeof(Int16)); }  }
        public Int16 LastSampleNumber     { get { return BitConverter.ToInt16(_data, 8); }      set { Array.Copy(BitConverter.GetBytes(value), 0, _data, 8, sizeof(Int16)); } }
        public Int16 MaxInterpolationGaps { get { return BitConverter.ToInt16(_data, 10); }     set { Array.Copy(BitConverter.GetBytes(value), 0, _data, 10, sizeof(Int16)); } }
        public float ScaleFactor { get { return BitConverter.ToSingle(_data, 12); }             set { Array.Copy(BitConverter.GetBytes(value), 0, _data,12, sizeof(float)); } }
        public Int16 DataStart { get { return BitConverter.ToInt16(_data, 16); }                set { Array.Copy(BitConverter.GetBytes(value), 0, _data, 16, sizeof(Int16)); } }
        public Int16 AnalogSamplesPerFrame { get { return BitConverter.ToInt16(_data, 18); }    set { Array.Copy(BitConverter.GetBytes(value), 0, _data, 18, sizeof(Int16)); } }
        public float FrameRate { get { return BitConverter.ToSingle(_data, 20); }               set { Array.Copy(BitConverter.GetBytes(value), 0, _data, 20, sizeof(float)); } }

        public bool Support4CharEventLabels { get { return BitConverter.ToInt16(_data, 149*2) == 12345; } set { Array.Copy(BitConverter.GetBytes(value == true? 12345:0), 0, _data, 149*2, sizeof(Int16)); } }

        internal void SetHeader(byte[] headerData)
        {
            Array.Copy(headerData, _data, 512);
        }

        internal byte [] GetRawData() 
        {
            return _data;
        }

    }
}
