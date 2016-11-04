// <copyright file=StudyLogger.cs
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

namespace motionEAPAdmin.Backend
{
    class StudyLogger
    {

        private static StudyLogger m_Instance;
        private StreamWriter m_StepWriter;
        private StreamWriter m_EventWriter;

        private StudyLogger() 
        {
            m_StepWriter = File.AppendText("study.csv");
            m_EventWriter = File.AppendText("studyevents.csv");


            // listen to the events
            WorkflowManager.Instance.WorkingStepCompleted += WorkingStepCompleted;
            WorkflowManager.Instance.AdaptivityLevelChanged += OnAdaptivityLevelChanged;
        }

        private void WorkingStepCompleted(object sender, WorkingStepCompletedEventArgs e)
        {
            logStep(e.LoadedWorkflow.Id + e.LoadedWorkflow.Name, WorkflowManager.Instance.ProducedPartsCounter, e.StepDurationTime, WorkflowManager.Instance.BoxErrorCounter, 
                WorkflowManager.Instance.AssemblyErrorCounter, WorkflowManager.Instance.AdaptivityLevelId);
        }

        private void OnAdaptivityLevelChanged(object sender, AdaptivityLevelChangedEventArgs e)
        {
            logAdaptivityChanged(e.OldLevel, WorkflowManager.Instance.AdaptivityLevelId, e.Reason);
        }

        public static StudyLogger Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new StudyLogger();
                }
                return m_Instance;
            }
        }

        // log when a working step was finished
        public void logStep(string workflow_id, int number_produced_parts, long timeTakenforStep, int pickingerrors, int assemblyerrors, int adaptivityId)
        {
            // #identifier; timestamp; datestamp; workflow; number_produced_parts; timeTakenforStep; pickingerrors; assemblyerrors; adaptivityId
            m_StepWriter.WriteLine("step performed" + ";" + DateTime.Now.ToLongTimeString() + ";" + DateTime.Now.ToLongDateString() + ";" +
                workflow_id + ";" + number_produced_parts + ";" + timeTakenforStep + ";" + pickingerrors + ";" + assemblyerrors + ";" + adaptivityId);
            m_StepWriter.Flush();
        }

        // working step is moved back by the worker using the footpedal
        public void logFootpedalPressedForward()
        {
            m_EventWriter.WriteLine("footpedal_pressed_forward" + ";" + DateTime.Now.ToLongTimeString() + ";" + DateTime.Now.ToLongDateString());
            m_EventWriter.Flush();
        }

        // working step is advanced by the worker using the footpedal
        public void logFootpedalPressedBackward()
        {
            m_EventWriter.WriteLine("footpedal_pressed_backward" + ";" + DateTime.Now.ToLongTimeString() + ";" + DateTime.Now.ToLongDateString());
            m_EventWriter.Flush();
        }

        // adaptivity level changed
        public void logAdaptivityChanged(int p_from_level, int p_to_level, string p_reason)
        {
            m_EventWriter.WriteLine("adaptivity_changed" + ";" + DateTime.Now.ToLongTimeString() + ";" + DateTime.Now.ToLongDateString() + ";" + p_from_level + ";" + p_to_level + ";" + p_reason);
            m_EventWriter.Flush();
        }


        // workflow loaded
        public void logWorkflowLoaded(string p_workflowid, string p_workflowName)
        {
            m_EventWriter.WriteLine("workflow_loaded" + ";" + DateTime.Now.ToLongTimeString() + ";" + DateTime.Now.ToLongDateString() + ";" + p_workflowid + ";" + p_workflowName);
            m_EventWriter.Flush();
        }


        // log when the app is started
        public void logApplicationStarted()
        {
            m_EventWriter.WriteLine("application_started" + ";" + DateTime.Now.ToLongTimeString() + ";" + DateTime.Now.ToLongDateString());
            m_EventWriter.Flush();
        }

    }
}
