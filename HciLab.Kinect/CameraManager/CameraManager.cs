// <copyright file=CameraManager.cs
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
using Emgu.CV;
using Emgu.CV.Structure;
using System.IO;

namespace HciLab.Kinect
{
    /// <summary>
    /// Manages different connected camera managers
    /// </summary>
    public class CameraManager
    {
        // singleton
        private static CameraManager m_Instance;

        // received images
        private Image<Bgra, Byte> m_ColorImage;
        private Image<Bgra, Byte> m_ColorImageCropped;
        private Image<Gray, Int32> m_DepthImage;
        private Image<Gray, Int32> m_DepthImageCropped;

        private Image<Bgra, Byte> m_OrgColorImage;
        private Image<Gray, Int32> m_OrgDepthImage;

        // events
        public delegate void AllFramesReady(object pSource, Image<Bgra, Byte> pColorFrame, Image<Bgra, Byte> pColorFrameCropped,
                                            Image<Gray, Int32> pDepthFrame, Image<Gray, Int32> pDepthFrameCropped);
        public event AllFramesReady OnAllFramesReady;

        public delegate void AllOrgFramesReady(object pSource, Image<Bgra, byte> pColorImage, Image<Gray, Int32> pDepthImage);
        public event AllOrgFramesReady OnAllOrgFramesReady;

        private string m_SelectedCamera;

        public string SelectedCameraInterface
        {
            get { return m_SelectedCamera; }
            set { m_SelectedCamera = value; }
        }


        public CameraManager()
        {

        }

        public static CameraManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new CameraManager();
                }
                return m_Instance;
            }
        }

        public void enableKinectV2EventMangementSystem()
        {
            try
            {
                EnsensoManager.Instance.stopEnsensoCapturing();
            }
            catch (IOException e)
            {
                Console.WriteLine("Ensenso Library not found: Have you installed the Ensenso SDK?");
            }
            OpenNIManager.Instance.stopSensor();
            KinectManager.Instance.KinectConnector.allFramesReady += KinectManager.Instance.m_KinectConnector_allFramesReady;
            SelectedCameraInterface = "KinectV2";
        }

        public void enableEnsensoEventManagementSystem()
        {
            KinectManager.Instance.KinectConnector.allFramesReady -= KinectManager.Instance.m_KinectConnector_allFramesReady;
            OpenNIManager.Instance.stopSensor();
            try
            {
                EnsensoManager.Instance.initEnsensoCapturing();
            }
            catch (IOException e)
            {
                Console.WriteLine("Ensenso Library not found: Have you installed the Ensenso SDK?");
            }
            SelectedCameraInterface = "Ensenso";
        }

        public void enableOpenNIEventManagementSystem(string deviceName)
        {
            OpenNIManager.Instance.stopSensor();
            try
            {
                EnsensoManager.Instance.stopEnsensoCapturing();
            }
            catch (IOException e)
            {
                Console.WriteLine("Ensenso Library not found: Have you installed the Ensenso SDK?");
            }

            KinectManager.Instance.KinectConnector.allFramesReady -= KinectManager.Instance.m_KinectConnector_allFramesReady;
            OpenNIManager.Instance.initAndStart(deviceName);
            SelectedCameraInterface = "OpenNI";
        }

        public void SetImages(Image<Bgra, byte> colorImg, Image<Bgra, byte> colorImgCropped, Image<Gray, int> m_DepthImg, Image<Gray, int> m_DepthImgCropped)
        {
            ColorImage = colorImg;
            ColorImageCropped = colorImgCropped;
            DepthImage = m_DepthImg;
            DepthImageCropped = m_DepthImgCropped;
            if (OnAllFramesReady != null)
            {
                OnAllFramesReady(this, m_ColorImage, m_ColorImageCropped, m_DepthImage, m_DepthImageCropped);
            }
        }

        public void SetOrgImages(object pSource, Image<Bgra, Byte> pColorImage, Image<Gray, Int32> pDepthImage)
        {
            m_OrgColorImage = pColorImage;
            m_OrgDepthImage = pDepthImage;
            if (OnAllOrgFramesReady != null)
            {
                OnAllOrgFramesReady(this, m_OrgColorImage, m_OrgDepthImage);
            }
        }

        #region getter and setter
        public Image<Bgra, Byte> ColorImage
        {
            get { return m_ColorImage; }
            set
            {
                m_ColorImage = value;
            }
        }

        public Image<Bgra, Byte> ColorImageCropped
        {
            get { return m_ColorImageCropped; }
            set
            {
                m_ColorImageCropped = value;
            }
        }

        public Image<Gray, Int32> DepthImage
        {
            get { return m_DepthImage; }
            set
            {
                m_DepthImage = value;
            }
        }

        public Image<Gray, Int32> DepthImageCropped
        {
            get { return m_DepthImageCropped; }
            set
            {
                m_DepthImageCropped = value;
            }
        }
        #endregion

    }
}
