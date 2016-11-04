// <copyright file=MaximumSmoothing.cs
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
using System.Threading.Tasks;

namespace HciLab.Kinect.DepthSmoothing
{
    internal class MaximumSmoothing
    {
        /// <summary>
        /// Will specify how many frames to hold in the Queue for maximuming
        /// </summary>
        private int m_maximumFrameCount = 4;

        private Queue<Int32[]> maximumQueue = new Queue<Int32[]>();

        public static readonly int MaxMaximumFrameCount = 42;

        public Int32[] CreateMaximumDepthArray(Int32[] pDepthArray, int pWidth, int pHeight)
        {
            // This is a method of Weighted Moving Maximum per pixel coordinate across several frames of depth data.
            // This means that newer frames are linearly weighted heavier than older frames to reduce motion tails,
            // while still having the effect of reducing noise flickering.

            maximumQueue.Enqueue(pDepthArray);

            CheckForDequeue();

            int[] maxDepthArray = new int[pDepthArray.Length];
            int[] sumPlayerArray = new int[pDepthArray.Length];
            
            int Count = 1;

            // REMEMBER!!! Queue's are FIFO (first in, first out).  This means that when you iterate
            // over them, you will encounter the oldest frame first.

            // We first create a single array, summing all of the pixels of each frame on a weighted basis
            // and determining the denominator that we will be using later.
            foreach (var item in maximumQueue)
            {
                // Process each row in parallel
                Parallel.For(0, pHeight, depthArrayRowIndex =>
                {
                    // Process each pixel in the row
                    for (int index = depthArrayRowIndex * pWidth; index < (depthArrayRowIndex + 1) * pWidth; index++)
                    {
                        if (item[index] > maxDepthArray[index])
                            maxDepthArray[index] = item[index];
                    }
                });
                Count++;
            }

            Int32[] maximumdDepthArray = new Int32[pDepthArray.Length];
            // Process each row in parallel
            Parallel.For(0, pHeight, depthArrayRowIndex =>
            {
                // Process each pixel in the row
                for (int index = depthArrayRowIndex * pWidth; index < (depthArrayRowIndex + 1) * pWidth; index++)
                {
                    maximumdDepthArray[index] = (maxDepthArray[index]);
                }
            });

            return maximumdDepthArray;
        }

        private void CheckForDequeue()
        {
            // We will recursively check to make sure we have Dequeued enough frames.
            // This is due to the fact that a user could constantly be changing the UI element
            // that specifies how many frames to use for averaging.
            if (maximumQueue.Count > m_maximumFrameCount)
            {
                maximumQueue.Dequeue();
                CheckForDequeue();
            }
        }

        #region Getter/Tetter
        /// <summary>
        /// Will specify how many frames to hold in the Queue for maximuming
        /// </summary>
        public int MaximumFrameCount
        {
            get
            {
                return m_maximumFrameCount;
            }
            set
            {
                if (value > 0 && value <= MaxMaximumFrameCount)
                    m_maximumFrameCount = value;
            }
        }
        #endregion
    }
}
