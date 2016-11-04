// <copyright file=PBDManager.cs
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

using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using motionEAPAdmin.Backend.AssembleyZones;
using motionEAPAdmin.Backend.Boxes;
using motionEAPAdmin.Backend.ObjectDetection;
using motionEAPAdmin.Model.Process;
using HciLab.motionEAP.InterfacesAndDataModel.Data;
using HciLab.motionEAP.InterfacesAndDataModel;
using HciLab.Utilities;
using motionEAPAdmin.GUI;
using HciLab.Kinect;

namespace motionEAPAdmin.Backend
{
    public class PBDManager
    {
        private static PBDManager m_Instance;


        //private bool m_IsRecRunning = false;
        private long m_StartTime;
        private long m_EndTime;
        private long m_DurationTime;

        private bool m_IsUserWorking = false;
        private long m_lastWorkingTimestampInMillis = 0;
        private long m_LlastWithdrawlTimestampInMillis = 0;
        private long m_CreatedFeedbackTimestampInMillis = 0;
        private bool m_BlockUntilObjectIsBackAgain = false;

        private bool m_BlockAssemblyCheck = false;
        private bool m_AlreadyDetectedAssembly = false;
        private long m_CsvNameTime;

        private const int m_FeedbackShow_Time = 5000;

        #region CONSTRUCTOR
        private PBDManager() {
        }
        #endregion

       

        /* public void deleteStep(int mode, AdaptiveScene adaptiveScene, string name, string endCondition)
        {
            // delete currently selected item
            var selectedItem = treeViewWorkflow.SelectedItem;
            if (selectedItem is WorkingStep)
            {
                WorkingStep workingStepItem = (WorkingStep)selectedItem;
                EditWorkflowManager.Instance.CurrentWorkflow.WorkingSteps.Remove(workingStepItem);
            }
        }
        */
        public bool startPBD()
        {

            bool ret = false;
            if (StateManager.Instance.State == AllEnums.State.IDLE)
            {
                //TODO: check if a previously demonstrated workflow is there

                if (BoxManager.Instance.CurrentLayout.Boxes.Count == 0)
                {
                    // Messagebox: no layout
                    //System.Windows.MessageBox.Show("No BoxLayout was defined! Please load or create a Boxlayout first.", "Problem", MessageBoxButton.OK, MessageBoxImage.Error);
                    System.Windows.MessageBox.Show("Kein BoxLayout vorhanden! Bitte laden oder erstellen Sie ein Boxlayout.", "Problem", MessageBoxButton.OK, MessageBoxImage.Error);
                   
                    return false;
                }
                else
                {
                    // create a new workflow
                    EditWorkflowManager.Instance.CurrentWorkflow = new Workflow();
                    EditWorkflowManager.Instance.CurrentWorkflow.BoxLayout = BoxManager.Instance.CurrentLayout;
                    //AssemblyZoneManager.Instance.CurrentLayout = new AssemblyZoneLayout(); // Assemblyzones are different -.-
                    AssemblyZoneManager.Instance.CurrentLayout.Clear();
                    EditWorkflowManager.Instance.CurrentWorkflow.AssemblyZoneLayout = AssemblyZoneManager.Instance.CurrentLayout;
                    EditWorkflowManager.Instance.CurrentWorkflow.ObjectZoneLayout = ObjectDetectionManager.Instance.CurrentLayout;
                    StateManager.Instance.SetNewState(this, AllEnums.State.RECORD);
                    m_BlockAssemblyCheck = true; // first step has to be withdrawl
                    ret = true;

                    // create the first snapshot
                    AssemblyZoneManager.Instance.createDepthSnapshot();

                    // check which object is currently placed on the zone
                    ObjectDetectionManager.Instance.InitPBDLogic();

                }
            }

            return ret;
        }

        public bool stopPBD()
        {
            bool ret = false;
            if (StateManager.Instance.State == AllEnums.State.RECORD || StateManager.Instance.State == AllEnums.State.RECORD_PAUSED)
            {
                StateManager.Instance.SetNewState(this, AllEnums.State.IDLE);
                EditWorkflowManager.Instance.createEndScene("Endcondition,", "end");

                ret = true;
            }
            return ret;
        }

        public bool pausePBD()
        {
            bool ret = false;
            if(StateManager.Instance.State == AllEnums.State.RECORD)
            {

                StateManager.Instance.SetNewState(this, AllEnums.State.RECORD_PAUSED);
                ret = true;
            }

            return ret;
        }

        public bool continuePBD()
        {
            bool ret = false;
            if (StateManager.Instance.State == AllEnums.State.RECORD_PAUSED)
            {

                StateManager.Instance.SetNewState(this, AllEnums.State.RECORD);
                ret = true;
            }

            return ret;
        }

        // is called whenever box is triggered
        public void OnBoxTriggeredPBD(Box box)
        {
            // check if rec is running
            if (StateManager.Instance.State == AllEnums.State.RECORD && !m_BlockUntilObjectIsBackAgain)
            {

                AdminView.Instance.Dispatcher.Invoke(
               System.Windows.Threading.DispatcherPriority.Normal,
               new Action(
                   () =>
                   {
                       // create a scene for displaying the withdrawel from the box
                       Scene.Scene autoScene = new Scene.Scene();
                       autoScene.Add(box.getDrawable(true));

                       AdaptiveScene adaptiveScene = new AdaptiveScene(autoScene, AdaptivityLevel.AdaptivityLevels.FirstOrDefault());

                       EditWorkflowManager.Instance.createStep(AllEnums.PBD_Mode.BOX_WITHDRAWEL, adaptiveScene, "Box-" + box.Id, box.TriggerMessage);

                       // temporarilly - only allow one assembly step per withdrawl
                       m_BlockAssemblyCheck = false;
                       m_LlastWithdrawlTimestampInMillis = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                   }
               )
           );

            }
        }

        public void OnObjectWasTakenTrigger(TrackableObject obj)
        {
            ObjectDetectionZone myZone = null;
            // find the zone that the object was last seen
            foreach( ObjectDetectionZone zone in ObjectDetectionManager.Instance.CurrentLayout.ObjectDetectionZones)
            {
                if(zone.Id == obj.LastSeenZoneId)
                {
                    // found the zone
                    myZone = zone;
                }
            }

             // check if rec is running
            if (StateManager.Instance.State == AllEnums.State.RECORD && !m_BlockUntilObjectIsBackAgain)
            {
                // create a scene for displaying the picking of the object
                Scene.Scene autoScene = new Scene.Scene();
                Scene.SceneRect rect = ObjectDetectionManager.Instance.createSceneRectForObjectDetectionZone(myZone, true);
                autoScene.Add(rect);

                AdaptiveScene adaptiveScene = new AdaptiveScene(autoScene, AdaptivityLevel.AdaptivityLevels.FirstOrDefault());

                EditWorkflowManager.Instance.createStep(AllEnums.PBD_Mode.OBJECT_RECOGNIZED, adaptiveScene, "Benutze Objekt-" + obj.Name, obj.Name);

                m_BlockUntilObjectIsBackAgain = true;
            }
        }

        public void disablePBDBlockAgain(){
            if (StateManager.Instance.State == AllEnums.State.RECORD)
            {
                m_BlockUntilObjectIsBackAgain = false;
            }
        }

        public void loadPBDWorkFlow()
        {
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.InitialDirectory = Directory.GetCurrentDirectory() + "\\" + ProjectConstants.WORKFLOW_DIR;
            dlg.Filter = "workflow files (*.work)|*.work";
            dlg.FilterIndex = 2;
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            Workflow w = new Workflow();
            bool isOkay = UtilitiesIO.GetObjectFromJson(ref w, dlg.FileName);

            if (!isOkay)
                return;

            EditWorkflowManager.Instance.CurrentWorkflow = w;
            //AdminView.Instance.SetWorkflowTextBlockDescription(EditWorkflowManager.Instance.CurrentWorkflow.Description);

            // set the databinding
            BackendControl.Instance.refreshGUI();
            EditWorkflowManager.Instance.CurrentWorkingStepNumber = EditWorkflowManager.Instance.HighestWorkingStepNumber;
            
        }

        /*public void saveToCSV(string step)
        {
            var csv = new StringBuilder();
            
            // check if scnes dir exists
            if (!Directory.Exists(ProjectConstants.STUDY_DIR))
            { 
                // if not create it
                Directory.CreateDirectory(ProjectConstants.STUDY_DIR);
            }
            string path = ProjectConstants.STUDY_DIR+ "\\PBD_"+ this.m_CsvNameTime +".csv";


            //var newLine = string.Format("{0},{1},{2}{3}", PBDManager.Instance.StartTime.ToString(), PBDManager.Instance.EndTime.ToString(), PBDManager.Instance.DurationTime.ToString(), Environment.NewLine);
            long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            var newLine = string.Format("{0},{1},{2}{3}", step, now, now - m_CsvNameTime, Environment.NewLine);
            csv.Append(newLine);

            // File.WriteAllText( path, csv.ToString());
            File.AppendAllText(path, csv.ToString());
        }*/

        public void checkAutomaticallyForAssemblyStep()
        {
            // PDB active?
            if (StateManager.Instance.State == AllEnums.State.RECORD && !m_BlockAssemblyCheck && !m_BlockUntilObjectIsBackAgain)
            {
                long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

                if (now - m_LlastWithdrawlTimestampInMillis > 10000) // was last withdrawl at least 10 seconds ago?
                {

                    if (now - m_lastWorkingTimestampInMillis > 3000) //threshold
                    {

                        //did we already take a snapshot of this period
                        if (!m_AlreadyDetectedAssembly)
                        {

                            // user might be waiting for automatically created step
                            // check if something happened
                            AssemblyZone z = AssemblyZoneManager.Instance.createAssemblyZoneFromChanges();

                            if (z != null)
                            {
                                // block assembly until next pick
                                m_BlockAssemblyCheck = true;

                                // create a scene for displaying assemblyzone and add it to workflow
                                Scene.Scene autoScene = new Scene.Scene();
                                autoScene.Add(z.getDrawable(true));
                                AdaptiveScene adaptiveScene = new AdaptiveScene(autoScene, AdaptivityLevel.AdaptivityLevels.FirstOrDefault());
                                EditWorkflowManager.Instance.createStep(AllEnums.PBD_Mode.ASSEMBLY_DONE, adaptiveScene, "Zone-" + z.Id, z.TriggerMessage);
                                AssemblyZoneManager.Instance.CurrentLayout.AssemblyZones.Add(z);
                                m_AlreadyDetectedAssembly = true;

                                // temporarilty display feedback that zone was detected
                                Scene.Scene feedbackScene = new Scene.Scene();
                                Scene.SceneText text = new Scene.SceneText(0.5 * KinectManager.Instance.ImageSize.Width, 0.5 * KinectManager.Instance.ImageSize.Height, "Step created", System.Windows.Media.Color.FromRgb(255, 255, 255), 10.0, new System.Windows.Media.FontFamily("Arial"));
                                feedbackScene.Add(text);
                                SceneManager.Instance.TemporaryFeedbackScene = feedbackScene;
                                SceneManager.Instance.DisplayTempFeedback = true;
                                m_CreatedFeedbackTimestampInMillis = now;
                            }

                            // create a new snapshot
                            if (!m_IsUserWorking)
                            {
                                AssemblyZoneManager.Instance.createDepthSnapshot();
                            }
                        }
                    }
                }

            }

            // maybe find a better place for this:
            // check if the feedback should still be displayed
            long nower = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            if (nower - m_CreatedFeedbackTimestampInMillis > m_FeedbackShow_Time)
            {
                SceneManager.Instance.DisplayTempFeedback = false;
                SceneManager.Instance.TemporaryFeedbackScene.Clear();
            }

        }

        #region GETTER / SETTER
        public static PBDManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new PBDManager();
                }
                return m_Instance;
            }
        }

        public long lastWorkingTimestampInMillis
        {
            get { return m_lastWorkingTimestampInMillis; }
            set { m_lastWorkingTimestampInMillis = value; }
        }

        public bool AlreadyDetectedAssembly
        {
            get { return m_AlreadyDetectedAssembly; }
            set { m_AlreadyDetectedAssembly = value; }
        }

        public long CsvNameTime
        {
            get { return m_CsvNameTime; }
            set { m_CsvNameTime = value; }
        }

        public bool IsUserWorking
        {
            get
            {
                return m_IsUserWorking;
            }
            set
            {
                m_IsUserWorking = value;
            }
        }

        public long LastWorkingTimestampInMillis
        {
            get
            {
                return m_lastWorkingTimestampInMillis;
            }
            set
            {
                m_lastWorkingTimestampInMillis = value;
            }
        }
        
        public long StartTime
        {
            get
            {
                return m_StartTime;
            }
            set
            {
                m_StartTime = value;
            }
        }

        public long EndTime
        {
            get
            {
                return m_EndTime;
            }
            set
            {
                m_EndTime = value;
            }
        }

        public long DurationTime
        {
            get
            {
                return m_DurationTime;
            }
            set
            {
                m_DurationTime = value;
            }
        }
        #endregion
    }
}
