// <copyright file=ObjectDetectionManager.cs
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

using Emgu.CV;
using Emgu.CV.Structure;
using HciLab.motionEAP.InterfacesAndDataModel;
using HciLab.motionEAP.InterfacesAndDataModel.Data;
using HciLab.Utilities;
using motionEAPAdmin.ContentProviders;
using motionEAPAdmin.Database;
using motionEAPAdmin.GUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Media;

namespace motionEAPAdmin.Backend.ObjectDetection
{
    public class ObjectDetectionManager : INotifyPropertyChanged
    {
        /// <summary>
        /// property changed e.g. for the databinding
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// the singleton instance
        /// </summary>
        private static ObjectDetectionManager m_Instance;

        private List<BlobObject> m_MasterBlob;

        private int m_IdCounter = 0;

        private ObjectDetectionZonesLayout m_CurrentLayout;

        private bool m_TriggerRecognizingLoop = false;

        private TrackableObject m_LastSeenObject = null;

        /// <summary>
        /// constructor
        /// </summary>
        private ObjectDetectionManager() 
        {
            m_CurrentLayout = new ObjectDetectionZonesLayout();
        }

        /// <summary>
        /// Singleton Constructor
        /// </summary>
        public static ObjectDetectionManager Instance
        {
            get 
            {
                if (m_Instance == null)
                {
                    m_Instance = new ObjectDetectionManager();
                }
                return m_Instance;
            }
        }

        private void NotifyPropertyChanged(string Obj)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(Obj));
            }
        }
        
        public Scene.SceneRect createSceneRectForObjectDetectionZone(ObjectDetectionZone z, bool isUsedForRecord)
        {
            int x_offset = SettingsManager.Instance.Settings.SettingsTable.KinectDrawing_AssemblyArea.X;
            int y_offset = SettingsManager.Instance.Settings.SettingsTable.KinectDrawing_AssemblyArea.Y;

            float h = ((float)z.Height / (float)SettingsManager.Instance.Settings.SettingsTable.KinectDrawing_AssemblyArea.Height);
            float w = ((float)z.Width / (float)SettingsManager.Instance.Settings.SettingsTable.KinectDrawing_AssemblyArea.Width);
            float x = ((float)(z.X - x_offset) / (float)SettingsManager.Instance.Settings.SettingsTable.KinectDrawing_AssemblyArea.Width);
            float y = 1.0f - h - ((float)(z.Y - y_offset) / (float)SettingsManager.Instance.Settings.SettingsTable.KinectDrawing_AssemblyArea.Height);

            Color c = System.Windows.Media.Color.FromRgb(255, 255, 0); // yellow
            
            // TODO: maybe later
            /*
            if (b.wasRecentlyTriggered() && !isUsedForRecord)
            {
                c = System.Windows.Media.Color.FromRgb(255, 0, 0); // red
            }
             */

            Scene.SceneRect rect = new Scene.SceneRect(x, y, w, h, c);
            return rect;
        }

        public Scene.SceneRect createSceneBoxForObjectDetectionZone(ObjectDetectionZone z, bool isUsedForRecord)
        {
            int x_offset = SettingsManager.Instance.Settings.SettingsTable.KinectDrawing_AssemblyArea.X;
            int y_offset = SettingsManager.Instance.Settings.SettingsTable.KinectDrawing_AssemblyArea.Y;

            float h = ((float)z.Height / (float)SettingsManager.Instance.Settings.SettingsTable.KinectDrawing_AssemblyArea.Height);
            float w = ((float)z.Width / (float)SettingsManager.Instance.Settings.SettingsTable.KinectDrawing_AssemblyArea.Width);
            float x = ((float)(z.X - x_offset) / (float)SettingsManager.Instance.Settings.SettingsTable.KinectDrawing_AssemblyArea.Width);
            float y = 1.0f - h - ((float)(z.Y - y_offset) / (float)SettingsManager.Instance.Settings.SettingsTable.KinectDrawing_AssemblyArea.Height);

            Color c = System.Windows.Media.Color.FromRgb(255, 255, 0); // yellow

            // TODO: maybe later
            /*
            if (b.wasRecentlyTriggered() && !isUsedForRecord)
            {
                c = System.Windows.Media.Color.FromRgb(255, 0, 0); // red
            }
             */
            return new Scene.SceneRect(x, y, w, h, c);
        }

        public Scene.SceneText createSceneTextHeadingObjectDetectionZone(ObjectDetectionZone z, string text)
        {
            int x_offset = SettingsManager.Instance.Settings.SettingsTable.KinectDrawing_AssemblyArea.X;
            int y_offset = SettingsManager.Instance.Settings.SettingsTable.KinectDrawing_AssemblyArea.Y;

            float h = ((float)z.Height / (float)SettingsManager.Instance.Settings.SettingsTable.KinectDrawing_AssemblyArea.Height);
            float w = ((float)z.Width / (float)SettingsManager.Instance.Settings.SettingsTable.KinectDrawing_AssemblyArea.Width);
            float x = ((float)(z.X - x_offset) / (float)SettingsManager.Instance.Settings.SettingsTable.KinectDrawing_AssemblyArea.Width);
            float y = 1.0f - h - ((float)(z.Y - y_offset) / (float)SettingsManager.Instance.Settings.SettingsTable.KinectDrawing_AssemblyArea.Height);

            Color c = System.Windows.Media.Color.FromRgb(255, 255, 0); // yellow
            Scene.SceneText textItem = new Scene.SceneText(x, y, text, c, 10.0, new FontFamily("Arial"));
            return textItem;
        }

           
        public void createObjectDetectionZoneFromFactory(int pX, int pY, int pWidth, int pHeight)
        {
            ObjectDetectionZone obj = new ObjectDetectionZone(m_IdCounter);
            obj.Name = "Objekt-Zone " + m_IdCounter + "";
            obj.TriggerMessage = "Object" + m_IdCounter;
            obj.X = pX;
            obj.Y = pY;
            obj.Width = pWidth;
            obj.Height = pHeight;
            m_IdCounter++;
            this.CurrentLayout.ObjectDetectionZones.Add(obj);
        }

        public void loadObjectDetectionZoneLayoutFromFile()
        {
            SetNewLayout(ObjectDetectionZonesLayout.loadObjectDetectionZoneLayout());
        }
        
        public void SetNewLayout(ObjectDetectionZonesLayout pLayout)
        {
            if (pLayout != null)
            {
                int highestID = 0;

                // workaround for databinding bug
                m_CurrentLayout.ObjectDetectionZones.Clear();
                foreach (ObjectDetectionZone z in pLayout.ObjectDetectionZones)
                {
                    m_CurrentLayout.ObjectDetectionZones.Add(z);

                    if(z.Id > highestID)
                    {
                        highestID = z.Id;
                    }
                }
                m_CurrentLayout.LayoutName = pLayout.LayoutName;

                m_IdCounter = highestID + 1;
            }
        }

        internal void saveObjectDetectionZoneLayoutToFile()
        {
            ObjectDetectionZonesLayout.saveObjectDetectionZoneLayoutToFile(m_CurrentLayout);
        }

        public void UpdateCurrentObjectDetectionZone(ObjectDetectionZone updatedObjectDetectionZone)
        {
            foreach (ObjectDetectionZone z in m_CurrentLayout.ObjectDetectionZones)
            {
                if (z.Id == updatedObjectDetectionZone.Id)
                {
                    m_CurrentLayout.ObjectDetectionZones.Remove(z);
                    m_CurrentLayout.ObjectDetectionZones.Add(updatedObjectDetectionZone);

                    break;
                }
            }
        }

        public void SaveAndAddObjectToDatabase(Image<Bgra, byte> tempCropImg)
        {
            // check if object dir exists
            if (!Directory.Exists(ProjectConstants.OBJECT_DIR))
            {
                // if not create it
                Directory.CreateDirectory(ProjectConstants.OBJECT_DIR);
            }

            // save the scanned image using milliseconds as a uid
            long millis = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            string imagePath = ProjectConstants.OBJECT_DIR + "\\" + millis + ".jpg";
            tempCropImg.Save(imagePath);

            TrackableObject obj = new TrackableObject();
            obj.Image = imagePath;
            obj.ImageFullPath = System.IO.Path.GetFullPath(imagePath);
            obj.Name = "Erkanntes Objekt";
            obj.Category = "Group 1"; // improve with pbd
            DatabaseManager.Instance.insertTrackableObject(obj);
            DatabaseManager.Instance.listTrackableObject(); // refresh
            BackendControl.Instance.refreshGUI();
        }

        /// <summary>
        /// Find an object in a object list
        /// </summary>
        /// <param name="currentBlob"></param>
        /// <param name="viBlobs"></param>
        /// <returns>BlobID</returns>
        public bool RecognizeObject(Image<Gray, byte> sourceImage, Image<Gray, byte> toCompare)
        {
            bool isDetect = false;

            // MFunk: Run this a couple of times to get more robust
            int numRuns = 3;

            for (int i = 0; i < numRuns; i++)
            {

                isDetect = UtilitiesImage.MatchIsSame(toCompare, sourceImage);

                if (isDetect)
                {
                    break;
                }
            }

            // seriously no idea whats happening here but somehow this works
            if (!isDetect)
            {
                isDetect = UtilitiesImage.MatchIsSame(toCompare.Canny(10, 50), sourceImage.Canny(10, 50));
                if (isDetect)
                {
                    isDetect = true;
                }
                else
                {

                }
            }

            return isDetect;
        }


        public void InitPBDLogic()
        {
            long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            foreach (TrackableObject obj in DatabaseManager.Instance.Objects)
            {
                // was the object seen in the last two seconds?
                if (now - obj.LastSeenTimeStamp < 2000)
                {
                    // it was seen - save it 
                    m_LastSeenObject = obj;
                    break;
                }

            }
        }

        public void runPBDObjectThereCheck()
        {
            if (m_LastSeenObject != null)
            {
                long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                foreach (TrackableObject obj in DatabaseManager.Instance.Objects)
                {

                    if (obj.Id == m_LastSeenObject.Id)
                    {
                        // was the object seen in the last ten seconds?
                        if (now - obj.LastSeenTimeStamp < 10000)
                        {
                            // it was seen - all good
                            m_LastSeenObject = obj;
                            PBDManager.Instance.disablePBDBlockAgain();

                        }
                        else
                        {
                            // it was not seen - trigger PBD stuff
                            PBDManager.Instance.OnObjectWasTakenTrigger(obj);
                        }
                        break;
                    }

                }
            }
        }

        #region Getter / Setter
        public ObjectDetectionZonesLayout CurrentLayout
        {
            get { return m_CurrentLayout; }
           /* set {
                m_CurrentLayout = value;
                NotifyPropertyChanged("CurrentLayout");
            }*/
        }
        
        #endregion
        
        #region vermutlich von paul

        /// <summary>
        /// Displays a dialog to edit the given Trackableobject
        /// </summary>
        public void DisplayObjectEditDialog(TrackableObject obj)
        {
            ObjectEditDialog gui = new ObjectEditDialog(obj);
            gui.Show(); //non-blocking
        }

        public bool ObjectAlteredCallback(TrackableObject obj)
        {
            bool ret = false;
            ret = Database.DatabaseManager.Instance.updateTrackableObject(obj);
            if (ret)
            {
                Database.DatabaseManager.Instance.listTrackableObject(); // refresh
                BackendControl.Instance.refreshGUI();
            }

            return ret;
        }

        public List<BlobObject> MasterBlob
        {
            get { return m_MasterBlob;}
            set { m_MasterBlob = value; }
        }

        public bool isInsideObject(float x, float y)
        {
            bool ret = false;
            if (m_MasterBlob != null)
            {
                foreach (BlobObject b in m_MasterBlob)
                {
                    if (b.Rect.Contains(x, 1-y))
                    {
                        ret = true;
                        break;
                    }
                }
            }

            return ret;
        }


    }
        #endregion
}
