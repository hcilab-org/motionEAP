// <copyright file=ScenePolygon.cs
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

using HciLab.Utilities.Mathematics.Core;
using HciLab.Utilities.Mathematics.Geometry2D;
using motionEAPAdmin.GUI.TypeEditor;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace motionEAPAdmin.Scene
{
    [Serializable()]
    public class ScenePolygon : SceneColor, ISerializable
    {
        private int m_SerVersion = 1;
        
        private Polygon m_Polygon = null;
        
        public ScenePolygon(Polygon pPolygon, Color pColor, double rotation = 0, double rotationCenterX = 0, double rotationCenterY = 0, double scale = 1.0f)
            : base(0, 0, 1000, 1000, pColor, rotation, rotationCenterX, rotationCenterY, scale)
        {
            m_Polygon = pPolygon;
            m_Polygon.PropertyChanged += m_Polygon_PropertyChanged;
            reconstrctDrawable();
        }

        public ScenePolygon()
            : base()
        {
        }

        protected ScenePolygon(SerializationInfo pInfo, StreamingContext context)
            : base(pInfo, context)
        {
            int pSerVersion = pInfo.GetInt32("m_SerVersion");

            if (pSerVersion < 1)
                return;

            m_Polygon = (Polygon)pInfo.GetValue("m_Polygon", typeof(Polygon));


            m_Polygon.PropertyChanged += m_Polygon_PropertyChanged;
            reconstrctDrawable();
        }

        public new void GetObjectData(SerializationInfo pInfo, StreamingContext pContext)
        {
            base.GetObjectData(pInfo, pContext);
            
            pInfo.AddValue("m_SerVersion", m_SerVersion);

            pInfo.AddValue("m_Polygon", m_Polygon);
        }

        protected override void reconstrctDrawable()
        {
            this.Visual3DModel = HciLab.Utilities.Mash3D.PolygonMash3D.newPolygon3DOWN(m_Polygon, Color, Z);
            
            isFlashing();
        }


        public override string Name
        {
            get
            {
                return "Polygon " + this.Id;
            }
        }

        /// <summary>
        /// Move of a Oject 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        internal new void Move(double pDeltaX, double pDeltaY)
        {
            foreach(Vector2 v in m_Polygon.Points)
            { 
                v.X += pDeltaX;
                v.Y += pDeltaY;
            }
            NotifyPropertyChanged("Move");
        }

        [Category("Position")]
        [DisplayName("Polygon Points")]
        [Description("The polygon points")]
        [EditorAttribute(typeof(PolygonTypeEditor), typeof(PolygonTypeEditor))]
        public Polygon Polygon
        {
            get
            {
                return m_Polygon;
            }
            set
            {
                m_Polygon.PropertyChanged -= m_Polygon_PropertyChanged;
                m_Polygon = value;
                m_Polygon.PropertyChanged += m_Polygon_PropertyChanged;
                NotifyPropertyChanged();
            }
        }

        void m_Polygon_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged("Polygon");
        }
    }
}



