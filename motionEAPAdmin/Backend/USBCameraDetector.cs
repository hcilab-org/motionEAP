// <copyright file=USBCameraDetector.cs
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

using HciLab.motionEAP.InterfacesAndDataModel;
using System;
using System.Collections.ObjectModel;
using System.Management;

namespace motionEAPAdmin.Backend.CameraManager
{
    /// <summary>
    /// Detects if a USB device is added or removed
    /// </summary>
    public static class USBCameraDetector
    {

        // system detected a new device 
        public const int DbtDevicearrival = 0x8000;
        // device is gone
        public const int DbtDeviceremovecomplete = 0x8004;
        // device change event
        public const int WmDevicechange = 0x0219;
        private const int DbtDevtypDeviceinterface = 5;
        // USB devices
        private static readonly Guid GuidDevinterfaceUSBDevice = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED");

        // list which holds all camera devices
        private static ObservableCollection<string> m_ConnectedUSBCameras = new ObservableCollection<string>();

        /// <summary>
        /// Get USB device infos and check if it's a camera
        /// </summary>
        public static void UpdateConnectedUSBCameras()
        {
            m_ConnectedUSBCameras.Clear();
            // create query for 
            System.Management.ManagementClass USBClass = new ManagementClass("Win32_PnPEntity");
            System.Management.ManagementObjectCollection USBCollection = USBClass.GetInstances();

            foreach (System.Management.ManagementObject usb in USBCollection)
            {
                // get whole device id string. The vendor Id has to be parsed
                string deviceId = usb["deviceid"].ToString();
                int vendorIdIndex = deviceId.IndexOf("VID_");
                int productIdIndex = deviceId.IndexOf("PID_");
                // + 4 to remove "VID_" and "PID_"                    
                string startingAtVendorId = deviceId.Substring(vendorIdIndex + 4);
                string startingAtProductId = deviceId.Substring(productIdIndex + 4);
                // vid and pid is four characters long
                string vendorId = "0x" + startingAtVendorId.Substring(0, 4);
                string productId = "0x" + startingAtProductId.Substring(0, 4);

                //Console.WriteLine("VendorId: " + vendorId + " ProductId: " + productId);

                if (vendorId.Equals(ProjectConstants.KINECTV1VENDOR) && productId.Equals(ProjectConstants.KINECTV1PRODUCT))
                {
                    if (!m_ConnectedUSBCameras.Contains(ProjectConstants.KINECTV1DESCRIPTION))
                    {
                        m_ConnectedUSBCameras.Add(ProjectConstants.KINECTV1DESCRIPTION);
                    }
                }
                if (vendorId.Equals(ProjectConstants.KINECTV2VENDOR) && (productId.Equals(ProjectConstants.KINECTV2PRODUCT) || productId.Equals(ProjectConstants.KINECTV2FWPRODUCT)))
                {
                    if (!m_ConnectedUSBCameras.Contains(ProjectConstants.KINECTV2DESCRIPTION))
                    {
                        m_ConnectedUSBCameras.Add(ProjectConstants.KINECTV2DESCRIPTION);
                    }
                }
                if (vendorId.Equals(ProjectConstants.ENSENSON10VENDOR) && productId.Equals(ProjectConstants.ENSENSON10PRODUCT))
                {
                    if (!m_ConnectedUSBCameras.Contains(ProjectConstants.ENSENSON10DESCRIPTION))
                    {
                        m_ConnectedUSBCameras.Add(ProjectConstants.ENSENSON10DESCRIPTION);
                    }
                }
                if (vendorId.Equals(ProjectConstants.STRUCTURESENSORVENDOR) && productId.Equals(ProjectConstants.STRUCTURESENSORPRODUCT))
                {
                    if (!m_ConnectedUSBCameras.Contains(ProjectConstants.STRUCTURESENSORDESCRIPTION))
                    {
                        m_ConnectedUSBCameras.Add(ProjectConstants.STRUCTURESENSORDESCRIPTION);
                    }
                }
            }
        }

        public static ObservableCollection<string> ConnectedUSBCameras
        {
            get { return USBCameraDetector.m_ConnectedUSBCameras; }
            set { USBCameraDetector.m_ConnectedUSBCameras = value; }
        }

    }
}
