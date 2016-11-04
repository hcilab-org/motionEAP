// <copyright file=WorkflowCondition.cs
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

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace motionEAPAdmin.Model.Process
{
    /// <summary>
    /// This class represents a conditon within a workflow.
    /// </summary>
    [Serializable()]
    public class WorkflowCondition : ISerializable, INotifyPropertyChanged
    {
        // Version
        private int m_SerVersion = 1;

        private string m_CheckMessage;

        public event PropertyChangedEventHandler PropertyChanged; // event for the databinding

        protected WorkflowCondition(SerializationInfo info, StreamingContext context)
        {
            m_SerVersion = info.GetInt32("m_SerVersion");
            m_CheckMessage = info.GetString("m_CheckMessage");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("m_SerVersion", m_SerVersion);
            info.AddValue("m_CheckMessage", m_CheckMessage);
        }

        public WorkflowCondition(string pCheckMessage)
        {
            m_CheckMessage = pCheckMessage;
        }

        public Boolean IsFullfiled(string pTriggerMessage) {
            return pTriggerMessage == m_CheckMessage;
        }

        public void reset() 
        {
        
        }

        public string CheckMessage
        {
            get { return m_CheckMessage; }
            set { m_CheckMessage = value; }
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
