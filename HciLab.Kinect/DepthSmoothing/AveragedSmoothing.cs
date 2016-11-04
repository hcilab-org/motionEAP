// <copyright file=AveragedSmoothing.cs
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
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HciLab.Kinect.DepthSmoothing
{
    internal class AveragedSmoothing
    {
        /// <summary>
        /// Will specify how many frames to hold in the Queue for averaging 
        /// </summary>
        private int m_MaximumFrameCount = 12;

        private Queue<Image<Gray, Int32>> averageQueue = new Queue<Image<Gray, Int32>>();

        public static readonly int MaxAverageFrameCount = 12;

        public Image<Gray, Int32> CreateAverageDepthArray(Image<Gray, Int32> pImageToSmooth)
        {

            // This is a method of Weighted Moving Average per pixel coordinate across several frames of depth data.
            // This means that newer frames are linearly weighted heavier than older frames to reduce motion tails,
            // while still having the effect of reducing noise flickering.

            Image<Gray, Int32> buffer = null;
            foreach (Image<Gray, Int32> item in averageQueue)
            {
                buffer = item;
                break;
            }
            if (buffer != null && (buffer.Width != pImageToSmooth.Width || buffer.Height  != pImageToSmooth.Height))
                averageQueue.Clear();

            averageQueue.Enqueue(pImageToSmooth);

            CheckForDequeue();

            long[,] sumDepthArray = new long[pImageToSmooth.Width, pImageToSmooth.Height];
            int[,] sumIsKnownDepth = new int[pImageToSmooth.Width, pImageToSmooth.Height];
            Int32[, ,] averagedDepthArray = new Int32[pImageToSmooth.Height, pImageToSmooth.Width, 1];

            int Count = 1;

            // REMEMBER!!! Queue's are FIFO (first in, first out).  This means that when you iterate
            // over them, you will encounter the oldest frame first.

            // We first create a single array, summing all of the pixels of each frame on a weighted basis
            // and determining the denominator that we will be using later.
            foreach (Image<Gray, Int32> img in averageQueue)
            {
                // Process each row in parallel
                Parallel.For(0, pImageToSmooth.Height-1, y =>
                {
                    // Process each pixel in the row
                    for (int x = 0; x < pImageToSmooth.Width; x++)
                    {
                        if (img.Data[y, x, 0] != 0)
                        {
                            sumDepthArray[x,y] += img.Data[y, x, 0] * Count;
                            sumIsKnownDepth[x,y] += Count;
                        }
                    }
                });
                Count++;
            }

            // Once we have summed all of the information on a weighted basis, we can divide each pixel
            // by our calculated denominator to get a weighted average.

            // Process each row in parallel
            Parallel.For(0, pImageToSmooth.Height-1, y =>
            {
                // Process each pixel in the row
                for (int x = 0; x < pImageToSmooth.Width; x++)
                {
                    if (sumIsKnownDepth[x,y] != 0)
                        averagedDepthArray[y, x, 0] = (Int32)(sumDepthArray[x, y] / sumIsKnownDepth[x, y]);
                    else
                        averagedDepthArray[y, x, 0] = 0;
                }
            });

            Image<Gray, Int32> ret = new Image<Gray, Int32>(pImageToSmooth.Width, pImageToSmooth.Height);
            ret.Data = averagedDepthArray;
            return ret;
        }

        private void CheckForDequeue()
        {
            // We will recursively check to make sure we have Dequeued enough frames.
            // This is due to the fact that a user could constantly be changing the UI element
            // that specifies how many frames to use for averaging.
            if (averageQueue.Count > m_MaximumFrameCount)
            {
                averageQueue.Dequeue();
                CheckForDequeue();
            }
        }

        #region Getter/Tetter
        /// <summary>
        /// Will specify how many frames to hold in the Queue for averaging 
        /// </summary>
        public int AverageFrameCount
        {
            get { return m_MaximumFrameCount; }
            set { if (value > 0 && value <= MaxAverageFrameCount) m_MaximumFrameCount = value; }
        }
        #endregion
    }
}
