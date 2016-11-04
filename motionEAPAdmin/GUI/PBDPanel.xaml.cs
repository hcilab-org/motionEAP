// <copyright file=PBDPanel.xaml.cs
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

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.VideoSurveillance;
using HciLab.motionEAP.InterfacesAndDataModel;
using motionEAPAdmin.Backend;
using motionEAPAdmin.Model.Process;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace motionEAPAdmin.GUI
{
    /// <summary>
    /// Interaktionslogik für PBDPanel.xaml
    /// </summary>
    public partial class PBDPanel : UserControl
    {
        
        private MotionHistory m_MotionHistory;
        private IBGFGDetector<Bgra> m_ForgroundDetector;

        private Image<Bgra, byte> m_LastImage;

        private int[] m_PreviousMoves;
        private int m_Cnt = 0;
        private int m_PreviousMed = 0;
        private const int m_ARRAYSIZE = 10;
        private bool m_Currentlyworking = false;
        private long m_LastWorkingStamp = 0;

        public PBDPanel()
        {
            InitializeComponent();
            StateManager.Instance.StateChange += StateChange;
            this.DataContext = EditWorkflowManager.Instance.CurrentWorkflow;

            m_MotionHistory = new MotionHistory(
                    0.1, //in second, the duration of motion history you wants to keep
                    0.05, //in second, parameter for cvCalcMotionGradient
                    0.5); //in second, parameter for cvCalcMotionGradient


            m_PreviousMoves = new int[m_ARRAYSIZE];
            for (int i = 0; i < m_ARRAYSIZE; i++)
            {
                m_PreviousMoves[i] = 0;
            }
        }

        public void refreshDataContext()
        {
            if (EditWorkflowManager.Instance.CurrentWorkflow != null)
            {
                this.DataContext = EditWorkflowManager.Instance.CurrentWorkflow;
            }
        }

        private void StateChange(object pSource, HciLab.motionEAP.InterfacesAndDataModel.AllEnums.State pState)
        {
            if(pState == HciLab.motionEAP.InterfacesAndDataModel.AllEnums.State.IDLE)
            {
                m_ButtonStart.IsEnabled = true;
                m_ButtonStop.IsEnabled = false;
                m_ButtonContinue.IsEnabled = false;
                m_ButtonSaveWorkFlow.IsEnabled = true;
                m_ButtonLoadWorkFlow.IsEnabled = false;
                m_ButtonRemoveWorkingStep.IsEnabled = false;
                m_ButtonUp.IsEnabled = false;
                m_ButtonDown.IsEnabled = false;
            }
            else if (pState == HciLab.motionEAP.InterfacesAndDataModel.AllEnums.State.RECORD)
            {
                m_ButtonStart.IsEnabled = false;
                m_ButtonStop.IsEnabled = true;
                m_ButtonContinue.IsEnabled = true;
                m_ButtonSaveWorkFlow.IsEnabled = false;
                m_ButtonLoadWorkFlow.IsEnabled = false;
                m_ButtonRemoveWorkingStep.IsEnabled = true;
                m_ButtonUp.IsEnabled = true;
                m_ButtonDown.IsEnabled = true;
            }
            else if (pState == HciLab.motionEAP.InterfacesAndDataModel.AllEnums.State.RECORD_PAUSED)
            {
                m_ButtonStart.IsEnabled = false;
                m_ButtonStop.IsEnabled = true;
                m_ButtonContinue.IsEnabled = true;
                m_ButtonSaveWorkFlow.IsEnabled = false;
                m_ButtonLoadWorkFlow.IsEnabled = false;
                m_ButtonRemoveWorkingStep.IsEnabled = false;
                m_ButtonUp.IsEnabled = false;
                m_ButtonDown.IsEnabled = false;
            }
            else
            {
                m_ButtonStart.IsEnabled = false;
                m_ButtonStop.IsEnabled = false;
                m_ButtonContinue.IsEnabled = false;
                m_ButtonSaveWorkFlow.IsEnabled = false;
                m_ButtonLoadWorkFlow.IsEnabled = false;
                m_ButtonRemoveWorkingStep.IsEnabled = false;
                m_ButtonUp.IsEnabled = false;
                m_ButtonDown.IsEnabled = false;
            }
        }

        private void buttonSaveWorkFlow_Click(object sender, RoutedEventArgs e)
        {
            if (StateManager.Instance.State == AllEnums.State.IDLE)
            {
                if (EditWorkflowManager.Instance.CurrentWorkflow != null)
                {
                    if (EditWorkflowManager.Instance.CurrentWorkflow.WorkingSteps.Count > 0)
                    {
                        // there is something that was created
                        // start saving 
                        EditWorkflowManager.Instance.CurrentWorkflow.Name = textBoxPBDWorkflowName.Text;
                        EditWorkflowManager.Instance.CurrentWorkflow.Description = textBoxPBDWorkflowDescription.Text;
                        EditWorkflowManager.Instance.SaveCurrentWorkflow(true);

                    }
                }
            }
        }

        private void pauseContinueButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (StateManager.Instance.State == HciLab.motionEAP.InterfacesAndDataModel.AllEnums.State.RECORD)
            {
                // => Pause
                bool success = PBDManager.Instance.pausePBD();
                if (success)
                {
                    m_LabelStatus.Content = "Status: Paused";
                }

            }
            else if (StateManager.Instance.State == HciLab.motionEAP.InterfacesAndDataModel.AllEnums.State.RECORD_PAUSED)
            {
                // => Record
                

                bool success = PBDManager.Instance.continuePBD();
                if(success)
                {
                    m_LabelStatus.Content = "Status: Record";
                }

            }

        }


        private void downButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = treeViewWorkflow.SelectedItem;
            if (selectedItem is WorkingStep)
            {
                WorkingStep workingStepItem = (WorkingStep)selectedItem;
                int oldindex = workingStepItem.StepNumber;
                int index = workingStepItem.StepNumber;
                int currentStep = workingStepItem.StepNumber;

                if (currentStep != EditWorkflowManager.Instance.HighestWorkingStepNumber)
                // if (EditWorkflowManager.Instance.CurrentWorkflow.WorkingSteps.Count > 0)
                {
                    foreach (WorkingStep step in EditWorkflowManager.Instance.CurrentWorkflow.WorkingSteps)
                    {
                        if (step.StepNumber == currentStep + 1)
                        {

                            // if (step.StepNumber != oldindex - 1)
                            //    {
                            step.StepNumber = currentStep;
                            step.createNewName(step.Mode);
                            //   }
                        }
                        if (step.StepNumber == (currentStep - 1))
                        {

                        }
                    }
                    workingStepItem.StepNumber = oldindex + 1;
                    workingStepItem.createNewName(workingStepItem.Mode);
                    EditWorkflowManager.Instance.CurrentWorkflow.WorkingSteps.Move(oldindex, oldindex + 1);
                    treeViewWorkflow.DataContext = EditWorkflowManager.Instance.CurrentWorkflow;
                }
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            bool success = PBDManager.Instance.startPBD();
            if (success)
            {
                PBDManager.Instance.StartTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                m_TextBoxTimeStart.Text = new DateTime(PBDManager.Instance.StartTime).ToLongDateString();
                
                PBDManager.Instance.CsvNameTime = PBDManager.Instance.StartTime;
                m_LabelStatus.Content = "Status: Running";

                // set the databinding
                treeViewWorkflow.DataContext = EditWorkflowManager.Instance.CurrentWorkflow;
                EditWorkflowManager.Instance.CurrentWorkingStepNumber = 0;

                if (textBoxPBDWorkflowDescription.Text.Equals(""))
                {
                    textBoxPBDWorkflowDescription.Text = "Diese Anleitung wurde mit MotionEAP per \"programming by demonstration\" erstellt";
                    //this.textBoxPBDWorkflowDescription.Text = "This workflow is created by \"programming by demonstration\" and MotionEAP";
                }
            }
        }

        private void MenuItem_DeleteDemoScene(object sender, RoutedEventArgs e)
        {

        }

        private void upButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = treeViewWorkflow.SelectedItem;
            if (selectedItem is WorkingStep)
            {
                WorkingStep workingStepItem = (WorkingStep)selectedItem;

                int oldindex = workingStepItem.StepNumber;
                int index = workingStepItem.StepNumber;
                int currentStep = workingStepItem.StepNumber;

                if (currentStep > 0)
                {

                    foreach (WorkingStep step in EditWorkflowManager.Instance.CurrentWorkflow.WorkingSteps)
                    {
                        if (step.StepNumber == currentStep - 1)
                        {

                            // if (step.StepNumber != oldindex - 1)
                            //    {
                            step.StepNumber = currentStep;
                            step.createNewName(step.Mode);
                            //   }
                        }
                        if (step.StepNumber == (currentStep + 1))
                        {

                        }
                    }
                    workingStepItem.StepNumber = oldindex - 1;
                    workingStepItem.createNewName(workingStepItem.Mode);
                    EditWorkflowManager.Instance.CurrentWorkflow.WorkingSteps.Move(oldindex, oldindex - 1);
                    treeViewWorkflow.DataContext = EditWorkflowManager.Instance.CurrentWorkflow;
                }
            }
        }

        private void buttonLoadWorkFlow_Click(object sender, RoutedEventArgs e)
        {
            StopButton_Click(null, null);
            PBDManager.Instance.loadPBDWorkFlow();

        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            bool success = PBDManager.Instance.stopPBD();
            if (success)
            {
                m_LabelStatus.Content = "Status: Stopped";
                PBDManager.Instance.EndTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                m_TextBoxTimeEnd.Text = new DateTime(PBDManager.Instance.EndTime).ToLongDateString();


                PBDManager.Instance.DurationTime = (PBDManager.Instance.EndTime - PBDManager.Instance.StartTime);
                m_TextBoxTimeSpan.Text = new DateTime(PBDManager.Instance.DurationTime).ToLongDateString();
            }

        }

        private void buttonRemoveWorkingStep_Click(object sender, RoutedEventArgs e)
        {
            // delete currently selected item
            var selectedItem = treeViewWorkflow.SelectedItem;

            if (selectedItem is WorkingStep)
            {
                WorkingStep workingStepItem = (WorkingStep)selectedItem;
                EditWorkflowManager.Instance.CurrentWorkflow.WorkingSteps.Remove(workingStepItem);
                EditWorkflowManager.Instance.CurrentWorkingStepNumber -= 1;
                EditWorkflowManager.Instance.HighestWorkingStepNumber = EditWorkflowManager.Instance.HighestWorkingStepNumber;

            }
        }

        public void checkIfUserIsWorking(Image<Bgra, byte> image)
        {
            // is pbd active at all ?
            if (StateManager.Instance.State == AllEnums.State.RECORD)
            {
                if (isUserWorking2(image))
                {
                    // user is working - set the values
                    PBDManager.Instance.lastWorkingTimestampInMillis = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    PBDManager.Instance.IsUserWorking = true;
                    PBDManager.Instance.AlreadyDetectedAssembly = false;


                    // DEBUG only
                    //SceneManager.Instance.TemporaryDebugScene.Clear();
                    //Scene.SceneRect rect = new Scene.SceneRect(null, 0.5f, 0.5f, 0.2f, 0.2f, System.Windows.Media.Color.FromRgb(0, 0, 255));
                    //SceneManager.Instance.TemporaryDebugScene.Add(rect);

                }
                else
                {
                    // user not working anymore
                    PBDManager.Instance.IsUserWorking = false;

                    // DEBUG only
                    // SceneManager.Instance.TemporaryDebugScene.Clear();
                }
            }

        }

        private bool isUserWorking2(Image<Bgra, byte> image)
        {

            if (m_LastImage == null)
            {
                m_LastImage = image.Clone();
                return false;
            }

            Image<Bgra, byte> diffFrame = image.AbsDiff(m_LastImage);
            int moves = diffFrame.CountNonzero()[0];
            m_PreviousMoves[m_Cnt] = moves;
            m_Cnt++;
            if (m_Cnt > m_ARRAYSIZE - 1)
            {
                m_Cnt = 0;
                int median = calculateMedian();
                int diff = Math.Abs(median - m_PreviousMed);
                
                if (diff > 300)
                {
                    m_Currentlyworking = true;
                    m_LastWorkingStamp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                }
                else
                {
                    long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    if (now - m_LastWorkingStamp > 1000) // 1 second cooldown
                    {
                        m_Currentlyworking = false;
                    }
                }

                m_PreviousMed = median;
            }


            /*
            if (moves > 38940) // value = wtf?
            {
                ret = true;
            }
            */


            m_LastImage = image.Clone();

            return m_Currentlyworking;
        }

        private int calculateMedian()
        {


            var ys = m_PreviousMoves.OrderBy(x => x).ToList();
            double mid = (ys.Count - 1) / 2.0;
            int med = (int)(ys[(int)(mid)] + ys[(int)(mid + 0.5)]) / 2;
            return med;
        }

        private bool isUserWorking(Image<Bgra, byte> image)
        {
            bool ret = false;
            using (MemStorage storage = new MemStorage()) //create storage for motion components
            {
                if (m_ForgroundDetector == null)
                {
                    //_forgroundDetector = new BGCodeBookModel<Bgr>();
                    //_forgroundDetector = new FGDetector<Bgr>(Emgu.CV.CvEnum.FORGROUND_DETECTOR_TYPE.FGD);
                    m_ForgroundDetector = new BGStatModel<Bgra>(image, Emgu.CV.CvEnum.BG_STAT_TYPE.FGD_STAT_MODEL);
                }

                m_ForgroundDetector.Update(image);

                // capturedImageBox.Image = image;

                //update the motion history
                m_MotionHistory.Update(m_ForgroundDetector.ForegroundMask);

                #region get a copy of the motion mask and enhance its color
                double[] minValues, maxValues;
                System.Drawing.Point[] minLoc, maxLoc;
                m_MotionHistory.Mask.MinMax(out minValues, out maxValues, out minLoc, out maxLoc);
                Image<Gray, Byte> motionMask = m_MotionHistory.Mask.Mul(255.0 / maxValues[0]);
                #endregion

                //create the motion image
                Image<Bgra, Byte> motionImage = new Image<Bgra, byte>(motionMask.Size);
                //display the motion pixels in blue (first channel)
                motionImage[0] = motionMask;

                //Threshold to define a motion area, reduce the value to detect smaller motion
                double minArea = 100;

                storage.Clear(); //clear the storage
                Seq<MCvConnectedComp> motionComponents = m_MotionHistory.GetMotionComponents(storage);

                //iterate through each of the motion component
                foreach (MCvConnectedComp comp in motionComponents)
                {
                    //reject the components that have small area;
                    if (comp.area < minArea) continue;

                    // find the angle and motion pixel count of the specific area
                    double angle, motionPixelCount;
                    m_MotionHistory.MotionInfo(comp.rect, out angle, out motionPixelCount);

                    //reject the area that contains too few motion
                    if (motionPixelCount < comp.area * 0.05) continue;

                    //Draw each individual motion in red
                    // DrawMotion(motionImage, comp.rect, angle, new Bgr(Color.Red));
                }

                // find and draw the overall motion angle
                double overallAngle, overallMotionPixelCount;
                m_MotionHistory.MotionInfo(motionMask.ROI, out overallAngle, out overallMotionPixelCount);
                //                DrawMotion(motionImage, motionMask.ROI, overallAngle, new Bgr(Color.Green));

                //Display the amount of motions found on the current image
                //UpdateText(String.Format("Total Motions found: {0}; Motion Pixel count: {1}", motionComponents.Total, overallMotionPixelCount));
                //Console.WriteLine("motions: " + motionComponents.Total);

                if (motionComponents.Total > 0)
                {
                    ret = true;
                }

            }

            return ret;
        }
    }
}
