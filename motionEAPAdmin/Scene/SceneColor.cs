// <copyright file=SceneColor.cs
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace motionEAPAdmin.Scene
{
    public abstract class SceneColor : SceneItem, ISerializable
    {
        private int m_SerVersionSceneColor = 1;

        private System.Windows.Media.Color m_Color;
        private System.Windows.Media.Color m_ColorStart = Color.FromRgb(0, 0, 0);
        private System.Windows.Media.Color m_ColorEnd = Color.FromRgb(255, 255, 255);
        
        protected bool m_IsColorPulsing = false;

        private double m_PluseTime = 10000;

        #region No Serializableization

        private System.Timers.Timer m_PluseTimer = new System.Timers.Timer();
        private readonly double m_PluseTimerUpdate = 100;

        private List<Color> m_ColorGradient = new List<Color>();
        private int m_ColorGradientIterator = 0;

        #endregion

        public SceneColor()
            : base()
        {
            init();
        }

        protected SceneColor(SerializationInfo pInfo, StreamingContext context)
            : base(pInfo, context)
        {

            m_SerVersionSceneColor = pInfo.GetInt32("m_SerVersionSceneColor");
            m_Color = (Color)pInfo.GetValue("m_Color", typeof(Color));
            m_ColorStart = (Color)pInfo.GetValue("m_ColorStart", typeof(Color));
            m_ColorEnd = (Color)pInfo.GetValue("m_ColorEnd", typeof(Color));

            m_IsColorPulsing = pInfo.GetBoolean("m_IsColorPulsing");
            m_PluseTime = pInfo.GetDouble("m_PluseTime");

            init();

         }

        public SceneColor(double pX, double pY, double pWidth, double pHeight, Color pColor, double pRotation = 0, double pRotationCenterX = 0, double pRotationCenterY = 0, double pScale = 1.0)
            : base(pX, pY, pWidth, pHeight, pRotation, pRotationCenterX, pRotationCenterY, pScale)
        {
            m_Color = pColor;
            m_ColorStart = pColor;

            init();
        }

        public new void GetObjectData(SerializationInfo pInfo, StreamingContext pContext)
        {
            base.GetObjectData(pInfo, pContext);
            pInfo.AddValue("m_SerVersionSceneColor", m_SerVersionSceneColor);
            pInfo.AddValue("m_Color", m_Color);
            pInfo.AddValue("m_ColorStart", m_ColorStart);
            pInfo.AddValue("m_ColorEnd", m_ColorEnd);
            pInfo.AddValue("m_IsColorPulsing", m_IsColorPulsing);
            pInfo.AddValue("m_PluseTime", m_PluseTime);
        }

        private void init()
        {
            initGradient();
            this.m_PluseTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEventPulse);
            this.m_PluseTimer.Start();
        }

        private void initGradient()
        {
            if (m_IsColorPulsing == false)
            {
                m_Color = m_ColorStart;
            }
            m_ColorGradient.Clear();
            m_ColorGradient.AddRange(CreateGradient(m_ColorEnd, m_ColorStart, (int)(m_PluseTime / m_PluseTimerUpdate)));
            m_ColorGradient.AddRange(CreateGradient(m_ColorStart, m_ColorEnd, (int)(m_PluseTime / m_PluseTimerUpdate)));
        }

        private void OnTimedEventPulse(object sender, System.Timers.ElapsedEventArgs e)
        {

            if (!m_IsColorPulsing)
                return;

            m_ColorGradientIterator++;

            if (m_ColorGradientIterator >= m_ColorGradient.Count)
                m_ColorGradientIterator = 0;

            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            {
                try
                {
                    m_Color = m_ColorGradient[m_ColorGradientIterator];
                }
                catch (Exception)
                {
                    m_Color = m_ColorStart;
                }
                NotifyPropertyChanged("Color");
            }));
        }
            
        static List<Color> CreateGradient(Color start, Color end, int steps)
        {
            List<Color> ret = new List<Color>();
            for (int i = 0; i < steps; i++)
            {
                ret.Add(Color.FromRgb
                (
                    (byte)(start.R + (i * (end.R - start.R) / steps)),
                    (byte)(start.G + (i * (end.G - start.G) / steps)),
                    (byte)(start.B + (i * (end.B - start.B) / steps))
                ));
            }
            return ret;
        }

        [Category("Representation")]
        [DisplayName("Is Color Pulsing")]
        [Description("If the scene color pulsing")]
        [Editor(typeof(bool), typeof(bool))]
        public bool IsColorPulsing
        {
            get { return m_IsColorPulsing; }
            set
            {
                m_IsColorPulsing = value;
                if (m_IsColorPulsing)
                    m_Color = m_ColorGradient[m_ColorGradientIterator];
                else
                    m_Color = m_ColorStart;
                NotifyPropertyChanged("IsColorPulsing");
            }
        }


        [Browsable(false)]
        public System.Windows.Media.Color Color
        {
            get {
                return m_Color;
            }
        }

        [Category("Representation")]
        [DisplayName("Color (Start)")]
        [Description("The color of the scene")]
        [Editor(typeof(Color), typeof(Color))]
        public System.Windows.Media.Color ColorStart
        {
            get { return m_ColorStart; }
            set
            {
                m_ColorStart = value;
                initGradient();
                NotifyPropertyChanged("ColorStart");
            }
        }

        [Category("Representation")]
        [DisplayName("Color End")]
        [Description("The color of the scene")]
        [Editor(typeof(Color), typeof(Color))]
        public System.Windows.Media.Color ColorEnd
        {
            get { return m_ColorEnd; }
            set
            {
                m_ColorEnd = value;
                initGradient();
                NotifyPropertyChanged("ColorEnd");
            }
        }


        [Category("Representation")]
        [DisplayName("Pulse Time")]
        [Description("The time of the color to pluse the scene")]
        [Editor(typeof(double), typeof(double))]
        public double PluseTime
        {
            get { return m_PluseTime; }
            set
            {
                m_PluseTime = value;
                initGradient();
                NotifyPropertyChanged("PluseTime");
            }
        }

    }    
}
