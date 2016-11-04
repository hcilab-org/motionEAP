// <copyright file=SceneImage.cs
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
// <date> 11/2/2016 12:26:00 PM</date>

using HciLab.Utilities.Mash3D;
using motionEAPAdmin.GUI.TypeEditor;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace motionEAPAdmin.Scene
{
    [Serializable()]
    class SceneImage : SceneItem, ISerializable
    {
        private int m_SerVersion = 1;

        private string m_Filename;

        private ImageSource m_ImageSource = null;

        public SceneImage(double x, double y, double w, double h, string pFilename, double rotation = 0, double rotationCenterX = 0, double rotationCenterY = 0, double scale = 1.0f)
            : base(x, y, w, h, rotation, rotationCenterX, rotationCenterY, scale)
        {
            this.m_Filename = pFilename;

            if (pFilename == null || pFilename.Equals(String.Empty))
            {
                m_ImageSource  = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                 Properties.Resources.placeholder.GetHbitmap(), 
                   IntPtr.Zero, 
                   System.Windows.Int32Rect.Empty,
                   BitmapSizeOptions.FromWidthAndHeight(Properties.Resources.placeholder.Width, Properties.Resources.placeholder.Height));
            }
            else
                m_ImageSource = new BitmapImage(new Uri(m_Filename));
                    
            Width = Properties.Resources.placeholder.Width;
            Height = Properties.Resources.placeholder.Height;

            reconstrctDrawable();
        }

        
        public SceneImage()
            : base()
        {
        }

        protected SceneImage(SerializationInfo pInfo, StreamingContext context)
            : base(pInfo, context)
        {
            int pSerVersion = pInfo.GetInt32("m_SerVersion");

            if (pSerVersion < 1)
                return;

            m_Filename = pInfo.GetString("m_Filename");

            m_ImageSource = new BitmapImage(new Uri(m_Filename));

            reconstrctDrawable();
        }

        public new void GetObjectData(SerializationInfo pInfo, StreamingContext pContext)
        {
            base.GetObjectData(pInfo, pContext);

            pInfo.AddValue("m_SerVersion", m_SerVersion);

            pInfo.AddValue("m_Filename", m_Filename);
        }

        protected override void reconstrctDrawable()
        {
            this.Visual3DModel = Image3DGeo.Image(X, Y, Width, Height, m_ImageSource, 0);
            isFlashing();
        }


        public override string Name
        {
            get
            {
                return "Image " + this.Id;
            }
        }

        [Category("Source")]
        [DisplayName("File Path")]
        [Description("The path to the image file")]
        [EditorAttribute(typeof(ImageBrowseTypeEditor), typeof(ImageBrowseTypeEditor))]
        public String FileName
        {
            get { return m_Filename; }
            set { 
                    m_Filename = value;
                    try
                    {
                        m_ImageSource = new BitmapImage(new Uri(m_Filename));
                    }
                    catch
                    {
                        m_ImageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                           Properties.Resources.placeholder.GetHbitmap(),
                             IntPtr.Zero,
                             System.Windows.Int32Rect.Empty,
                             BitmapSizeOptions.FromWidthAndHeight(Properties.Resources.placeholder.Width, Properties.Resources.placeholder.Height));     
                    }

                    NotifyPropertyChanged("FileName");
                }
        }
    }
}
