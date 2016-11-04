// <copyright file=FreeSpaceManager.cs
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
using HciLab.Utilities.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace motionEAPAdmin.Backend
{     
    public static class FreeSpaceManager
    {
        public static int[,] RenderObject(PointF[] points, int[,] takenOrFree, float imageWidth, float imageHeight)
        {
            System.Drawing.Point[] boxPoints = new System.Drawing.Point[4];
            List<IEnumerable<System.Drawing.Point>> renderObject = new List<IEnumerable<System.Drawing.Point>>();

            for (int i = 0; i < points.Length; i++)
            {
                boxPoints[i] = WhichBox(points[i], takenOrFree.GetLength(0), takenOrFree.GetLength(1), imageWidth, imageHeight);
            }

            boxPoints = boxPoints.OrderBy(s => s.X).ThenBy(s => s.Y).ToArray();

            foreach (System.Drawing.Point p in boxPoints)
            {
                takenOrFree[p.Y, p.X] = 1;
            }

            renderObject.Add(Bresenham.RenderLine(boxPoints[0], boxPoints[1]));
            renderObject.Add(Bresenham.RenderLine(boxPoints[0], boxPoints[2]));
            renderObject.Add(Bresenham.RenderLine(boxPoints[1], boxPoints[3]));
            renderObject.Add(Bresenham.RenderLine(boxPoints[2], boxPoints[3]));

            foreach (IEnumerable<System.Drawing.Point> po in renderObject)
            {
                foreach (System.Drawing.Point p in po)
                {
                    //System.Console.WriteLine(p.X+" / "+p.Y);
                    takenOrFree[p.Y, p.X] = 1;
                }
            }
            return takenOrFree;
        }

        public static Point WhichBox(PointF point, int numWidth, int numHeight, float observedWidth, float observedHeight)
        {
            int x = 0, y = 0;
            Point isTaken = new Point();
            float boxWidth = (observedWidth / numWidth);
            float boxHeight = (observedHeight / numHeight);
            float imagePointX = point.X * observedWidth;
            float imagePointY = point.Y * observedHeight;
            if (imagePointX > 0 & imagePointY > 0 & imagePointX < observedWidth & imagePointY < observedHeight)
            {
                x = (int)(imagePointX / boxWidth);
                y = (int)(imagePointY / boxHeight);

            }
            else
            {
                if (imagePointX < 0)
                {
                    x = 0;
                }
                if (imagePointY < 0)
                {
                    y = 0;
                }
                if (imagePointX > observedWidth)
                {
                    x = numWidth - 1;
                }
                if (imagePointY > observedHeight)
                {
                    y = numHeight - 1;
                }

            }

            return isTaken = new Point(x, y);
        }

        public static Image<Bgra, byte> DrawMaxSubmatrix(Image<Bgra, byte> Image, int[,] isTaken)
        {
            int x1, y1, width, height;
            float boxW = (float)Image.Width / isTaken.GetLength(0);
            float boxH =(float)Image.Height / isTaken.GetLength(1);
            MaxSubmatrix(isTaken, out x1, out y1, out width, out height);

            PointF upLeft = new PointF(x1 * boxW, y1 * boxH);
            PointF bottumLeft = new PointF(x1 * boxW, y1 * boxH + boxH * height);
            PointF upRight = new PointF(x1 * boxW + boxW * width, y1 * boxH);
            PointF bottumRight = new PointF(x1 * boxW + boxW * width , y1 * boxH + boxH * height);

            LineSegment2DF up = new LineSegment2DF(upLeft, upRight);
            LineSegment2DF bottum = new LineSegment2DF(bottumLeft, bottumRight);
            LineSegment2DF left = new LineSegment2DF(upLeft, bottumLeft);
            LineSegment2DF right = new LineSegment2DF(upRight, bottumRight);

            Image.Draw(up, new Bgra(0, 255, 0 ,0 ), 2);
            Image.Draw(bottum, new Bgra(0, 255, 0, 0), 2);
            Image.Draw(left, new Bgra(0, 255, 0, 0), 2);
            Image.Draw(right, new Bgra(0, 255, 0, 0), 2);
            return Image;
        }

        public static RectangleF DrawMaxSubmatrix(int[,] isTaken, float boxW, float boxH)
        {
            int x1, y1, width, height;
            MaxSubmatrix(isTaken, out x1, out y1, out width, out height);
            float y = Math.Abs(y1 * boxH + height * boxH - 1);
            RectangleF freeBox = new RectangleF(x1 * boxW, y ,width * boxW, height * boxH);
            return freeBox;
        }

        public static void MaxSubmatrix(int[,] matrix, out int x1, out int y1, out int width, out int height)
        {
            int n = matrix.GetLength(0); // Number of rows  
            int m = matrix.GetLength(1); // Number of columns  
            width = 0;
            height = 0;
            int maxArea = -1, tempArea = -1;

            // Top-left corner (x1, y1); bottom-right corner (x2, y2)  
            x1 = 0;
            y1 = 0;
            int x2 = 0, y2 = 0;

            // Maximum row containing a 1 in this column  
            int[] d = new int[m];

            // Initialize array to -1  
            for (int i = 0; i < m; i++)
            {
                d[i] = -1;
            }

            // Furthest left column for rectangle  
            int[] d1 = new int[m];

            // Furthest right column for rectangle  
            int[] d2 = new int[m];

            Stack<int> stack = new Stack<int>();

            // Work down from top row, searching for largest rectangle  
            for (int i = 0; i < n; i++)
            {
                // 1. Determine previous row to contain a '1'  
                for (int j = 0; j < m; j++)
                {
                    if (matrix[i, j] == 1)
                    {
                        d[j] = i;
                    }
                }

                stack.Clear();

                // 2. Determine the left border positions  
                for (int j = 0; j < m; j++)
                {
                    while (stack.Count > 0 && d[stack.Peek()] <= d[j])
                    {
                        stack.Pop();
                    }

                    // If stack is empty, use -1; i.e. all the way to the left  
                    d1[j] = (stack.Count == 0) ? -1 : stack.Peek();

                    stack.Push(j);
                }

                stack.Clear();

                // 3. Determine the right border positions  
                for (int j = m - 1; j >= 0; j--)
                {
                    while (stack.Count > 0 && d[stack.Peek()] <= d[j])
                    {
                        stack.Pop();
                    }

                    d2[j] = (stack.Count == 0) ? m : stack.Peek();

                    stack.Push(j);
                }

                // 4. See if we've found a new maximum submatrix  
                for (int j = 0; j < m; j++)
                {
                    // (i - d[j]) := current row - last row in this column to contain a 1  
                    // (d2[j] - d1[j] - 1) := right border - left border - 1  
                    tempArea = (i - d[j]) * (d2[j] - d1[j] - 1);

                    if (tempArea > maxArea)
                    {
                        maxArea = tempArea;

                        // Top left  
                        x1 = d1[j] + 1;
                        y1 = d[j] + 1;

                        // Bottom right  
                        x2 = d2[j];
                        y2 = i + 1;
                    }
                }
                width = x2 - x1;
                height = y2 - y1;

            }
        }  

    }
}
