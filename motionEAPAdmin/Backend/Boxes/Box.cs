// <copyright file=Box.cs
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
// <date> 11/2/2016 12:25:58 PM</date>

using motionEAPAdmin.ContentProviders;
using motionEAPAdmin.Network;
using motionEAPAdmin.Scene;
using System;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace motionEAPAdmin.Backend.Boxes
{
    [Serializable()]
    public class Box : WorkflowBase, ISerializable
    {
        // Standard value for the timeout
        public static int TriggeredThreshold = 2500;

        protected static Color YELLOW = System.Windows.Media.Color.FromRgb(255, 255, 0);
        protected static Color GREEN = System.Windows.Media.Color.FromRgb(0, 255, 0);
        protected static Color RED = System.Windows.Media.Color.FromRgb(255, 0, 0);
        protected static Color BLACK = System.Windows.Media.Color.FromRgb(0, 0, 0);

        private int m_SerVersion = 3;
        private double m_DepthMean;

        private double m_MatchPercentageOffset = 0.0;
        private bool m_IsBoxErroneous = false;
        private int m_LowerThreshold;
        private int m_UpperThreshold;

        #region No Serializableization
        private long m_LastTriggeredTimestamp = 0;
        private long m_LastFalselyTriggeredTimestamp = 0;
        #endregion

        public Box()
            : base()
        {
            m_SceneItem = new SceneRect();

            updateScene();
        }

        public Box(int pId)
            : base(pId)
        {
            m_SceneItem = new SceneRect();
            updateScene();
        }

        protected Box(SerializationInfo pInfo, StreamingContext pContext)
            : base(pInfo, pContext)
        {
            // for version evaluation while deserializing
            int pSerVersion = pInfo.GetInt32("m_SerVersion");

            if (m_SerVersion >= 1)
            {
                m_DepthMean = pInfo.GetDouble("m_DepthMean");
                m_SceneItem = new SceneRect();
            }

            if (m_SerVersion >= 2)
            {
                m_MatchPercentageOffset = pInfo.GetDouble("m_MatchPercentageOffset");
            }

            if (m_SerVersion >= 3)
            {
                m_LowerThreshold = pInfo.GetInt32("m_LowerThreshold");
                m_UpperThreshold = pInfo.GetInt32("m_UpperThreshold");
            }

            updateScene();
        }
        
        public new void GetObjectData(SerializationInfo pInfo, StreamingContext pContext)
        {
            base.GetObjectData(pInfo, pContext);

            pInfo.AddValue("m_SerVersion", m_SerVersion);
            pInfo.AddValue("m_DepthMean", m_DepthMean);
            pInfo.AddValue("m_MatchPercentageOffset", m_MatchPercentageOffset);
            pInfo.AddValue("m_LowerThreshold", m_LowerThreshold);
            pInfo.AddValue("m_UpperThreshold", m_UpperThreshold);
        }

        public void Trigger()
        {
            if (!wasRecentlyTriggered())
            {
                m_LastTriggeredTimestamp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                OnTrigger(this);
            } 
            
            if (!wasRecentlyFalselyTriggered())
            {
                m_LastFalselyTriggeredTimestamp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            }

        }

        public bool wasRecentlyTriggered()
        {
            long currentMillis = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            if(currentMillis - m_LastTriggeredTimestamp < TriggeredThreshold)
                return true;
            return false;
        }

        public bool wasRecentlyFalselyTriggered()
        {
            long currentMillis = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            if (currentMillis - m_LastFalselyTriggeredTimestamp > TriggeredThreshold)
            {
                return false;
            }
            return true;
        }

        public Scene.SceneItem drawErrorFeedback(bool isUsedForRecord = false)
        {
            TriggeredThreshold = SettingsManager.Instance.Settings.BoxFeedbackTimeout;
            if (m_CustomScene == null)
            {
                System.Windows.Media.Color c = BLACK;
                if (wasRecentlyFalselyTriggered() && !isUsedForRecord)
                {
                    c = RED;
                    if (SettingsManager.Instance.Settings.UDPStreamingEnabled)
                    {
                        NetworkManager.Instance.SendDataOverUDP(SettingsManager.Instance.Settings.UDPIPTarget, 30000, "1");
                    }
                }
                else
                {
                    this.IsBoxErroneous = false;
                }

                if (m_SceneItem is SceneRect)
                    (m_SceneItem as SceneRect).ColorStart = c;
                else
                    throw new NotSupportedException();

                return m_SceneItem;
            }
            else
            {
                System.Windows.Media.Color c = BLACK;
                if (wasRecentlyFalselyTriggered() && !isUsedForRecord)
                {
                    c = RED;
                    if (SettingsManager.Instance.Settings.UDPStreamingEnabled)
                    {
                        NetworkManager.Instance.SendDataOverUDP(SettingsManager.Instance.Settings.UDPIPTarget, 30000, "1");
                    }
                }
                else
                {
                    this.IsBoxErroneous = false;
                }

                foreach (SceneItem item in m_CustomScene.Items)
                {
                    if (item is SceneColor)
                        (item as SceneColor).ColorStart = c;
                }

                return m_CustomScene;

            }
        }

        public override Scene.SceneItem getDrawable(bool isUsedForRecord = false)
        {
            TriggeredThreshold = SettingsManager.Instance.Settings.BoxFeedbackTimeout;
            if (m_CustomScene == null)
            {
                System.Windows.Media.Color c = GREEN;
                if (wasRecentlyTriggered() && !isUsedForRecord)
                {
                    c = YELLOW;
                }

                if (m_SceneItem is SceneRect)
                {
                    if ((m_SceneItem as SceneRect).ColorStart != c)
                        (m_SceneItem as SceneRect).ColorStart = c;
                }
                else
                    throw new NotSupportedException();

                return m_SceneItem;
            }
            else
            {

                System.Windows.Media.Color c = GREEN;
                if (wasRecentlyTriggered() && !isUsedForRecord)
                {
                    c = YELLOW;
                }

                foreach(SceneItem item in m_CustomScene.Items)
                {
                    if (item is SceneColor)
                        (item as SceneColor).ColorStart = c;
                }

                return m_CustomScene;

            }
        }

        #region Getter / Setter
        public double Depthmean
        {
            get
            {
                return m_DepthMean;
            }
            set
            {
                m_DepthMean = value;
                NotifyPropertyChanged("Depthmean");
            }
        }

        public double MatchPercentageOffset
        {
            get { return m_MatchPercentageOffset; }
            set
            {
                m_MatchPercentageOffset = value;
                NotifyPropertyChanged("MatchPercentageOffset");
                NotifyPropertyChanged("NameWithPercentage");
            }
        }


        public int LowerThreshold
        {

            get { return m_LowerThreshold; }
            set
            {
                m_LowerThreshold = value;
                NotifyPropertyChanged("LowerThreshold");
            }
        }


        public int UpperThreshold
        {
            get { return m_UpperThreshold; }
            set
            {
                m_UpperThreshold = value;
                NotifyPropertyChanged("UpperThreshold");
            }
        }


        public String NameWithPercentage
        {
            get
            {
                return Name + " (" + MatchPercentageOffset.ToString("+#;-#;+0") + "%)";
            }
        }

        public bool IsBoxErroneous
        {
            get { return m_IsBoxErroneous; }
            set { m_IsBoxErroneous = value; }
        }
        #endregion
    }
}
