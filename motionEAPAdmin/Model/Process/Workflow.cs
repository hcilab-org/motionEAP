// <copyright file=Workflow.cs
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
// <date> 11/2/2016 12:25:59 PM</date>

using motionEAPAdmin.Backend.AssembleyZones;
using motionEAPAdmin.Backend.Boxes;
using motionEAPAdmin.Backend.ObjectDetection;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using HciLab.Utilities;

namespace motionEAPAdmin.Model.Process
{
    /// <summary>
    /// This class represents a workflow:
    /// A workflow defines the exact procedure for the production of a piece.
    /// 
    /// </summary>
    /// 
    /// 
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    [Serializable()]
    public class Workflow : ISerializable, INotifyPropertyChanged
    {

        private string m_Name; // the name of this workflow
        
        private string m_Id;   // the unique id of this workflow - try and define the workflow like a namespace
                             // e.g.: "org.hcilab.workflows.exampleproduct"

        private string m_Description; // a description giving details about the workflow

        private HardwareSetup m_HardwareSetup; // represents the hardwaresetup that is neccessary for this workflow

        private AsyncObservableCollection<WorkingStep> m_WorkingsSteps = new AsyncObservableCollection<WorkingStep>(); // represents all workingsteps belonging to this workflow

        private BoxLayout m_BoxLayout; // represents the boxlayout

        private ObjectDetectionZonesLayout m_ObjectZoneLayout; // zones for object detection

        private AssemblyZoneLayout m_AssemblyZoneLayout; // zones for detecting assembly


        public event PropertyChangedEventHandler PropertyChanged; // property changed for the databinding

        // constructor
        public Workflow()
        {
        }

        protected Workflow(SerializationInfo info, StreamingContext context)
        {
            m_Name = info.GetString("m_Name");
            m_Id = info.GetString("m_Id");
            m_Description = info.GetString("m_Description");
            m_HardwareSetup = (HardwareSetup) info.GetValue("m_HardwareSetup",typeof(HardwareSetup));
            m_WorkingsSteps = (AsyncObservableCollection<WorkingStep>)info.GetValue("m_WorkingsSteps", typeof(AsyncObservableCollection<WorkingStep>));
            m_BoxLayout = (BoxLayout)info.GetValue("m_BoxLayout", typeof(BoxLayout));
            m_ObjectZoneLayout = (ObjectDetectionZonesLayout)info.GetValue("m_ObjectZoneLayout", typeof(ObjectDetectionZonesLayout));
            m_AssemblyZoneLayout = (AssemblyZoneLayout)info.GetValue("m_AssemblyZoneLayout", typeof(AssemblyZoneLayout));
        }


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

            info.AddValue("m_Name", m_Name);
            info.AddValue("m_Id", m_Id);
            info.AddValue("m_Description", m_Description);
            info.AddValue("m_HardwareSetup", m_HardwareSetup);
            info.AddValue("m_WorkingsSteps", m_WorkingsSteps);
            info.AddValue("m_BoxLayout", m_BoxLayout);
            info.AddValue("m_ObjectZoneLayout", m_ObjectZoneLayout);
            info.AddValue("m_AssemblyZoneLayout", m_AssemblyZoneLayout);

            //throw new NotImplementedException();
        }

        public void AddWorkingStep(WorkingStep pStep)
        {
            if(m_WorkingsSteps != null)
            {
                m_WorkingsSteps.Add(pStep);
            }
        }

        private void NotifyPropertyChanged(string Obj)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(Obj));
            }
        }


        #region getters/setters

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

        public string Id
        {
            get
            {
                return m_Id;
            }
            set
            {
                m_Id = value;
                NotifyPropertyChanged("ID");
            }
        }

        public string Description
        {
            get
            {
                return m_Description;
            }
            set
            {
                m_Description = value;
                NotifyPropertyChanged("DESCRIPTION");
            }
        }

        public HardwareSetup HardwareSetup
        {
            get
            {
                return m_HardwareSetup;
            }
            set
            {
                m_HardwareSetup = value;
                NotifyPropertyChanged("HARDWARESETUP");
            }
        }


        public AsyncObservableCollection<WorkingStep> WorkingSteps
        {
            get
            {
                return m_WorkingsSteps;
            }
            set
            {
                m_WorkingsSteps = value;
                NotifyPropertyChanged("WORKINGSTEPS");
            }
        }

        public BoxLayout BoxLayout
        {
            get
            {
                return m_BoxLayout;
            }
            set
            {
                m_BoxLayout = value;
                NotifyPropertyChanged("BOXLAYOUT");
            }
        }

        public ObjectDetectionZonesLayout ObjectZoneLayout
        {
            get
            {
                return m_ObjectZoneLayout;
            }
            set
            {
                m_ObjectZoneLayout = value;
                NotifyPropertyChanged("OBJECTDETECTIONZONELAYOUT");
            }
        }

        public AssemblyZoneLayout AssemblyZoneLayout
        {
            get
            {
                return m_AssemblyZoneLayout;
            }
            set
            {
                m_AssemblyZoneLayout = value;
                NotifyPropertyChanged("ASSEMBLYZONELAYOUT");
            }
        }
        #endregion
    }
}
