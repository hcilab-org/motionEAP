// <copyright file=CalibrationDialog.xaml.cs
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
using HciLab.Utilities;
using motionEAPAdmin.Backend;
using motionEAPAdmin.ContentProviders;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


namespace motionEAPAdmin.GUI
{
    /// <summary>
    /// Interaction logic for ConfigurationDialog.xaml
    /// </summary>
    public partial class CalibrationDialog : Window
    {

        Rectangle m_Rect = new Rectangle();
        Rectangle m_RectDrawingArea = new Rectangle();
        Rectangle m_RectAssemblyArea = new Rectangle();
        private AllEnums.Direction m_DragMode;
        bool m_DragEnabled = false;

        public static readonly int BOX_BORDERWIDTH = 15;

        public CalibrationDialog()
        {
            InitializeComponent();

            if (AdminView.Instance.IsKinectActive)
            {
                m_RectDrawingArea = SettingsManager.Instance.Settings.SettingsTable.KinectDrawing;
                m_RectAssemblyArea = SettingsManager.Instance.Settings.SettingsTable.KinectDrawing_AssemblyArea;
            }

            if (AdminView.Instance.IsEnsensoActive)
            {
                m_RectDrawingArea = SettingsManager.Instance.Settings.SettingsTable.EnsensoDrawing;
                m_RectAssemblyArea = SettingsManager.Instance.Settings.SettingsTable.EnsensoDrawing_AssemblyArea;
            }

            m_Rect = m_RectDrawingArea;

            TransformGroup group = new TransformGroup();
            group.Children.Add(new ScaleTransform());
            group.Children.Add(new TranslateTransform());

            m_Image.RenderTransform = group;

            this.DataContext = SettingsManager.Instance.Settings.SettingsTable;

            
        }

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        /// 
        /// 
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // KinectManager.Instance.allFramesReady += new KinectManager.AllFramesReadyHandler(Instance_allFramesReady);
            CameraManager.Instance.OnAllFramesReady += Instance_allFramesReady;
            // Go into Calibration-Mode
            CalibrationManager.Instance.StartCalibration();
        }


        /// <summary>
        /// This method is performed when the ConfigurationDialog is closed
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">CancelEventArgs</param>
        /// 
        /// 
        private void WindowClosed(object sender, System.EventArgs e)
        {
            CameraManager.Instance.OnAllFramesReady -= Instance_allFramesReady;
            CalibrationManager.Instance.StopCalibration(false);
        }

        bool gui_inizialized = false;

        /// <summary>
        /// Event handler for Kinect sensor's ColorFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        void Instance_allFramesReady(object pSource, Image<Bgra, byte> pColorImage, Image<Bgra, byte> pColorImageCropped, Image<Gray, Int32> pDepthImage, Image<Gray, Int32> pDepthImageCropped)
        {

            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                new Action(() =>{

                    if (gui_inizialized)
                    {
                        m_Image.Height = pColorImage.Height;
                        m_Image.Width = pColorImage.Width;
                        gui_inizialized = true;
                    }
                    pColorImage.Draw(new Rectangle(m_RectAssemblyArea.X, m_RectAssemblyArea.Y, m_RectAssemblyArea.Width, m_RectAssemblyArea.Height), new Bgra(0, 255, 0, 0), 0);
                    pColorImage.Draw(new Rectangle(m_RectDrawingArea.X, m_RectDrawingArea.Y, m_RectDrawingArea.Width, m_RectDrawingArea.Height), new Bgra(255, 0, 0, 0), 0);

                    UtilitiesImage.ToImage(m_Image, pColorImage);
                })
            );
        }



        
        /// <summary>
        /// Callback for the mousedown event on the configurationImage
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">eventarguments</param>
        /// 
        /// 
        private void m_Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // create initial rectangle
            if (m_Rect.Width == 0 & m_Rect.Height == 0)
            {
                // define a new box
                m_Rect = new Rectangle((int)e.GetPosition(m_Image).X, (int)e.GetPosition(m_Image).Y, 50, 50);
            }
            else
            {
                System.Windows.Point p = e.GetPosition(m_Image);
                initializeDrag(p);
            }

        }
        private void m_Image_MouseMove(object sender, MouseEventArgs e)
        {
            newRectSize(e.GetPosition(m_Image));
        }
        private void m_Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            newRectSize(e.GetPosition(m_Image));
            m_DragEnabled = false;
        }



        private void newRectSize(System.Windows.Point p)
        {
            if (m_DragEnabled)
            {
                if (m_DragMode == AllEnums.Direction.NORTH) // North
                {
                    if (p.Y > m_Rect.Y && p.Y < m_Rect.Y + m_Rect.Height)
                    {
                        m_Rect.Height = (m_Rect.Y + m_Rect.Height) - ((int)p.Y);
                        m_Rect.Y = (int)p.Y;
                    }

                    else if (p.Y < m_Rect.Y)
                    {
                        m_Rect.Height = m_Rect.Y + m_Rect.Height - ((int)p.Y);
                        m_Rect.Y = (int)p.Y;
                    }
                }
                else if (m_DragMode == AllEnums.Direction.EAST) // East
                {
                    if (p.X > m_Rect.X && p.X < m_Rect.X + m_Rect.Width)
                    {
                        m_Rect.Width = ((int)p.X) - m_Rect.X;
                    }

                    else if (p.X > m_Rect.X + m_Rect.Width)
                    {
                        m_Rect.Width = ((int)p.X) - m_Rect.X;
                    }
                }

                else if (m_DragMode == AllEnums.Direction.SOUTH) // South
                {
                    if (p.Y < m_Rect.Y + m_Rect.Height && p.Y > m_Rect.Y)
                    {
                        m_Rect.Height = ((int)p.Y) - m_Rect.Y;
                    }

                    else if (p.Y > m_Rect.Y + m_Rect.Height)
                    {
                        m_Rect.Height = ((int)p.Y) - m_Rect.Y;
                    }
                }
                else if (m_DragMode == AllEnums.Direction.WEST) // West
                {
                    if (p.X > m_Rect.X && p.X < m_Rect.X + m_Rect.Width)
                    {
                        m_Rect.Width = m_Rect.X + m_Rect.Width - ((int)p.X);
                        m_Rect.X = ((int)p.X);
                    }

                    else if (p.X < m_Rect.X)
                    {
                        m_Rect.Width = m_Rect.X + m_Rect.Width - ((int)p.X);
                        m_Rect.X = ((int)p.X);
                    }
                }
            }

            if (radioButtonAssembly.IsChecked == true)
                m_RectAssemblyArea = m_Rect;
            else
                m_RectDrawingArea = m_Rect;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (AdminView.Instance.IsKinectActive)
            {
                // save kinect data
                SettingsManager.Instance.Settings.SettingsTable.KinectDrawing = m_RectDrawingArea;
                SettingsManager.Instance.Settings.SettingsTable.KinectDrawing_AssemblyArea = m_RectAssemblyArea;
            }
            if (AdminView.Instance.IsEnsensoActive)
            {
                // save ensenso data
                SettingsManager.Instance.Settings.SettingsTable.EnsensoDrawing = m_RectDrawingArea;
                SettingsManager.Instance.Settings.SettingsTable.EnsensoDrawing_AssemblyArea = m_RectAssemblyArea;
            }
            


            CalibrationManager.Instance.StopCalibration(true);
            this.Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            CalibrationManager.Instance.StopCalibration(false);
            this.Close();
        }

        


        private void initializeDrag(System.Windows.Point p)
        {
            m_DragMode = isMouseonBoxFrame(p, m_Rect);
            if (m_DragMode != AllEnums.Direction.NONE)
            {
                m_DragEnabled = true;
            }
            else
            {
                m_DragEnabled = false;
            }
        }

        private AllEnums.Direction isMouseonBoxFrame(System.Windows.Point pPoint, Rectangle pRect)
        {
            if (pPoint.X >= pRect.X && pPoint.X <= (pRect.X + pRect.Width)
                    && pPoint.Y >= pRect.Y - (BOX_BORDERWIDTH / 2) 
                    && pPoint.Y <= pRect.Y + (BOX_BORDERWIDTH / 2))
            {
                return AllEnums.Direction.NORTH;
            }
            else if (pPoint.X >= pRect.X && pPoint.X <= (pRect.X + pRect.Width)
                    && pPoint.Y >= pRect.Y + pRect.Height - (BOX_BORDERWIDTH / 2)
                && pPoint.Y <= pRect.Y + (BOX_BORDERWIDTH / 2) + pRect.Height)
            {
                return AllEnums.Direction.SOUTH;
            }
            else if (pPoint.Y >= pRect.Y && pPoint.Y <= (pRect.Y + pRect.Height)
                    && pPoint.X >= pRect.X - (BOX_BORDERWIDTH / 2)
                    && pPoint.X  <= pRect.X + (BOX_BORDERWIDTH / 2))
            {
                return AllEnums.Direction.WEST;
            }
            else if (pPoint.Y >= pRect.Y && pPoint.Y <= (pRect.Y + pRect.Height)
                    && pPoint.X >= pRect.X + pRect.Width - (BOX_BORDERWIDTH / 2)
                    && pPoint.X <= pRect.X + (BOX_BORDERWIDTH / 2) + pRect.Width)
            {
                return AllEnums.Direction.EAST;
            }

            return AllEnums.Direction.NONE;
        }

        private void radioWButtonDrawing_Checked(object sender, RoutedEventArgs e)
        {
            m_Rect = m_RectDrawingArea;
        }

        private void radioButtonAssembly_Checked(object sender, RoutedEventArgs e)
        {
            m_Rect = m_RectAssemblyArea;
        }

        private void m_CbCheckerboard_Checked(object sender, RoutedEventArgs e)
        {
            motionEAPAdmin.Frontend.TableWindow3D.Instance.ShowCheckerboard();
        }

        private void m_CbCheckerboard_Unchecked(object sender, RoutedEventArgs e)
        {
            motionEAPAdmin.Frontend.TableWindow3D.Instance.HideCheckerboard();
        }
    }
}
