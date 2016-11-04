// <copyright file=QRDetectManager.cs
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

using ZXing;

namespace motionEAPAdmin.Backend
{
    public class QRDetectManager
    {
        private static QRDetectManager m_Instance;

        public delegate void CodeDetectedHandler(object sender, CodeDetectedEventArgs e);

        public event CodeDetectedHandler OnCodeDetected;

        public bool TryHarder = false;

        private QRDetectManager()
        {
            //HciLab.Kinect.CameraManager.Instance.OnAllFramesReady += Instance_OnAllFramesReady;
        }

        void Instance_OnAllFramesReady(object pSource, Emgu.CV.Image<Emgu.CV.Structure.Bgra, byte> pColorFrame, Emgu.CV.Image<Emgu.CV.Structure.Bgra, byte> pColorFrameCropped, Emgu.CV.Image<Emgu.CV.Structure.Gray, int> pDepthFrame, Emgu.CV.Image<Emgu.CV.Structure.Gray, int> pDepthFrameCropped)
        {
            //SimpleScan(pColorFrameCropped);
        }
        
        public static QRDetectManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new QRDetectManager();
                }
                return m_Instance;
            }
        }

        public void SimpleScan(Emgu.CV.Image<Emgu.CV.Structure.Bgra, byte> pColorImage)
        {
            if (OnCodeDetected == null) return;
            if (pColorImage == null) return;

            BarcodeReader reader = new BarcodeReader();

            ZXing.Common.DecodingOptions options = new ZXing.Common.DecodingOptions();
            options.TryHarder = TryHarder;
            reader.Options = options;
            
            var barcodeBitmap = pColorImage.Bitmap;
            if (barcodeBitmap != null)
            {
                Result result = reader.Decode(barcodeBitmap);

                if (result != null) {
                    handleResult(result);
                }
            }
        }

        private void handleResult(Result result)
        {
            CodeDetectedEventArgs args = new CodeDetectedEventArgs();
            args.Result = result;
            args.Text = result.Text;
            OnCodeDetected(this, args);
        }

        public void FullScan(Emgu.CV.Image<Emgu.CV.Structure.Bgra, byte> pColorImage)
        {
            if (OnCodeDetected == null) return;

            BarcodeReader reader = new BarcodeReader();

            ZXing.Common.DecodingOptions options = new ZXing.Common.DecodingOptions();
            options.TryHarder = TryHarder;
            reader.Options = options;

            var barcodeBitmap = pColorImage.Bitmap;
            // detect and decode the barcode inside the bitmap
            Result[] result = reader.DecodeMultiple(barcodeBitmap);
            
            // do something with the result
            foreach (Result r in result)
            {
                if (r != null)
                {
                    handleResult(r);
                }
            }
        }


    }
    public class CodeDetectedEventArgs
    {
        public string Text { get; set; }
        public Result Result { get; set; }

    }
}
