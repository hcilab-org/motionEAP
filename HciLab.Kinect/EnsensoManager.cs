// <copyright file=EnsensoManager.cs
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
using Ensenso;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace HciLab.Kinect
{
    public class EnsensoManager
    {
        // constants
        private const int WIDTH  = 640;
        private const int HEIGHT = 480;

        // TODO: Move that to settings
        private const string ENSENSOSETTINGS = "ensensosettings.json";

        private static EnsensoManager m_Instance;
        private static bool m_InitFailed = false;
        private NxLibItem m_EnsensoCamera;
        private Bitmap m_Bitmap;
        private bool m_IsEnsensoCapturing;
        private Thread m_BackgroundThread;
        
        private EnsensoManager() {
        }

        public static EnsensoManager Instance
        {
            get 
            {
                if (m_InitFailed) return null;

                if (m_Instance == null)
                {
                    try
                    {
                        m_Instance = new EnsensoManager();
                    }
                    catch
                    {
                        m_InitFailed = true;
                        System.Console.WriteLine("Ensenso not initialized.");
                        return null;
                    }
                }
                return m_Instance;
            }
        }

        private void backgroundWorkerDoWork()
        {
            Thread.Sleep(1500);
            NxLibCommand capture = new NxLibCommand(NxLib.cmdCapture);

            NxLibCommand computeDisparityMap = new NxLibCommand(NxLib.cmdComputeDisparityMap);

            try
            {
                while (m_IsEnsensoCapturing)
                {

                    capture.Execute();
                    // compute disparity
                    computeDisparityMap.Execute();

                    int minDisp = m_EnsensoCamera[NxLib.itmParameters][NxLib.itmDisparityMap][NxLib.itmStereoMatching][NxLib.itmMinimumDisparity].AsInt();
                    int numDisp = m_EnsensoCamera[NxLib.itmParameters][NxLib.itmDisparityMap][NxLib.itmStereoMatching][NxLib.itmNumberOfDisparities].AsInt();

                    double timestamp;
                    int nboc;
                    short[] binaryData;

                    int width, height, channels, bpe;
                    bool isFloat;

                    m_EnsensoCamera[NxLib.itmImages][NxLib.itmDisparityMap].GetBinaryDataInfo(out width, out height, out channels, out bpe, out isFloat, out timestamp);
                    m_EnsensoCamera[NxLib.itmImages][NxLib.itmDisparityMap].GetBinaryData(out binaryData, out nboc, out timestamp);

                    int disparityScale = 16;
                    int scaledMinDisp = disparityScale * minDisp;
                    int scaledNumDisp = disparityScale * numDisp;

                    m_Bitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
                    ColorPalette ncp = m_Bitmap.Palette;
                    for (int i = 0; i < 256; i++)
                        ncp.Entries[i] = Color.FromArgb(255, i, i, i);
                    m_Bitmap.Palette = ncp;

                    BitmapData bmpData = m_Bitmap.LockBits(new Rectangle(0, 0, m_Bitmap.Width, m_Bitmap.Height), ImageLockMode.WriteOnly, m_Bitmap.PixelFormat);

                    int numBytes = bmpData.Height * bmpData.Stride;
                    var data = new byte[numBytes];
                    for (int i = 0; i < bmpData.Height; i++)
                    {
                        for (int j = 0; j < bmpData.Width; j++)
                        {
                            int d = binaryData[i * bmpData.Width + j];
                            int o = i * bmpData.Stride + j;
                            if (d < scaledMinDisp)
                            {
                                data[o] = (byte)255;
                            }
                            else if (d > scaledMinDisp + scaledNumDisp)
                            {
                                data[o] = (byte)0;
                            }
                            else
                            {
                                data[o] = (byte)(255.0 - ((d - scaledMinDisp) * (255.0 / scaledNumDisp)) + 0.5);
                            }
                        }
                    }
                    Marshal.Copy(data, 0, bmpData.Scan0, numBytes);

                    m_Bitmap.UnlockBits(bmpData);

                    Image<Bgra, Byte> colorFrame = new Image<Bgra, Byte>(m_Bitmap);
                    Image<Gray, Int32> xFrame = new Image<Gray, float>(m_Bitmap).Convert<Gray, Int32>();

                    Image<Gray, Byte> grayImage = (new Image<Gray, float>(m_Bitmap)).Convert<Gray, Byte>();

                    //grayImage = grayImage.SmoothMedian(15);
                    colorFrame = grayImage.Convert<Bgra, Byte>();

                    // event driven update
                    CameraManager.Instance.SetImages(colorFrame, colorFrame, xFrame, xFrame);
                }
            }
            catch (NxLibException exception)
            {
                Console.WriteLine("A NxLibException occured: " + exception.GetErrorText());
                Console.WriteLine(exception.StackTrace);
                Console.WriteLine("Error code: " + exception.GetErrorCode());
                NxLibItem captureResult = capture.Result()[NxLib.itmErrorText];
                Console.WriteLine(captureResult.AsString());
            }
            catch (Exception exception)
            {
                Console.WriteLine("An exception in the EnsensoManager occured: " + exception.Message);
            }
        }

        public void initEnsensoCapturing()
        {
            //Init NxLib
            int result;
            NxLib.Initialize(out result, true);
            NxLib.CheckReturnCode(result);

            //do stuff
            NxLibItem root = new NxLibItem();
            NxLibItem cameras = root[NxLib.itmCameras][NxLib.itmBySerialNo];

            for (int i = 0; i < cameras.Count(); i++)
            {
                NxLibItem camera = cameras[i];
                if (camera.Exists() && (camera[NxLib.itmType].Compare(NxLib.valStereo) == 0) && (camera[NxLib.itmStatus].Compare(NxLib.valAvailable) == 0))
                {
                    m_EnsensoCamera = cameras[camera[NxLib.itmSerialNumber].AsString()];

                    NxLibCommand open = new NxLibCommand(NxLib.cmdOpen);
                    open.Parameters()[NxLib.itmCameras].Set(m_EnsensoCamera[NxLib.itmSerialNumber].AsString());
                    open.Execute();

                    // read parameters from JSON
                    if (File.Exists(ENSENSOSETTINGS))
                    {
                        String jsonSettings = File.ReadAllText(ENSENSOSETTINGS);
                        jsonSettings = Regex.Replace(jsonSettings, @"\t|\n|\r", "");
                        m_EnsensoCamera[NxLib.itmParameters].SetJson(jsonSettings, true);
                    }

                    // setup thread
                    this.m_IsEnsensoCapturing = true;
                    m_BackgroundThread = new Thread(new ThreadStart(backgroundWorkerDoWork));
                    m_BackgroundThread.Start();
                    break;
                }
            }
        }


        public void stopEnsensoCapturing()
        {
            m_IsEnsensoCapturing = false;
            Thread.Sleep(1000);
            NxLibCommand close = new NxLibCommand(NxLib.cmdClose);
            try
            {
                close.Execute();
            }
            catch (NxLibException exception)
            {
                Console.WriteLine("Exception occurred during closing Ensenso N10: " + exception.GetErrorText());
                Console.WriteLine(exception.StackTrace);
                Console.WriteLine("Error code: " + exception.GetErrorCode());
            }
        }
    }
}
