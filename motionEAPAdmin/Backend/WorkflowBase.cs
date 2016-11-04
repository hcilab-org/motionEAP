// <copyright file=WorkflowBase.cs
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

using HciLab.Utilities;
using HciLab.Utilities.Mathematics.Geometry2D;
using motionEAPAdmin.Scene;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Media.Media3D;

namespace motionEAPAdmin.Backend
{
    public abstract class WorkflowBase : DataBaseClass, ISerializable, INotifyPropertyChanged
    {
        /// <summary>
        /// property changed for the databinding
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        private int m_SerVersionWorkflowBase = 3;

        protected int m_X = 0; // pixel in color image
        protected int m_Y = 0; // pixel in color image
        protected int m_Z; // millimeters in real world

        protected int m_Width = 1; // pixel in color imaeg
        protected int m_Height = 1; // pixel in color image

        protected string m_TriggerMessage; // message to be triggered
        protected string m_Name;

        protected Scene.Scene m_CustomScene = null;

        /// <summary>
        /// 
        /// </summary>
        private Polygon m_Contour = null;

        #region No Serializableization

        protected GeometryModel3D m_Drawable = null;
        private bool m_IsSelected = true;

        protected Scene.SceneItem m_SceneItem = null;

        #endregion

        public WorkflowBase()
            : base()
        {
        }
        public WorkflowBase(int pId)
            : base(pId)
        {
        }

        public WorkflowBase(SerializationInfo pInfo, StreamingContext pContext)
            : base(pInfo, pContext)
        {
            // for version evaluation while deserializing
            int pSerVersion = pInfo.GetInt32("m_SerVersionWorkflowBase");
            m_X = pInfo.GetInt32("m_X");
            m_Y = pInfo.GetInt32("m_Y");
            m_Z = pInfo.GetInt32("m_Z");
            m_Width = pInfo.GetInt32("m_Width");
            m_Height = pInfo.GetInt32("m_Height");
            m_Name = pInfo.GetString("m_Name");
            m_TriggerMessage = pInfo.GetString("m_TriggerMessage");


            if (pSerVersion > 1)
            {
                m_CustomScene = (Scene.Scene)pInfo.GetValue("m_CustomScene", typeof(Scene.Scene));
            }

            if (pSerVersion > 2)
            {
                m_Contour = (Polygon)pInfo.GetValue("m_Contour", typeof(Polygon));
            }

            

            this.trigger += WorkflowManager.Instance.OnTriggered;
        }

        public new void GetObjectData(SerializationInfo pInfo, StreamingContext pContext)
        {
            base.GetObjectData(pInfo, pContext);
            pInfo.AddValue("m_SerVersionWorkflowBase", m_SerVersionWorkflowBase);
            pInfo.AddValue("m_X", m_X);
            pInfo.AddValue("m_Y", m_Y);
            pInfo.AddValue("m_Z", m_Z);
            pInfo.AddValue("m_Width", m_Width);
            pInfo.AddValue("m_Height", m_Height);
            pInfo.AddValue("m_Name", m_Name);
            pInfo.AddValue("m_TriggerMessage", m_TriggerMessage);
            pInfo.AddValue("m_CustomScene", m_CustomScene);
            pInfo.AddValue("m_Contour", m_Contour);
        }

        public abstract Scene.SceneItem getDrawable(bool pIsForRecord = false);

        
        public delegate void TriggerHandler(WorkflowBase pSource);

        public event TriggerHandler trigger;

        public void OnTrigger(WorkflowBase pSource)
        {
            if (this.trigger != null)
                trigger(pSource);
        }

        protected void NotifyPropertyChanged(string Obj)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(Obj));
            }
            updateScene();
        }

        protected void updateScene()
        {
            if (m_Contour != null)
            {
                if (m_SceneItem == null || !(m_SceneItem is ScenePolygon))
                    m_SceneItem = new ScenePolygon(m_Contour, System.Windows.Media.Color.FromRgb(0, 255, 0));
            }
            else
            {
                if (m_SceneItem == null || !(m_SceneItem is SceneRect))
                    m_SceneItem = new SceneRect();
 
            }

            m_SceneItem.X = X;
            m_SceneItem.Y = Y;
            m_SceneItem.Width = Width;
            m_SceneItem.Height = Height;
            m_SceneItem.Touchy = false;
        }

        #region Getter / Setter
        public int X
        {
            get
            {
                return m_X;
            }
            set
            {
                m_X = value;
                NotifyPropertyChanged("X");
            }
        }

        public int Y
        {
            get
            {
                return m_Y;
            }
            set
            {
                m_Y = value;
                NotifyPropertyChanged("Y");
            }
        }

        public int Z
        {
            get
            {
                return m_Z;
            }
            set
            {
                m_Z = value;
                NotifyPropertyChanged("Z");
            }
        }

        public int Width
        {
            get
            {
                return m_Width;
            }
            set
            {
                m_Width = value;
                NotifyPropertyChanged("Width");
            }
        }

        public int Height
        {
            get
            {
                return m_Height;
            }
            set
            {
                m_Height = value;
                NotifyPropertyChanged("Height");
            }
        }

        public string TriggerMessage
        {
            get
            {
                return m_TriggerMessage;
            }
            set
            {
                m_TriggerMessage = value;
                NotifyPropertyChanged("TriggerMessage");
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

        public Scene.Scene CustomScene
        {
            get { return m_CustomScene; }
            set { m_CustomScene = value; }
        }


        public Polygon Contour
        {
            get
            {
                return m_Contour;
            }
            set
            {
                m_Contour = value;
                NotifyPropertyChanged("Contour");
            }
        }

        #region Getter and setter for CheckedListBox
        
        public bool IsSelected
        {
            get
            {
                return m_IsSelected;
            }
            set
            {
                m_IsSelected = value;
                NotifyPropertyChanged("IsSelected");
            }
        }
        #endregion

        #endregion
    }
}
