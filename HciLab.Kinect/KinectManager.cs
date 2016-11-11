// <copyright file=KinectManager.cs
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
// <date> 11/2/2016 12:25:56 PM</date>

using System;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using HciLab.Kinect.DepthSmoothing;
using HciLab.Utilities;
using System.Drawing;
using HciLab.motionEAP.InterfacesAndDataModel.Data;

namespace HciLab.Kinect
{
    /// <summary>
    /// Handles all Kinect interaction.
    /// Listeners can subscribe to the events and get the Kinect data.
    /// </summary>
    /// <remarks>ATTENTION! When your delegate is called from this handler your delegate is run in the Kinect's thread</remarks>
    public class KinectManager
    {

        //public static Int16 SCALE_FACTOR = Int16.MaxValue / 1500;
        public static Int16 SCALE_FACTOR = 1;
        public static readonly string VIDEO_DIR = "Video";

        private static KinectManager instance;

        private bool captureNextFrame = false;
        private bool recordingActive = false;

        private Boolean m_SmoothingOn = true;

        private FilteredSmoothing m_SmoothingFilter = new FilteredSmoothing();
        private AveragedSmoothing m_SmoothingAverage = new AveragedSmoothing();
        private MaximumSmoothing m_SmoothingMaximum = new MaximumSmoothing();

        private KinectConnector m_KinectConnector = null;

        private Rectangle m_AssemblyArea = new Rectangle();

        private SettingsKinect m_KinectSettings = new SettingsKinect();

        private Image<Bgra, Byte> colorImg = null;
        private Image<Bgra, Byte> colorImgCropped = null;
        private Image<Gray, Int32> m_DepthImg = null;
        private Image<Gray, Int32> m_DepthImgCropped = null;

        private KinectManager()
        {
            try
            {
                m_KinectConnector = new KinectConnector();
                m_KinectConnector.allFramesReady += m_KinectConnector_allFramesReady;
                m_KinectConnector.Start();
            } catch(BadImageFormatException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Please install the Kinect SDK V2");
            }
        }

        public void setKinectSettings(SettingsKinect pKinectSettings, Rectangle pAssemblyArea)
        {
            m_KinectSettings = pKinectSettings;
            m_AssemblyArea = pAssemblyArea;
        }

        public void setAssemblyArea()
        {
        }


        public static KinectManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new KinectManager();
                }
                else
                {
                    /*if (instance.m_AssemblyArea.Width == 0 ||
                        instance.m_AssemblyArea.Height == 0)
                        throw new NotSupportedException();*/
                }
                return instance;
            }
        }

        private System.Drawing.Size m_ImageSize;


        public System.Drawing.Size ImageSize
        {
            get
            {
                return m_ImageSize;
            }
        }

        /// <summary>
        /// call this method if the next frame should be captured 
        /// </summary>
        public void takeColorPicture()
        {
            captureNextFrame = true;
        }


        public void StartKinectRecording()
        {
            // check if object dir exists
            if (!Directory.Exists(VIDEO_DIR))
            {
                // if not create it
                Directory.CreateDirectory(VIDEO_DIR);
            }

            long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            //motionVideo.VideoWriter.Start(ProjectConstants.OBJECT_DIR + "\\"+ now + "kinect_recording" + ".mp4");
            recordingActive = true;
        }

        public void StopKinectRecording()
        {
            //motionVideo.VideoWriter.Stop();
            recordingActive = false;
        }



        public void m_KinectConnector_allFramesReady(object pSource, Image<Bgra, Byte> pColorFrame, Image<Gray, Int16> pDepthFrame)
        {
            if (pColorFrame == null || pDepthFrame == null)
                return;

            OnOrgAllReady(this, pColorFrame, pDepthFrame);

            Image<Gray, Int16> depthFrameBuffer = pDepthFrame.Copy();

            if (m_KinectSettings.Ratio == 0)
                m_KinectSettings.Ratio = 2;

            Image<Bgra, Byte> colorFrameBuffer = pColorFrame.Resize((int)(pColorFrame.Width / m_KinectSettings.Ratio), (int)(pColorFrame.Height / m_KinectSettings.Ratio), Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);


            int xS = (int)(m_KinectSettings.XScale / m_KinectSettings.Ratio);
            int yS = (int)(m_KinectSettings.YScale / m_KinectSettings.Ratio);
            int xD = 0;
            int yD = 0;
            int xC = 0;
            int yC = 0;
            int w = 0;
            int h = depthFrameBuffer.Height;

            // depth image
            // what is the new width?
            if (xS < 0 && (depthFrameBuffer.Width + xS > colorFrameBuffer.Width))
            {
                // rechts und links raus -- sollte nie vorkommen
                w = colorFrameBuffer.Width;
                xD = -xS;
                xC = 0;
            }
            else if (depthFrameBuffer.Width + xS > colorFrameBuffer.Width)
            {
                // rechts raus
                w = colorFrameBuffer.Width - xS;
                xD = 0;
                xC = xS; // xS is positive
            }
            else if (xS < 0)
            {
                // links raus
                w = depthFrameBuffer.Width + xS;
                xD = -xS;
                xC = 0;
            }
            else
            {
                // drin
                w = depthFrameBuffer.Width;
                xD = 0;
                xC = xS;
            }

            // what is the new height?
            if (yS < 0 && (depthFrameBuffer.Height + yS > colorFrameBuffer.Height))
            {
                // oben und unten raus
                h = colorFrameBuffer.Height;
                yD = -yS;
                yC = 0;
            }
            else if (depthFrameBuffer.Height + yS > colorFrameBuffer.Height)
            {
                // nur unten raus
                h = colorFrameBuffer.Height - yS;
                yD = 0;
                yC = yS;

            }
            else if (yS < 0)
            {
                // nur oben raus
                h = depthFrameBuffer.Height + yS;
                yD = -yS;
                yC = 0;
            }
            else
            {
                // drin
                h = depthFrameBuffer.Height;
                yD = 0;
                yC = yS;
            }

            //Ausschnitt Setzen
            depthFrameBuffer.ROI = new System.Drawing.Rectangle(xD, yD, w, h);
            colorFrameBuffer.ROI = new System.Drawing.Rectangle(xC, yC, w, h);

            m_ImageSize = new System.Drawing.Size(w, h);

            if (colorImg == null || colorImg.Width != w || colorImg.Height != h)
                colorImg = new Image<Bgra, Byte>(w, h);
            if (m_DepthImg == null || m_DepthImg.Width != w || m_DepthImg.Height != h)
                m_DepthImg = new Image<Gray, Int32>(w, h);

            colorImg = colorFrameBuffer.Convert<Bgra, Byte>().Copy();
            CvInvoke.cvResetImageROI(colorFrameBuffer);

            colorImgCropped = UtilitiesImage.CropImage(colorImg, m_AssemblyArea);
            // colorImgCropped = colorImg;
            Image<Gray, Int32> newDepthImgCropped = UtilitiesImage.CropImage(m_DepthImg, m_AssemblyArea);
            // Image<Gray, Int32> newDepthImgCropped = m_DepthImg;


            m_DepthImg = depthFrameBuffer.Convert<Gray, float>().SmoothGaussian(5).Convert<Gray, Int32>();
            // m_DepthImg = pDepthFrame.Convert<Gray, float>().Convert<Gray, Int32>();

            //Shmoothing
            if (m_SmoothingOn)
            {
                /*this.m_SmoothingFilter.InnerBandThreshold = (int)InnerBandThresholdInput.Value;
                this.m_SmoothingFilter.OuterBandThreshold = (int)OuterBandThresholdInput.Value;
                this.m_SmoothingAverage.AverageFrameCount = (int)AverageFrameCountInput.Value;
                this.m_SmoothingMaximum.MaximumFrameCount = (int)AverageFrameCountInput.Value;*/

                //depthImg = this.m_SmoothingFilter.CreateFilteredDepthArray(pDepthFrame, m_KinectConnector.GetDepthFrameDescription().Width, m_KinectConnector.GetDepthFrameDescription().Height);
                //depthImg = this.m_SmoothingMaximum.CreateMaximumDepthArray(depthPixel, depthFrame.Width, depthFrame.Height);
                m_DepthImgCropped = this.m_SmoothingAverage.CreateAverageDepthArray(newDepthImgCropped);

            }
            else
            {
                m_DepthImgCropped = newDepthImgCropped;
            }


            OnAllFramesReady(this, colorImg, colorImgCropped, m_DepthImg, m_DepthImgCropped);

            // event driven update
            CameraManager.Instance.SetImages(colorImg, colorImgCropped, m_DepthImg, m_DepthImgCropped);
            CameraManager.Instance.SetOrgImages(pSource, pColorFrame, pDepthFrame.Convert<Gray, Int32>());
            
        }

        public delegate void AllFramesReadyHandler(object pSource, Image<Bgra, Byte> pColorImage, Image<Bgra, Byte> pColorImageCropped,
            Image<Gray, Int32> pDepthImage, Image<Gray, Int32> pDepthImageCropped);

        public event AllFramesReadyHandler allFramesReady;

        public void OnAllFramesReady(object pSource, Image<Bgra, Byte> pColorImage, Image<Bgra, Byte> pColorImageCropped,
            Image<Gray, Int32> pDepthImage, Image<Gray, Int32> pDepthImageCropped)
        {
            if (this.allFramesReady != null)
                allFramesReady(pSource, pColorImage, pColorImageCropped, pDepthImage, pDepthImageCropped);
        }
        public delegate void OrgAllReadyHandler(object pSource, Image<Bgra, Byte> pColorImage, Image<Gray, Int16> pDepthImage);
        public event OrgAllReadyHandler orgAllReady;

        public void OnOrgAllReady(object pSource, Image<Bgra, Byte> pColorImage, Image<Gray, Int16> pDepthImage)
        {
            if (this.orgAllReady != null)
                orgAllReady(pSource, pColorImage, pDepthImage);
        }

        public Image<Gray, Int32> GetCurrentDepthImageCropped()
        {
            return m_DepthImgCropped;
        }

        public Image<Gray, int> GetCurrentDepthImage()
        {
            return m_DepthImg;
        }

        public KinectConnector KinectConnector
        {
            get { return m_KinectConnector; }
            set { m_KinectConnector = value; }
        }

        /* /// <summary>
         /// reads the path from settings.ini (IMAGE_PATH="x:\...\...")
         /// to save the kinect image in the corresponding path 
         /// </summary>
         /// <returns></returns>
         public string ReadPath()
         {
             string file = System.IO.File.ReadAllText(".\\settings.ini");

             StringBuilder sb = new StringBuilder();
             using (System.IO.StreamReader sr = new System.IO.StreamReader("settings.ini"))
             {
                 String line;
                 // Read and display lines from the file until the end of 
                 // the file is reached.
                 while ((line = sr.ReadLine()) != null)
                 {
                     if (line.Contains("IMAGE_PATH="))
                     {
                         sb.AppendLine(line);
                     }
                 }
             }

             string allines = sb.ToString();
             //removes everything but the Path
             string path = allines.Substring(allines.IndexOf('=') + 1);
             //removes fragments of the conversion (\r\n)
             return path = path.Remove(path.Length - 2, 2);
         }

         public void SaveBitmap(WriteableBitmap writeableBitmap, string path)
         {
             string timestamp = DateTime.Now.ToString();
             timestamp = timestamp.Replace(".", "");
             timestamp = timestamp.Replace(" ", "");
             timestamp = timestamp.Replace(":", "");
             string ImagePath = path + timestamp.ToString() + ".jpg";

             UtilitiesImage.BitmapFromWriteableBitmap(writeableBitmap).Save(ImagePath, System.Drawing.Imaging.ImageFormat.Jpeg);
            
             string text = "timey whimy2";
             System.IO.File.WriteAllText(@ImagePath, text);
         }

         public void SaveColorImageToFile()
         {
             KinectManager.Instance.takeColorPicture();
         }

         public void SaveDepthImageToFile()
         {
         
         }

        /// <summary>
        /// returns the width of the color image from the kinect
        /// </summary>
        public System.Drawing.Size ColorImageSizeOrg
        {
            get
            {
                return new System.Drawing.Size(m_KinectConnector.GetColorFrameDescription().Height, m_KinectConnector.GetColorFrameDescription().Width);
            }
        }

        public System.Drawing.Size DepthImageSizeOrg
        {
            get
            {
                return new System.Drawing.Size(m_KinectConnector.GetDepthFrameDescription().Height, m_KinectConnector.GetDepthFrameDescription().Width);
            }
        }
         */
    }
}
