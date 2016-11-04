// <copyright file=Logger.cs
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
using System.Collections.ObjectModel;
using System.IO;
using motionEAPAdmin.GUI;

namespace motionEAPAdmin.Backend
{
    /// <summary>
    /// Some sort of Logger
    /// </summary>
    /// 
    /// 
    public class Logger
    {
        public enum LoggerState
        {
            INFORMATION = 0,
            WARNING = 1,
            ERROR = 2,
            CRITICAL = 3
        }

        private static Logger m_Instance;
        private StreamWriter m_Writer;

        private AdminView m_ViewHandle;

        private string m_ErrorImagePath = "Resources\\Erroricon.png";
        private string m_WarningImagePath = "Resources\\Warningicon.png";
        private string m_InformationImagePath = "Resources\\Informationicon.png";
        private string m_CriticalImagePath = "Resources\\Criticalicon.png";

        private ObservableCollection<LogMessage> m_AllLogsList = new ObservableCollection<LogMessage>();
        private ObservableCollection<LogMessage> m_FilteredLogsList = new ObservableCollection<LogMessage>();

        private bool m_Informations = true;
        private bool m_Warnings = true;
        private bool m_Errors = true;
        private bool m_Criticals = true;

        public ObservableCollection<LogMessage> Rows
        {
            get { return m_FilteredLogsList; }
            set { m_FilteredLogsList = value; }
        }

        public bool Informations
        {
            get
            {
                return m_Informations;
            }
            set
            {
                m_Informations = value;
            }
        }


        public bool Warnings
        {
            get
            {
                return m_Warnings;
            }
            set
            {
                m_Warnings = value;
            }
        }

        public bool Errors
        {
            get
            {
                return m_Errors;
            }
            set
            {
                m_Errors = value;
            }
        }

        public bool Criticals
        {
            get
            {
                return m_Criticals;
            }
            set
            {
                m_Criticals = value;
            }
        }

        private Logger() 
        {
            m_Writer = File.AppendText("log.txt");
        }

        public static Logger Instance
        {
            get 
            {
                if (m_Instance == null)
                {
                    m_Instance = new Logger();
                }
                return m_Instance;
            }
        }


        public AdminView ViewHandle
        {
            set
            {
                this.m_ViewHandle = value;
            }
        }

        private void addImageAccordingToErrorLevel(LoggerState errorLevel, LogMessage @log)
        {
            if (errorLevel == LoggerState.INFORMATION)
            {
                string full = Path.GetFullPath(m_InformationImagePath);

                log.Image = full;
            }

            if (errorLevel == LoggerState.WARNING)
            {
                string full = Path.GetFullPath(m_WarningImagePath);

                log.Image = full;
            }

            if (errorLevel == LoggerState.ERROR)
            {
                string full = Path.GetFullPath(m_ErrorImagePath);

                log.Image = full;
            }

            if (errorLevel == LoggerState.CRITICAL)
            {
                string full = Path.GetFullPath(m_CriticalImagePath);

                log.Image = full;
            }

        }

        public void applyFilter()
        {
            if (m_ViewHandle != null)
            {
                m_ViewHandle.Dispatcher.Invoke((Action)(() =>
                {
                    m_FilteredLogsList.Clear();
                    foreach (LogMessage log in m_AllLogsList)
                    {
                        if (m_Informations && (log.ErrorLevel == LoggerState.INFORMATION))
                        {
                            m_FilteredLogsList.Add(log);
                        }

                        if (m_Warnings && (log.ErrorLevel == LoggerState.WARNING))
                        {
                            m_FilteredLogsList.Add(log);
                        }

                        if (m_Errors && (log.ErrorLevel == LoggerState.ERROR))
                        {
                            m_FilteredLogsList.Add(log);
                        }

                        if (m_Criticals && (log.ErrorLevel == LoggerState.CRITICAL))
                        {
                            m_FilteredLogsList.Add(log);
                        }

                    }

                }));
            }
        }

        public void applyTextFilter(String toFilter)
        {
            // in chase the filter textbox is empty you can restore the list
            if(toFilter.Equals(""))
            {
                m_FilteredLogsList.Clear();
                foreach(LogMessage log in m_AllLogsList)
                {
                    if (m_Informations && (log.ErrorLevel == LoggerState.INFORMATION))
                        {
                            m_FilteredLogsList.Add(log);                    
                        }

                    if (m_Warnings && (log.ErrorLevel == LoggerState.WARNING))
                        {
                            m_FilteredLogsList.Add(log);
                        }

                    if (m_Errors && (log.ErrorLevel == LoggerState.ERROR))
                        {
                            m_FilteredLogsList.Add(log);
                        }

                    if (m_Criticals && (log.ErrorLevel == LoggerState.CRITICAL))
                        {
                            m_FilteredLogsList.Add(log);
                        }
                }    
            }
            else
            {
                m_FilteredLogsList.Clear();
                foreach (LogMessage log in m_AllLogsList)
                {
                    if (m_Informations && (log.ErrorLevel == LoggerState.INFORMATION) && log.Message.Contains(toFilter))
                    {
                        m_FilteredLogsList.Add(log);
                    }

                    if (m_Warnings && (log.ErrorLevel == LoggerState.WARNING) && log.Message.Contains(toFilter))
                    {
                        m_FilteredLogsList.Add(log);
                    }

                    if (m_Errors && (log.ErrorLevel == LoggerState.ERROR) && log.Message.Contains(toFilter))
                    {
                        m_FilteredLogsList.Add(log);
                    }

                    if (m_Criticals && (log.ErrorLevel == LoggerState.CRITICAL) && log.Message.Contains(toFilter))
                    {
                        m_FilteredLogsList.Add(log);
                    }
                }
            }    
        }



        public void Log(string pLogMessage, LoggerState pErrorLevel)
        {
            // Propagate Log to GUI
            if (m_ViewHandle != null)
            {
                LogMessage LogMessageT1 = new LogMessage();
                LogMessageT1.Message = pLogMessage;
                LogMessageT1.ErrorLevel = pErrorLevel;

                addImageAccordingToErrorLevel(pErrorLevel,LogMessageT1);
    
                this.m_AllLogsList.Add(LogMessageT1);
                applyFilter();
            }   
            
            
            switch (pErrorLevel)
            {
                case LoggerState.INFORMATION:
                    m_Writer.Write("\r\nINFORMATION: ");
                    m_Writer.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),DateTime.Now.ToLongDateString());
                    m_Writer.WriteLine(pLogMessage);
                    m_Writer.WriteLine("-------------------------------");
                    break;
                case LoggerState.WARNING:
                    m_Writer.Write("\r\nWARNING: ");
                    m_Writer.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),DateTime.Now.ToLongDateString());
                    m_Writer.WriteLine(pLogMessage);
                    m_Writer.WriteLine("-------------------------------");
                    break;
                case LoggerState.ERROR:
                    m_Writer.Write("\r\nERROR: ");
                    m_Writer.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),DateTime.Now.ToLongDateString());
                    m_Writer.WriteLine(pLogMessage);
                    m_Writer.WriteLine("-------------------------------");
                    break;
                case LoggerState.CRITICAL:
                    m_Writer.Write("\r\nCRITICAL: ");
                    m_Writer.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),DateTime.Now.ToLongDateString());
                    m_Writer.WriteLine(pLogMessage);
                    m_Writer.WriteLine("-------------------------------");
                    break;

                default:
                    // TODO: do something smart here
                    break;
            }

            m_Writer.Flush();


        }
  
    }
}
