// <copyright file=CalibrateKinectDialog.xaml.cs
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
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Threading;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using HciLab.motionEAP.InterfacesAndDataModel.Data;
using HciLab.Utilities;

namespace HciLab.Kinect
{
    /// <summary>
    /// Interaktionslogik für CalibrateKinect.xaml
    /// </summary>
    public partial class CalibrateKinectDialog : Window, INotifyPropertyChanged
    {

        CameraManager.AllOrgFramesReady myHandler;

        int depthSatrtX = 0;
        int depthSatrtY = 0;

        SettingsKinect m_NewSettings;

        private bool m_BoundsOkay;

        public bool BoundsOkay
        {
            get { return m_BoundsOkay; }
            set { m_BoundsOkay = value; NotifyPropertyChanged("BoundsOkay"); }
        }


        public CalibrateKinectDialog(SettingsKinect pSettings)
        {
            InitializeComponent();
            
            sliderDepthRatio.Value = pSettings.Ratio;
            sliderDepthX.Value = pSettings.XScale;
            sliderDepthY.Value = pSettings.YScale;

            myHandler = Instance_allOrgFramesReady;
            CameraManager.Instance.OnAllOrgFramesReady += myHandler;

            DataContext = this;
        }

        /// <summary>
        /// Event handler for Kinect sensor's ColorFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        void Instance_allOrgFramesReady(object pSource, Image<Bgra, byte> pColorImage, Image<Gray, Int32> pDepthImage)
        {
            Dispatcher.Invoke(DispatcherPriority.DataBind,
                new Action(() =>
                {
                    Image<Bgra, byte> tempDepth = pDepthImage.Convert<Bgra, byte>();
                    tempDepth = tempDepth.Resize(sliderDepthRatio.Value, INTER.CV_INTER_LINEAR);

                    Image<Bgra, byte> tempColor = pColorImage.Copy();
                    int xS = (int)sliderDepthX.Value;
                    int yS = (int)sliderDepthY.Value;
                    int xD = 0;
                    int yD = 0;
                    int xC = 0;
                    int yC = 0;
                    int w = tempDepth.Width;
                    int h = tempDepth.Height;

                    if (xS >= 0 && tempColor.Width < tempDepth.Width + xS)
                    {
                        // rechts raus & links drin
                        w -= tempDepth.Width + xS - tempColor.Width;
                        xC = xS;
                    }
                    else if (xS < 0 && tempColor.Width < tempDepth.Width + xS)
                    {
                        // rechts raus & links raus
                        w = tempColor.Width;
                        xD = -xS;
                    }
                    else if (xS < 0)
                    {
                        // links raus
                        xD = -xS;
                        w += xS;
                    }
                    else
                    {
                        //mitte horizontal
                        xC = xS;
                    }

                    if (yS >= 0 && tempColor.Height < tempDepth.Height + yS)
                    {
                        //unten raus & oben drin
                        h -= tempDepth.Height + yS - tempColor.Height;
                        yC = yS;
                    }
                    else if (yS < 0 && tempColor.Height < tempDepth.Height + yS)
                    {
                        //unten raus & oben raus
                        h = tempColor.Height;
                        yD = -yS;
                    }
                    else if (yS < 0)
                    {
                        //oben raus
                        h += yS;
                        yD = -yS;
                    }
                    else
                    {
                        //mitte vetical
                        yC = yS;
                    }

                    try
                    {
                        //Ausschnitt Setzen
                        tempDepth.ROI = new Rectangle(xD, yD, w, h);
                        tempColor.ROI = new Rectangle(xC, yC, w, h);

                        //Bild Kopieren
                        tempDepth.CopyTo(tempColor);

                        BoundsOkay = true;
                    }
                    catch (CvException)
                    {
                        //Prevent crash when offsets are out of bounds
                        BoundsOkay = false;
                    }

                    //Auschnitt zurück setzen
                    tempColor.ROI = new Rectangle(0, 0, pColorImage.Width, pColorImage.Height);

                    UtilitiesImage.ToImage(m_Image, tempColor);
                })
            );
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            m_NewSettings  = new SettingsKinect((int)sliderDepthX.Value, (int)sliderDepthY.Value, sliderDepthRatio.Value);
            DialogResult = true;
            Close();
        }

        public SettingsKinect getSettings()
        {
            return m_NewSettings;
        }

        private void cancleButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            CameraManager.Instance.OnAllOrgFramesReady -= myHandler;
            base.OnClosed(e);
        }

        [Obsolete("use ShowDialog() for full functionality", true)]
        public new void Show()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string Obj)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(Obj));
            }
        }
    }
}
