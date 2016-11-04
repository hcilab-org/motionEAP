// <copyright file=WorkflowFailState.cs
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

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace motionEAPAdmin.Model.Process
{
    /// <summary>
    /// This class represents a fail state associated with a working step (e.g. incorrect assembly)
    /// It defines it's own condition(s) and scene to be shown when conditions are met
    /// </summary>
    [Serializable()]
    public class WorkflowFailState : ISerializable, INotifyPropertyChanged
    {
        // Version
        private int m_SerVersion = 1;

        private string m_Name;

        private ObservableCollection<WorkflowCondition> m_Conditions;

        private Scene.Scene m_Scene;

        public event PropertyChangedEventHandler PropertyChanged; // event for the databinding

        public WorkflowFailState(string pName, string pTriggerMessage, Scene.Scene pScene)
        {
            Name = pName;
            m_Conditions = new ObservableCollection<WorkflowCondition>();
            Conditions.Add(new WorkflowCondition(pTriggerMessage));
            Scene = pScene;
        }

        protected WorkflowFailState(SerializationInfo info, StreamingContext context)
        {
            m_SerVersion = info.GetInt32("m_SerVersion");
            m_Name = info.GetString("m_Name");
            m_Conditions = (ObservableCollection<WorkflowCondition>)info.GetValue("m_Conditions", typeof(ObservableCollection<WorkflowCondition>));
            m_Scene = (Scene.Scene)info.GetValue("m_Scene", typeof(Scene.Scene));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("m_SerVersion", m_SerVersion);
            info.AddValue("m_Name", m_Name);
            info.AddValue("m_Conditions", m_Conditions);
            info.AddValue("m_Scene", m_Scene);
        }

        public Boolean CheckForFail(string pTriggerMessage)
        {
            foreach (var condition in m_Conditions)
            {
                if (condition.IsFullfiled(pTriggerMessage))
                {
                    return true;
                }
            }
            return false; //Only return false if no error condition is fullfiled
        }

        public void AddCondition(string pTriggerMessage)
        {
            m_Conditions.Add(new WorkflowCondition(pTriggerMessage));
            NotifyPropertyChanged("ConditionsNames");
        }

        public void ClearConditions()
        {
            m_Conditions.Clear();
            NotifyPropertyChanged("ConditionsNames");
        }

        #region getters/setters
        public int SerVersion
        {
            get { return m_SerVersion; }
            set { m_SerVersion = value; }
        }

        public Scene.Scene Scene
        {
            get { return m_Scene; }
            set 
            { 
                m_Scene = value;
                NotifyPropertyChanged("Scene");
            }
        }

        public ObservableCollection<WorkflowCondition> Conditions
        {
            get { return m_Conditions; }
            set 
            {
                m_Conditions = value;
                NotifyPropertyChanged("Conditions");
                NotifyPropertyChanged("ConditionsNames");
            }
        }

        public string ConditionsNames
        {
            get
            {
                return String.Join(", ", m_Conditions.Select(w => w.CheckMessage).ToArray());
            }
        }

        public string Name
        {
            get { return m_Name; }
            set 
            {
                m_Name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private void NotifyPropertyChanged(string Obj)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(Obj));
            }
        }
        #endregion

        public static  WorkflowFailState BOX_FAILSTATE = new WorkflowFailState("box","box",new Scene.Scene());

    }
}
