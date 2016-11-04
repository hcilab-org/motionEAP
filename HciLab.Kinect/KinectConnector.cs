// <copyright file=KinectConnector.cs
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
using System.ComponentModel;
using Microsoft.Kinect;
using System.Runtime.InteropServices;

namespace HciLab.Kinect
{
    public class KinectConnector : INotifyPropertyChanged
    {

        /// <summary>
        /// Map depth range to byte range
        /// </summary>
        private const Int16 MapDepthToByte = 8000 / Int16.MaxValue;

        private KinectSensor m_Sensor = null;
        private MultiSourceFrameReader m_reader;

        private FrameDescription m_DepthFrameDescription = null;
        private FrameDescription m_ColorFrameDescription = null;

        private Image<Bgra, Byte> m_ColorImg = null;
        private Image<Gray, Int16> m_DepthImg = null;

        byte[] m_ColorImgArrayBuffer;

        public FrameDescription GetColorFrameDescription()
        {
            return m_ColorFrameDescription;
        }

        public FrameDescription GetDepthFrameDescription()
        {
            return m_DepthFrameDescription;
        }


        public KinectConnector()
        {
        }

        public bool Start()
        {
            try
            {
                m_Sensor = KinectSensor.GetDefault();
                if (null != this.m_Sensor && !m_Sensor.IsOpen)
                {
                    // Turn on the color stream to receive color frames
                    m_DepthFrameDescription = m_Sensor.DepthFrameSource.FrameDescription;
                    m_ColorFrameDescription = m_Sensor.ColorFrameSource.FrameDescription;

                    m_reader = m_Sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth);
                    m_reader.MultiSourceFrameArrived += KinectConnector_MultiSourceFrameArrived;

                    m_Sensor.IsAvailableChanged += m_Sensor_IsAvailableChanged;

                    // open the sensor
                    m_Sensor.Open();

                }
                return true;
            }
            catch (System.InvalidOperationException e)
            {
                return false;
            }
        }       

        public bool Stop()
        {
            m_Sensor = KinectSensor.GetDefault();
            if (null != m_Sensor && m_Sensor.IsOpen)
            {

                m_reader = m_Sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth);
                m_reader.MultiSourceFrameArrived -= KinectConnector_MultiSourceFrameArrived;

                m_Sensor.Close();
                return true;
            }
            return false;
        }

        private void KinectConnector_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            MultiSourceFrameReference frameReference = e.FrameReference;
            try
            {
                MultiSourceFrame reference = frameReference.AcquireFrame();

                // ColorFrame is IDisposable
                using (ColorFrame colorFrame = reference.ColorFrameReference.AcquireFrame())
                {
                    if (colorFrame != null)
                    {
                        if (m_ColorImgArrayBuffer == null)
                            m_ColorImgArrayBuffer = new byte[m_ColorFrameDescription.Width * m_ColorFrameDescription.Height * 4];

                        if (colorFrame.RawColorImageFormat == ColorImageFormat.Rgba)
                        {
                            colorFrame.CopyRawFrameDataToArray(this.m_ColorImgArrayBuffer);
                        }
                        else
                        {
                            colorFrame.CopyConvertedFrameDataToArray(this.m_ColorImgArrayBuffer, ColorImageFormat.Rgba);
                        }


                        Image<Rgba, byte> t = new Image<Rgba, byte>(GetColorFrameDescription().Width, GetColorFrameDescription().Height);
                        t.Bytes = m_ColorImgArrayBuffer;
                        m_ColorImg = new Image<Bgra, byte>(t.Width, t.Height);
                        CvInvoke.cvCopy(t.Convert<Bgra, byte>().Ptr, m_ColorImg.Ptr, IntPtr.Zero);
                    }
                }

                using (DepthFrame depthFrame = reference.DepthFrameReference.AcquireFrame())
                {
                    if (depthFrame != null)
                    {
                        // the fastest way to process the body index data is to directly access 
                        // the underlying buffer
                        using (KinectBuffer depthBuffer = depthFrame.LockImageBuffer())
                        {
                            // verify data and write the color data to the display bitmap
                            if (((this.m_DepthFrameDescription.Width * this.m_DepthFrameDescription.Height) == (depthBuffer.Size / this.m_DepthFrameDescription.BytesPerPixel)))
                            {
                                int size = m_DepthFrameDescription.Width * m_DepthFrameDescription.Height * 2;
                                byte[] managedArray = new byte[size];
                                Marshal.Copy(depthBuffer.UnderlyingBuffer, managedArray, 0, size);

                                Image<Gray, Int16> t = new Image<Gray, Int16>(GetDepthFrameDescription().Width, GetDepthFrameDescription().Height);
                                t.Bytes = managedArray;

                                t = t.ConvertScale<Int16>(KinectManager.SCALE_FACTOR, 0);
                                m_DepthImg = t;
                            }
                        }
                    }
                }

                this.OnAllFramesReady(this, m_ColorImg, m_DepthImg);

            }
            catch (Exception ex)
            {
                // ignore if the frame is no longer available
            }
        }

        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Handels the ecent which the sensor becoms unavalible (e.g. paused, closed, unplugged)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        static int test = 0;

        public delegate void AllFramesReadyHandler(object pSource, Image<Bgra, Byte> pColorFrame, Image<Gray, Int16> pDepthFrame);

        public event AllFramesReadyHandler allFramesReady;

        public void OnAllFramesReady(object pSource, Image<Bgra, Byte> pColorFrame, Image<Gray, Int16> pDepthFrame)
        {
            if (this.allFramesReady != null)
                allFramesReady(pSource, pColorFrame, pDepthFrame);
        }
    }
}
