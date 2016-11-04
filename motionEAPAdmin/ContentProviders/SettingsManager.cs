// <copyright file=SettingsManager.cs
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

using HciLab.Kinect;
using HciLab.motionEAP.InterfacesAndDataModel;
using HciLab.motionEAP.InterfacesAndDataModel.Data;
using HciLab.Utilities;
using System;

namespace motionEAPAdmin.ContentProviders
{
    public class SettingsManager
    {
        private static SettingsManager instance;

        // Initialize settings
        private Settings m_Settings = new Settings();

        // get the singleton instance
        public static SettingsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SettingsManager();
                }
                return instance;
            }
        }

        public void load()
        {
            // Get Values from XML and set these values to the fields
            Boolean isOkay = UtilitiesIO.GetObjectFromJson(ref m_Settings, ProjectConstants.SETTINGS_FILE);
            m_Settings.PropertyChanged += m_Settings_PropertyChanged;
        }

        void m_Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            KinectManager.Instance.setKinectSettings(m_Settings.SettingsKinect, m_Settings.SettingsTable.KinectDrawing_AssemblyArea);
            save();
        }

        public void save()
        {
            bool successful = UtilitiesIO.SaveObjectToJson(m_Settings, ProjectConstants.SETTINGS_FILE);
        }

        public Settings Settings {
            get { return m_Settings; }
            set { m_Settings = value; }
        }
    }
}
