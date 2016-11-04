// <copyright file=WorkflowStats.cs
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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace motionEAPAdmin.Statistics
{
    public class WorkflowStats : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        private string m_WorkflowId;
        
        private List<WorkingStepStats> m_WorkingStepsStats;

        private int m_Completed;
        
        public WorkflowStats(string workflowId, int numSteps)
        {
            m_WorkflowId = workflowId;
            m_WorkingStepsStats = new List<WorkingStepStats>();
            for (int i = 0; i < numSteps; i++)
            {
                var vStats = new WorkingStepStats(i);
                m_WorkingStepsStats.Add(vStats);
                vStats.PropertyChanged += OnWorkingStepStatsPropertyChanged;
            }
        }

        protected void NotifyPropertyChanged(string Obj)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(Obj));
            }
        }

        private void OnWorkingStepStatsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Passed":
                    NotifyPropertyChanged("Workingsteps_Passed");
                    break;
                case "Skipped":
                    NotifyPropertyChanged("Workingsteps_Skipped");
                    break;
                default:
                    NotifyPropertyChanged(e.PropertyName);
                    break;
            }
        }        

        /// <summary>
        /// Id of the associated workflow
        /// </summary>
        public string WorkflowId
        {
            get { return m_WorkflowId; }
            set { m_WorkflowId = value; }
        }

        /// <summary>
        /// Length of the associated workflow i.e. number of steps to complete
        /// </summary>
        public int WorkflowLength
        {
            get { return m_WorkingStepsStats.Count; }
        }

        /// <summary>
        /// SubStats divided by workingstep
        /// </summary>
        public List<WorkingStepStats> WorkingStepsStats
        {
            get { return m_WorkingStepsStats; }
            set { m_WorkingStepsStats = value; NotifyPropertyChanged("WorkingStepsStats"); }
        }

        /// <summary>
        /// Number of times the workflow was completed
        /// </summary>
        public int Completed
        {
            get { return m_Completed; }
            set { m_Completed = value; NotifyPropertyChanged("Completed"); }
        }

        /// <summary>
        /// Number of workingsteps passed (in any possible way)
        /// </summary>
        public int Workingsteps_Passed
        {
            get { return m_WorkingStepsStats.Aggregate(0, (sum, wss) => sum + wss.Passed); }
        }

        /// <summary>
        /// Number of workingsteps skipped manually
        /// </summary>
        public int Workingsteps_Skipped
        {
            get { return m_WorkingStepsStats.Aggregate(0, (sum, wss) => sum + wss.Skipped); }
            
        }

        /// <summary>
        /// Number of all errors occured
        /// </summary>
        public int Errors
        {
            get { return m_WorkingStepsStats.Aggregate(0, (sum, wss) => sum + wss.Errors); }
        }

        public double ErrorRate
        {
            get 
            {
                if (Workingsteps_Passed == 0) return 0.0;
                return (double)Errors / (double)Workingsteps_Passed; 
            }
        }

        public double SkipRate
        {
            get 
            {
                if (Workingsteps_Passed == 0) return 0.0;
                return (double)Errors / (double)Workingsteps_Passed; 
            }
        }

        /// <summary>
        /// total time spent in milliseconds 
        /// </summary>
        public long TotalTime
        {
            get { return m_WorkingStepsStats.Aggregate((long)0, (sum, wss) => sum + wss.TotalTime); }
        }

        public double MeanTime
        {
            get
            {
                if (Workingsteps_Passed == 0) return 0.0;
                return (double)TotalTime / (double)Workingsteps_Passed;
            }
        }

    }
}
