// <copyright file=AssemblyZoneManager.cs
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
using HciLab.motionEAP.InterfacesAndDataModel.Data;
using HciLab.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using motionEAPAdmin.GUI;
using motionEAPAdmin.ContentProviders;
using System.ComponentModel;
using motionEAPAdmin.Scene;
using HciLab.Utilities.Mathematics.Geometry2D;
using HciLab.Utilities.Mathematics.Core;

namespace motionEAPAdmin.Backend.AssembleyZones
{
    public class AssemblyZoneManager : INotifyPropertyChanged
    {
        /// <summary>
        /// property changed e.g. for the databinding
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The singleton instance
        /// </summary>
        private static AssemblyZoneManager m_Instance;

        /// <summary>
        /// AssemblyZone Id Conter
        /// </summary>
        private int m_IdCounter = 0;

        /// <summary>
        /// All AssemblyZones are containt here
        /// </summary>
        private AssemblyZoneLayout m_CurrentLayout = new AssemblyZoneLayout();

        /// <summary>
        /// A snapshot of the cropped depth image
        /// </summary>
        private Image<Gray, Int32> m_DepthCroppedSnapshot = null;

        /// <summary>
        /// Heap Scene witch contains all AssemblyZone Scenes
        /// </summary>
        private Scene.Scene m_SceneOfAllAssemblyZones = new Scene.Scene();

        /// <summary>
        /// Font that is used for UI drawinigns
        /// </summary>
        private static MCvFont UI_FONT = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_SIMPLEX, 0.5, 0.5);


        /// <summary>
        /// Singleton Constructor
        /// </summary>
        public static AssemblyZoneManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new AssemblyZoneManager();
                }
                return m_Instance;
            }
        }

        #region constructor
        private AssemblyZoneManager()
        {
        }
        #endregion

        private void NotifyPropertyChanged(string Obj)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(Obj));
            }
        }

        public AssemblyZone createAssemblyZoneFromFactory()
        {
            AssemblyZone ret = new AssemblyZone(m_IdCounter);
            ret.Name = m_IdCounter.ToString();
            ret.TriggerMessage = "z" + m_IdCounter;
            m_IdCounter++;
            ret.trigger += WorkflowManager.Instance.OnTriggered;

            return ret;
        }
        
        public void updateCurrentAssemblyZone(AssemblyZone updatedAssemblyZone)
        {
            m_CurrentLayout.AssemblyZones.ReplaceOrAdd(updatedAssemblyZone);
        }
        
        public void decreaseIDByOne()
        {
            if (m_IdCounter > 0)
            {
                m_IdCounter = m_IdCounter - 1;
            }
        }

        public void saveAssemblyZoneLayoutToFile()
        {
            AssemblyZoneLayout.saveAssemblyZoneLayoutToFile(m_CurrentLayout);
        }
        
        public void loadAssemblyZoneLayoutFromFile()
        {
            SetNewLayout(AssemblyZoneLayout.loadAssemblyZoneLayoutFromFile());

            BackendControl.Instance.refreshGUI();
            AdminView.Instance.refreshDataContext();
        }

        public void SetNewLayout(AssemblyZoneLayout pLayout)
        {
            if (pLayout != null)
            {
                int highestID = 0;

                // workaround for databinding bug
                m_CurrentLayout.AssemblyZones.Clear();
                foreach (AssemblyZone zone in pLayout.AssemblyZones)
                {
                    m_CurrentLayout.AssemblyZones.Add(zone);

                    if(zone.Id > highestID)
                    {
                        highestID = zone.Id;
                    }
                }
                m_CurrentLayout.LayoutName = pLayout.LayoutName;

                m_IdCounter = highestID + 1;
            }

        }

        public void createDepthSnapshot()
        {
            m_DepthCroppedSnapshot = HciLab.Kinect.CameraManager.Instance.DepthImage;
            
            System.Console.WriteLine("Snapshot taken");
        }

        public AssemblyZone createAssemblyZoneFromChanges()
        {
            //TODO: wait for smoothing
            Image<Gray, Int32> currentDepthImageCropped = HciLab.Kinect.CameraManager.Instance.DepthImageCropped;

            AssemblyZone z = createAssemblyZoneFromChanges(currentDepthImageCropped, false);
            AssemblyZone dz = createAssemblyZoneFromChanges(currentDepthImageCropped, true);

            if (z == null && dz == null)
                return null;

            if (z != null && dz == null)
                return z;

            if (dz != null && z == null)
                return dz;

            if (z.Area > dz.Area)
                return z;
            else
                return dz;
        }

        /// <summary>
        /// Create AssemblyZone based on differences to previously saved DepthSnapshot
        /// </summary>
        /// <param name="m_DepthCroppedSnapshot"></param>
        /// <param name="pDetectRemoval"></param>
        /// <returns></returns>
        private AssemblyZone createAssemblyZoneFromChanges(Image<Gray, Int32> currentDepthImageCropped, bool pDetectRemoval)
        {
            if (m_DepthCroppedSnapshot == null)
                return null;
            
            Image<Gray, Byte> imageDeltaMaskByte;
            if(pDetectRemoval)
                imageDeltaMaskByte = m_DepthCroppedSnapshot.Cmp(currentDepthImageCropped, Emgu.CV.CvEnum.CMP_TYPE.CV_CMP_LT);
            else
                imageDeltaMaskByte = m_DepthCroppedSnapshot.Cmp(currentDepthImageCropped, Emgu.CV.CvEnum.CMP_TYPE.CV_CMP_GT);

            CvInvoke.cvShowImage(pDetectRemoval.ToString() + "Snapshot", imageDeltaMaskByte.Ptr);

            Image<Gray, Int32> imageDeltaMaskInt = imageDeltaMaskByte.Convert<Int32>(delegate(Byte b)
            {
                if (b == 0)
                    return 0;
                else
                    return Int32.MaxValue;
            });

            Image<Gray, Int32> imageDelta = m_DepthCroppedSnapshot.AbsDiff(currentDepthImageCropped).And(imageDeltaMaskInt);

            double valueThreshold;
            int areaThreshold;

            if (pDetectRemoval)
            {
                valueThreshold = SettingsManager.Instance.Settings.AssemblyZonesInputMinValueChangeRemoval;
                areaThreshold = SettingsManager.Instance.Settings.AssemblyZonesInputMinAreaRemoval;
            }
            else
            {
                valueThreshold = SettingsManager.Instance.Settings.AssemblyZonesInputMinValueChangeAdding;
                areaThreshold = SettingsManager.Instance.Settings.AssemblyZonesInputMinAreaAdding;
            }

            Image<Gray, Int16> thImage = imageDelta.Convert<Gray, Int16>();
            thImage = thImage.ThresholdToZero(new Gray(valueThreshold*20));

            imageDelta = thImage.Convert<Gray, Int32>();
            #region Draw bounderis around detected areas
            List<BlobBound> listBlobBounds = BlobManager.FindAllBlob(imageDelta, pDetectRemoval, areaThreshold, valueThreshold);
            

            if (listBlobBounds.Count > 0)
            {
                BlobBound e = listBlobBounds.First();
                
                Rectangle bounds = e.Rect;

                Polygon contourInGlobalImage = new Polygon();

                foreach (Vector2 v in e.Contour.Points)
                {
                    if (AdminView.Instance.IsKinectActive)
                        contourInGlobalImage.Points.Add(new Vector2(v.X + SettingsManager.Instance.Settings.SettingsTable.KinectDrawing_AssemblyArea.X, v.Y + SettingsManager.Instance.Settings.SettingsTable.KinectDrawing_AssemblyArea.Y));
                    else
                        contourInGlobalImage.Points.Add(new Vector2(v.X + SettingsManager.Instance.Settings.SettingsTable.EnsensoDrawing_AssemblyArea.X, v.Y + SettingsManager.Instance.Settings.SettingsTable.EnsensoDrawing_AssemblyArea.Y));
                }

                if (AdminView.Instance.IsKinectActive)
                    return AssemblyZoneManager.Instance.createAssemblyZoneFromFactory(bounds.X + SettingsManager.Instance.Settings.SettingsTable.KinectDrawing_AssemblyArea.X,
                        bounds.Y + SettingsManager.Instance.Settings.SettingsTable.KinectDrawing_AssemblyArea.Y,
                        bounds.Width,
                        bounds.Height,
                        e.BlobMask,
                        true,
                        pDetectRemoval,
                        e.Area,
                        contourInGlobalImage);
                else
                    return AssemblyZoneManager.Instance.createAssemblyZoneFromFactory(bounds.X + SettingsManager.Instance.Settings.SettingsTable.EnsensoDrawing_AssemblyArea.X,
                        bounds.Y + SettingsManager.Instance.Settings.SettingsTable.EnsensoDrawing_AssemblyArea.Y,
                        bounds.Width,
                        bounds.Height,
                        e.BlobMask,
                        true,
                        pDetectRemoval,
                        e.Area,
                        contourInGlobalImage);
            }
            #endregion

            return null;
        }

        private AssemblyZone createAssemblyZoneFromFactory(int pX, int pY, int pWidth, int pHeight, bool[,] pMask, bool p7, bool pDetectRemoval, int pArea, Polygon pContour = null)
        {
            
            AssemblyZone z = new AssemblyZone(m_IdCounter);
            z.Name = m_IdCounter.ToString();
            z.TriggerMessage = "z" + m_IdCounter;
            m_IdCounter++;
            if (pDetectRemoval)
                z.IsDisassemblyZone = true;
            z.X = pX;
            z.Y = pY;
            z.Width = pWidth;
            z.Height = pHeight;
            z.DepthMask = pMask;
            z.UseDepthmask = true;
            z.Area = pArea;
            z.Contour = pContour;
            z.DepthArray = getDepthArrayFromAssemblyZone(z);

            z.trigger += WorkflowManager.Instance.OnTriggered;

            return z;
        }


        public static int[,] getDepthArrayFromAssemblyZone(AssemblyZone pZone)
        {

            Image<Gray, Int32> pDepthImg = HciLab.Kinect.CameraManager.Instance.DepthImage;

            //pDepthImg.ROI = new Rectangle(pZone.X, pZone.Y, pZone;
            int[,] depthArray = new int[pZone.Width, pZone.Height];
            for (int x = pZone.X; x < pZone.X + pZone.Width; x++)
            {
                for (int y = pZone.Y; y < pZone.Y + pZone.Height; y++)
                {
                    depthArray[x - pZone.X, y - pZone.Y] = pDepthImg.Data[y, x, 0];
                }
            }
            return depthArray;
        }
        
        public Scene.Scene drawProjectorUI()
        {
            m_SceneOfAllAssemblyZones.Clear();

            foreach (AssemblyZone b in m_CurrentLayout.AssemblyZones)
            {
                SceneItem s = b.getDrawable(false);
                if (s is Scene.Scene)
                {
                    foreach (SceneItem i in ((Scene.Scene)s).Items)
                        m_SceneOfAllAssemblyZones.Add(i);
                }
                else
                {
                    m_SceneOfAllAssemblyZones.Add(s);
                }

            }

            return m_SceneOfAllAssemblyZones;
        }


        public Image<Bgra, byte> drawAdminUI(Image<Bgra, byte> pImage)
        {
            if (m_CurrentLayout == null)
                return null;

            foreach (AssemblyZone zone in m_CurrentLayout.AssemblyZones)
            {
                if (zone.IsSelected)
                {
                    // draw ID
                    pImage.Draw(zone.Id + "", ref UI_FONT, new System.Drawing.Point(zone.X, zone.Y), new Bgra(0, 0, 0, 0));
                    // draw PercentageMatched
                    pImage.Draw((int)(zone.LastPercentageMatched * 100) + "%", ref UI_FONT, new System.Drawing.Point(zone.X, zone.Y + zone.Height), new Bgra(0, 0, 0, 0));
                    // draw Frame
                    if (zone.wasRecentlyTriggered())
                    {
                        pImage.Draw(new Rectangle(zone.X, zone.Y, zone.Width, zone.Height), new Bgra(0, 255, 0, 0), 0);
                    }
                    else
                    {
                        pImage.Draw(new Rectangle(zone.X, zone.Y, zone.Width, zone.Height), new Bgra(0, 0, 255, 0), 0);
                    }
                }
            }
            return pImage;
        }

        #region Getter/Setter

        public AssemblyZoneLayout CurrentLayout
        {
            get { return m_CurrentLayout; }
       /*     set
            {
                m_CurrentLayout = value;
                NotifyPropertyChanged("CurrentLayout");
            }*/
        }
        #endregion
    }
}
