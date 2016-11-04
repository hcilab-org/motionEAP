// <copyright file=StatsManager.cs
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
// <date> 11/2/2016 12:26:00 PM</date>

﻿using HciLab.motionEAP.InterfacesAndDataModel;
using motionEAPAdmin.Backend;
using motionEAPAdmin.Model.Process;
using System;
 using System.ComponentModel;
using System.Linq;

namespace motionEAPAdmin.Statistics
{
    class StatsManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private static StatsManager m_Instance;

        private bool m_Initialized = false;

        private WorkflowStats m_CurrentWorkflowStats = null;

        public static StatsManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new StatsManager();
                }
                return m_Instance;
            }
        }

        public void initialize()
        {
            if (!m_Initialized) {
                var handle = WorkflowManager.Instance;
                handle.WorkflowLoaded += handle_WorkflowLoaded;
                //handle.WorkflowStarted += handle_WorkflowStarted;
                handle.WorkflowCompleted += handle_WorkflowCompleted;
                //handle.WorkingStepStarted += handle_WorkingStepStarted;
                handle.WorkingStepCompleted += handle_WorkingStepCompleted;
                handle.FailStateOccured += handle_FailStateOccured;
                //handle.AdaptivityLevelChanged += handle_AdaptivityLevelChanged;
                m_Initialized = true;
            }
            
        }

        #region event handlers
        void handle_WorkflowLoaded(object sender, WorkflowLoadedEventArgs e)
        {
            AssociateWorkflow(e.LoadedWorkflow);
        }

        void handle_AdaptivityLevelChanged(object sender, AdaptivityLevelChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void handle_FailStateOccured(object sender, FailStateOccuredEventArgs e)
        {
            var wStats = m_CurrentWorkflowStats.WorkingStepsStats.ElementAt(e.WorkingStepNumber);
            wStats.Errors++;
        }

        void handle_WorkingStepCompleted(object sender, WorkingStepCompletedEventArgs e)
        {
            var wStats = m_CurrentWorkflowStats.WorkingStepsStats.ElementAt(e.WorkingStepNumber);
            
            wStats.Passed++;
            wStats.TotalTime += e.StepDurationTime;
            if (e.triggerType == AllEnums.WorkingStepEndConditionTrigger.FOOTPEDAL && !e.WorkingStep.IsManualStep)
            {
                wStats.Skipped++;
            }
        }

        void handle_WorkingStepStarted(object sender, WorkingStepStartedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void handle_WorkflowCompleted(object sender, WorkflowCompletedEventArgs e)
        {
            m_CurrentWorkflowStats.Completed++;
        }

        void handle_WorkflowStarted(object sender, WorkflowStartedEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion

        protected void AssociateWorkflow(Workflow wf)
        {
            var wfs = m_CurrentWorkflowStats;
            if (wfs == null || wfs.WorkflowId != wf.Id || wfs.WorkflowLength != wf.WorkingSteps.Count)
            {
                m_CurrentWorkflowStats = new WorkflowStats(wf.Id, wf.WorkingSteps.Count);
            }
        }


        #region getters/setters
        public WorkflowStats CurrentWorkflowStats
        {
            get { return m_CurrentWorkflowStats; }
            set { m_CurrentWorkflowStats = value; }
        }

        #endregion
    }
}
