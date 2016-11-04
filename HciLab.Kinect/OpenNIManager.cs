// <copyright file=OpenNIManager.cs
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
using System.Collections.Generic;
using OpenNIWrapper;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace HciLab.Kinect
{
    class OpenNIManager
    {
        private const int SUPPORTEDWIDTH = 640;
        private const int SUPPORTEDHEIGHT = 480;

        private Device m_CurrentDevice;
        private VideoStream m_CurrentSensor;
        private Bitmap m_Bitmap = new Bitmap(SUPPORTEDWIDTH, SUPPORTEDHEIGHT, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        private System.Drawing.Size m_SupportedResoulution = new System.Drawing.Size(SUPPORTEDWIDTH, SUPPORTEDHEIGHT);

        // singleton
        private static OpenNIManager m_Instance;

        internal static OpenNIManager Instance
        {
            get
            { 
                if (m_Instance == null)
                {
                    m_Instance = new OpenNIManager();
                }

                return m_Instance;
            }
            set { m_Instance = value; }
        }



        public OpenNIManager()
        {
            
        }

        
        public void initAndStart(string deviceName)
        {
            // Initialize OpenNI
            OpenNI.Initialize();

            DeviceInfo device = updateDeviceList(deviceName);
            // init sensor
            bool initSuccessful = initSensor(device);
            if (initSuccessful)
            {
                // start sensor
                startSensor();
            }
        }

        private DeviceInfo updateDeviceList(string deviceName)
        {
            DeviceInfo[] devices = OpenNI.EnumerateDevices();
            Console.WriteLine("Scanning for new devices...");
            foreach (var device in devices)
            {
                Console.WriteLine("New devices found: " + device);
                if (device.ToString().Equals(deviceName))
                {
                    return device;
                }
            }
            return null;
        }

        public bool initSensor(DeviceInfo device)
        {
            try
            {
                if (device != null)
                {
                    // get first device and open it
                    this.m_CurrentDevice = device.OpenDevice();
                    // set transmission mode (color, depth, IR) - as for now we always take the depth stream
                    this.m_CurrentSensor = this.m_CurrentDevice.CreateVideoStream(Device.SensorType.Depth);
                    // set video mode
                    IEnumerable<VideoMode> videoModes = this.m_CurrentSensor.SensorInfo.GetSupportedVideoModes();
                    foreach (VideoMode mode in videoModes)
                    {
                        if (mode.DataPixelFormat == VideoMode.PixelFormat.Depth100Um && mode.Resolution == m_SupportedResoulution)
                        {
                            this.m_CurrentSensor.VideoMode = (VideoMode)mode;
                            break;
                        }
                    }
                }
            }
            catch (OpenNIException openNIException)
            {
                Console.WriteLine("An exception ocurred while initializing a sensor.");
                Console.WriteLine(openNIException.Message);
                return false;
            }
            return true;
        }

        // start sensor
        public void startSensor()
        {
            if (this.m_CurrentSensor.Start() == OpenNI.Status.Ok)
            {
                this.m_CurrentSensor.OnNewFrame += currentFrame;
            }
        }

        // stop sensor
        public void stopSensor()
        {
            if (this.m_CurrentSensor != null && this.m_CurrentSensor.IsValid)
            {
                this.m_CurrentSensor.Stop();
                this.m_CurrentSensor.OnNewFrame -= this.currentFrame;
            }
        }

        // gets the current frame and is triggered every new frame
        private void currentFrame(VideoStream vStream)
        {
            if (vStream.IsValid && vStream.IsFrameAvailable())
            {
                using (OpenNIWrapper.VideoFrameRef frame = vStream.ReadFrame())
                {
                    if (frame.IsValid)
                    {
                        VideoFrameRef.CopyBitmapOptions options = VideoFrameRef.CopyBitmapOptions.None | VideoFrameRef.CopyBitmapOptions.Force24BitRgb;
                        lock (this.m_Bitmap)
                        {
                            try
                            {
                                frame.UpdateBitmap(m_Bitmap, options);
                                Image<Bgra, Byte> colorFrame = new Image<Bgra, Byte>(m_Bitmap);
                                Image<Gray, Int32> xFrame = new Image<Gray, float>(m_Bitmap).Convert<Gray, Int32>();

                                CameraManager.Instance.SetImages(colorFrame, colorFrame, xFrame, xFrame);
                            }
                            catch (Exception exception)
                            {
                                Console.WriteLine("Exception occurred while updating frame.");
                                Console.WriteLine(exception.Message);
                            }
                        }
                    }
                }
            }
        }

    }
}
