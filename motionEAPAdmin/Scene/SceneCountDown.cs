// <copyright file=SceneCountDown.cs
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
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace motionEAPAdmin.Scene
{
    class SceneCountDown : SceneItem, ISerializable
    {
        private int m_SerVersion = 1;

        private string m_Label;
        private FontFamily m_FontFamily;
        private double m_FontSize;
        private System.Windows.Media.Color m_Color;

        private int m_StartTimeLeft = 0;
        private int m_OldStartTimeLeft = 0;
        private int m_TimeLeft = 0;

        private System.Timers.Timer m_TimerCountDown = new System.Timers.Timer();
        private int updateMillis = 10;

        private DateTime m_LastTriggerTime;

        private ImageSource m_ImageSource = null;

        public SceneCountDown(double x, double y, string pLabel, System.Windows.Media.Color pColor, double pFontSize, FontFamily pFontFamily, double rotation = 0, double rotationCenterX = 0, double rotationCenterY = 0, double scale = 1.0f)
            : base(x, y, 0.1, 0.1, rotation, rotationCenterX, rotationCenterY, scale)
        {
            m_Label = pLabel;
            m_FontFamily = pFontFamily;
            m_Color = pColor;
            m_FontSize = pFontSize;

            m_TimerCountDown.Interval = updateMillis;

            this.m_TimerCountDown.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
            m_LastTriggerTime = DateTime.Now;
            this.m_TimerCountDown.Start();

            reconstrctDrawable();
        }

        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {

            if (m_OldStartTimeLeft != m_StartTimeLeft)
            {
                m_TimeLeft = m_StartTimeLeft;
                m_OldStartTimeLeft = m_StartTimeLeft;
            }

            var delta = (DateTime.Now - m_LastTriggerTime).TotalMilliseconds;

            Dispatcher.Invoke(
               System.Windows.Threading.DispatcherPriority.Normal,
               new Action(
                   () =>
                   {
                       this.TimeLeft = this.TimeLeft - (int)delta;
                   }
               )
            );
            m_LastTriggerTime = DateTime.Now;
        }


        public SceneCountDown()
            : base()
        {
        }

        protected SceneCountDown(SerializationInfo pInfo, StreamingContext context)
            : base(pInfo, context)
        {
            int pSerVersion = pInfo.GetInt32("m_SerVersion");

            if (pSerVersion < 1)
                return;

            m_Label = pInfo.GetString("m_Filename");

            m_ImageSource = new BitmapImage(new Uri(m_Label));

            reconstrctDrawable();
        }

        public new void GetObjectData(SerializationInfo pInfo, StreamingContext pContext)
        {
            base.GetObjectData(pInfo, pContext);

            pInfo.AddValue("m_SerVersion", m_SerVersion);

            pInfo.AddValue("m_Filename", m_Label);
        }

        protected override void reconstrctDrawable()
        {
            var countDownText = Label + (TimeLeft / 1000) + "s";
            this.Visual3DModel = HciLab.Utilities.Mash3D.Text3DGeo.CreateTextLabel(X, Y, countDownText, m_Color, m_FontSize, m_FontFamily);
            isFlashing();
        }

        public override string Name
        {
            get
            {
                return "CountDown " + this.Id;
            }
        }

        public string Label
        {
            get { return m_Label; }
            set
            {
                m_Label = value;
                NotifyPropertyChanged("Label");
            }
        }

        public int StartTimeLeft
        {
            get { return m_StartTimeLeft; }
            set
            {
                m_StartTimeLeft = value;
            }
        }

        public int TimeLeft
        {
            get { return m_TimeLeft; }
            set 
            {
                m_TimeLeft = value;
                NotifyPropertyChanged("TimeLeft");
            }
        }


    }
}
