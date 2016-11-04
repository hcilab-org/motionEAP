// <copyright file=BlobManager.cs
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
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using HciLab.motionEAP.InterfacesAndDataModel.Data;
using HciLab.Utilities;
using HciLab.Utilities.Mathematics.Core;
using HciLab.Utilities.Mathematics.Geometry2D;
using motionEAPAdmin.Database;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace motionEAPAdmin.Backend
{
    public class BlobManager
    {
        private static BlobManager m_Instance;

        private static Bgra BLUE = new Bgra(255, 0, 0, 0);
        private static Bgra RED = new Bgra(0, 0, 255, 0);
        private static Bgra GREEN = new Bgra(0, 255, 0, 0);

        private static bool m_LastWasCorrupted = false;

        private int m_Id = 10;
        private List<BlobObject> m_MasterBlob = new List<BlobObject>();
        
        private BlobManager() { }

        public List<BlobObject> MasterBlob
        {
            get { return m_MasterBlob; }
            set { m_MasterBlob = value; }
        }

        public int Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }

        public static BlobManager Instance
        {
            get 
            {
                if (m_Instance == null)
                {
                    m_Instance = new BlobManager();
                }
                return m_Instance;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pImage"></param>
        /// <param name="pDetectRemoval"></param>
        /// <param name="pMinArea"></param>
        /// <param name="pValueThreshold"></param>
        /// <returns></returns>
        public static List<BlobBound> FindAllBlob(Image<Gray, Int32> pImage, bool pDetectRemoval, double pMinArea = 0.0, double pValueThreshold = 0.0)
        {
            List<BlobBound> retList = new List<BlobBound>();

            //Arbitray factor to compensate biger value bandwith of new Image formate
            Image<Bgra, Byte> outputImage = pImage.Convert<Bgra, Byte>();
            outputImage._GammaCorrect(0.5);

            #region using contour

            //only RETR_TYPE.CV_RETR_CCOMP dose work with Int32
            for (Contour<System.Drawing.Point> contours = pImage.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_CCOMP); contours != null; contours = contours.HNext)
            {
                Seq<System.Drawing.Point> convexHull = contours.GetConvexHull(ORIENTATION.CV_CLOCKWISE);

                if (contours.BoundingRectangle.Width > 0 && contours.BoundingRectangle.Height > 0 && convexHull.Area > pMinArea)
                {

                    //Approximiere Kontur durch ein Polygon
                    //using (MemStorage storage = new MemStorage()) //allocate storage for contour approximation
                    //Contour<Point> currentContour = contours.ApproxPoly(contours.Perimeter * 0.05, storage);



                    Polygon poly = new Polygon();
                    foreach (Point p in convexHull)
                    {
                        poly.Points.Add(new Vector2(p.X, p.Y));
                    }

                    Rectangle bounds = contours.BoundingRectangle;
                    double summedValues = 0;
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
                        BlobBound bbound = new BlobBound(bounds, area, (int)summedValues, mask, poly);
                        retList.Add(bbound);
                        //outputImage.Draw(bbound.Points, GREEN, 1);
                        outputImage.Draw(bounds, BLUE, 1);
                        outputImage.Draw(convexHull, RED, 1);
                    }
                }
            }

            #endregion

            CvInvoke.cvShowImage(pDetectRemoval.ToString() + "-DepthChanges", outputImage.Ptr);

            retList.Sort((a, b) => a.Area.CompareTo(b.Area));
            retList.Reverse();

            return retList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="openCVImg"></param>
        /// <param name="masterBlobs"></param>
        /// <param name="colorBS"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static List<BlobObject> FindAllBlob(Image<Gray, Int32> openCVImg,
            List<BlobObject> masterBlobs,
            Image<Bgra, byte> colorBS,
            bool mode)
        {
            List<BlobObject> retList = new List<BlobObject>();
            try
            {
                Image<Gray, byte> gray_image = openCVImg.Convert<Gray, byte>();
                List<BlobObject> newBlobs = new List<BlobObject>();
                if (mode == false)
                {
                    #region using cvBlob
                    Emgu.CV.Cvb.CvBlobs resultingBlobs = new Emgu.CV.Cvb.CvBlobs();
                    Emgu.CV.Cvb.CvBlobDetector bDetect = new Emgu.CV.Cvb.CvBlobDetector();
                    uint numWebcamBlobsFound = bDetect.Detect(gray_image, resultingBlobs);

                    using (MemStorage stor = new MemStorage())
                    {
                        foreach (Emgu.CV.Cvb.CvBlob targetBlob in resultingBlobs.Values)
                        {
                            if (targetBlob.Area > 200)
                            {
                                var contour = targetBlob.GetContour(stor);

                                MCvBox2D box = contour.GetMinAreaRect();

                                PointF[] boxCorner = UtilitiesImage.ToPercent(contour.GetMinAreaRect().GetVertices(), gray_image.Width, gray_image.Height);

                                PointF center = UtilitiesImage.ToPercent(contour.GetMinAreaRect().center, gray_image.Width, gray_image.Height);

                                RectangleF rect = UtilitiesImage.ToPercent(targetBlob.BoundingBox, gray_image.Width, gray_image.Height);

                                Image<Gray, byte> newCropImg = UtilitiesImage.CropImage(colorBS.Convert<Gray, byte>(), rect);
                                newBlobs.Add(new BlobObject(newCropImg, null, boxCorner, rect, center, 0, 0, 0 + ""));
                                //stor.Clear();
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region using contour
                    using (MemStorage storage = new MemStorage())
                    {
                        //Find contours with no holes try CV_RETR_EXTERNAL to find holes
                        Contour<System.Drawing.Point> contours = gray_image.FindContours(
                         Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE,
                         Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_EXTERNAL,
                         storage);

                        for (int i = 0; contours != null; contours = contours.HNext)
                        {
                            i++;

                            //double area = contours.Area;
                            if (contours.Area > 200)
                            {

                                PointF[] boxCorner = UtilitiesImage.ToPercent(contours.GetMinAreaRect().GetVertices(), gray_image.Width, gray_image.Height);
                                PointF center = UtilitiesImage.ToPercent(contours.GetMinAreaRect().center, gray_image.Width, gray_image.Height);
                                RectangleF rect = UtilitiesImage.ToPercent(contours.BoundingRectangle, gray_image.Width, gray_image.Height);
                                Image<Bgra, byte> newCropImg = UtilitiesImage.CropImage(colorBS, rect);
                                newBlobs.Add(new BlobObject(newCropImg.Convert<Gray, byte>(), null, boxCorner, rect, center, 0, 0, 0 + ""));

                            }
                        }
                    }
                    #endregion
                }

                // read objects from database now
                List<TrackableObject> objects = DatabaseManager.Instance.Objects.ToList();

                if (objects.Count == 0)
                {
                    foreach (BlobObject b in newBlobs)
                    {
                        retList.Add(new BlobObject(b.Image, null, b.CornerPoints, b.Rect, b.Center, 0, 0, 0 + ""));
                    }
                }
                else
                {
                    #region

                    // size and position werden abgeglichen
                    List<Tuple<double, double, int, int>> trackInfo = new List<Tuple<double, double, int, int>>();
                    for (int newblob = 0; newblob < newBlobs.Count; newblob++)
                    {
                        for (int master = 0; master < masterBlobs.Count; master++)
                        {
                            double d = UtilitiesImage.Distance(newBlobs[newblob].Center, masterBlobs[master].Center);
                            double s = UtilitiesImage.DiffSize(newBlobs[newblob].Rect.Size, masterBlobs[master].Rect.Size);
                            trackInfo.Add(new Tuple<double, double, int, int>(d, s, master, newblob));
                        }
                    }


                    trackInfo.Sort((x, y) => x.Item1.CompareTo(y.Item1));
                    List<int> newItem = new List<int>();

                    if (!m_LastWasCorrupted)
                    {
                        while (trackInfo.Count != 0)
                        {
                            if (trackInfo[0].Item2 < 0.2)
                            {
                                int masterId = trackInfo[0].Item3;
                                int newId = trackInfo[0].Item4;
                                BlobObject newObject = new BlobObject(newBlobs[newId].Image, newBlobs[newId].DepthStructur, newBlobs[newId].CornerPoints, newBlobs[newId].Rect, newBlobs[newId].Center, masterBlobs[masterId].Hits, masterBlobs[masterId].Id, masterBlobs[masterId].Name);
                                retList.Add(newObject);
                                trackInfo.RemoveAll(item => item.Item3 == masterId);
                                trackInfo.RemoveAll(item => item.Item4 == newId);
                                newItem.Add(newId);
                            }
                            else
                            {
                                trackInfo.RemoveAt(0);
                            }

                        }
                        newItem.Sort();
                        //}


                        // check the images based on their features
                        for (int i = newBlobs.Count - 1; i >= 0; i--)
                        {
                            if (newItem.Count != 0 && i == newItem.Last())
                            {
                                newItem.RemoveAt(newItem.Count - 1);
                            }
                            else
                            {
                                // set the name according to the recognized object
                                string currentBlobId = RecognizeObject(newBlobs[i], objects);
                                newBlobs[i].Name = currentBlobId;
                                retList.Add(newBlobs[i]);
                            }
                        }
                    }
                }
            }
            catch (CvException e)
            {
                Logger.Instance.Log(e.Message, Logger.LoggerState.ERROR);
                return retList;
            }
            catch (Exception e)
            {
                Logger.Instance.Log(e.Message, Logger.LoggerState.ERROR);
                //mark a flag that the last frame was corrupt
                m_LastWasCorrupted = true;
            }
                    #endregion

            return retList;
        }


        /// <summary>
        /// Find an object in a object list
        /// </summary>
        /// <param name="currentBlob"></param>
        /// <param name="viBlobs"></param>
        /// <returns>BlobID</returns>
        public static string RecognizeObject(BlobObject currentBlob, List<TrackableObject> dbObjects)
        {
            bool isDetect = false;
            string blobId = "not identified";

            bool careAboutWorkflow = false;
            if (WorkflowManager.Instance.LoadedWorkflow != null)
            {
                careAboutWorkflow = true;
            }

            if (dbObjects.Count != 0)
            {

                foreach (TrackableObject obj in dbObjects)
                {
                    if (careAboutWorkflow)
                    {
                        // first check if object belongs to current workingstep
                        if (obj.Category == "" + WorkflowManager.Instance.CurrentWorkingStep.StepNumber)
                        {

                            // MFunk: Run this a couple of times to get more robust
                            int numRuns = 3;

                            for (int i = 0; i < numRuns; i++)
                            {

                                isDetect = UtilitiesImage.MatchIsSame(obj.EmguImage, currentBlob.Image);

                                if (isDetect)
                                {
                                    break;
                                }
                            }

                            if (isDetect)
                            {
                                WorkflowManager.Instance.OnObjectRecognized(obj);
                                blobId = obj.Name;
                                break;
                            }
                            else
                            {
                                isDetect = UtilitiesImage.MatchIsSame(obj.EmguImage.Canny(10, 50).Convert<Gray, Int32>(), currentBlob.Image.Canny(10, 50).Convert<Gray, Int32>());
                                if (isDetect)
                                {
                                    isDetect = true;
                                    blobId = obj.Name;
                                    WorkflowManager.Instance.OnObjectRecognized(obj);
                                    break;
                                }
                                else
                                {
                                    blobId = "not identified";
                                }
                            }

                            if (isDetect)
                            {
                                m_LastWasCorrupted = false;
                            }
                        }

                    }
                    else
                    {
                        // do not care about workflow

                        // MFunk: Run this a couple of times to get more robust
                        int numRuns = 3;

                        for (int i = 0; i < numRuns; i++)
                        {

                            isDetect = UtilitiesImage.MatchIsSame(obj.EmguImage, currentBlob.Image);

                            if (isDetect)
                            {
                                break;
                            }
                        }

                        if (isDetect)
                        {
                            WorkflowManager.Instance.OnObjectRecognized(obj);
                            blobId = obj.Name;
                            break;
                        }
                        else
                        {
                            isDetect = UtilitiesImage.MatchIsSame(obj.EmguImage.Canny(10, 50).Convert<Gray, Int32>(), currentBlob.Image.Canny(10, 50).Convert<Gray, Int32>());
                            if (isDetect)
                            {
                                isDetect = true;
                                blobId = obj.Name;
                                WorkflowManager.Instance.OnObjectRecognized(obj);
                                break;
                            }
                            else
                            {
                                blobId = "not identified";
                            }
                        }
                    }
                }
            }

            if (isDetect)
            {
                m_LastWasCorrupted = false;
            }
            return blobId;
        }


        /// <summary>
        /// Calculate the rotation of an object from image
        /// </summary>
        /// <param name="colorBS"></param>
        /// <param name="currentBlob"></param>
        /// <param name="viBlobs"></param>
        /// <returns>angle degree</returns>
        public static float GetRotation(Image<Bgra, byte> colorBS, BlobObject currentBlob, List<BlobObject> viBlobs)
        {
            Image<Bgra, byte> observedImage = UtilitiesImage.CropImage(colorBS, currentBlob.Rect);
            //observedImage.Save("current.jpg");
            float degree = 360;
            if (viBlobs.Count == 0)
            {
                return degree;
            }
            else
            {
                VectorOfKeyPoint keypoints1;
                VectorOfKeyPoint keypoints2;
                Matrix<float> symMatches;
                foreach (BlobObject viblob in viBlobs)
                {
                    //viblob.Image.Save(viblob.Id + ".jpg");
                    bool isDetect = UtilitiesImage.MatchIsSame(viblob.Image, observedImage.Convert<Gray, byte>(), out keypoints1, out keypoints2, out symMatches);

                    if (isDetect)
                    {

                        degree = UtilitiesImage.GetRotationDiff(symMatches, keypoints1, keypoints2);
                    }
                }
                return degree;
            }

        }
    }
}
