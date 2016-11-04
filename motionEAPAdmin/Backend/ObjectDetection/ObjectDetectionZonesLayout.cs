// <copyright file=ObjectDetectionZonesLayout.cs
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

using HciLab.Utilities;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.IO;
using HciLab.motionEAP.InterfacesAndDataModel;
using System.Windows.Forms;

namespace motionEAPAdmin.Backend.ObjectDetection
{
    [Serializable()]
    public class ObjectDetectionZonesLayout : INotifyPropertyChanged, ISerializable
    {
        public event PropertyChangedEventHandler PropertyChanged; // property changed for the databinding

        private int m_SerVersion = 1;

        private ObservableCollection<ObjectDetectionZone> m_ObjectDetectionZonesList = new ObservableCollection<ObjectDetectionZone>();

        private string m_LayoutName = "";

        public ObjectDetectionZonesLayout()
        {
        }
        
        protected ObjectDetectionZonesLayout(SerializationInfo pInfo, StreamingContext pContext)
        {
            int pSerVersion = pInfo.GetInt32("m_SerVersion");
            m_LayoutName = pInfo.GetString("m_LayoutName");
            m_ObjectDetectionZonesList = (ObservableCollection<ObjectDetectionZone>)pInfo.GetValue("m_ObjectDetectionZonesList", typeof(ObservableCollection<ObjectDetectionZone>));
        }

        public void GetObjectData(SerializationInfo pInfo, StreamingContext pContext)
        {
            pInfo.AddValue("m_SerVersion", m_SerVersion);
            pInfo.AddValue("m_ObjectDetectionZonesList", m_ObjectDetectionZonesList);
            pInfo.AddValue("m_LayoutName", m_LayoutName);
        }

        private void NotifyPropertyChanged(string Obj)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(Obj));
            }
        }


        public static void saveObjectDetectionZoneLayoutToFile(ObjectDetectionZonesLayout pCurrentLayout)
        {
            // check if scnes OBJECTDETECTIONZONES_DIR exists
            if (!Directory.Exists(ProjectConstants.OBJECTDETECTIONZONES_DIR))
            {
                Directory.CreateDirectory(ProjectConstants.OBJECTDETECTIONZONES_DIR);
            }

            System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
            dlg.InitialDirectory = Directory.GetCurrentDirectory() + "\\" + ProjectConstants.OBJECTDETECTIONZONES_DIR;
            dlg.Filter = "ozone files (*.ozone)|*.ozone";

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            string filename = dlg.FileName;
            if (!filename.EndsWith(ProjectConstants.OBJECTDETECTIONZONES_FILE_ENDING))
            {
                filename = filename + ProjectConstants.OBJECTDETECTIONZONES_FILE_ENDING;
            }

            UtilitiesIO.SaveObjectToJson(pCurrentLayout, filename);
        }

        public static ObjectDetectionZonesLayout loadObjectDetectionZoneLayout()
        {
            // filechooser
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.InitialDirectory = Directory.GetCurrentDirectory() + "\\" + ProjectConstants.OBJECTDETECTIONZONES_DIR;
            dlg.Filter = "ozone files (*.ozone)|*.ozone";
            dlg.FilterIndex = 2;
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                ObjectDetectionZonesLayout ret = null;
                bool isOkay = UtilitiesIO.GetObjectFromJson(ref ret, dlg.FileName);
                if (!isOkay)
                    return null;
                else
                    return ret;
            }

            return null;
        }

        #region Getter / Setter
        public string LayoutName
        {
            get
            {
                return m_LayoutName;
            }
            set
            {
                m_LayoutName = value;
                NotifyPropertyChanged("LayoutName");
            }
        }

        public ObservableCollection<ObjectDetectionZone> ObjectDetectionZones
        {
            get { return m_ObjectDetectionZonesList; }
            set
            {
                m_ObjectDetectionZonesList = value;
                NotifyPropertyChanged("ObjectDetectionZones");
            }
        }
        #endregion
    }
}
