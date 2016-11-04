// <copyright file=BackendControl.cs
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
using motionEAPAdmin.GUI;
using motionEAPAdmin.ContentProviders;
using HciLab.Kinect;
using System.IO;

namespace motionEAPAdmin.Backend
{
    class BackendControl
    {
        private static BackendControl m_Instance;

        private AdminView m_GuiHandle = null;

        private BackendControl() { }

        public static BackendControl Instance
        {
            get 
            {
                if (m_Instance == null)
                {
                    m_Instance = new BackendControl();
                }
                return m_Instance;
            }
        }

        public void startUpApplication()
        {
            // load settings

            SettingsManager.Instance.load();

            KinectManager.Instance.setKinectSettings(SettingsManager.Instance.Settings.SettingsKinect, SettingsManager.Instance.Settings.SettingsTable.KinectDrawing_AssemblyArea);

            m_GuiHandle = new AdminView();
            m_GuiHandle.InitializeComponent();
            m_GuiHandle.Show();

            // open the frontend GUI
            motionEAPAdmin.App app = new motionEAPAdmin.App();

            StateManager.Instance.SetNewState(this, HciLab.motionEAP.InterfacesAndDataModel.AllEnums.State.IDLE);

            app.Run();
        }

        /// <summary>
        /// Refresh the GUI to deal with databinding issues
        /// </summary>
        public void refreshGUI()
        {
            m_GuiHandle.Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Normal,
                new Action(
                    () =>
                    {
                        m_GuiHandle.ObjectsListViewRefresh();
                        m_GuiHandle.refreshWorkflowUI();
                        m_GuiHandle.refreshLayoutName();
                        m_GuiHandle.refreshDataContext();
                     }
                )
            );
        }

        public void shoutDownApplication()
        {

            SettingsManager.Instance.save();
            try
            {
                EnsensoManager.Instance.stopEnsensoCapturing();
            }
            catch (IOException e)
            {
                Console.WriteLine("Ensenso Library not found: Have you installed the Ensenso SDK?");
            }

            // kill Leap
            /*if (LeapManager.Instance != null)
                LeapManager.Instance.shutDownLeapMotion();*/

            // kill Object Recognition
            Environment.Exit(0);
        }
    }
}
