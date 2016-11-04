// <copyright file=SettingsTable.cs
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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Windows.Media.Media3D;

namespace HciLab.motionEAP.InterfacesAndDataModel.Data
{

    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    [Serializable()]
    public class SettingsTable : ISerializable, INotifyPropertyChanged
    {

        // Version
        private int m_SerVersion = 7;

        // all the important settings of this software
        // State of all checkboxes for debug mode
        private bool m_ShowFPS = false;
        private bool m_ShowDemoAnimation = false;
        private bool m_EditMode = false;
        private bool m_ShowInformations = false;
        private bool m_ShowWarnings = false;
        private bool m_ShowErrors = false;
        private bool m_ShowCriticals = false;
        // State of all checkboxes for video mode
        private bool m_IsFreeSpace = false;
        private bool m_IsObjectMapping = false;
        private bool m_IsTrackObject = false;
        private bool m_IsStartProjection = false;
        // State of all checkboxes for settings mode
        private bool m_IsSmoothingOn = false;

        private Point3D m_ProjCamPosition = new Point3D(300, 300, 300);
        private Vector3D m_ProjCamLookDirection = new Vector3D(0, -0.1, -1);
        private double m_ProjCamFOV = 45;

        private Rectangle m_KinectDrawing = new Rectangle();

        private Rectangle m_KinectDrawing_AssemblyArea = new Rectangle();

        private int m_tableHeight = 0;

        private String m_ImagePath = "";
        private bool m_CheckBoxBoxFaultDetection;
        private bool m_CheckBoxEnableEnsensoSmoothing;

        private Rectangle m_EnsensoDrawing = new Rectangle();
        private Rectangle m_EnsensoDrawing_AssemblyArea = new Rectangle();

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public SettingsTable()
        {
        }

        protected SettingsTable(SerializationInfo info, StreamingContext context)
        {
            int SerVersion = info.GetInt32("m_SerVersion");
            // Check version of XML version
            if (SerVersion < 1)
                return;
            // 2D Touch
            m_ShowFPS = info.GetBoolean("m_ShowFPS");
            m_ShowDemoAnimation = info.GetBoolean("m_ShowDemoAnimation");
            m_EditMode = info.GetBoolean("m_EditMode");
            m_ShowInformations = info.GetBoolean("m_ShowInformations");
            m_ShowWarnings = info.GetBoolean("m_ShowWarnings");
            m_ShowErrors = info.GetBoolean("m_ShowErrors");
            m_ShowCriticals = info.GetBoolean("m_ShowCriticals");
            m_IsFreeSpace = info.GetBoolean("m_IsFreeSpace");
            m_IsObjectMapping = info.GetBoolean("m_IsObjectMapping");
            m_IsTrackObject = info.GetBoolean("m_IsTrackObject");
            m_IsStartProjection = info.GetBoolean("m_IsStartProjection");
            m_IsSmoothingOn = info.GetBoolean("m_IsSmoothingOn");

            m_tableHeight = info.GetInt32("m_tableHeight");

            m_ImagePath = info.GetString("m_ImagePath");

            if (SerVersion < 2)
                return;

            m_KinectDrawing_AssemblyArea = (Rectangle)info.GetValue("m_KinectDrawing_AssemblyAreaColor", typeof(Rectangle));
            m_KinectDrawing = (Rectangle)info.GetValue("m_KinectDrawing", typeof(Rectangle));

            if (SerVersion < 3)
                return;

            m_ProjCamPosition = (Point3D)info.GetValue("m_ProjCamPosition", typeof(Point3D));
            m_ProjCamLookDirection = (Vector3D)info.GetValue("m_ProjCamLookDirection", typeof(Vector3D));
            m_ProjCamFOV = info.GetDouble("m_ProjCamFOV");

            if (SerVersion < 4)
                return;

            m_CheckBoxBoxFaultDetection = info.GetBoolean("m_CheckBoxBoxFaultDetection");

            if (SerVersion < 5)
                return;

            // m_CheckBoxEnableEnsensoSmoothing = info.GetBoolean("m_CheckBoxEnableEnsensoSmoothing");

            if (SerVersion < 6)
                return;

            m_EnsensoDrawing = (Rectangle)info.GetValue("m_EnsensoDrawing", typeof(Rectangle));
            m_EnsensoDrawing_AssemblyArea = (Rectangle)info.GetValue("m_EnsensoDrawing_AssemblyArea", typeof(Rectangle));

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("m_SerVersion", m_SerVersion);

            info.AddValue("m_ShowFPS", m_ShowFPS);
            info.AddValue("m_ShowDemoAnimation", m_ShowDemoAnimation);
            info.AddValue("m_EditMode", m_EditMode);
            info.AddValue("m_ShowInformations", m_ShowInformations);
            info.AddValue("m_ShowWarnings", m_ShowWarnings);
            info.AddValue("m_ShowErrors", m_ShowErrors);
            info.AddValue("m_ShowCriticals", m_ShowCriticals);
            info.AddValue("m_IsFreeSpace", m_IsFreeSpace);
            info.AddValue("m_IsObjectMapping", m_IsObjectMapping);
            info.AddValue("m_IsTrackObject", m_IsTrackObject);
            info.AddValue("m_IsStartProjection", m_IsStartProjection);
            info.AddValue("m_IsSmoothingOn", m_IsSmoothingOn);

            info.AddValue("m_KinectDrawing", m_KinectDrawing);

            info.AddValue("m_KinectDrawing_AssemblyAreaColor", m_KinectDrawing_AssemblyArea);

            info.AddValue("m_tableHeight", m_tableHeight);
            info.AddValue("m_ImagePath", m_ImagePath);

            info.AddValue("m_ProjCamPosition", m_ProjCamPosition);
            info.AddValue("m_ProjCamLookDirection", m_ProjCamLookDirection);
            info.AddValue("m_ProjCamFOV", m_ProjCamFOV);
            info.AddValue("m_CheckBoxBoxFaultDetection", m_CheckBoxBoxFaultDetection);

            info.AddValue("m_CheckBoxEnableEnsensoSmoothing", m_CheckBoxEnableEnsensoSmoothing);

            info.AddValue("m_EnsensoDrawing", m_EnsensoDrawing);
            info.AddValue("m_EnsensoDrawing_AssemblyArea", m_EnsensoDrawing_AssemblyArea);
        }

        public delegate void ShowDemoAnimationHandler(object pSource, bool pVisible);

        public event ShowDemoAnimationHandler ShowDemoAnimationEvent;

        public void OnShowDemoAnimationEvent(object pSource, bool pVisible)
        {
            if (this.ShowDemoAnimationEvent != null)
                ShowDemoAnimationEvent(pSource, pVisible);
        }

        public String ImagePath
        {
            get { return m_ImagePath; }
            set
            {
                m_ImagePath = value;
                NotifyPropertyChanged("ImagePath");
            }
        }

        public bool ShowFPS
        {
            get { return m_ShowFPS; }
            set
            {
                m_ShowFPS = value;
                NotifyPropertyChanged("ShowFPS");
            }
        }

        public Boolean ShowDemoAnimation
        {
            get { return m_ShowDemoAnimation; }
            set
            {
                m_ShowDemoAnimation = value;
                OnShowDemoAnimationEvent(this, m_ShowDemoAnimation);

                NotifyPropertyChanged("ShowFPS");
            }
        }

        public bool EditMode
        {
            get
            {
                return m_EditMode;
            }
            set
            {
                m_EditMode = value;
                NotifyPropertyChanged("EditMode");
            }
        }

        public bool ShowWarnings
        {
            get
            {
                return m_ShowWarnings;
            }
            set
            {
                m_ShowWarnings = value;
                NotifyPropertyChanged("ShowWarnings");
            }
        }

        public bool ShowInformations
        {
            get
            {
                return m_ShowInformations;
            }
            set
            {
                m_ShowInformations = value;
                NotifyPropertyChanged("ShowInformations");
            }
        }

        public bool ShowCriticals
        {
            get
            {
                return m_ShowCriticals;
            }
            set
            {
                m_ShowCriticals = value;
                NotifyPropertyChanged("ShowCriticals");
            }
        }

        public bool ShowErrors
        {
            get
            {
                return m_ShowErrors;
            }
            set
            {
                m_ShowErrors = value;
                NotifyPropertyChanged("ShowErrors");
            }
        }

        public Point3D ProjCamPosition
        {
            get
            {
                return m_ProjCamPosition;
            }
            set
            {
                m_ProjCamPosition = value;
                NotifyPropertyChanged("ProjCamPosition");
            }
        }

        public Vector3D ProjCamLookDirection
        {
            get
            {
                return m_ProjCamLookDirection;
            }
            set
            {
                m_ProjCamLookDirection = value;
                NotifyPropertyChanged("ProjCamLookDirection");
            }
        }

        public double ProjCamFOV
        {
            get
            {
                return m_ProjCamFOV;
            }
            set
            {
                m_ProjCamFOV = value;
                NotifyPropertyChanged("ProjCamFOV");
            }
        }


        public Rectangle KinectDrawing
        {
            get
            {
                return m_KinectDrawing;
            }
            set
            {
                m_KinectDrawing = value;
                NotifyPropertyChanged("KinectDrawing");
            }
        }

        public Rectangle KinectDrawing_AssemblyArea
        {
            get
            {
                return m_KinectDrawing_AssemblyArea;
            }
            set
            {
                m_KinectDrawing_AssemblyArea = value;
                NotifyPropertyChanged("KinectDrawing_AssemblyArea");
            }
        }

        public int TableHeight
        {
            get
            {
                return m_tableHeight;
            }
            set
            {
                m_tableHeight = value;
                NotifyPropertyChanged("TableHeight");
            }
        }

        public bool EnableFaultBoxMode
        {
            get { return m_CheckBoxBoxFaultDetection; }
            set
            {
                m_CheckBoxBoxFaultDetection = value;
                NotifyPropertyChanged("EnableFaultBoxMode");
            }
        }

        public bool CheckBoxEnableEnsensoSmoothing
        {
            get { return m_CheckBoxEnableEnsensoSmoothing; }
            set
            {
                m_CheckBoxEnableEnsensoSmoothing = value;
                NotifyPropertyChanged("CheckBoxEnableEnsensoSmoothing");
            }
        }
        public Rectangle EnsensoDrawing
        {
            get { return m_EnsensoDrawing; }
            set 
            {
                m_EnsensoDrawing = value;
                NotifyPropertyChanged("EnsensoDrawing");
            }
        }

        public Rectangle EnsensoDrawing_AssemblyArea
        {
            get { return m_EnsensoDrawing_AssemblyArea; }
            set 
            { 
                m_EnsensoDrawing_AssemblyArea = value;
                NotifyPropertyChanged("EnsensoDrawing_AssemblyArea");
            }
        }

    }
}
