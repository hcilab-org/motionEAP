// <copyright file=SceneCircle.cs
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
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace motionEAPAdmin.Scene
{
    [Serializable()]
    public class SceneCircle : SceneColor, ISerializable
    {
        private int m_SerVersion = 1;
        
        private Double m_Radius, m_StartAngel, m_EndAngel;

        public SceneCircle(double pX, double pY, double pRadius, double startAngel, double endAngel, Color pColor)
            : base(pX, pY, pRadius * 2, pRadius * 2, pColor)
        {
            this.m_Radius = pRadius;
            this.m_StartAngel = startAngel;
            this.m_EndAngel = endAngel;

            reconstrctDrawable();
        }


        public SceneCircle()
            : base()
        {
        }

        protected SceneCircle(SerializationInfo pInfo, StreamingContext context)
            : base(pInfo, context)
        {
            int pSerVersion = pInfo.GetInt32("m_SerVersion");

            if (pSerVersion < 1)
                return;

            m_Radius = pInfo.GetDouble("m_Radius");
            m_StartAngel = pInfo.GetDouble("m_StartAngel");
            m_EndAngel = pInfo.GetDouble("m_EndAngel");

            reconstrctDrawable();
        }

        public new void GetObjectData(SerializationInfo pInfo, StreamingContext pContext)
        {
            base.GetObjectData(pInfo, pContext);

            pInfo.AddValue("m_SerVersion", m_SerVersion);

            pInfo.AddValue("m_Radius", m_Radius);
            pInfo.AddValue("m_StartAngel", m_StartAngel);
            pInfo.AddValue("m_EndAngel", m_EndAngel);
        }

        protected override void reconstrctDrawable()
        {
            m_Radius = Width / 2;
            if (Height != Width)
                Height = Width;

            this.Visual3DModel = Circle3DGeo.GetCircleModel(X, Y, m_Radius, Color, Z);
            isFlashing();
        }


        public override string Name
        {
            get
            {
                return "Circle " + this.Id;
            }
        }

        [Browsable(false)]
        public new double Width { get { return base.Width; } set { base.Width = value; } }


        [Browsable(false)]
        public new double Height { get { return base.Height; } set { base.Height = value; } }

        #region getter / setter

        [Category("Layout")]
        [DisplayName("Radius")]
        [Description("The radius of the circle")]
        [Editor(typeof(Double), typeof(Double))]
        public Double Radius
        {
            get
            {
                return m_Radius;
            }

            set
            {
                m_Radius = value;
                Width = m_Radius * 2;
                Height = m_Radius * 2;
            }
        }
        #endregion
    }
}
