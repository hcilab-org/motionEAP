// <copyright file=WorkflowManager.cs
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

using HciLab.motionEAP.InterfacesAndDataModel;
using HciLab.motionEAP.InterfacesAndDataModel.Data;
using HciLab.motionEAP.InterfacesAndDataModel.Utilities;
using HciLab.Utilities;
using motionEAPAdmin.Network;
using motionEAPAdmin.Backend.AssembleyZones;
using motionEAPAdmin.Backend.Boxes;
using motionEAPAdmin.Backend.ObjectDetection;
using motionEAPAdmin.ContentProviders;
using motionEAPAdmin.GUI;
using motionEAPAdmin.Model.Process;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using motionEAPAdmin.Statistics;

namespace motionEAPAdmin.Backend
{
    public class WorkflowManager : INotifyPropertyChanged
    {
        /// <summary>
        /// property changed for the databinding
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        //Events for tracking Progress
        public event EventHandler<WorkingStepCompletedEventArgs> WorkingStepCompleted;
        public event EventHandler<WorkingStepStartedEventArgs> WorkingStepStarted;
        public event EventHandler<WorkflowLoadedEventArgs> WorkflowLoaded;
        public event EventHandler<WorkflowCompletedEventArgs> WorkflowCompleted;
        public event EventHandler<WorkflowStartedEventArgs> WorkflowStarted;
        public event EventHandler<WorkflowStopedEventArgs> WorkflowStopped;
        public event EventHandler<FailStateOccuredEventArgs> FailStateOccured;

        public event EventHandler<AdaptivityLevelChangedEventArgs> AdaptivityLevelChanged; 

        /// <summary>
        /// the instance
        /// </summary>
        private static WorkflowManager m_Instance;

        //TODO: Put someplace sensible
        /// <summary>
        /// (known) Object recognized by the SURF Algorithm.
        /// </summary>

        public static event Action<TrackableObject> ObjectRecognized;
        public static event Action<Box> BoxTriggered;
        public static event Action<AssemblyZone> AssemblyZoneTriggered;
        /// <summary>
        /// Workstep changed by Condition
        /// </summary>
        public static event Action WorkingstepChanged;

        /// <summary>
        /// the currently loaded workflow
        /// </summary>
        private Workflow m_LoadedWorkflow = null;
        private string m_LoadedWorkflowFilename = "";
        private long m_StartTime;
        private long m_EndTime;
        private long m_DurationTime;
        private long m_StepStartTime;
        private long m_CsvNameTime;
        //private bool m_IsRunning = false;
        //private bool m_IsLoaded = false;
        private long m_UsedObjectLastSeenTimestamp = 0;
        private bool m_ObjectwasFoundOnce = false;
        private int m_CurrentWorkingStepNumber = 0;
        private int m_BoxErrorCounter = 0;   // counting the picking errors
        private int m_AssemblyErrors = 0;    // counting the assembly errors
        private int m_producedParts = 0;     // counting the produced parts

        private int m_ErrorFreeCount = 0;
        private int m_ErrorCount = 0;
        private bool m_ErrorFreeStep = true;

        private bool m_QSMode = false;
        private int m_NumQS = 0;

        private bool[] m_QSFullfilled;

        private int calculatedAdaptivityLevel;


        private List<AdaptivityLevel>  m_AdaptivityLevels = AdaptivityLevel.AdaptivityLevels;

        /// <summary>
        /// Currently selected adaptivity level id
        /// </summary>
        private int m_adaptivityLevelId;

        private delegate void RefreshAdaptivityLevelDelegate();


        /// <summary>
        /// Current NetworkTableDependency
        /// </summary>
        private NetworkTableMonitor m_CurrentNetworkTableDependency = null;

        private WorkflowTimeTrigger m_CurrentWorkingStepTimeOut = null;
        
        private WorkflowTimeTrigger m_CurrentWorkingStepExpectedDurationTimeOut = null;

        private delegate void LoadCurrentWorkingStepDelegate();

        #region CONSTRUCTOR
        private WorkflowManager()
        {
            //TODO: Register to event in a proper way

            //BoxTriggered = new Action<Box>();
            AssemblyZoneTriggered += checkEndConditionspublic;

            BoxTriggered += checkEndConditionspublic;
            BoxTriggered += checkExtraActions;
            BoxTriggered += PBDManager.Instance.OnBoxTriggeredPBD;

            ObjectRecognized += this.checkObjectCondition;

            //Set adaptivitylevelId from Settings
            m_adaptivityLevelId = motionEAPAdmin.ContentProviders.SettingsManager.Instance.Settings.AdaptivityLevelId;

            //FailStateOccured += this.OnFailStateOccured;
            QRDetectManager.Instance.OnCodeDetected += Instance_OnCodeDetected;
        }
        #endregion

        public static WorkflowManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new WorkflowManager();
                }
                return m_Instance;
            }
        }

        protected void NotifyPropertyChanged(string Obj)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(Obj));
            }
        }

        public void OnObjectRecognized(TrackableObject obj)
        {
            if (m_LoadedWorkflow != null)
                ObjectRecognized(obj);
        }

        public void OnTriggered(WorkflowBase w)
        {
            if (w is AssemblyZone)
            {
                Console.WriteLine("Triggered AssemblyZone " + w.Id + " Trigger-Message is: " + w.TriggerMessage);
                AssemblyZoneTriggered(w as AssemblyZone);
            }
            else if (w is Box)
            {
                Console.WriteLine("Triggered Box " + w.Id + " Trigger-Message is: " + w.TriggerMessage);
                BoxTriggered(w as Box);
            }
        }

        public bool loadWorkflow()
        {
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.InitialDirectory = Directory.GetCurrentDirectory() + "\\" + ProjectConstants.WORKFLOW_DIR;
            dlg.Filter = "workflow files (*.work)|*.work";
            dlg.FilterIndex = 2;
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() != DialogResult.OK)
                return false;

            return loadWorkflow(dlg.FileName);
        }

        public bool loadWorkflow(string path)
        {
            UtilitiesIO.GetObjectFromJson(ref m_LoadedWorkflow, path);
            m_LoadedWorkflowFilename = path.Split(Path.DirectorySeparatorChar).Last<String>();

            // set all layouts
            BoxManager.Instance.SetNewLayout(m_LoadedWorkflow.BoxLayout);
            ObjectDetectionManager.Instance.SetNewLayout(m_LoadedWorkflow.ObjectZoneLayout);
            AssemblyZoneManager.Instance.SetNewLayout(m_LoadedWorkflow.AssemblyZoneLayout);

            BackendControl.Instance.refreshGUI();

            m_CurrentWorkingStepNumber = 0;
            StateManager.Instance.SetNewState(this, AllEnums.State.WORKFLOW_LOADED);

            // log that we loaded the workflow
            StudyLogger.Instance.logWorkflowLoaded(m_LoadedWorkflow.Id, m_LoadedWorkflow.Name);


            // remove feedback artifacts from previous stuff
            SceneManager.Instance.DisplayTempFeedback = false;
            SceneManager.Instance.TemporaryFeedbackScene.Clear();

            StatsManager.Instance.initialize();

            OnWorkflowLoaded();

            return true;
        }

        public void startWorkflow()
        {
            if (StateManager.Instance.State == AllEnums.State.WORKFLOW_LOADED)
            {
                StateManager.Instance.SetNewState(this, AllEnums.State.WORKFLOW_PLAYING);

                DoStartWorkflow();
            }
        }

        private void DoStartWorkflow()
        {
            //TODO: Change length of array to number of QS Worksteps...
            m_QSFullfilled = new bool[m_LoadedWorkflow.WorkingSteps.Count - 1];
            Array.Clear(m_QSFullfilled, 0, m_QSFullfilled.Length);
            
            this.m_CurrentWorkingStepNumber = 0;
            calculatedAdaptivityLevel = calculateAdaptivityLevel();
            OnWorkflowStarted();
            OnWorkingStepStarted();
            LoadCurrentWorkingStep();

            /* // TODO: make statsViewer optional and configurable
            SceneManager.Instance.TemporaryStatsScene.Clear();
            var statsViewer = new SceneStatsViewer(
                0.0,
                0.0,
                "STATS:",
                System.Windows.Media.Color.FromRgb(255, 255, 255),
                12.0, new System.Windows.Media.FontFamily("Arial")
                );
            SceneManager.Instance.TemporaryStatsScene.Add(statsViewer);
             */

            this.m_StartTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            this.m_CsvNameTime = m_StartTime;
            this.saveToCSV("Start");
        }

        private void Instance_OnCodeDetected(object sender, CodeDetectedEventArgs e)
        {
            //Try to load workflow if not already loaded
            if (e.Text.Substring(Math.Max(0, e.Text.Length - 5)) == ".work" && e.Text != m_LoadedWorkflowFilename)
            {
                string path = Directory.GetCurrentDirectory() + "\\" + ProjectConstants.WORKFLOW_DIR + "\\" + e.Text;
                if (File.Exists(path))
                {
                    if (loadWorkflow(path))
                    {
                        startWorkflow();
                    }
                }
            }
        }

        public void stopWorkflow()
        {
            if (StateManager.Instance.State == AllEnums.State.WORKFLOW_PLAYING)
            {
                StateManager.Instance.SetNewState(this, AllEnums.State.WORKFLOW_LOADED);
                DoStopWorkflow();
            }
        }

        private void DoStopWorkflow()
        {
            this.m_EndTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            this.m_DurationTime = (this.m_EndTime - this.m_StartTime);
            OnWorkflowStopped();
            this.saveToCSV("Stop");

            //AdminView.Instance.m_GUI_SurveyPanel.m_ListBoxWorkflowHistory.Items.Add(m_DurationTime.ToString() + ";" + m_BoxErrorCounter.ToString());
            m_BoxErrorCounter = 0;
            m_AssemblyErrors = 0;
            if (m_CurrentWorkingStepNumber + 1 == LoadedWorkflow.WorkingSteps.Count)
            {
                WorkflowManager.Instance.NextWorkingStep(AllEnums.WorkingStepEndConditionTrigger.HACK);
                BackendControl.Instance.refreshGUI();
            }
            if (m_CurrentWorkingStepTimeOut != null) m_CurrentWorkingStepTimeOut.Dispose();
            if (m_CurrentWorkingStepExpectedDurationTimeOut != null) m_CurrentWorkingStepExpectedDurationTimeOut.Dispose();
            
            if (m_CurrentNetworkTableDependency != null) m_CurrentNetworkTableDependency.Dispose();
        }

        public void restartWorkflow()
        {
            if (StateManager.Instance.State == AllEnums.State.WORKFLOW_PLAYING)
            {
                DoStopWorkflow();
                DoStartWorkflow();
            }
        }

        public void NextWorkingStep(AllEnums.WorkingStepEndConditionTrigger trigger)
        {
            if (StateManager.Instance.State == AllEnums.State.WORKFLOW_PLAYING)
            {
                if (m_CurrentWorkingStepNumber != LoadedWorkflow.WorkingSteps.Count)
                {
                    //Trigger Step Completed Event
                    OnWorkingStepCompleted(trigger);

                    // reset error counter
                    m_AssemblyErrors = 0;
                    m_BoxErrorCounter = 0;

                    m_CurrentWorkingStepNumber = m_CurrentWorkingStepNumber + 1;

                    if (m_CurrentWorkingStepNumber < LoadedWorkflow.WorkingSteps.Count)
                    {
                        //Trigger Step Started Event
                        OnWorkingStepStarted();


                        // load next scene onto scenemanager
                        LoadCurrentWorkingStep();
                        string stepCsv = "Step" + (m_CurrentWorkingStepNumber - 1);
                        this.saveToCSV(stepCsv);
                    }
                    else
                    {
                        // finished
                    }
                }
            }
        }

        public void PreviousWorkingStep()
        {
            if (m_CurrentWorkingStepNumber > 0)
            {
                m_CurrentWorkingStepNumber = m_CurrentWorkingStepNumber - 1;
                OnWorkingStepStarted();

                LoadCurrentWorkingStep();
            }
        }

        private void LoadCurrentWorkingStep()
        {
            if (!AdminView.Instance.Dispatcher.CheckAccess())
            {
                AdminView.Instance.Dispatcher.Invoke(new LoadCurrentWorkingStepDelegate(LoadCurrentWorkingStep));
                return; // Important to leave the culprit thread
            }

            WorkingStep step = LoadedWorkflow.WorkingSteps.ElementAt(m_CurrentWorkingStepNumber);
            if (step.IsQSStep && m_QSMode == false)
            {
                m_QSMode = true;
                NotifyPropertyChanged("QSModeEnabled");
                WorkingStep stepIt = step;
                m_NumQS = 0;
                while (stepIt.IsQSStep)
                {
                    m_NumQS++;
                    stepIt = LoadedWorkflow.WorkingSteps.ElementAt(m_CurrentWorkingStepNumber + m_NumQS);
                }
                m_QSFullfilled = new bool[m_NumQS];
                Array.Clear(m_QSFullfilled, 0, m_QSFullfilled.Length);
            }

            if (m_QSMode && m_CurrentWorkingStepNumber < m_LoadedWorkflow.WorkingSteps.Count - 1)
            {
                LoadQSModeScene();
            } else {
                SceneManager.Instance.CurrentScene = step.getAdaptiveScene(m_adaptivityLevelId).Scene;
                BackendControl.Instance.refreshGUI();
            }            

            if (m_CurrentWorkingStepTimeOut != null) m_CurrentWorkingStepTimeOut.Dispose();
            if (step.TimeOut > 0)
            {
                m_CurrentWorkingStepTimeOut = new WorkflowTimeTrigger(step.TimeOut);
                m_CurrentWorkingStepTimeOut.TimeOutTriggered += timeOutWorkstep;
            }

            if (m_CurrentWorkingStepExpectedDurationTimeOut != null) m_CurrentWorkingStepExpectedDurationTimeOut.Dispose();
            if (step.ExpectedDuration > 0)
            {
                m_CurrentWorkingStepExpectedDurationTimeOut = new WorkflowTimeTrigger(step.ExpectedDuration);
                m_CurrentWorkingStepExpectedDurationTimeOut.TimeOutTriggered += timeOutExpectedDuration;
            }

            //If EndCondition depends on other Table
            if (m_CurrentNetworkTableDependency != null) m_CurrentNetworkTableDependency.Dispose();
            //TODO: Change this when refactoring Endconditions altogether
            if (step.EndConditionObjectName.Length > 3 && step.EndConditionObjectName.Substring(0, 3) == "net")
            {
                var tableId = step.EndConditionObjectName.Substring(3).Split(':').First();
                m_CurrentNetworkTableDependency = new NetworkTableMonitor(tableId);
                m_CurrentNetworkTableDependency.PropertyChanged += checkNetworkEndConditionspublic;
            }
            else
            {
                m_CurrentNetworkTableDependency = null;
            }

        }

        private void LoadQSModeScene()
        {
            SceneManager.Instance.CurrentScene = new Scene.Scene();
            for (int i = m_CurrentWorkingStepNumber; i < m_CurrentWorkingStepNumber + m_NumQS; i++)
            {
                if (!m_QSFullfilled[i])
                {
                    var step = LoadedWorkflow.WorkingSteps.ElementAt(i);
                    foreach (var item in step.getAdaptiveScene(m_adaptivityLevelId).Scene.Items)
                    {
                        SceneManager.Instance.CurrentScene.Add(item);
                    }
                }
            }
            BackendControl.Instance.refreshGUI();
        }

        private void timeOutWorkstep(object sender, EventArgs e)
        {
            if (m_LoadedWorkflow != null)
            {
                if (m_CurrentWorkingStepNumber < m_LoadedWorkflow.WorkingSteps.Count - 1)
                {
                    NextWorkingStep(AllEnums.WorkingStepEndConditionTrigger.TIMEOUT);
                }
                else
                {
                    restartWorkflow();
                    m_producedParts++; // is this a good place for that?
                }
            }
        }

        private void timeOutExpectedDuration(object sender, EventArgs e)
        {
            if (m_LoadedWorkflow != null)
            {
                var oldLevel = AdaptivityLevelId;
                DecreaseAdaptivityLevel();
                OnAdaptivityLevelChanged(new AdaptivityLevelChangedEventArgs { Reason = "timeOut", OldLevel = oldLevel });
            }
        }

        private void checkNetworkEndConditionspublic(object sender, PropertyChangedEventArgs e)
        {
            if (m_LoadedWorkflow != null)
            {
                if (m_CurrentWorkingStepNumber < m_LoadedWorkflow.WorkingSteps.Count)
                {
                    if (sender.GetType() == typeof(NetworkTableMonitor)) 
                    {

                        var monitor = (NetworkTableMonitor) sender;
                        WorkingStep step = m_LoadedWorkflow.WorkingSteps.ElementAt(m_CurrentWorkingStepNumber);
                        if (step.EndConditionObjectName.Length > 3)
                        {

                        
                            //TODO: Implement proper endCondition objects
                            var endParams = step.EndConditionObjectName.Substring(3).Split(':');
                            var tableId = endParams.First();
                            int endStep = -1;
                            if (endParams.Length > 1) {
                                endStep = Int32.Parse(endParams.ElementAt(1));
                            }
                        
                            var triggerNext = false;
                            if (tableId == monitor.TableName)
                            {
                                if (endStep > 0 && e.PropertyName == "StepNumber" && monitor.StepNumber >= endStep) {
                                    triggerNext = true;
                                } else if (e.PropertyName == "ProducedParts" && monitor.ProducedParts > CommunicationManager.Instance.ServerInfo.SelfStatus.ProducedParts) {
                                    triggerNext = true;
                                }
                            }

                            if (triggerNext)
                            {
                                // trigger next step
                                NextWorkingStep(AllEnums.WorkingStepEndConditionTrigger.NETWORK_TABLE);
                            }

                        }

                    }                    
                }
            }
        }

        public void checkEndConditionspublic(Box box)
        {
            if (m_LoadedWorkflow != null)
            {
                if (m_CurrentWorkingStepNumber < m_LoadedWorkflow.WorkingSteps.Count)
                {
                    WorkingStep step = m_LoadedWorkflow.WorkingSteps.ElementAt(m_CurrentWorkingStepNumber);
                    if (step.EndConditionObjectName == ("" + box.TriggerMessage))
                    {
                        // trigger next step
                        NextWorkingStep(AllEnums.WorkingStepEndConditionTrigger.BOX);
                    }
                    else
                    {
                        if (!box.wasRecentlyFalselyTriggered())
                        {
                            OnFailStateOccured(WorkflowFailState.BOX_FAILSTATE);
                            m_BoxErrorCounter++;

                            if (SettingsManager.Instance.Settings.SettingsTable.EnableFaultBoxMode)
                            {
                                box.IsBoxErroneous = true;
                            }
                        }
                    }
                }
            }
        }

        private void checkExtraActions(Box box)
        {
            if (m_LoadedWorkflow != null)
            {
                switch (box.TriggerMessage)
                {
                    case "_nextStep":
                        if (m_CurrentWorkingStepNumber < m_LoadedWorkflow.WorkingSteps.Count)
                        {
                            // trigger next step
                            NextWorkingStep(AllEnums.WorkingStepEndConditionTrigger.BOXBUTTON);
                        }
                        break;
                    case "_previousStep":
                        if (m_CurrentWorkingStepNumber > 1)
                        {
                            // trigger previous step
                            PreviousWorkingStep();
                        }
                        break;
                    case "_easyAdaptivityLevel":
                        AdaptivityLevelId = 1;
                        break;
                    case "_mediumAdaptivityLevel":
                        AdaptivityLevelId = 2;
                        break;
                    case "_hardAdaptivityLevel":
                        AdaptivityLevelId = 3;
                        break;
                    default:
                        break;
                }
            }
        }

        public void checkEndConditionspublic(AssemblyZone zone)
        {
            if (m_LoadedWorkflow != null)
            {
                if (m_CurrentWorkingStepNumber < m_LoadedWorkflow.WorkingSteps.Count)
                {
                    if (m_QSMode)
                    {
                        checkQSModeEndConditions(zone);
                    }
                    else
                    {
                        WorkingStep step = m_LoadedWorkflow.WorkingSteps.ElementAt(m_CurrentWorkingStepNumber);
                        if (step.EndConditionObjectName == ("" + zone.TriggerMessage))
                        {
                            // trigger next step
                            NextWorkingStep(AllEnums.WorkingStepEndConditionTrigger.ASSEMBLY_ZONE);
                        }
                        int i = 0;
                        while (step.HasFailstate(i))
                        {
                            if (step.GetFailState(i).CheckForFail(zone.TriggerMessage))
                            {
                                // trigger FailEvent
                                OnFailStateOccured(step.GetFailState(i));
                                // Show Error Scene
                                SceneManager.Instance.CurrentScene = step.GetFailStateScene(i);

                                m_AssemblyErrors++;
                            }
                            i++;
                        }
                    }
                }
            }
        }

        public void checkQSModeEndConditions(AssemblyZone zone)
        {
            bool allDone = true;
            for (int i = m_CurrentWorkingStepNumber; i < m_CurrentWorkingStepNumber + m_NumQS; i++)
            {
                if (!m_QSFullfilled[i])
                {
                    var step = LoadedWorkflow.WorkingSteps.ElementAt(i);
                    if (step.EndConditionObjectName == ("" + zone.TriggerMessage))
                    {
                        m_QSFullfilled[i] = true;
                        LoadQSModeScene();
                    }
                    else
                    {
                        allDone = false;
                    }
                }
            }
            if (allDone)
            {
                m_QSMode = false;
                NotifyPropertyChanged("QSModeEnabled");
                m_CurrentWorkingStepNumber = m_CurrentWorkingStepNumber + m_NumQS;
                LoadCurrentWorkingStep();
            }
        }

        public void DecreaseAdaptivityLevel()
        {
            if (AdaptivityLevelId > 1)
            {
                AdaptivityLevelId--;
            }
        }

        public void IncreaseAdaptivityLevel()
        {
            if (AdaptivityLevelId < 3)
            {
                AdaptivityLevelId++;
            }
        }

        //checks whether object fullfils current ending conditions
        public void checkObjectCondition(TrackableObject obj)
        {

            if (m_CurrentWorkingStepNumber < m_LoadedWorkflow.WorkingSteps.Count)
            {
                string endConditionObjectName = m_LoadedWorkflow.WorkingSteps.ElementAt(m_CurrentWorkingStepNumber).EndConditionObjectName;

                if (m_LoadedWorkflow.WorkingSteps.ElementAt(m_CurrentWorkingStepNumber).Mode == AllEnums.PBD_Mode.OBJECT_RECOGNIZED)
                {
                    long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    if (m_ObjectwasFoundOnce)
                    {
                        if (now - m_UsedObjectLastSeenTimestamp > 3000) // < 3seconds
                        {
                            // object was used correctly and is back again
                            NextWorkingStep(AllEnums.WorkingStepEndConditionTrigger.TRACKABLE_OBJECT);
                            WorkingstepChanged();
                            m_ObjectwasFoundOnce = false;
                        }
                    }

                    if (obj.Name == endConditionObjectName)
                    {
                        m_UsedObjectLastSeenTimestamp = now;
                        m_ObjectwasFoundOnce = true;
                    }
                }
                else if (obj.Name == endConditionObjectName)
                {
                    NextWorkingStep(AllEnums.WorkingStepEndConditionTrigger.TRACKABLE_OBJECT);
                    WorkingstepChanged();
                }
            }
        }


        public void saveToCSV(string step)
        {
            var csv = new StringBuilder();

            // check if scnes dir exists
            if (!Directory.Exists(ProjectConstants.STUDY_DIR))
            {
                // if not create it
                Directory.CreateDirectory(ProjectConstants.STUDY_DIR);
            }
            string path = ProjectConstants.STUDY_DIR + "\\Workflow_" + this.m_CsvNameTime + ".csv";


            //var newLine = string.Format("{0},{1},{2}{3}", PBDManager.Instance.StartTime.ToString(), PBDManager.Instance.EndTime.ToString(), PBDManager.Instance.DurationTime.ToString(), Environment.NewLine);
            long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            var newLine = string.Format("{0},{1},{2}{3}", step, now, (now - m_CsvNameTime), Environment.NewLine);
            csv.Append(newLine);

            // File.WriteAllText( path, csv.ToString());
            File.AppendAllText(path, csv.ToString());
        }

        public int calculateAdaptivityLevel()
        {
            if (!SettingsManager.Instance.Settings.AdaptivityEnabled || m_ErrorFreeCount == 0) return SettingsManager.Instance.Settings.AdaptivityLevelId;
            else
            {
                double errorRatio = (double)m_ErrorCount / (double)m_ErrorFreeCount;
                if (errorRatio > SettingsManager.Instance.Settings.AdaptivityThresholdMedium) return 1;
                else if (errorRatio > SettingsManager.Instance.Settings.AdaptivityThresholdHard) return 2;
                else return 3;
            }
        }

        public void RefreshAdaptivityLevel()
        {
            if (!App.Current.Dispatcher.CheckAccess())
            {
                App.Current.Dispatcher.Invoke(new RefreshAdaptivityLevelDelegate(RefreshAdaptivityLevel));
                return;
            }

            SceneManager.Instance.CurrentScene = LoadedWorkflow.WorkingSteps.ElementAt(m_CurrentWorkingStepNumber).getAdaptiveScene(m_adaptivityLevelId).Scene;
        }

        #region GETTER/SETTER
        public Workflow LoadedWorkflow
        {
            get
            {
                return m_LoadedWorkflow;
            }
            set
            {
                m_LoadedWorkflow = value;
                NotifyPropertyChanged("LoadedWorkflow");
            }
        }

        public int CurrentWorkingStepNumber
        {
            get
            {
                return m_CurrentWorkingStepNumber;
            }
            set 
            {
                m_CurrentWorkingStepNumber = value;
                NotifyPropertyChanged("CurrentWorkingStepNumber");
            }
        }

        public WorkingStep CurrentWorkingStep
        {
            get
            {
                WorkingStep ret = null;
                if(m_LoadedWorkflow != null)
                {
                    ret = m_LoadedWorkflow.WorkingSteps.ElementAt(m_CurrentWorkingStepNumber);
                }
                return ret;
            }
        }


        public int BoxErrorCounter
        {
            get
            {
                return m_BoxErrorCounter;
            }
        }

        public int AssemblyErrorCounter
        {
            get
            {
                return m_AssemblyErrors;
            }
        }

        public int ProducedPartsCounter
        {
            get
            {
                return m_producedParts;
            }
            set
            {
                m_producedParts = value;
            }
        }

        public List<AdaptivityLevel> AdaptivityLevels
        {
            get { return m_AdaptivityLevels; }
            set 
            {
                m_AdaptivityLevels = value;  
                NotifyPropertyChanged("AdaptivityLevels");
            }
        }

        public bool QSModeEnabled
        {
            get { return m_QSMode; }
            set { throw new FieldAccessException("QSMode is set automatically from workingstep properties. Global switch not implemented yet"); }
        }

        public int AdaptivityLevelId
        {
            get
            {
                return m_adaptivityLevelId;
            }
            set
            {
                m_adaptivityLevelId = value;
                //For now just remember last selected level in settings
                motionEAPAdmin.ContentProviders.SettingsManager.Instance.Settings.AdaptivityLevelId = value;
                //Change Scene if workflow is currently playing
                if (StateManager.Instance.State == AllEnums.State.WORKFLOW_PLAYING)
                {
                    RefreshAdaptivityLevel();
                }
                NotifyPropertyChanged("AdaptivityLevelId");

            }
        }
        #endregion
        

        #region EVENTS
        protected void OnWorkflowLoaded()
        {
            var e = new WorkflowLoadedEventArgs();
            e.LoadedWorkflow = m_LoadedWorkflow;
            EventHandler<WorkflowLoadedEventArgs> handler = WorkflowLoaded;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void OnWorkingStepCompleted(AllEnums.WorkingStepEndConditionTrigger trigger)
        {
            //Error Tracking
            if (m_ErrorFreeStep) m_ErrorFreeCount++;

            WorkingStepCompletedEventArgs e = new WorkingStepCompletedEventArgs();
            e.WorkingStepNumber = CurrentWorkingStepNumber;
            e.WorkingStep = CurrentWorkingStep;
            e.LoadedWorkflow = LoadedWorkflow;
            e.WorkflowStartTime = m_StartTime;
            e.StepStartTime = m_StepStartTime;
            e.StepEndTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            e.StepDurationTime = e.StepEndTime - e.StepStartTime;
            EventHandler<WorkingStepCompletedEventArgs> handler = WorkingStepCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
            //Trigger WorkflowCompleted event as soon as the last step (end step) is reached
            if (CurrentWorkingStepNumber == m_LoadedWorkflow.WorkingSteps.Count)
            {
                OnWorkflowCompleted();
            }
        }

        protected void OnWorkflowStarted()
        {
            var e = new WorkflowStartedEventArgs();
            e.LoadedWorkflow = m_LoadedWorkflow;
            EventHandler<WorkflowStartedEventArgs> handler = WorkflowStarted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void OnWorkflowStopped()
        {
            var e = new WorkflowStartedEventArgs();
            e.LoadedWorkflow = m_LoadedWorkflow;
            EventHandler<WorkflowStartedEventArgs> handler = WorkflowStarted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void OnWorkflowCompleted()
        {
            var e = new WorkflowStartedEventArgs();
            e.LoadedWorkflow = m_LoadedWorkflow;
            EventHandler<WorkflowStartedEventArgs> handler = WorkflowStarted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void OnAdaptivityLevelChanged(AdaptivityLevelChangedEventArgs e)
        {
            EventHandler<AdaptivityLevelChangedEventArgs> handler = AdaptivityLevelChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void OnWorkingStepStarted()
        {
            //Error Tracking
            m_ErrorFreeStep = true;
            //var calculatedAdaptivityLevel =  calculateAdaptivityLevel();
            if (AdaptivityLevelId != calculatedAdaptivityLevel)
            {
                var oldLevel = AdaptivityLevelId;
                AdaptivityLevelId = calculatedAdaptivityLevel;
                OnAdaptivityLevelChanged(new AdaptivityLevelChangedEventArgs { Reason = "errorRatioChanged", OldLevel = oldLevel });
            }

            m_StepStartTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            WorkingStepStartedEventArgs e = new WorkingStepStartedEventArgs();
            e.WorkingStepNumber = CurrentWorkingStepNumber;
            e.LoadedWorkflow = LoadedWorkflow;
            e.WorkflowStartTime = m_StartTime;
            e.StepStartTime = m_StepStartTime;
            EventHandler<WorkingStepStartedEventArgs> handler = WorkingStepStarted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void OnFailStateOccured(WorkflowFailState failstate)
        {
            FailStateOccuredEventArgs e = new FailStateOccuredEventArgs();
            e.Failstate = failstate;
            e.LoadedWorkflow = m_LoadedWorkflow;
            e.WorkingStepNumber = m_CurrentWorkingStepNumber;
            m_ErrorCount++;
            m_ErrorFreeStep = false;
            var oldLevel = AdaptivityLevelId;
            DecreaseAdaptivityLevel();
            OnAdaptivityLevelChanged(new AdaptivityLevelChangedEventArgs { Reason = "FailStateOccured", OldLevel = oldLevel });
            EventHandler<FailStateOccuredEventArgs> handler = FailStateOccured;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

    }

    #region EVENTARGS

    public class WorkingStepCompletedEventArgs : EventArgs
    {
        public int WorkingStepNumber { get; set; }
        public WorkingStep WorkingStep { get; set; }
        public Workflow LoadedWorkflow { get; set; }
        public long WorkflowStartTime { get; set; }
        public long StepStartTime { get; set; }
        public long StepEndTime { get; set; }
        public long StepDurationTime { get; set; }
        public AllEnums.WorkingStepEndConditionTrigger triggerType { get; set; }
    }

    public class WorkingStepStartedEventArgs : EventArgs
    {
        public int WorkingStepNumber { get; set; }
        public Workflow LoadedWorkflow { get; set; }
        public long StepStartTime { get; set; }
        public long WorkflowStartTime { get; set; }
    }

    public class WorkflowLoadedEventArgs : EventArgs
    {
        public Workflow LoadedWorkflow { get; set; }
    }

    public class WorkflowCompletedEventArgs : EventArgs
    {
        public Workflow LoadedWorkflow { get; set; }
    }

    public class WorkflowStartedEventArgs : EventArgs
    {
        public Workflow LoadedWorkflow { get; set; }
    }

    public class WorkflowStopedEventArgs : EventArgs
    {
        public Workflow LoadedWorkflow { get; set; }
    }

    public class FailStateOccuredEventArgs
    {
        public WorkflowFailState Failstate { get; set; }
        public Workflow LoadedWorkflow { get; set; }
        public int WorkingStepNumber { get; set; }

    }

    public class AdaptivityLevelChangedEventArgs
    {
        public string Reason { get; set; }
        public int OldLevel { get; set; }
    }

    #endregion
}
