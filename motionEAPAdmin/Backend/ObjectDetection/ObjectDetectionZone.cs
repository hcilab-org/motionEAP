// <copyright file=ObjectDetectionZone.cs
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

using motionEAPAdmin.Scene;
using System;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace motionEAPAdmin.Backend.ObjectDetection
{
    [Serializable()]
    public class ObjectDetectionZone : WorkflowBase, ISerializable
    {
        private int m_SerVersion = 1;

        private int m_Depth; // something

        private double m_DepthMean;

        private long m_LastTriggeredTimestamp = 0;

        public ObjectDetectionZone()
            : base()
        {
            m_SceneItem = new SceneRect();
            updateScene();
        }

        public ObjectDetectionZone(int pId)
            : base(pId)
        {
            m_SceneItem = new SceneRect();
            updateScene();
        }

        protected ObjectDetectionZone(SerializationInfo pInfo, StreamingContext pContext)
            : base(pInfo, pContext)
        {
            // for version evaluation while deserializing
            int pSerVersion = pInfo.GetInt32("m_SerVersion");
            m_Depth = pInfo.GetInt32("m_Depth");
            m_DepthMean = pInfo.GetDouble("m_DepthMean");
            m_SceneItem = new SceneRect();
            updateScene();
        }


        public new void GetObjectData(SerializationInfo pInfo, StreamingContext pContext)
        {
            base.GetObjectData(pInfo, pContext);
            pInfo.AddValue("m_SerVersion", m_SerVersion);
            pInfo.AddValue("m_Depth", m_Depth);
            pInfo.AddValue("m_DepthMean", m_DepthMean);
        }

        public void Trigger()
        {
            if (!wasRecentlyTriggered())
            {
                m_LastTriggeredTimestamp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                OnTrigger(this);
            }
        }

        public bool wasRecentlyTriggered()
        {
            bool ret = false;
            int TriggeredThreshold = 1000;
            long currentMillis = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            if(currentMillis - m_LastTriggeredTimestamp < TriggeredThreshold)
            {
                ret = true;
            }

            return ret;
        }

        public override Scene.SceneItem getDrawable(bool isUsedForRecord = false)
        {
            System.Windows.Media.Color color = Color.FromRgb(255, 255, 0);

            if (wasRecentlyTriggered() && !isUsedForRecord)
            {
                color = Color.FromRgb(0, 255, 0);
            }

            if (m_SceneItem is SceneRect)
                (m_SceneItem as SceneRect).ColorStart = color;
            else
                throw new NotSupportedException();

            return m_SceneItem;
        }

        #region Getter / Setter
        public int Depth
        {
            get
            {
                return m_Depth;
            }
            set
            {
                m_Depth = value;
                NotifyPropertyChanged("Depth");
            }
        }

        public double DepthMean
        {
            get
            {
                return m_DepthMean;
            }
            set
            {
                m_DepthMean = value;
                NotifyPropertyChanged("DepthMean");
            }
        }
        #endregion
    }
}
