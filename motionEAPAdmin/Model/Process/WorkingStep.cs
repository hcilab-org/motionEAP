// <copyright file=WorkingStep.cs
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

using System.Collections.ObjectModel;
using System.ComponentModel;
using HciLab.motionEAP.InterfacesAndDataModel;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System;
using System.Linq;

namespace motionEAPAdmin.Model.Process
{
    /// <summary>
    /// This class represents a working step. A working step is a task that the user has to perform in order
    /// to create a product. Usually a workflow consists of 1..n working steps.
    /// </summary>
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    [Serializable()]
    public class WorkingStep : ISerializable,  INotifyPropertyChanged
    {
        // Version
        private int m_SerVersion = 6;

        // members
        private int m_StepNumber; // the position of this Workingstep in the workflow

        private string m_Name; // a descriptive name of the workingstep

        private string m_WithDrawel;

        private AllEnums.PBD_Mode m_Mode;

        private ObservableCollection<AdaptiveScene> m_AdaptiveScenes = new ObservableCollection<AdaptiveScene>(); // represents the adaptive scenes belonging to this workingstep

        private readonly ObservableCollection<WorkflowFailState> m_FailStates = new ObservableCollection<WorkflowFailState>();

        // endcondition
        private string m_EndConditionObjectName; //Name of the object to be recognized at the end of the workingstep
        //TODO: Change this to some Condition object to allow for more complex conditions.

        //time after which workingstep is skipped automatically in milliseconds 
        private int m_TimeOut = 0;

        private int m_ExpectedDuration = 0;

        private bool m_IsManualStep;

        private bool m_IsQSStep = false;


        public event PropertyChangedEventHandler PropertyChanged; // event for the databinding


        // constructor
        public WorkingStep()
        {
            createAdaptiveScenesAccordingToAdaptivityLevels();
        }


        public WorkingStep(string pName, string pWithDrawel, string pEndCondition, int pStepNumber, int pTimeOut = 0)
        {
            m_Name = pName;
            m_EndConditionObjectName = pEndCondition;
            m_StepNumber = pStepNumber;
            m_WithDrawel = pWithDrawel;

            createAdaptiveScenesAccordingToAdaptivityLevels();
            m_FailStates = new ObservableCollection<WorkflowFailState>();
        }


        protected WorkingStep(SerializationInfo info, StreamingContext context)
        {
            if (info.MemberCount > 6) m_SerVersion = info.GetInt32("m_SerVersion");
            else m_SerVersion = 1;
            
            m_StepNumber = info.GetInt32("m_StepNumber");
            m_Name = info.GetString("m_Name");
            m_WithDrawel = info.GetString("m_WithDrawel");
            m_Mode = (AllEnums.PBD_Mode)info.GetValue("m_Mode", typeof(AllEnums.PBD_Mode));
            m_AdaptiveScenes = (ObservableCollection<AdaptiveScene>)info.GetValue("m_AdaptiveScenes", typeof(ObservableCollection<AdaptiveScene>));
            m_EndConditionObjectName = info.GetString("m_EndConditionObjectName");

            if (SerVersion < 2)
                return;

            m_FailStates = (ObservableCollection<WorkflowFailState>)info.GetValue("m_FailStates", typeof(ObservableCollection<WorkflowFailState>));

            if (SerVersion < 3)
                return;

            m_TimeOut = info.GetInt32("m_TimeOut");

            if (SerVersion < 3)
                return;

            m_ExpectedDuration = info.GetInt32("m_ExpectedDuration");

            if (SerVersion < 5)
                return;

            m_IsManualStep = info.GetBoolean("m_IsManualStep");

            if (SerVersion < 6)
                return;

            m_IsQSStep = info.GetBoolean("m_IsQSStep");

        }


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("m_SerVersion", m_SerVersion);
            info.AddValue("m_StepNumber", m_StepNumber);
            info.AddValue("m_Name", m_Name);
            info.AddValue("m_WithDrawel", m_WithDrawel);
            info.AddValue("m_Mode", m_Mode);
            info.AddValue("m_AdaptiveScenes", m_AdaptiveScenes);
            info.AddValue("m_EndConditionObjectName", m_EndConditionObjectName);
            info.AddValue("m_FailStates", m_FailStates);
            info.AddValue("m_TimeOut", m_TimeOut);
            info.AddValue("m_ExpectedDuration", m_ExpectedDuration);
            info.AddValue("m_IsManualStep", m_IsManualStep);
            info.AddValue("m_IsQSStep", m_IsQSStep);
        }


        public void createNewName(AllEnums.PBD_Mode mode)
        {
            if (mode == AllEnums.PBD_Mode.BOX_WITHDRAWEL)
            {
                string newName = "Step " + m_StepNumber + ": Entnahme " + m_WithDrawel;
                Name = newName;
            }
            else if (mode == AllEnums.PBD_Mode.ASSEMBLY_DONE)
            {
                string newName = "Step " + m_StepNumber + ": Assembly ";
                Name = newName;
            }
            if (mode == AllEnums.PBD_Mode.END_CONDITION)
            {
                string newName = "Fertig";
                Name = newName;
            }
        }

        /// <summary>
        /// This method checks adaptive scenes if all adaptivitylevels have a adaptivescene
        /// </summary>
        /// 
        /// 
        private void createAdaptiveScenesAccordingToAdaptivityLevels()
        {
            foreach( AdaptivityLevel level in AdaptivityLevel.AdaptivityLevels)
            {
                bool found = false;
                foreach (AdaptiveScene scene in m_AdaptiveScenes)
                {
                    if(scene.Level.Id == level.Id)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    // create a default adaptive scene
                    m_AdaptiveScenes.Add(new AdaptiveScene(null, level));
                }
            }
        }

        public void CreateFailState(string pTriggerMessage)
        {
            m_FailStates.Add(new WorkflowFailState("Error "+ (m_FailStates.Count+1), pTriggerMessage, new Scene.Scene()));
            NotifyPropertyChanged("ErrorConditionObjectName");
        }

        public void ChangeFailStateTriggerMessage(string pTriggerMessage)
        {
            GetFirstFailState().Conditions.FirstOrDefault().CheckMessage = pTriggerMessage;
            NotifyPropertyChanged("ErrorConditionObjectName");
        }


        public Boolean HasFailState()
        {
            return HasFailstate(0);
        }

        public Boolean HasFailstate(int index)
        {
            if (m_FailStates.Count > index) return true;
            else return false;
        }

        public WorkflowFailState GetFirstFailState()
        {
            if (m_FailStates.Count > 0) return m_FailStates.FirstOrDefault();
            else return null;
        }

        public Scene.Scene GetFirstFailStateScene()
        {
            if (m_FailStates.Count > 0) return m_FailStates.FirstOrDefault().Scene;
            else return null;
        }

        public WorkflowFailState GetFailState(int index)
        {
            if (m_FailStates.Count > index) return m_FailStates.ElementAt(index);
            else return null;
        }

        public Scene.Scene GetFailStateScene(int index)
        {
            if (m_FailStates.Count > index) return m_FailStates.ElementAt(index).Scene;
            else return null;
        }

        public AdaptiveScene getAdaptiveScene(int pLevelId)
        {
            AdaptiveScene aScene = null;
            foreach (AdaptiveScene scene in m_AdaptiveScenes)
            {
                //Legacy Support: Set Scene AdaptivityLevel if none is defined
                if (scene.Level == null)
                {
                    scene.Level = AdaptivityLevel.AdaptivityLevels.ToArray()[0];
                }
                if (scene.Level.Id == pLevelId)
                {
                    aScene = scene;               
                }
            }

            if (aScene == null)
            {
                // create a default adaptive scene
                aScene = new AdaptiveScene(new Scene.Scene(), AdaptivityLevel.AdaptivityLevels.Find(al => al.Id == pLevelId));

                m_AdaptiveScenes.Add(aScene);
            }

            return aScene;
        }


        // getter/setter

        public int SerVersion
        {
            get { return m_SerVersion; }
            set { m_SerVersion = value; }
        }

        public int StepNumber
        {
            get
            {
                return m_StepNumber;
            }
            set
            {
                m_StepNumber = value;
                NotifyPropertyChanged("STEPNUMBER");
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
                NotifyPropertyChanged("NAME");
            }
        }

        public string Withdrawel
        {
            get
            {
                return m_WithDrawel;
            }
            set
            {
                m_WithDrawel = value;
                NotifyPropertyChanged("WITHDRAWEL");
            }
        }


        public AllEnums.PBD_Mode Mode
        {
            get
            {
                return m_Mode;
            }
            set
            {
                m_Mode = value;
                NotifyPropertyChanged("WITHDRAWEL");
            }
        }

        public ObservableCollection<AdaptiveScene> AdaptiveScenes
        {
            get
            {
                return m_AdaptiveScenes;
            }
            set
            {
                m_AdaptiveScenes = value;
                NotifyPropertyChanged("ADAPTIVESCENES");
            }
        
        }

        public string EndConditionObjectName
        {
            get
            {
                return m_EndConditionObjectName;
            }
            set
            {
                m_EndConditionObjectName = value;
                NotifyPropertyChanged("ENDCONDITIONOBJECTNAME");
            }
        }

        public ObservableCollection<WorkflowFailState> FailStates
        {
            get
            {
                return m_FailStates;
            }
        }

        public string ErrorConditionObjectName
        {
            get
            {
                if (m_FailStates.Count > 0) return m_FailStates.FirstOrDefault().Conditions.FirstOrDefault().CheckMessage;
                else return "";
            }
            set
            {
                throw new Exception("Only for testing purposes. Have a look at fail states collection");
            }
        }

        public int TimeOut
        {
            get { return m_TimeOut; }
            set { m_TimeOut = value; NotifyPropertyChanged("TimeOut"); }
        }

        public int ExpectedDuration
        {
            get { return m_ExpectedDuration; }
            set { m_ExpectedDuration = value; NotifyPropertyChanged("ExpectedDuration"); }
        }

        public bool IsManualStep
        {
            get { return m_IsManualStep; }
            set { m_IsManualStep = value; NotifyPropertyChanged("IsManualStep"); }
        }

        public bool IsQSStep
        {
            get { return m_IsQSStep; }
            set
            {
                if (CanBeQSStep)
                {
                    m_IsQSStep = value;
                    NotifyPropertyChanged("IsQSStep");
                }
                else
                {
                    m_IsQSStep = false;
                    NotifyPropertyChanged("IsQSStep");
                }
            }
        }

        public bool CanBeQSStep
        {
            get {  return EndConditionObjectName != "end"; }

        }

        private void NotifyPropertyChanged(string Obj)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(Obj));
            }
        }
    }
}
