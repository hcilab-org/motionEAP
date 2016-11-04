// <copyright file=WorkflowPanel.xaml.cs
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

using System.Windows;
using System.Windows.Controls;
using motionEAPAdmin.Backend;
using MouseKeyboardActivityMonitor;
using MouseKeyboardActivityMonitor.WinApi;
using motionEAPAdmin.Network;
using HciLab.motionEAP.InterfacesAndDataModel;

namespace motionEAPAdmin.GUI
{
    /// <summary>
    /// Interaktionslogik für Workflow.xaml
    /// </summary>
    public partial class WorkflowPanel : UserControl
    {
        private readonly KeyboardHookListener m_KeyboardHookManager;




        public WorkflowPanel()
        {
            InitializeComponent();
            StateManager.Instance.StateChange += StateChange;
            this.DataContext = WorkflowManager.Instance;

            m_KeyboardHookManager = new KeyboardHookListener(new GlobalHooker());
            m_KeyboardHookManager.Enabled = true;
            m_KeyboardHookManager.KeyDown += Presenter_KeyDown;
            m_KeyboardHookManager.KeyDown += FootPedal_KeyDown;

            //TODO: Bind Combobox Value to WorkflowManager Property, might be tricky
            m_ComboboxAdaptivityLevel.DataContext = WorkflowManager.Instance;
            //m_ComboboxAdaptivityLevel.SelectedValue = WorkflowManager.Instance.AdaptivityLevelId;

            m_OutputProducedParts.DataContext = CommunicationManager.Instance.ServerInfo;

        }


        private void Presenter_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            /*
          * Right Button
          */
            if (e.Modifiers == System.Windows.Forms.Keys.None && e.KeyValue == 34)
            {
                buttonNext_Click(sender, null);
            }
            /*
             * Left Button
             */
            else if (e.Modifiers == System.Windows.Forms.Keys.None && e.KeyValue == 33)
            {
                buttonPrevious_Click(sender, null);
            }
            /*
             * Diplay Play Button
             */
            else if (e.Modifiers == System.Windows.Forms.Keys.None && e.KeyValue == 27)
            { }
            /*
             * Display Button
             */
            else if (e.Modifiers == System.Windows.Forms.Keys.None && e.KeyValue == 190)
            { 
            }

        }


        private void FootPedal_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {

            // pedal forward
            if (e.Modifiers == System.Windows.Forms.Keys.None && e.KeyValue == 35)  // Home
            {
                WorkflowManager.Instance.NextWorkingStep(AllEnums.WorkingStepEndConditionTrigger.FOOTPEDAL);

                // log the message
                StudyLogger.Instance.logFootpedalPressedForward();

            }


            //pedal backwards
            else if (e.Modifiers == System.Windows.Forms.Keys.None && e.KeyValue == 36)  // End
            {
                buttonPrevious_Click(sender, null);

                // log the message
                StudyLogger.Instance.logFootpedalPressedBackward();
            }

        }


        private void StateChange(object pSource, HciLab.motionEAP.InterfacesAndDataModel.AllEnums.State pState)
        {
            if (pState == HciLab.motionEAP.InterfacesAndDataModel.AllEnums.State.IDLE)
            {
                m_ButtonStartWorkflow.IsEnabled = false;
                m_ButtonStopWorkflow.IsEnabled = false;
                m_ButtonPrevious.IsEnabled = false;
                m_ButtonNext.IsEnabled = false;
            }
            else if (pState == HciLab.motionEAP.InterfacesAndDataModel.AllEnums.State.WORKFLOW_LOADED)
            {
                m_ButtonStartWorkflow.IsEnabled = true;
                m_ButtonStopWorkflow.IsEnabled = false;
                m_ButtonPrevious.IsEnabled = false;
                m_ButtonNext.IsEnabled = false;
            }
            else if (pState == HciLab.motionEAP.InterfacesAndDataModel.AllEnums.State.WORKFLOW_PLAYING)
            {
                m_ButtonStartWorkflow.IsEnabled = false;
                m_ButtonStopWorkflow.IsEnabled = true;
                m_ButtonPrevious.IsEnabled = true;
                m_ButtonNext.IsEnabled = true;
            }
            else
            {
                m_ButtonStartWorkflow.IsEnabled = false;
                m_ButtonStopWorkflow.IsEnabled = false;
                m_ButtonPrevious.IsEnabled = false;
                m_ButtonNext.IsEnabled = false;
            }
        }

        /// <summary>
        /// Needed that hack-ish method to make the listview not selectable
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">SelectionChangedEventArgs</param>
        /// 
        /// 
        private void listViewWorkingStepCarrier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listViewWorkingStepCarrier.SelectedIndex = -1;
        }

        private void buttonPrevious_Click(object sender, RoutedEventArgs e)
        {
            WorkflowManager.Instance.PreviousWorkingStep();
            AdminView.Instance.refreshWorkflowUI();
        }

        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            WorkflowManager.Instance.NextWorkingStep(AllEnums.WorkingStepEndConditionTrigger.WORKFLOWPANEL_BUTTON);
            AdminView.Instance.refreshWorkflowUI();
        }

        private void startWorkflowButton_Click(object sender, RoutedEventArgs e)
        {
            WorkflowManager.Instance.startWorkflow();

            AdminView.Instance.refreshWorkflowUI();
        }

        private void stopWorkflowButton_Click(object sender, RoutedEventArgs e)
        {
            WorkflowManager.Instance.stopWorkflow();
            AdminView.Instance.refreshWorkflowUI();
        }

        private void m_ComboboxAdaptivityLevel_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (m_ComboboxAdaptivityLevel.SelectedValue != null)
            {
                WorkflowManager.Instance.AdaptivityLevelId = (int)m_ComboboxAdaptivityLevel.SelectedValue;
            }
        }

        private void m_buttonResetPartsProduced_Click(object sender, RoutedEventArgs e)
        {
            if (CommunicationManager.Instance.ServerInfo.SelfStatus != null)
            { 
                CommunicationManager.Instance.setProducedParts(0);
                CommunicationManager.Instance.ServerInfo.SelfStatus.ProducedParts = 0;
            }
        }
    }
}
