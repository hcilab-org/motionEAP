// <copyright file=WorkingStepStats.cs
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

using System.ComponentModel;

namespace motionEAPAdmin.Statistics
{
    public class WorkingStepStats : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int m_WorkingStepIndex;

        private int m_Passed;
        private int m_Skipped;
        private int m_Errors;
        private long m_TotalTime;

        public WorkingStepStats(int index)
        {
            m_WorkingStepIndex = index;
        }

        /// <summary>
        /// Index of associated Workingstep
        /// </summary>
        public int WorkingStepIndex
        {
            get { return m_WorkingStepIndex; }
            set { m_WorkingStepIndex = value; }
        }

        protected void NotifyPropertyChanged(string Obj)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(Obj));
            }
        }

        /// <summary>
        /// Number of times the WorkingStep was passed (in any possible way)
        /// </summary>
        public int Passed
        {
            get { return m_Passed; }
            set 
            {
                m_Passed = value;
                NotifyPropertyChanged("Passed");
                NotifyPropertyChanged("MeanTime");
                NotifyPropertyChanged("ErrorRate");
                NotifyPropertyChanged("SkipRate");
            }
        }

        /// <summary>
        /// Number of times the workingstep was skipped manually
        /// </summary>
        public int Skipped
        {
            get { return m_Skipped; }
            set 
            {
                m_Skipped = value;
                NotifyPropertyChanged("Skipped");
                NotifyPropertyChanged("SkipRate");
            }
        }

        /// <summary>
        /// Number of all errors occured during the workingstep
        /// </summary>
        public int Errors
        {
            get { return m_Errors; }
            set 
            { 
                m_Errors = value; 
                NotifyPropertyChanged("Errors");
                NotifyPropertyChanged("ErrorRate");
            }
        }
        
        public double ErrorRate
        {
            get 
            {
                if (m_Passed == 0) return 0.0;
                return (double)m_Errors / (double)m_Passed; 
            }
        }

        public double SkipRate
        {
            get 
            {
                if (m_Passed == 0) return 0.0;
                return (double)m_Skipped / (double)m_Passed; 
            }
        }

        /// <summary>
        /// total time spent in this step in milliseconds 
        /// </summary>
        public long TotalTime
        {
            get { return m_TotalTime; }
            set 
            { 
                m_TotalTime = value;
                NotifyPropertyChanged("TotalTime");
                NotifyPropertyChanged("MeanTime");
            }
        }

        public double MeanTime
        {
            get
            {
                if (m_Passed == 0) return 0.0;
                return (double)m_TotalTime / (double)m_Passed;
            }
        }

    }
}
