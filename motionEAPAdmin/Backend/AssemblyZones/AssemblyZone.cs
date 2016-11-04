// <copyright file=AssemblyZone.cs
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
using motionEAPAdmin.Scene;
using System;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace motionEAPAdmin.Backend.AssembleyZones
{
    [Serializable()]
    public class AssemblyZone : WorkflowBase, ISerializable
    {
        public static readonly int TriggeredThreshold = 1000;

        public static readonly Color DISASSEMBLY_COLOR = System.Windows.Media.Color.FromRgb(255, 0, 0); // green
        public static readonly Color MATCH_COLOR = System.Windows.Media.Color.FromRgb(0, 255, 0); // green
        public static readonly Color STANDARD_COLOR = System.Windows.Media.Color.FromRgb(0, 255, 0); // green

        private int m_SerVersion = 2;

        /// <summary>
        /// Array of depth values to match against
        /// </summary>
        private int[,] m_DepthArray;

        /// <summary>
        /// Masks irrelevant Pixels for matching
        /// </summary>
        private bool[,] m_DepthMask;

        /// <summary>
        /// whether to use the depthmask for matching
        /// </summary>
        private bool m_UseDepthMask = false;

        /// <summary>
        /// Wether this Zone requires disassamblage (e.g. detected through removed depth data)
        /// </summary>
        private bool m_IsDisassemblyZone = false;

        private double m_MatchPercentageOffset = 0.0;

        
        /// <summary>
        /// Area of the Assemblyzone
        /// </summary>

        private int m_Area = 0;



        #region No Serializableization

            //used for development purposes
            private double m_LastPercentageMatched;

            /// <summary>
            /// True if assemblyZone endcondition is met
            /// </summary>
            private bool m_Matched = false;

            private long m_LastTriggeredTimestamp = 0;
        
        #endregion

        public AssemblyZone(int pId, double pMatchPercentageOffset = 0.0)
            : base(pId)
        {
            m_MatchPercentageOffset = pMatchPercentageOffset;
            updateScene();
        }
        
        protected AssemblyZone(SerializationInfo pInfo, StreamingContext pContext)
            : base(pInfo, pContext)
        {
            // for version evaluation while deserializing
            int pSerVersion = pInfo.GetInt32("m_SerVersion");

            if (pSerVersion >= 1) 
            { 
                m_Area = pInfo.GetInt32("m_Area");
                m_DepthArray = (int[,])pInfo.GetValue("m_DepthArray", typeof(int[,]));
                m_DepthMask = (bool[,])pInfo.GetValue("m_DepthMask", typeof(bool[,]));
                m_IsDisassemblyZone = pInfo.GetBoolean("m_IsDisassemblyZone");
                m_UseDepthMask = pInfo.GetBoolean("m_UseDepthMask");
            }

            if (pSerVersion >= 2)
            {
                m_MatchPercentageOffset = pInfo.GetDouble("m_MatchPercentageOffset");
            }

            updateScene();
        }


        public new void GetObjectData(SerializationInfo pInfo, StreamingContext pContext)
        {
            base.GetObjectData(pInfo, pContext);
            pInfo.AddValue("m_SerVersion", m_SerVersion);
            pInfo.AddValue("m_Area", m_Area);
            pInfo.AddValue("m_DepthArray", m_DepthArray);
            pInfo.AddValue("m_DepthMask", m_DepthMask);
            pInfo.AddValue("m_IsDisassemblyZone", m_IsDisassemblyZone);
            pInfo.AddValue("m_UseDepthMask", m_UseDepthMask);
            pInfo.AddValue("m_MatchPercentageOffset", m_MatchPercentageOffset);
        }

        public override Scene.SceneItem getDrawable(bool isUsedForRecord = false)
        {
            
            System.Windows.Media.Color c = STANDARD_COLOR;

            if (!m_IsDisassemblyZone)
            {
                c = DISASSEMBLY_COLOR;
            }
            if (m_Matched && !isUsedForRecord)
            {
                c = MATCH_COLOR;
            }


            if (m_CustomScene == null)
            {
                if (m_SceneItem == null)
                    return null;
                else if (m_SceneItem is SceneRect && (m_SceneItem as SceneRect).Color != c)
                    (m_SceneItem as SceneRect).ColorStart = c;
                else if (m_SceneItem is SceneCircle && (m_SceneItem as SceneCircle).Color != c)
                    (m_SceneItem as SceneCircle).ColorStart = c;
                else if (m_SceneItem is SceneText && (m_SceneItem as SceneText).Color != c)
                    (m_SceneItem as SceneText).ColorStart = c;
                else if (m_SceneItem is ScenePolygon && (m_SceneItem as ScenePolygon).Color != c)
                    (m_SceneItem as ScenePolygon).ColorStart = c;

                return m_SceneItem;
            }
            else
            {
                foreach (SceneItem item in m_CustomScene.Items)
                {
                    if (item is SceneRect)
                        (item as SceneRect).ColorStart = c;
                    else if (item is SceneCircle)
                        (item as SceneCircle).ColorStart = c;
                    else if (item is SceneText)
                        (item as SceneText).ColorStart = c;
                    else if (item is ScenePolygon)
                        (item as ScenePolygon).ColorStart = c;
                }

                return m_CustomScene;
            }
        }

        public void Trigger()
        {
            if (!wasRecentlyTriggered())
            {
                m_Matched = true;
                m_LastTriggeredTimestamp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                OnTrigger(this);
            }
        }

        public void TriggerNoMatch()
        {
            if (!wasRecentlyTriggered())
            {
                m_Matched = false;
            }
        }

        public bool wasRecentlyTriggered()
        {
            bool ret = false;
            long currentMillis = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            if (currentMillis - m_LastTriggeredTimestamp < TriggeredThreshold)
            {
                ret = true;
            }

            return ret;
        }
        
    
        #region Getter / Setter

        public int[,] DepthArray
        {
            get
            {
                return m_DepthArray;
            }
            set
            {
                m_DepthArray = value;
                NotifyPropertyChanged("DepthArray");
            }
        }

        public bool[,] DepthMask
        {
            get
            {
                return m_DepthMask;
            }
            set
            {
                m_DepthMask = value;
                NotifyPropertyChanged("DepthMask");
            }
        }

        public bool UseDepthmask
        {
            get
            {
                return m_UseDepthMask;
            }
            set
            {
                m_UseDepthMask = value;
                NotifyPropertyChanged("UseDepthMask");
            }
        }
        
        public bool IsDisassemblyZone
        {
            get
            {
                return m_IsDisassemblyZone;
            }
            set
            {
                m_IsDisassemblyZone = value;
                NotifyPropertyChanged("IsDisassemblyZone");
            }
        }

        public int Area
        {
            get
            {
                return m_Area;
            }
            set
            {
                m_Area = value;
                NotifyPropertyChanged("Area");
            }
        }

        public double LastPercentageMatched
        {
            get
            {
                return m_LastPercentageMatched;
            }
            set
            {
                m_LastPercentageMatched = value;
                NotifyPropertyChanged("LastPercentageMatched");
            }
        }

        public double MatchPercentage {
            get {
                return (double)SettingsManager.Instance.Settings.AssemblyZonesInputMatchPercentage + m_MatchPercentageOffset;
            } 
        }

        public double MatchPercentageOffset
        {
            get { return m_MatchPercentageOffset; }
            set {
                m_MatchPercentageOffset = value;
                NotifyPropertyChanged("MatchPercentageOffset");
                NotifyPropertyChanged("MatchPercentage");
                NotifyPropertyChanged("NameWithPercentage");
            }
        }

        public String NameWithPercentage {
            get { return Name + " (" + MatchPercentageOffset.ToString("+#;-#;+0") + "%)"; }
        }

        #endregion

    }
}
