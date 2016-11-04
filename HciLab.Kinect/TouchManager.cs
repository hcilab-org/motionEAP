// <copyright file=TouchManager.cs
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

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using HciLab.Utilities.Mathematics.Core;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace HciLab.Kinect
{
    public class TouchManager
    {
        private static TouchManager instance;
        
        private Image<Gray, int> m_GroundTruthImg = null;

        private TouchManager()
        {
            KinectManager.Instance.allFramesReady += onAllFramesReady;
        }

        public static TouchManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TouchManager();
                }
                return instance;
            }
        }

        public void setGroundTruth()
        {
            m_GroundTruthImg = m_DepthImageCropped;
        }

        void onAllFramesReady(object pSource,
            Image<Bgra, byte> pColorImage,
            Image<Bgra, byte> pColorImageCropped,
            Image<Gray, int> pDepthImage,
            Image<Gray, int> pDepthImageCropped)
        {
            
            m_DepthImageCropped = pDepthImageCropped;
        }

        Image<Gray, int> m_DepthImageCropped = null;

        public void detect()
        {
            //new pDepthImageCropped diff on m_GroundTruthImg
            //Image<Gray, Byte> imageDeltaMaskByte = m_GroundTruthImg.Cmp(m_DepthImageCropped.Add(new Gray(2*KinectManager.SCALE_FACTOR)), Emgu.CV.CvEnum.CMP_TYPE.CV_CMP_GT);
            Image<Gray, Byte> imageDeltaMaskByte = m_GroundTruthImg.Cmp(m_DepthImageCropped, Emgu.CV.CvEnum.CMP_TYPE.CV_CMP_GE);

            CvInvoke.cvShowImage("imageDeltaMask", imageDeltaMaskByte.Ptr);
            Image<Gray, Int32> imageDeltaMaskInt = imageDeltaMaskByte.Convert<Int32>(delegate(Byte b)
            {
                if (b == 0)
                    return 0;
                else
                    return Int32.MaxValue;
            });
            CvInvoke.cvShowImage("cimageDeltaMask ", imageDeltaMaskInt.Ptr);

            Image<Gray, Int32> imageDelta = m_GroundTruthImg.AbsDiff(m_DepthImageCropped).And(imageDeltaMaskInt);

            double valueThreshold;
            int areaThreshold;

            valueThreshold = 50;
            areaThreshold = 30;

            List<Vector2> listBlobBounds = FindAllBlob(imageDelta, areaThreshold, valueThreshold);
        }


        public static List<Vector2> FindAllBlob(Image<Gray, Int32> pImage, double pMinArea = 0.0, double pValueThreshold = 0.0)
        {
            List<Vector2> retList = new List<Vector2>();

            Image<Bgra, Byte> outputImage = pImage.Convert<Bgra, Byte>();
            outputImage._GammaCorrect(0.5);

            //only RETR_TYPE.CV_RETR_CCOMP dose work with Int32
            for (Contour<System.Drawing.Point> contours = pImage.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_CCOMP); contours != null; contours = contours.HNext)
            {
                Seq<System.Drawing.Point> convexHull = contours.GetConvexHull(ORIENTATION.CV_CLOCKWISE);

                if (contours.BoundingRectangle.Width > 0 && contours.BoundingRectangle.Height > 0 && convexHull.Area > pMinArea)
                {

                    //Approximiere Kontur durch ein Polygon
                    //using (MemStorage storage = new MemStorage()) //allocate storage for contour approximation
                    //Contour<Point> currentContour = contours.ApproxPoly(contours.Perimeter * 0.05, storage);

                    Rectangle bounds = contours.BoundingRectangle;
                    int summedValues = 0;
                    int area = 0;

                    bool[,] mask = new bool[bounds.Width, bounds.Height];
                    Array.Clear(mask, 0, mask.Length);

                    for (int y = bounds.Y; y < bounds.Height + bounds.Y; y++)
                    {
                        for (int x = bounds.X; x < bounds.Width + bounds.X; x++)
                        {
                            if (pImage.Data[y, x, 0] > 0 && convexHull.InContour(new PointF(x, y)) > 0)
                            {
                                summedValues += pImage.Data[y, x, 0];
                                area++;
                                mask[x - bounds.X, y - bounds.Y] = true;
                            }
                        }
                    }

                    if (area > 0 && summedValues / area > pValueThreshold)
                    {
                        retList.Add(new Vector2(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2));
                        outputImage.Draw(bounds, new Bgra(0, 0, 255, 0), 1);
                    }
                }
            }

            CvInvoke.cvShowImage("Touch points", outputImage.Ptr);
            return retList;
        }
    }
}
