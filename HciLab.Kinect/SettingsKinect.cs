// <copyright file=SettingsKinect.cs
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

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace HciLab.motionEAP.InterfacesAndDataModel.Data
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    [Serializable()]
    public class SettingsKinect : ISerializable, INotifyPropertyChanged
    {
        // Version
        private int m_SerVersion = 1;

        private int m_XScale = 0;
        private int m_YScale = 0;
        private Double m_Ratio = 0;

        public SettingsKinect() { }

        public SettingsKinect(int pXScale, int pYScale, Double pRatio)
        {
            XScale = pXScale;
            YScale = pYScale;
            Ratio = pRatio;
        }

        protected SettingsKinect(SerializationInfo info, StreamingContext context)
        {
            int SerVersion = info.GetInt32("m_SerVersion");
            // Check version of XML version
            if (SerVersion < 1)
            {
                return;
            }
            m_XScale = info.GetInt32("m_XScale");
            m_YScale = info.GetInt32("m_YScale");
            m_Ratio = info.GetDouble("m_Ratio");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("m_SerVersion", m_SerVersion);

            info.AddValue("m_XScale", m_XScale);
            info.AddValue("m_YScale", m_YScale);
            info.AddValue("m_Ratio", m_Ratio);
        }

        public int XScale
        {
            get { return m_XScale; }
            set {
                m_XScale = value;
                NotifyPropertyChanged("XScale");
            }
        }

        public int YScale
        {
            get { return m_YScale; }
            set {
                m_YScale = value;
                NotifyPropertyChanged("m_YScale");
            }
        }

        public Double Ratio
        {
            get { return m_Ratio; }
            set {
                m_Ratio = value;
                NotifyPropertyChanged("Ratio");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
