// <copyright file=FilteredSmoothing.cs
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

using System.Threading.Tasks;
using System;

namespace HciLab.Kinect.DepthSmoothing
{
    internal class FilteredSmoothing
    {
        /// <summary>
        /// Will specify how many non-zero pixels within a 1 pixel band
        /// around the origin there should be before a filter is applied 
        /// </summary>
        private int m_InnerBandThreshold;
        
        /// <summary>
        /// Will specify how many non-zero pixels within a 2 pixel band
        /// around the origin there should be before a filter is applied
        /// </summary>
        private int m_OuterBandThreshold;
        
        public static readonly int MaxInnerBandThreshold = 8;
        public static readonly int MaxOuterBandThreshold = 16;

        public FilteredSmoothing()
        {
            this.m_InnerBandThreshold = 2;
            this.m_OuterBandThreshold = 5;
        }

        public FilteredSmoothing(int InnerBandThreshold, int OuterbandThreshold)
            : this()
        {
            this.InnerBandThreshold = InnerBandThreshold;
            this.OuterBandThreshold = OuterBandThreshold;
        }

        public Int32[] CreateFilteredDepthArray(Int32[] pDepthArray, int pWidth, int pHeight)
        {
            /////////////////////////////////////////////////////////////////////////////////////
            // I will try to comment this as well as I can in here, but you should probably refer
            // to my Code Project article for a more in depth description of the method.
            /////////////////////////////////////////////////////////////////////////////////////

            Int32[] smoothDepthArray = new Int32[pDepthArray.Length];

            // We will be using these numbers for constraints on indexes
            int widthBound = pWidth - 1;
            int heightBound = pHeight - 1;

            // We process each row in parallel
            Parallel.For(0, pHeight, depthArrayRowIndex =>
            {
                // Process each pixel in the row
                for (int depthArrayColumnIndex = 0; depthArrayColumnIndex < pWidth; depthArrayColumnIndex++)
                {
                    var depthIndex = depthArrayColumnIndex + (depthArrayRowIndex * pWidth);

                    // We are only concerned with eliminating 'white' noise from the data.
                    // We consider any pixel with a depth of 0 as a possible candidate for filtering.
                    if (pDepthArray[depthIndex] != 0)
                    {
                        // From the depth index, we can determine the X and Y coordinates that the index
                        // will appear in the image.  We use this to help us define our filter matrix.
                        int x = depthIndex % pWidth;
                        int y = (depthIndex - x) / pWidth;

                        // The filter collection is used to count the frequency of each
                        // depth value in the filter array.  This is used later to determine
                        // the statistical mode for possible assignment to the candidate.
                        Int32[,] filterCollection = new Int32[24, 2];

                        // The inner and outer band counts are used later to compare against the threshold 
                        // values set in the UI to identify a positive filter result.
                        int innerBandCount = 0;
                        int outerBandCount = 0;

                        // The following loops will loop through a 5 X 5 matrix of pixels surrounding the 
                        // candidate pixel.  This defines 2 distinct 'bands' around the candidate pixel.
                        // If any of the pixels in this matrix are non-0, we will accumulate them and count
                        // how many non-0 pixels are in each band.  If the number of non-0 pixels breaks the
                        // threshold in either band, then the average of all non-0 pixels in the matrix is applied
                        // to the candidate pixel.
                        for (int yi = -2; yi < 3; yi++)
                        {
                            for (int xi = -2; xi < 3; xi++)
                            {
                                // yi and xi are modifiers that will be subtracted from and added to the
                                // candidate pixel's x and y coordinates that we calculated earlier.  From the
                                // resulting coordinates, we can calculate the index to be addressed for processing.

                                // We do not want to consider the candidate pixel (xi = 0, yi = 0) in our process at this point.
                                // We already know that it's 0
                                if (xi != 0 || yi != 0)
                                {
                                    // We then create our modified coordinates for each pass
                                    var xSearch = x + xi;
                                    var ySearch = y + yi;

                                    // While the modified coordinates may in fact calculate out to an actual index, it 
                                    // might not be the one we want.  Be sure to check to make sure that the modified coordinates
                                    // match up with our image bounds.
                                    if (xSearch >= 0 && xSearch <= widthBound && ySearch >= 0 && ySearch <= heightBound)
                                    {
                                        var index = xSearch + (ySearch * pWidth);
                                        // We only want to look for non-0 values
                                        if (pDepthArray[depthIndex] != 0)
                                        {
                                            // We want to find count the frequency of each depth
                                            for (int i = 0; i < 24; i++)
                                            {
                                                if (filterCollection[i, 0] == pDepthArray[index])
                                                {
                                                    // When the depth is already in the filter collection
                                                    // we will just increment the frequency.
                                                    filterCollection[i, 1]++;
                                                    break;
                                                }
                                                else if (filterCollection[i, 0] == 0)
                                                {
                                                    // When we encounter a 0 depth in the filter collection
                                                    // this means we have reached the end of values already counted.
                                                    // We will then add the new depth and start it's frequency at 1.
                                                    filterCollection[i, 0] = pDepthArray[index];
                                                    filterCollection[i, 1]++;
                                                    break;
                                                }
                                            }

                                            // We will then determine which band the non-0 pixel
                                            // was found in, and increment the band counters.
                                            if (yi != 2 && yi != -2 && xi != 2 && xi != -2)
                                                innerBandCount++;
                                            else
                                                outerBandCount++;
                                        }
                                    }
                                }
                            }
                        }

                        // Once we have determined our inner and outer band non-zero counts, and accumulated all of those values,
                        // we can compare it against the threshold to determine if our candidate pixel will be changed to the
                        // statistical mode of the non-zero surrounding pixels.
                        if (innerBandCount >= m_InnerBandThreshold || outerBandCount >= m_OuterBandThreshold)
                        {
                            Int32 frequencyDepth = 0;
                            Int32 depth = 0;
                            // This loop will determine the statistical mode
                            // of the surrounding pixels for assignment to
                            // the candidate.
                            for (int i = 0; i < 24; i++)
                            {
                                // This means we have reached the end of our
                                // frequency distribution and can break out of the
                                // loop to save time.
                                if (filterCollection[i, 0] == 0)
                                    break;
                                if (filterCollection[i, 1] > frequencyDepth)
                                {
                                    depth = filterCollection[i, 0];
                                    frequencyDepth = filterCollection[i, 1];
                                }
                            }

                            smoothDepthArray[depthIndex] = depth;
                        }

                    }
                    else
                    {
                        // If the pixel is not zero, we will keep the original depth.
                        smoothDepthArray[depthIndex] = pDepthArray[depthIndex];
                    }
                }
            });

            return smoothDepthArray;
        }

        #region Getter/Tetter
        public int OuterBandThreshold
        {
            get { return m_OuterBandThreshold; }
            set { if (value > 0 && value <= MaxOuterBandThreshold) m_OuterBandThreshold = value; }
        }
        public int InnerBandThreshold
        {
            get { return m_InnerBandThreshold; }
            set
            {
                if (value > 0 && value <= MaxInnerBandThreshold)
                    m_InnerBandThreshold = value;
            }
        }
        #endregion
    }
}
