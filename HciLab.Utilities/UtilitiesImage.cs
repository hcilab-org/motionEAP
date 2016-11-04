// <copyright file=UtilitiesImage.cs
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
// <date> 11/2/2016 12:25:57 PM</date>

using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace HciLab.Utilities
{
    public static class UtilitiesImage
    {
        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pValue"></param>
        /// <param name="pMin"></param>
        /// <param name="pMax"></param>
        /// <returns></returns>
        public static System.Drawing.Color ToRGBSpec(int pValue, int pMin, int pMax)
        {
            int range = pMax - pMin;
            double value = (360.0 / range) * (double)(pValue - pMin);

            int red = (int)(Math.Sin(value + 2) * 127.0 + 128.0);
            int green = (int)(Math.Sin(value * 2 + 2) * 127.0 + 128.0);
            int blue = (int)(Math.Sin(value * 4 + 2) * 127.0 + 128.0);

            return HsvToRgb(value, 1, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static System.Drawing.Color HsvToRgb(double h, double s, double v)
        {
            int hi = (int)Math.Floor(h / 60.0) % 6;
            double f = (h / 60.0) - Math.Floor(h / 60.0);

            double p = v * (1.0 - s);
            double q = v * (1.0 - (f * s));
            double t = v * (1.0 - ((1.0 - f) * s));

            System.Drawing.Color ret;

            switch (hi)
            {
                case 0:
                    ret = GetRgb(v, t, p);
                    break;
                case 1:
                    ret = GetRgb(q, v, p);
                    break;
                case 2:
                    ret = GetRgb(p, v, t);
                    break;
                case 3:
                    ret = GetRgb(p, q, v);
                    break;
                case 4:
                    ret = GetRgb(t, p, v);
                    break;
                case 5:
                    ret = GetRgb(v, p, q);
                    break;
                default:
                    ret = System.Drawing.Color.FromArgb(0);
                    break;
            }
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static System.Drawing.Color GetRgb(double r, double g, double b)
        {
            return System.Drawing.Color.FromArgb(255, (byte)(r * 255.0), (byte)(g * 255.0), (byte)(b * 255.0));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectDepth"></param>
        /// <returns></returns>
        [System.Obsolete("use WriteableBitmap", false)]
        public static BitmapSource DepthToImage(int[,] objectDepth)
        {
            const int BlueIndex = 0;
            const int GreenIndex = 1;
            const int RedIndex = 2;
            int colorIndex = 0;

            var pixels = new byte[objectDepth.GetLength(1) * objectDepth.GetLength(0) * 4];
            for (int y = 0; y < objectDepth.GetLength(1); y++)
            {
                for (int x = 0; x < objectDepth.GetLength(0); x++)
                {
                    int depth = objectDepth[x, y];
                    byte intensity = (byte)(depth >= 800 && depth < 1040 ? depth : 0);

                    // Apply the intensity to the color channels
                    pixels[colorIndex + BlueIndex] = intensity; //blue
                    pixels[colorIndex + GreenIndex] = intensity; //green
                    pixels[colorIndex + RedIndex] = intensity; //red 
                    colorIndex += 4;
                }
            }
            return BitmapSource.Create(objectDepth.GetLength(0), objectDepth.GetLength(1), 96, 96, PixelFormats.Bgr32, null, pixels, objectDepth.GetLength(0) * 4);
        }

        /// <summary>
        /// Crop the depthImageFrame depending on rectangle.
        /// </summary>
        /// <param name="colorBS"></param>
        /// <param name="rect"></param>
        /// <returns>cropped image</returns>
        public static Image<Gray, Int32> CropImage(Image<Gray, Int32> colorBS, RectangleF rect)
        {
            Image<Gray, Int32> tempCropImg = colorBS.Copy();
            int x = (int)(rect.X * tempCropImg.Width);
            int y = (int)(rect.Y * tempCropImg.Height) - 10;
            int width = (int)(rect.Width * tempCropImg.Width) + 5;
            int height = (int)(rect.Height * tempCropImg.Height) + 5;
            System.Drawing.Rectangle newRec = new System.Drawing.Rectangle(x, y, width, height);
            tempCropImg.ROI = newRec;
            tempCropImg = tempCropImg.Copy();

            CvInvoke.cvResetImageROI(tempCropImg.Ptr);
            return tempCropImg;
        }

        /// <summary>
        /// Crop the depthImageFrame depending on rectangle.
        /// </summary>
        /// <param name="colorBS"></param>
        /// <param name="rect"></param>
        /// <returns>cropped image</returns>
        public static Image<Bgra, byte> CropImage(Image<Bgra, byte> pImage, Rectangle pCropArea)
        {
            Image<Bgra, byte> pImageToCrop = pImage.Clone();
            Image<Bgra, byte> imageCropped = new Image<Bgra, byte>(pCropArea.Width, pCropArea.Height);
            pImageToCrop.ROI = pCropArea;
            CvInvoke.cvCopy(pImageToCrop.Ptr, imageCropped.Ptr, IntPtr.Zero);
            return imageCropped;
        }

        public static Image<Bgra, byte> CropImage(Image<Bgra, byte> colorBS, RectangleF rect)
        {
            Image<Bgra, byte> tempCropImg = colorBS.Copy();
            int x = (int)(rect.X * tempCropImg.Width);
            int y = (int)(rect.Y * tempCropImg.Height) - 10;
            int width = (int)(rect.Width * tempCropImg.Width) + 5;
            int height = (int)(rect.Height * tempCropImg.Height) + 5;
            System.Drawing.Rectangle newRec = new System.Drawing.Rectangle(x, y, width, height);
            tempCropImg.ROI = newRec;
            tempCropImg = tempCropImg.Copy();

            CvInvoke.cvResetImageROI(tempCropImg.Ptr);
            return tempCropImg;
        }

        public static Image<Gray, byte> CropImage(Image<Gray, byte> colorBS, RectangleF rect)
        {
            Image<Gray, byte> tempCropImg = colorBS.Copy();
            int x = (int)(rect.X * tempCropImg.Width);
            int y = (int)(rect.Y * tempCropImg.Height) - 10;
            int width = (int)(rect.Width * tempCropImg.Width) + 5;
            int height = (int)(rect.Height * tempCropImg.Height) + 5;
            System.Drawing.Rectangle newRec = new System.Drawing.Rectangle(x, y, width, height);
            tempCropImg.ROI = newRec;
            tempCropImg = tempCropImg.Copy();

            CvInvoke.cvResetImageROI(tempCropImg.Ptr);
            return tempCropImg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pImageToCrop"></param>
        /// <param name="pCropArea"></param>
        /// <returns></returns>
        public static Image<Gray, Int32> CropImage(Image<Gray, Int32> pImage, Rectangle pCropArea)
        {
            Image<Gray, Int32> pImageToCrop = pImage.Clone();
            Image<Gray, Int32> imageCropped = new Image<Gray, Int32>(pCropArea.Width, pCropArea.Height);
            pImageToCrop.ROI = pCropArea;
            CvInvoke.cvCopy(pImageToCrop.Ptr, imageCropped.Ptr, IntPtr.Zero);
            return imageCropped;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="colorPixelsRGB"></param>
        /// <param name="pBytesPerPixel"></param>
        /// <param name="pImageWidth"></param>
        /// <param name="pRect"></param>
        /// <returns></returns>
        public static byte[] CropImage(byte[] colorPixelsRGB, int pBytesPerPixel, int pImageWidth, Rectangle pRect)
        {
            //Image<Gray, byte> ret = new Image<Gray, byte>(pRect.Width, pRect.Height);
            byte[] ret = new byte[pRect.Width * pRect.Height * pBytesPerPixel];
            int colorIndex = 0;

            for (int y = pRect.Y; y < pRect.Y + pRect.Height; y++)
                for (int x = pRect.X; x < pRect.X + pRect.Width; x++)
                {
                    int pixelPos = ((y * pImageWidth) + x) * pBytesPerPixel;

                    for (int i = 0; i < pBytesPerPixel; i++)
                        ret[colorIndex + i] = colorPixelsRGB[pixelPos + i];
                    //1.byte = blue 2.byte = green 3.byte = red; 4.byte = alpha 
                    colorIndex += pBytesPerPixel;
                }
            return ret;
        }

        /// <summary>
        /// from System.Media.BitmapImage to System.Drawing.Bitmap
        /// </summary>
        /// <param name="pBitmapSource"></param>
        /// <returns></returns>
        [System.Obsolete("use WriteableBitmap", true)]
        public static Bitmap ToBitmap(BitmapSource pBitmapSource)
        {
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(pBitmapSource));
                enc.Save(outStream);
                return new System.Drawing.Bitmap(outStream);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writeBmp"></param>
        /// <returns></returns>
        public static Bitmap ToBitmap(WriteableBitmap writeBmp)
        {
            System.Drawing.Bitmap bmp;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create((BitmapSource)writeBmp));
                enc.Save(outStream);
                bmp = new System.Drawing.Bitmap(outStream);
            }
            return bmp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        [System.Obsolete("use WriteableBitmap", true)]
        public static BitmapSource ToBitmapSource(Bitmap source)
        {
            IntPtr ptr = source.GetHbitmap();
            BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                ptr,
                IntPtr.Zero,
                Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(ptr);
            return bs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pValue"></param>
        /// <param name="pMin"></param>
        /// <param name="pMax"></param>
        /// <returns></returns>
        public static Gray ToGraySpec(double pValue, double pMin, double pMax)
        {
            return new Gray(((double)byte.MaxValue / (pMax - pMin)) * (pValue - pMin));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="pBytesPerPixel"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Image<Bgra, byte> ToImage(byte[] bytes, uint pBytesPerPixel, int width, int height)
        {
            Image<Bgra, byte> img = new Image<Bgra, byte>(width, height);
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            IntPtr imageHeaderForBytes = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MIplImage)));

            CvInvoke.cvInitImageHeader(
               imageHeaderForBytes,
               new System.Drawing.Size(width, height),
               Image<Bgra, Byte>.CvDepth,
               (int)pBytesPerPixel,
               1,
               (int)pBytesPerPixel);
            Marshal.WriteIntPtr(imageHeaderForBytes,
               (int)Marshal.OffsetOf(typeof(MIplImage), "imageData"),
               handle.AddrOfPinnedObject());
            CvInvoke.cvCopy(imageHeaderForBytes, img, IntPtr.Zero);

            Marshal.FreeHGlobal(imageHeaderForBytes);
            handle.Free();
            return img.Convert<Bgra, byte>();
        }
        
        /// <summary>
        /// Creat lines between the vertices, for drawing in viewbox
        /// </summary>
        /// <param name="poi"></param>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        /// <returns>list of lines</returns>
        public static List<LineSegment2DF> PointsToImageLine(PointF[] percentPoint, int imageWidth, int imageHeight)
        {
            List<LineSegment2DF> boxLine = new List<LineSegment2DF>();
            PointF upLeft = new PointF(percentPoint[0].X * imageWidth, percentPoint[0].Y * imageHeight);
            PointF buttomLeft = new PointF(percentPoint[1].X * imageWidth, percentPoint[1].Y * imageHeight);
            PointF upRight = new PointF(percentPoint[2].X * imageWidth, percentPoint[2].Y * imageHeight);
            PointF buttomRight = new PointF(percentPoint[3].X * imageWidth, percentPoint[3].Y * imageHeight);
            boxLine.Add(new LineSegment2DF(upLeft, upRight));
            boxLine.Add(new LineSegment2DF(buttomLeft, buttomRight));
            boxLine.Add(new LineSegment2DF(upLeft, buttomLeft));
            boxLine.Add(new LineSegment2DF(upRight, buttomRight));
            return boxLine;
        }

        /// <summary>
        /// Converts Rectangle depending on the image size.
        /// </summary>
        /// <param name="poi"></param>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        /// <returns>rectangle coordinates between 0 and 1 back</returns>
        public static RectangleF ToPercent(System.Drawing.Rectangle rect, int imageWidth, int imageHeight)
        {
            return new RectangleF((float)rect.Location.X / imageWidth, (float)rect.Location.Y / imageHeight, (float)rect.Width / imageWidth, (float)rect.Height / imageHeight);
        }

        /// <summary>
        /// Converts point coordinates in order depending on the image size.
        /// </summary>
        /// <param name="poi"></param>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        /// <returns>point coordinates between 0 and 1</returns>
        public static PointF ToPercent(System.Drawing.Point poi, int imageWidth, int imageHeight)
        {
            return new PointF(poi.X / imageWidth, poi.Y / imageHeight);

        }

        /// <summary>
        /// Converts point coordinates in order depending on the image size.
        /// </summary>
        /// <param name="poi"></param>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        /// <returns>point coordinates between 0 and 1 back.</returns>
        public static PointF ToPercent(PointF poi, int imageWidth, int imageHeight)
        {
            return new PointF(poi.X / imageWidth, poi.Y / imageHeight);

        }

        /// <summary>
        /// Converts vertices depending on the image size.
        /// </summary>
        /// <param name="poi"></param>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        /// <returns>vertices coordinates between 0 and 1 back.</returns>
        public static PointF[] ToPercent(PointF[] po, int imageWidth, int imageHeight)
        {
            PointF[] convertPointF = new PointF[4];
            po = po.OrderBy(s => s.X).ThenBy(s => s.Y).ToArray();
            for (int i = 0; i < po.Count(); i++)
                convertPointF[i] = new PointF(po[i].X / imageWidth, po[i].Y / imageHeight);
            return convertPointF;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Image"></param>
        /// <returns></returns>
        /*public static Bitmap ToBitmap(ColorImageFrame Image)
        {
            byte[] pixeldata = new byte[Image.PixelDataLength];
            Image.CopyPixelDataTo(pixeldata);
            Bitmap bmap = new Bitmap(Image.Width, Image.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            BitmapData bmapdata = bmap.LockBits(
                new Rectangle(0, 0, Image.Width, Image.Height),
                ImageLockMode.WriteOnly,
                bmap.PixelFormat);
            IntPtr ptr = bmapdata.Scan0;
            Marshal.Copy(pixeldata, 0, ptr, Image.PixelDataLength);
            bmap.UnlockBits(bmapdata);
            return bmap;
        }*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="depthPixels"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static int[] SaveBackground(Image<Gray,Int32> depthPixels, System.Drawing.Rectangle r)
        {
            int[] depth = new int[r.Width * r.Height];
            int index = 0;

            Parallel.For(r.Y, r.Y + r.Height, y =>
            {
                for (int x = r.X; x < r.X + r.Width; ++x)
                {
                    depth[(y - r.Y) * r.Height + x - r.X] = depthPixels.Data[y,x,0];
                    index++;
                }
            });
            return depth;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pPoint1"></param>
        /// <param name="pPoint2"></param>
        /// <returns></returns>
        public static System.Drawing.Rectangle ToRectangle(System.Drawing.Point pPoint1, System.Drawing.Point pPoint2)
        {
            return new Rectangle(pPoint1, new System.Drawing.Size(pPoint2.X - pPoint1.X, pPoint2.Y - pPoint1.Y));
        }



        public static System.Drawing.Bitmap BitmapFromWriteableBitmap(WriteableBitmap writeBmp)
        {
            System.Drawing.Bitmap bmp;
            using (System.IO.MemoryStream outStream = new System.IO.MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create((BitmapSource)writeBmp));
                enc.Save(outStream);
                bmp = new System.Drawing.Bitmap(outStream);
            }
            return bmp;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="m_DepthPixels"></param>
        /// <param name="maxDepth"></param>
        /// <param name="minDepth"></param>
        /// <param name="pFramePixelDataLength"></param>
        /// <returns></returns>
        public static byte[] ToGray(Int32[] m_DepthPixels, int maxDepth, int minDepth,
            int pFramePixelDataLength)
        {

            byte[] colorPixelsDepth = new byte[pFramePixelDataLength * sizeof(int)];
            byte intensity = (byte)0;
            // Convert the depth to RGB
            int colorPixelIndex = 0;
            for (int i = 0; i < m_DepthPixels.Length; ++i)
            {
                // Get the depth for this pixel
                Int32 depth = m_DepthPixels[i];

                // To convert to a byte, we're discarding the most-significant
                // rather than least-significant bits.
                // We're preserving detail, although the intensity will "wrap."
                // Values outside the reliable depth range are mapped to 0 (black).

                // Note: Using conditionals in this loop could degrade performance.
                // Consider using a lookup table instead when writing production code.
                // See the KinectDepthViewer class used by the KinectExplorer sample
                // for a lookup table example.
                intensity = (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0);


                // Write out blue byte
                colorPixelsDepth[colorPixelIndex++] = intensity;

                // Write out green byte
                colorPixelsDepth[colorPixelIndex++] = intensity;

                // Write out red byte                        
                colorPixelsDepth[colorPixelIndex++] = intensity;

                // We're outputting Bgra, the last byte in the 32 bits is unused so skip it
                // If we were outputting BGRA, we would write alpha here.
                ++colorPixelIndex;

            }

            return colorPixelsDepth;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pDepthPixels"></param>
        /// <param name="pMaxDepth"></param>
        /// <param name="pMinDepth"></param>
        /// <param name="pFramePixelDataLength"></param>
        /// <param name="pWidth"></param>
        /// <param name="pHeight"></param>
        /// <returns></returns>
        public static Image<Gray, Int32> ToImageGray(Int32[] pDepthPixels, int pWidth, int pHeight)
        {
            try
            {
                Image<Gray, Int32> ret = new Image<Gray, Int32> (pWidth, pHeight);
                byte[] bytes = new byte[pWidth * pHeight * 4];
                for (int i = 0; i < pDepthPixels.Length; i = i +4)
                {
                    byte[] b = BitConverter.GetBytes(pDepthPixels[i]);
                    bytes[i] = b[0];
                    bytes[i+1] = b[1];
                    bytes[i+2] = b[2];
                    bytes[i+3] = b[3];

                }
                ret.Bytes = bytes;

                return ret;
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Convert the depth to RGB
        /// </summary>
        /// <param name="pDepthPixels"></param>
        /// <param name="pMinDepth"></param>
        /// <param name="pMaxDepth"></param>
        /// <returns></returns>
        public static byte[] ToGrayByte(Int32[] pDepthPixels,
            int pMinDepth,
            int pMaxDepth,
            int pFramePixelDataLength)
        {
            byte[] ret = new byte[pFramePixelDataLength];
            byte intensity = (byte)0;

            for (int i = 0; i < pDepthPixels.Length; i = i + 4)
            {
                // Get the depth for this pixel
                Int32 depth = pDepthPixels[i];

                // To convert to a byte, we're discarding the most-significant
                // rather than least-significant bits.
                // We're preserving detail, although the intensity will "wrap."
                // Values outside the reliable depth range are mapped to 0 (black).

                // Note: Using conditionals in this loop could degrade performance.
                // Consider using a lookup table instead when writing production code.
                // See the KinectDepthViewer class used by the KinectExplorer sample
                // for a lookup table example.
                intensity = (byte)(depth >= pMinDepth && depth <= pMaxDepth ? depth : 0);

                // Write out blue byte
                ret[i] = intensity;

                // Write out green byte
                ret[i + 1] = intensity;

                // Write out red byte                        
                ret[i + 2] = intensity;

                // We're outputting Bgra, the last byte in the 32 bits is unused so skip it
                // If we were outputting BGRA, we would write alpha here.
                //ret[i+3] = Alpa;
            }
            return ret;
        }

        /// <summary>
        /// 
        /// the depthImageFrame depending on two points.
        /// </summary>
        /// <param name="pDepthPixels"></param>
        /// <param name="pDepthImageWidth"></param>
        /// <param name="pPoint0"></param>
        /// <param name="pPoint1"></param>
        /// <returns>cropped image</returns>
        public static Image<Gray, Int32> CropImage(Int32[] pDepthPixels,
            int pDepthImageWidth,
            int pMaxDepth,
            int pMinDepth,
            Rectangle pRect,
            out int[][] pDepthMask)
        {
            int [][] depthMask = new int[pRect.Height][];
            var pixels = new byte[pRect.Height * pRect.Width * 4];
            Image<Gray, Int32> ret = new Image<Gray, Int32>(pRect.Width, pRect.Height);
            int index = 0;

            Parallel.For(pRect.Y, pRect.Y + pRect.Height, y =>
            {
                depthMask[y - pRect.Y] = new int[pRect.Width];
                for (int x = pRect.X; x < pRect.X + pRect.Width; x++)
                {
                    int depthIndex = (y * pDepthImageWidth) + x;

                    depthMask[(y - pRect.Y)][(x - pRect.X)] = pDepthPixels[depthIndex];

                    ret[y - pRect.Y, x - pRect.X] = ToGraySpec(pDepthPixels[depthIndex], pMinDepth, pMaxDepth);

                    index++;
                }
            });
            pDepthMask = depthMask;
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [System.Obsolete("use WriteableBitmap", true)]
        public static BitmapSource ToBitmapSource(UIElement element)
        {
            RenderTargetBitmap target = new RenderTargetBitmap((int)(element.RenderSize.Width), (int)(element.RenderSize.Height), 96, 96, PixelFormats.Pbgra32);
            VisualBrush brush = new VisualBrush(element);

            DrawingVisual visual = new DrawingVisual();
            var drawingContext = visual.RenderOpen();

            drawingContext.DrawRectangle(brush, null, new Rect(new System.Windows.Point(0, 0),
                new System.Windows.Point(element.RenderSize.Width, element.RenderSize.Height)));

            drawingContext.PushOpacityMask(brush);

            drawingContext.Close();

            target.Render(visual);

            return target;
        }

        [System.Obsolete("use WriteableBitmap", true)]
        public static BitmapSource ToBitmapSource(System.Windows.Controls.Canvas element)
        {
            RenderTargetBitmap target = new RenderTargetBitmap((int)(element.RenderSize.Width), (int)(element.RenderSize.Height), 96, 96, PixelFormats.Pbgra32);
            VisualBrush brush = new VisualBrush(element);

            DrawingVisual visual = new DrawingVisual();
            var drawingContext = visual.RenderOpen();

            drawingContext.DrawRectangle(brush, null, new Rect(new System.Windows.Point(0, 0),
                new System.Windows.Point(element.RenderSize.Width, element.RenderSize.Height)));

            drawingContext.PushOpacityMask(brush);

            drawingContext.Close();

            target.Render(visual);

            return target;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Point"></param>
        /// <param name="rect"></param>
        /// <param name="depthMask"></param>
        /// <returns></returns>
        public static int[,,] ObjectDepthPoint(PointF Point, RectangleF rect, Image<Gray,Int32> depthMask)
        {
            depthMask.ROI = new Rectangle((int)Point.X, (int)Point.Y, (int)rect.Width, (int)rect.Height);
            Image<Gray, Int32> mask = depthMask.Copy();
            return mask.Data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelImage"></param>
        /// <param name="observedImage"></param>
        /// <param name="homography"></param>
        public static void FindMatch(Image<Gray, Int32> modelImage, Image<Gray, Int32> observedImage, out HomographyMatrix homography)
        {
            VectorOfKeyPoint modelKeyPoints, observedKeyPoints;
            Matrix<int> indices;
            Matrix<byte> mask;
            int k = 2;
            double uniquenessThreshold = 0.8;
            SURFDetector surfCPU = new SURFDetector(500, false);

            homography = null;

            //extract features from the object image
            modelKeyPoints = new VectorOfKeyPoint();
            Matrix<float> modelDescriptors = surfCPU.DetectAndCompute(modelImage.Convert<Gray, byte>(), null, modelKeyPoints);


            // extract features from the observed image
            observedKeyPoints = new VectorOfKeyPoint();
            Matrix<float> observedDescriptors = surfCPU.DetectAndCompute(observedImage.Convert<Gray, byte>(), null, observedKeyPoints);
            BruteForceMatcher<float> matcher = new BruteForceMatcher<float>(DistanceType.L2);
            matcher.Add(modelDescriptors);

            indices = new Matrix<int>(observedDescriptors.Rows, k);
            Matrix<float> dist = new Matrix<float>(observedDescriptors.Rows, k);
            matcher.KnnMatch(observedDescriptors, indices, dist, k, null);
            mask = new Matrix<byte>(dist.Rows, 1);
            mask.SetValue(255);
            Features2DToolbox.VoteForUniqueness(dist, uniquenessThreshold, mask);

            int nonZeroCount = CvInvoke.cvCountNonZero(mask.Ptr);
            if (nonZeroCount >= 4)
            {
                nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints, indices, mask, 1.5, 20);
                if (nonZeroCount >= 4)
                    homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, observedKeyPoints, indices, mask, 2);
            }

        }

        /// <summary>
        /// Convert a number to a color between green and red
        /// </summary>
        /// <param name="count"></param>
        /// <returns>color value</returns>
        public static Bgra MappingColor(int count)
        {
            int colorG, colorR;
            int color = count * 8;
            if (color <= 255)
            {
                colorG = 255;
                colorR = color;
            }
            else
            {
                colorG = 255 - (int)(color / 2);
                colorR = 255;
            }

            return new Bgra(0, colorG, colorR, 0);
        }


        /// <summary>
        /// Detect the number of FeaturePoints
        /// </summary>
        /// <param name="colorBS"></param>
        /// <param name="rect"></param>
        /// <returns>FeaturePoint count</returns>
        public static int FeaturePerObject(Image<Bgra, byte> colorBS, RectangleF rect)
        {
            Image<Bgra, byte> smallColor = CropImage(colorBS, rect);

            SURFDetector surfCPU = new SURFDetector(500, false);
            MKeyPoint[] modelKeyPoints = surfCPU.DetectKeyPoints(smallColor.Convert<Gray, byte>(), null);

            return modelKeyPoints.Count();
        }



        public static bool MatchIsSame(Image<Gray, Int32> image1, Image<Gray, Int32> image2, out VectorOfKeyPoint keypoints1, out VectorOfKeyPoint keypoints2, out  Matrix<float> symMatches)
        {
            return MatchIsSame(image1.Convert<Gray, byte>(), image2.Convert<Gray, byte>(), out keypoints1, out keypoints2, out symMatches);
        }

        /// <summary>
        /// Match faeture points using symmetry test
        /// </summary>
        /// <param name="image1">input image1</param>
        /// <param name="image2">input image2</param>
        /// <param name="keypoints1">output keypoint1</param>
        /// <param name="keypoints2">output keypoint2</param>
        /// <returns></returns>
        public static bool MatchIsSame(Image<Gray, byte> image1, Image<Gray, byte> image2, out VectorOfKeyPoint keypoints1, out VectorOfKeyPoint keypoints2, out  Matrix<float> symMatches)
        {
            Feature2DBase<float> detector = new SURFDetector(100, false);
            bool isSame = false;

            //1a. Detection of the SURF features
            keypoints1 = detector.DetectKeyPointsRaw(image1, null);
            keypoints2 = detector.DetectKeyPointsRaw(image2, null);

            //1b. Extraction of the SURF descriptors
            Matrix<float> descriptors1 = detector.ComputeDescriptorsRaw(image1, null, keypoints1);
            Matrix<float> descriptors2 = detector.ComputeDescriptorsRaw(image2, null, keypoints2);

            //2. Match the two image descriptors
            //Construction of the match
            BruteForceMatcher<float> matcher = new BruteForceMatcher<float>(DistanceType.L2);
            //from image 1 to image 2
            //based on k nearest neighbours (with k=2)
            matcher.Add(descriptors1);
            //Number of nearest neighbors to search for
            int k = 2;
            int n = descriptors2.Rows;
            //The resulting n*k matrix of descriptor index from the training descriptors
            Matrix<int> trainIdx1 = new Matrix<int>(n, k);
            //The resulting n*k matrix of distance value from the training descriptors
            Matrix<float> distance1 = new Matrix<float>(n, k);
            matcher.KnnMatch(descriptors2, trainIdx1, distance1, k, null);

            //from image 1 to image 2
            matcher = new BruteForceMatcher<float>(DistanceType.L2);
            matcher.Add(descriptors2);
            n = descriptors1.Rows;
            //The resulting n*k matrix of descriptor index from the training descriptors
            Matrix<int> trainIdx2 = new Matrix<int>(n, k);
            //The resulting n*k matrix of distance value from the training descriptors
            Matrix<float> distance2 = new Matrix<float>(n, k);
            matcher.KnnMatch(descriptors1, trainIdx2, distance2, k, null);

            //3. Remove matches for which NN ratio is > than threshold
            int removed = RatioTest(ref trainIdx1, ref distance1);
            removed = RatioTest(ref trainIdx2, ref distance2);

            //4. Create symmetrical matches
            int symNumber = SymmetryTest(trainIdx1, distance1, trainIdx2, distance2, out symMatches);

            if (symNumber > 2)
            {
                isSame = true;
            }
            return isSame;
        }

        public static bool MatchIsSame(Image<Gray, Int32> image1, Image<Gray, Int32> image2)
        {
            return MatchIsSame(image1.Convert<Gray, byte>(), image2.Convert<Gray, byte>());
        }

        public static bool MatchIsSame(Image<Gray, byte> image1, Image<Gray, byte> image2)
        {
            Matrix<float> symMatches;
            Feature2DBase<float> detector = new SURFDetector(100, false);
            bool isSame = false;

            //1a. Detection of the SURF features
            VectorOfKeyPoint keypoints1 = detector.DetectKeyPointsRaw(image1, null);
            VectorOfKeyPoint keypoints2 = detector.DetectKeyPointsRaw(image2, null);

            if (keypoints1.Size == 0 || keypoints2.Size == 0)
            {
                return isSame;
            }

            //1b. Extraction of the SURF descriptors
            Matrix<float> descriptors1 = detector.ComputeDescriptorsRaw(image1, null, keypoints1);
            Matrix<float> descriptors2 = detector.ComputeDescriptorsRaw(image2, null, keypoints2);

            //2. Match the two image descriptors
            //Construction of the match
            BruteForceMatcher<float> matcher = new BruteForceMatcher<float>(DistanceType.L2);
            //from image 1 to image 2
            //based on k nearest neighbours (with k=2)
            matcher.Add(descriptors1);
            //Number of nearest neighbors to search for
            int k = 2;
            int n = descriptors2.Rows;
            //The resulting n*k matrix of descriptor index from the training descriptors
            Matrix<int> trainIdx1 = new Matrix<int>(n, k);
            //The resulting n*k matrix of distance value from the training descriptors
            Matrix<float> distance1 = new Matrix<float>(n, k);
            matcher.KnnMatch(descriptors2, trainIdx1, distance1, k, null);
            //matcher.Dispose();

            //from image 1 to image 2
            matcher = new BruteForceMatcher<float>(DistanceType.L2);
            matcher.Add(descriptors2);
            n = descriptors1.Rows;
            //The resulting n*k matrix of descriptor index from the training descriptors
            Matrix<int> trainIdx2 = new Matrix<int>(n, k);
            //The resulting n*k matrix of distance value from the training descriptors
            Matrix<float> distance2 = new Matrix<float>(n, k);
            matcher.KnnMatch(descriptors1, trainIdx2, distance2, k, null);

            //3. Remove matches for which NN ratio is > than threshold
            int removed = RatioTest(ref trainIdx1, ref distance1);
            removed = RatioTest(ref trainIdx2, ref distance2);

            //4. Create symmetrical matches
            int symNumber = SymmetryTest(trainIdx1, distance1, trainIdx2, distance2, out symMatches);

            if (symNumber > 2)
            {
                isSame = true;
            }
            return isSame;
        }

        /// <summary>
        /// Clear matches for which NN ratio is > than threshold
        /// </summary>
        /// <param name="trainIdx">match descriptor index</param>
        /// <param name="distance">match distance value</param>
        /// <returns>return the number of removed points</returns>
        public static int RatioTest(ref Matrix<int> trainIdx, ref Matrix<float> distance)
        {
            float ratio = 0.55f;
            int removed = 0;
            for (int i = 0; i < distance.Rows; i++)
            {
                if (distance[i, 0] / distance[i, 1] > ratio)
                {
                    trainIdx[i, 0] = -1;
                    trainIdx[i, 1] = -1;
                    removed++;
                }
            }
            return removed;
        }

        /// <summary>
        /// Create symMatches vector
        /// </summary>
        /// <param name="trainIdx1">match descriptor index 1</param>
        /// <param name="distance1">match distance value 1</param>
        /// <param name="trainIdx2">match descriptor index 2</param>
        /// <param name="distance2">match distance value 2</param>
        /// <param name="symMatches">return symMatches vector</param>
        /// <returns>return the number of symmetrical matches</returns>
        public static int SymmetryTest(Matrix<int> trainIdx1, Matrix<float> distance1, Matrix<int> trainIdx2, Matrix<float> distance2, out Matrix<float> symMatches)
        {
            symMatches = new Matrix<float>(trainIdx1.Rows, 4);
            int count = 0;
            //for all matches image1 -> image2
            for (int i = 0; i < trainIdx1.Rows; i++)
            {
                //ignore deleted matches
                if (trainIdx1[i, 0] == -1 && trainIdx1[i, 1] == -1)
                {
                    continue;
                }

                //for all matches image2 -> image1
                for (int j = 0; j < trainIdx2.Rows; j++)
                {
                    //ignore deleted matches
                    if (trainIdx2[j, 0] == -1 && trainIdx2[j, 1] == -1)
                    {
                        continue;
                    }

                    //Match symmetry test
                    //if (trainIdx1[i, 0] == trainIdx2[j, 1] &&
                    //    trainIdx1[i, 1] == trainIdx2[j, 0])
                    if (trainIdx1[i, 0] == j && trainIdx2[j, 0] == i)
                    {
                        symMatches[i, 0] = j;
                        symMatches[i, 1] = i;
                        symMatches[i, 2] = distance1[i, 0];
                        symMatches[i, 3] = distance1[i, 1];
                        count++;
                        break;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Calculate the rotation
        /// </summary>
        /// <param name="matches"></param>
        /// <param name="keyPoints1"></param>
        /// <param name="keyPoints2"></param>
        /// <returns>angle degree</returns>
        public static int GetRotationDiff(Matrix<float> matches, VectorOfKeyPoint keyPoints1, VectorOfKeyPoint keyPoints2)
        {
            List<PointF> selPoint1 = new List<PointF>();
            List<PointF> selPoint2 = new List<PointF>();

            for (int i = 0; i < matches.Rows; i++)
            {
                if (matches[i, 0] == 0 && matches[i, 1] == 0)
                {
                    continue;
                }
                selPoint1.Add(keyPoints1[(int)matches[i, 0]].Point);
                selPoint2.Add(keyPoints2[(int)matches[i, 1]].Point);
            }

            double d = 0;
            // warum tuple id reicht doch (distanze zischen 0 und x)
            // oder  tuple und distance zischen x und y berechnen.
            Tuple<int, int> bestPoints = new Tuple<int, int>(0, 0);
            for (int i = 1; i < selPoint1.Count; i++)
            {
                double tempd = Distance(selPoint1[0], selPoint1[i]);
                if (tempd > d)
                {
                    d = tempd;
                    bestPoints = new Tuple<int, int>(0, i);
                }
            }

            double firstGard = GetGradient(selPoint1[0], selPoint1[bestPoints.Item2]);
            double secGard = GetGradient(selPoint2[0], selPoint2[bestPoints.Item2]);


            return (int)(firstGard - secGard);
        }

        /// <summary>
        /// Calculate the gradient.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns>gradient</returns>
        public static double GetGradient(PointF p1, PointF p2)
        {
            float yDiff = p2.Y - p1.Y;
            float xDiff = p2.X - p1.X;
            double grad = Math.Atan2(yDiff, xDiff) * 180 / Math.PI;
            return grad;
        }

        /// <summary>
        /// Calculate the distance between two points
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns>distance</returns>
        public static double Distance(PointF p1, PointF p2)
        {
            double d;
            d = Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
            return d;
        }

        public static double DiffSize(SizeF box, SizeF box2)
        {
            double d;
            double boxArea = box.Height * box.Width;
            double boxArea2 = box2.Height * box2.Width;
            if (boxArea > boxArea2)
            {
                d = boxArea - boxArea2;
            }
            else
            {
                d = boxArea - boxArea2;
            }

            return d;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rgbImage"></param>
        public static void AnalyseObjectColor(Image<Gray, Int32> rgbImage)
        {
            //FeaturePoint
            SURFDetector surfCPU = new SURFDetector(500, false);
            MKeyPoint[] modelKeyPoints = surfCPU.DetectKeyPoints(rgbImage.Convert<Gray, byte>(), null);
            int keyPoints = modelKeyPoints.Count();

            //Colorintensity
            int intensity = (int)rgbImage.GetAverage().Intensity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="corners"></param>
        public static void SortCorners(PointF[] corners)
        {
            corners = corners.OrderBy(p => p.X).ThenBy(p => p.Y).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        public static MeshGeometry3D BuildMesh(int[,] depth)
        {
            MeshGeometry3D tmesh = new MeshGeometry3D();
            List<Vector3D> Normalvec = new List<Vector3D>();
            // collection of corners for the triangles
            Point3DCollection corners = new Point3DCollection();
            // collection of points
            Point3D[] points_array = new Point3D[4];
            // collection of all the triangles
            Int32Collection Triangles = new Int32Collection();
            // collection of all the cross product normals
            Vector3DCollection Normals = new Vector3DCollection();

            // collection of vectors
            Vector3D[] vectors_array = new Vector3D[5];
            int i = 0;
            for (int y = 0; y < depth.GetLength(2); y++)
            {
                for (int x = 0; x < depth.GetLength(1); x++)
                {

                    // triangle point locations
                    points_array[0] = new Point3D(x, y++, depth[x, y++]);
                    points_array[1] = new Point3D(x, y, depth[x, y]);
                    points_array[2] = new Point3D(x++, y, depth[x++, y]);
                    points_array[3] = new Point3D(x++, y++, depth[x++, y++]);

                    // create vectors of size difference between points
                    vectors_array[0] = new Vector3D(points_array[1].X - points_array[0].X, points_array[1].Y - points_array[0].Y, points_array[1].Z - points_array[0].Z);
                    vectors_array[1] = new Vector3D(points_array[1].X - points_array[2].X, points_array[1].Y - points_array[2].Y, points_array[1].Z - points_array[2].Z);
                    vectors_array[2] = new Vector3D(points_array[2].X - points_array[0].X, points_array[2].Y - points_array[0].Y, points_array[2].Z - points_array[0].Z);
                    vectors_array[3] = new Vector3D(points_array[0].X - points_array[3].X, points_array[0].Y - points_array[3].Y, points_array[0].Z - points_array[3].Z);
                    vectors_array[4] = new Vector3D(points_array[2].X - points_array[3].X, points_array[2].Y - points_array[3].Y, points_array[2].Z - points_array[3].Z);

                    // add the corners to the 2 triangles to form a square
                    corners.Add(points_array[0]);
                    corners.Add(points_array[1]);
                    corners.Add(points_array[2]);
                    corners.Add(points_array[2]);
                    corners.Add(points_array[3]);
                    corners.Add(points_array[0]);

                    // add triangles to the collection
                    Triangles.Add(i);
                    Triangles.Add(i + 1);
                    Triangles.Add(i + 2);
                    Triangles.Add(i + 3);
                    Triangles.Add(i + 4);
                    Triangles.Add(i + 5);

                    // find the normals of the triangles by taking the cross product
                    Normals.Add(Vector3D.CrossProduct(vectors_array[0], vectors_array[2]));
                    Normals.Add(Vector3D.CrossProduct(vectors_array[1], vectors_array[0]));
                    Normals.Add(Vector3D.CrossProduct(vectors_array[1], vectors_array[2]));
                    Normals.Add(Vector3D.CrossProduct(vectors_array[1], vectors_array[2]));
                    Normals.Add(Vector3D.CrossProduct(vectors_array[3], vectors_array[4]));
                    Normals.Add(Vector3D.CrossProduct(vectors_array[0], vectors_array[2]));
                    i = i + 6;
                }
            }

            // add texture to all the points
            tmesh.Positions = corners;
            tmesh.TriangleIndices = Triangles;
            tmesh.Normals = Normals;

            return tmesh;
        }

        public static void ToImage(System.Windows.Controls.Image pImage, Image<Bgra, byte> pImageToDraw)
        {

            //Zeichenn
            if (!(pImage.Source is WriteableBitmap) || pImage.Source.Width != pImageToDraw.Width || pImage.Source.Height != pImageToDraw.Height)
                pImage.Source = new WriteableBitmap(pImageToDraw.Width, pImageToDraw.Height, 96.0, 96.0, PixelFormats.Bgr32, null);

            WriteableBitmap source = pImage.Source as WriteableBitmap;

            // Write the pixel data into our bitmap
            source.WritePixels(
                new Int32Rect(0, 0, source.PixelWidth, source.PixelHeight),
                pImageToDraw.Bytes,
                source.PixelWidth * 4,
                0);
        }

        public static void ToImage(System.Windows.Controls.Image pImage, Image<Gray, Int32> pImageToDraw)
        {

            Image<Gray, Int16> imageCon = pImageToDraw.Convert<Gray, Int16>();

            if (!(pImage.Source is WriteableBitmap) || pImage.Source.Width != pImageToDraw.Width || pImage.Source.Height != pImageToDraw.Height)
                pImage.Source = new WriteableBitmap(imageCon.Width, imageCon.Height, 96.0, 96.0, PixelFormats.Gray16, null);

            WriteableBitmap source = pImage.Source as WriteableBitmap;

            // Write the pixel data into our bitmap
            source.WritePixels(
                new Int32Rect(0, 0, source.PixelWidth, source.PixelHeight),
                imageCon.Bytes,
                source.PixelWidth * 2,
                0);
        }
    }
}
