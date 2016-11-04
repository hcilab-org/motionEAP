// <copyright file=BlobObject.cs
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
using System.ComponentModel;
using System.Drawing;

namespace HciLab.motionEAP.InterfacesAndDataModel.Data
{
    public class BlobObject : INotifyPropertyChanged
    {
        /// <summary>
        /// property changed for the databinding
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        private PointF[] m_CornerPoints;

        private RectangleF m_Rect;

        private PointF m_Center;

        private Image<Gray, byte> m_Image;

        private int m_Hits;

        private int m_Id;

        private string m_Name;

        private int[, ,] m_DepthStructur;


        /// <summary>
        /// The constructor contains all the information of detected objects.
        /// </summary>
        /// <param name="pImage">image</param>
        /// <param name="pCornerPoints">cornerPoints</param>
        /// <param name="pRect">pos rect</param>
        /// <param name="pCenter">center</param>
        /// <param name="pHit">width</param>
        /// <param name="pId">height</param>
        public BlobObject(Image<Gray, byte> pImage, int[, ,] pDepthStructur, PointF[] pCornerPoints, RectangleF pRect, PointF pCenter, int pHit, int pId, string pNname)
        {
            m_CornerPoints = pCornerPoints;
            m_Center = pCenter;
            m_Rect = pRect;
            m_Image = pImage;
            m_Id = pId;
            m_Name = pNname;
            m_Hits = pHit;
            m_DepthStructur = pDepthStructur;
        }

        protected void NotifyPropertyChanged(string Obj)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(Obj));
            }
        }


        public PointF[] CornerPoints
        {
            get
            {
                return m_CornerPoints;
            }
            set
            {
                m_CornerPoints = value;
                NotifyPropertyChanged("CornerPoints");
            }
        }
        public RectangleF Rect
        {
            get
            {
                return m_Rect;
            }
            set
            {
                m_Rect = value;
                NotifyPropertyChanged("Rect");
            }
        }
        public PointF Center
        {
            get
            {
                return m_Center;
            }
            set
            {
                m_Center = value;
                NotifyPropertyChanged("Center");
            }
        }
        public Image<Gray, byte> Image
        {
            get
            {
                return m_Image;
            }
            set
            {
                m_Image = value;
                NotifyPropertyChanged("Image");
            }
        }

        public int Hits
        {
            get
            {
                return m_Hits;
            }
            set
            {
                m_Hits = value;
                NotifyPropertyChanged("Hits");
            }
        }
        public int Id
        {
            get
            {
                return m_Id;
            }
            set
            {
                m_Id = value;
                NotifyPropertyChanged("Id");
            }
        }
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
                NotifyPropertyChanged("Name");
            }
        }
        public int[, ,] DepthStructur
        {
            get
            {
                return m_DepthStructur;
            }
            set
            {
                m_DepthStructur = value;
                NotifyPropertyChanged("DepthStructur");
            }
        }
    }
}
