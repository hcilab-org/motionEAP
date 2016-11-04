// <copyright file=CalibrationManager.cs
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

using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace motionEAPAdmin.Backend
{
    /// <summary>
    /// This class handles the calibration of the projector and the cameras.
    /// It holds and stores the configuration.
    /// 
    /// </summary>
    /// 
    /// 
    public class CalibrationManager : INotifyPropertyChanged
    {

        private System.Drawing.Rectangle m_ProjectionArea;

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static CalibrationManager m_Instance;

        /// <summary>
        /// bool about the CalibrationManagers status. Are we in Calibration-Mode?
        /// </summary>
        private bool m_IsInCalibrationMode = false;

        public static CalibrationManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new CalibrationManager();
                }
                return m_Instance;
            }
        }

        /// <summary>
        ///  Hidden Constructor
        /// </summary>
        private CalibrationManager()
        {
            m_ProjectionArea = new System.Drawing.Rectangle(0, 0, 1024, 768);
        }


        public Boolean IsInCalibrationMode
        {
            get {
                return m_IsInCalibrationMode;
            }
        }

        /// <summary>
        /// This method starts the calibration mode and handles everything that is necessary.
        /// </summary>
        /// 
        /// 
        public void StartCalibration()
        {
            if (!m_IsInCalibrationMode)
            {
                m_IsInCalibrationMode = true;              
            }
            OnChangedCalibrationMode(this, m_IsInCalibrationMode, false);
        }

        /// <summary>
        /// This method stops the calibration mode if the CalibrationManager is in Calibration-Mode
        /// </summary>
        /// 
        /// 
        public void StopCalibration(Boolean pSaveCalibration = false)
        {
            if (m_IsInCalibrationMode)
            {
                m_IsInCalibrationMode = false;
            }
            OnChangedCalibrationMode(this, m_IsInCalibrationMode, pSaveCalibration);
        }
         
        public System.Drawing.Rectangle GetProjectionArea()
        {
            return m_ProjectionArea;
        }

        public delegate void ChangedCalibrationModeHandler(object pSource, Boolean pIsInCalibrationMode, Boolean pSaveCalibration);

        public event ChangedCalibrationModeHandler changedCalibrationMode;

        public void OnChangedCalibrationMode(object pSource, Boolean pIsInCalibrationMode, Boolean pSaveCalibration)
        {
            if (this.changedCalibrationMode != null)
                changedCalibrationMode(pSource, pIsInCalibrationMode, pSaveCalibration);
        }

        public BitmapSource renderCheckerboard(int wFields, int hFields, int xRes, int yRes, int xOffset, int yOffset)
        {
            var bm = new System.Drawing.Bitmap(xRes, yRes);
            var g = System.Drawing.Graphics.FromImage(bm);
            System.Drawing.Brush color1, color2;

            //Location (Upper Left Pixel) of the inner corners
            //checkerboardCorners = new Point[(wFields - 1), (hFields - 1)];
            //checkerboardCornersNormalized = new PointF[(wFields - 1), (hFields - 1)];

            color1 = System.Drawing.Brushes.White;

            int wField = (xRes - 2 * xOffset) / wFields;
            int hField = (yRes - 2 * yOffset) / hFields;

            g.FillRectangle(color1, 0, 0, xRes, yRes);

            for (int i = 0; i < hFields; i++)
            {
                if (i % 2 == 0)
                {
                    color1 = System.Drawing.Brushes.Black;
                    color2 = System.Drawing.Brushes.White;
                }
                else
                {
                    color1 = System.Drawing.Brushes.White;
                    color2 = System.Drawing.Brushes.Black;
                }
                for (int j = 0; j < wFields; j++)
                {
                    if (j % 2 == 0)
                        g.FillRectangle(color1, xOffset + (j * wField), yOffset + (i * hField), wField, hField);
                    else
                        g.FillRectangle(color2, xOffset + (j * wField), yOffset + (i * hField), wField, hField);

                    if (i != 0 && j != 0)
                    {
                        //checkerboardCorners[(j - 1), (i - 1)] = new Point(xOffset + (j * wField), yOffset + (i * hField));
                        //checkerboardCornersNormalized[(j - 1), (i - 1)] = new PointF(((float)xOffset + (j * wField)) / xRes, ((float)yOffset + (i * hField)) / yRes);
                    }
                }
            }
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                      bm.GetHbitmap(),
                      IntPtr.Zero,
                      System.Windows.Int32Rect.Empty,
                      BitmapSizeOptions.FromEmptyOptions());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
