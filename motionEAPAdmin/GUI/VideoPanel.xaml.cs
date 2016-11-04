// <copyright file=VideoPanel.xaml.cs
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
using HciLab.Kinect;
using HciLab.motionEAP.InterfacesAndDataModel;
using HciLab.motionEAP.InterfacesAndDataModel.Data;
using HciLab.Utilities;
using HciLab.Utilities.Mathematics.Core;
using HciLab.Utilities.Mathematics.Geometry2D;
using motionEAPAdmin.Backend;
using motionEAPAdmin.Backend.CameraManager;
using motionEAPAdmin.Backend.ObjectDetection;
using motionEAPAdmin.ContentProviders;
using motionEAPAdmin.Database;
using motionEAPAdmin.Scene;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace motionEAPAdmin.GUI
{
    /// <summary>
    /// Interaktionslogik für Video.xaml
    /// </summary>
    public partial class VideoPanel : UserControl
    {
        private List<BlobObject> m_ViBlob = new List<BlobObject>();
        private bool m_Free_Checked;
        private bool m_Tracking_Checked;
        private bool m_Map_Checked;

        Image<Bgra, byte> m_ColorImage;

        public VideoPanel()
        {
            InitializeComponent();
            this.DataContext = SettingsManager.Instance.Settings;

            USBCameraDetector.ConnectedUSBCameras.CollectionChanged += ConnectedUSBCameras_CollectionChanged;

        }

        private void ConnectedUSBCameras_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            m_ComboBoxSelectedCameras.Items.Clear();
            // check for combobox updates
            foreach (var camera in USBCameraDetector.ConnectedUSBCameras)
            {
                m_ComboBoxSelectedCameras.Items.Add(camera);
            }

            if (m_ComboBoxSelectedCameras.Items.Count > 0)
            {
                m_ComboBoxSelectedCameras.SelectedIndex = 0;
            }
        }


        public void ProcessFrame(Image<Bgra, byte> pColorImage, Image<Bgra, byte> pColorImageCropped,
             Image<Gray, Int32> pDepthImage, Image<Gray, Int32> pDepthImageCropped)
        {

            if (pColorImage != null)
            {
                m_ColorImage = pColorImage;
                UtilitiesImage.ToImage(this.KinectRGBImage, CameraManager.Instance.ColorImage);
            }
            if (pColorImageCropped != null)
                UtilitiesImage.ToImage(this.KinectRGBCropImage, CameraManager.Instance.ColorImageCropped);
            if (pDepthImage != null)
            {
                UtilitiesImage.ToImage(this.KinectDepthImage, CameraManager.Instance.DepthImage);
            }
            if (pDepthImageCropped != null)
                UtilitiesImage.ToImage(this.KinectDepthCropImage, CameraManager.Instance.DepthImageCropped);
        }


        private void buttonScan_Click(object sender, RoutedEventArgs e)
        {
            foreach (BlobObject blob in BlobManager.Instance.MasterBlob)
            {

                if (m_ViBlob.Count == 0)
                {
                    int[, ,] depthStructur = UtilitiesImage.ObjectDepthPoint(blob.Rect.Location,
                        blob.Rect, KinectManager.Instance.GetCurrentDepthImageCropped());
                    this.m_ViBlob.Add(new BlobObject(blob.Image, depthStructur, blob.CornerPoints, blob.Rect, blob.Center, blob.Hits, BlobManager.Instance.Id, BlobManager.Instance.Id.ToString()));
                    blob.Id = BlobManager.Instance.Id;
                    BlobManager.Instance.Id++;

                    Image<Gray, Int32> testImage = new Image<Gray, int>(depthStructur.GetLength(0), depthStructur.GetLength(1));

                    Parallel.For(0, depthStructur.GetLength(1), y =>
                    {
                        for (int x = 0; x < depthStructur.GetLength(0); x++)
                        {
                            testImage.Data[y, x, 0] = depthStructur[y, x, 0];
                        }
                    });

                    Image<Gray, byte> cannyImage = testImage.Canny(10, 30);

                    // check if object dir exists
                    if (!Directory.Exists(ProjectConstants.OBJECT_DIR))
                    {
                        // if not create it
                        Directory.CreateDirectory(ProjectConstants.OBJECT_DIR);
                    }

                    // save the scanned image using milliseconds as a uid
                    long millis = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    string imagePath = ProjectConstants.OBJECT_DIR + "\\" + millis + ".jpg";
                    blob.Image.Save(imagePath);

                    TrackableObject obj = new TrackableObject();
                    obj.Image = imagePath;
                    obj.ImageFullPath = System.IO.Path.GetFullPath(imagePath);
                    obj.Name = "Peter";
                    obj.Category = "Group 1";
                    DatabaseManager.Instance.insertTrackableObject(obj);
                    DatabaseManager.Instance.listTrackableObject(); // refresh

                }
                else
                {
                    //blobId = DepthHelper.RecognizeObjekt(blob, this.viBlob);

                    //TODO: check previously saved objects here

                    if (blob.Id == 1)
                    {
                        int[, ,] depthStructur = UtilitiesImage.ObjectDepthPoint(blob.Rect.Location,
                            blob.Rect, KinectManager.Instance.GetCurrentDepthImage());
                        this.m_ViBlob.Add(new BlobObject(blob.Image, depthStructur,
                            blob.CornerPoints, blob.Rect, blob.Center, blob.Hits,
                            BlobManager.Instance.Id, BlobManager.Instance.Id.ToString()));
                        blob.Id = BlobManager.Instance.Id;
                        BlobManager.Instance.Id++;
                    }
                }
            }
        }

        private void buttonId_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < BlobManager.Instance.MasterBlob.Count; i++)
            {
                List<TrackableObject> dbObjects = DatabaseManager.Instance.Objects.ToList();
                string currentBlobID = BlobManager.RecognizeObject(BlobManager.Instance.MasterBlob[i], dbObjects);
                BlobManager.Instance.MasterBlob[i].Name = currentBlobID;

            }
        }

        private void buttonRotation_Click(object sender, RoutedEventArgs e)
        {
            float degree;
            for (int i = 0; i < BlobManager.Instance.MasterBlob.Count; i++)
            {
                degree = BlobManager.GetRotation(m_ColorImage, BlobManager.Instance.MasterBlob[i], this.m_ViBlob);
            }
        }


        private void buttonRecordVideo_Click(object sender, RoutedEventArgs e)
        {
            KinectManager.Instance.StartKinectRecording();
        }

        private void buttonRecordVideoStop_Click(object sender, RoutedEventArgs e)
        {
            KinectManager.Instance.StopKinectRecording();
        }


        public void ProcessBoxProjection(Image<Bgra, byte> pColorBitSource)
        {
            if (SettingsManager.Instance.Settings.CheckBoxStartProjection == false)
                return;

            Image<Gray, Int32> depthImg = KinectManager.Instance.GetCurrentDepthImageCropped();
            int[,] map = new int[SettingsManager.Instance.Settings.IntegerUpDownXBox, SettingsManager.Instance.Settings.IntegerUpDownYBox];
            BlobManager.Instance.MasterBlob = BlobManager.FindAllBlob(
                depthImg,
                BlobManager.Instance.MasterBlob,
                pColorBitSource,
                SettingsManager.Instance.Settings.BlobRadio);
            Image<Bgra, byte> outputImage = depthImg.Convert<Bgra, byte>();
            Tuple<RectangleF, System.Windows.Media.Color> outputFree = new Tuple<RectangleF, System.Windows.Media.Color>(new RectangleF(), new System.Windows.Media.Color());
            List<Tuple<PointF[], System.Windows.Media.Color>> outputBoxPoint = new List<Tuple<PointF[], System.Windows.Media.Color>>();
            List<Tuple<PointF, String, System.Windows.Media.Color>> outputString = new List<Tuple<PointF, String, System.Windows.Media.Color>>();

            Bgra color = new Bgra(0, 255, 0, 0);

            if (BlobManager.Instance.MasterBlob != null)
            {
                for (int i = 0; i < BlobManager.Instance.MasterBlob.Count; i++)
                {
                    BlobObject currentBlob = BlobManager.Instance.MasterBlob[i];
                    //currentBlob.Id == 0;

                    if (m_Free_Checked)
                    {
                        map = FreeSpaceManager.RenderObject(currentBlob.CornerPoints, map, depthImg.Width, depthImg.Height);
                    }

                    // map_check fliegt raus
                    if (m_Map_Checked)
                    {
                        if (BlobManager.Instance.MasterBlob[i].Hits == 0)
                        {
                            int count;
                            count = UtilitiesImage.FeaturePerObject(pColorBitSource, BlobManager.Instance.MasterBlob[i].Rect);

                            BlobManager.Instance.MasterBlob[i].Hits = count;
                        }
                        color = UtilitiesImage.MappingColor(BlobManager.Instance.MasterBlob[i].Hits);
                    }

                    if (m_Tracking_Checked)
                    {
                        System.Drawing.Point center = new System.Drawing.Point((int)(currentBlob.Center.X * depthImg.Width), (int)(currentBlob.Center.Y * depthImg.Height));
                        //outputImage.Draw(currentBlob.Name.ToString(), ref m_Font, center, new Bgr(System.Drawing.Color.Red));
                        outputString.Add(new Tuple<PointF, String, System.Windows.Media.Color>(currentBlob.Center, currentBlob.Name.ToString(), System.Windows.Media.Color.FromRgb(0, 255, 0)));
                    }

                    List<LineSegment2DF> depthboxLines = UtilitiesImage.PointsToImageLine(currentBlob.CornerPoints, depthImg.Width, depthImg.Height);
                    foreach (LineSegment2DF line in depthboxLines)
                    {
                        outputImage.Draw(line, color, 2);
                    }

                    PointF[] corner = new PointF[4];
                    for (int cur = 0; cur < 4; cur++)
                    {
                        corner[cur] = new PointF(BlobManager.Instance.MasterBlob[i].CornerPoints[cur].X, 1 - BlobManager.Instance.MasterBlob[i].CornerPoints[cur].Y);
                    }

                    outputBoxPoint.Add(new Tuple<PointF[], System.Windows.Media.Color>(corner, System.Windows.Media.Color.FromRgb((byte)color.Red, (byte)color.Green, (byte)color.Blue)));
                }
            }

            // update the reference
            ObjectDetectionManager.Instance.MasterBlob = BlobManager.Instance.MasterBlob;

            RectangleF freeSpace = new RectangleF();
            if (m_Free_Checked)
            {
                depthImg = FreeSpaceManager.DrawMaxSubmatrix(depthImg.Convert<Bgra, byte>(), map).Convert<Gray, Int32>();
                freeSpace = FreeSpaceManager.DrawMaxSubmatrix(map, (float)(depthImg.Width / (float)SettingsManager.Instance.Settings.IntegerUpDownXBox / depthImg.Width),
                    (float)(depthImg.Height / (float)SettingsManager.Instance.Settings.IntegerUpDownYBox / depthImg.Height));
            }

            //draw on table
            # region projection
            if (SettingsManager.Instance.Settings.CheckBoxStartProjection)
            {
                // create a new scene
                //Scene.Scene blankScene = new Scene.Scene();
                SceneManager.Instance.TemporaryScene.Clear();

                outputFree = new Tuple<System.Drawing.RectangleF, System.Windows.Media.Color>(freeSpace, System.Windows.Media.Color.FromRgb(0, 255, 0));
                SceneRect freeBox = new SceneRect(outputFree.Item1.X, outputFree.Item1.Y, outputFree.Item1.Width, outputFree.Item1.Height, outputFree.Item2);
                SceneManager.Instance.TemporaryScene.Add(freeBox);

                foreach (Tuple<PointF, String, System.Windows.Media.Color> t in outputString)
                {
                    SceneText idInfo = new SceneText(t.Item1.X / 2, (1 - t.Item1.Y) / 2, t.Item2, t.Item3, 10, new System.Windows.Media.FontFamily("Arial"));
                    SceneManager.Instance.TemporaryScene.Add(idInfo);
                }

                foreach (Tuple<PointF[], System.Windows.Media.Color> b in outputBoxPoint)
                {
                    Polygon p = new Polygon();
                    foreach (PointF point in b.Item1)
                        p.Points.Add(new Vector2(point.X, point.Y));

                    SceneManager.Instance.TemporaryScene.Add(new ScenePolygon(p, b.Item2));
                }
            }
            #endregion
        }

        private void m_ComboBoxSelectedCameras_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m_ComboBoxSelectedCameras.SelectedItem != null)
            {
                if (m_ComboBoxSelectedCameras.SelectedItem.ToString().Equals(ProjectConstants.KINECTV2DESCRIPTION))
                {
                    CameraManager.Instance.enableKinectV2EventMangementSystem();
                }
                else if (m_ComboBoxSelectedCameras.SelectedItem.ToString().Equals(ProjectConstants.ENSENSON10DESCRIPTION))
                {
                    CameraManager.Instance.enableEnsensoEventManagementSystem();
                }
                else if (m_ComboBoxSelectedCameras.SelectedItem.ToString().Equals(ProjectConstants.STRUCTURESENSORDESCRIPTION))
                {
                    CameraManager.Instance.enableOpenNIEventManagementSystem("PS1080");
                }
                else if (m_ComboBoxSelectedCameras.SelectedItem.ToString().Equals(ProjectConstants.KINECTV1DESCRIPTION))
                {
                    CameraManager.Instance.enableOpenNIEventManagementSystem("Kinect");
                }
            }

        }
    }
}
