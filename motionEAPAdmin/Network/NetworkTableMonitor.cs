// <copyright file=NetworkTableMonitor.cs
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

using HciLab.Kinect;
using motionEAPAdmin.Scene;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace motionEAPAdmin.Network
{
    public class NetworkTableMonitor : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private CancellationTokenSource _tokenSource = new CancellationTokenSource();
        
        private string m_TableName;

        private int m_StepNumber;
        private int m_ProducedParts;
        private string m_WorkflowId;
        private SceneCountDown m_CountDownSceneItem;
        private string m_WaitText;

        public NetworkTableMonitor(string pTableName)
        {
            m_TableName = pTableName;
            var scene = SceneManager.Instance.TemporaryWaitingScene;
            //var text = new SceneText(scene, 0.5 * KinectManager.Instance.ImageSize.Width, 0.5 * KinectManager.Instance.ImageSize.Height, "Waiting for Table", System.Windows.Media.Color.FromRgb(255, 255, 255), 10.0, new System.Windows.Media.FontFamily("Arial"));
            m_CountDownSceneItem = new SceneCountDown
                (
                0.5 * KinectManager.Instance.ImageSize.Width,
                0.5 * KinectManager.Instance.ImageSize.Height,
                "Waiting for Table " + pTableName + " - Time left: ",
                System.Windows.Media.Color.FromRgb(255, 255, 255),
                7.0,
                new System.Windows.Media.FontFamily("Arial")
                );
            scene.Add(m_CountDownSceneItem);
            Task.Run(async () => await PollAsync());
        }

        public void Dispose()
        {
            _tokenSource.Cancel();
            SceneManager.Instance.TemporaryWaitingScene.Clear();
        }

        protected void NotifyPropertyChanged(string Obj)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(Obj));
            }
        }

        public async Task PollAsync()
        {
            var status = await CommunicationManager.Instance.getTableStatus(m_TableName);
            StepNumber = status.StepNumber;
            ProducedParts = status.ProducedParts;

            m_CountDownSceneItem.StartTimeLeft = status.EstimatedTimeLeft;
            await Task.Delay(TimeSpan.FromSeconds(1.0), _tokenSource.Token);

            

            while (!_tokenSource.Token.IsCancellationRequested)
            {
                status = await CommunicationManager.Instance.getTableStatus(m_TableName);

                if (StepNumber != status.StepNumber) this.StepNumber = status.StepNumber;
                if (ProducedParts != status.ProducedParts) this.ProducedParts = status.ProducedParts;
                m_CountDownSceneItem.StartTimeLeft = status.EstimatedTimeLeft;

                await Task.Delay(TimeSpan.FromSeconds(1.0),_tokenSource.Token);
            }
        }

        

        public string TableName
        {
            get { return m_TableName; }
            set { m_TableName = value; }
        }

        public int StepNumber
        {
            get { return m_StepNumber; }
            set 
            {
                m_StepNumber = value;
                NotifyPropertyChanged("StepNumber");
            }
        }

        public int ProducedParts
        {
            get { return m_ProducedParts; }
            set
            {
                m_ProducedParts = value;
                NotifyPropertyChanged("ProducedParts");
            }
        }

        public string WorkflowId
        {
            get { return m_WorkflowId; }
            set
            {
                m_WorkflowId = value;
                NotifyPropertyChanged("WorkflowId");
            }
        }
        public string WaitText
        {
            get { return m_WaitText; }
            set
            {
                m_WaitText = value;
                NotifyPropertyChanged("WaitText");
            }
        }
    }
}
