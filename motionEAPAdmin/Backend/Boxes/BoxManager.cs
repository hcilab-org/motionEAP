// <copyright file=BoxManager.cs
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
using HciLab.Kinect;
using HciLab.Utilities;
using motionEAPAdmin.ContentProviders;
using motionEAPAdmin.Scene;
using System.ComponentModel;
using System.Drawing;

namespace motionEAPAdmin.Backend.Boxes
{
    public class BoxManager : INotifyPropertyChanged
    {
        /// <summary>
        /// property changed e.g. for the databinding
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// the singleton instance
        /// </summary>
        private static BoxManager m_Instance;

        /// <summary>
        /// 
        /// </summary>
        private int m_IdCounter = 0;

        /// <summary>
        /// All Boxes are containt here
        /// </summary>
        private BoxLayout m_CurrentLayout = new BoxLayout();

        /// <summary>
        /// Heap Scene witch contains all AssemblyZone Scenes
        /// </summary>
        private Scene.Scene m_SceneOfAllBoxes = new Scene.Scene();

        /// <summary>
        /// Font that is used for UI drawinigns
        /// </summary>
        private static MCvFont UI_FONT = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_SIMPLEX, 0.5, 0.5);

        /// <summary>
        /// Singleton Constructor
        /// </summary>
        public static BoxManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new BoxManager();
                }
                return m_Instance;
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        private BoxManager()
        {
            KinectManager.Instance.allFramesReady += refreshTrigger;
        }

        private void NotifyPropertyChanged(string Obj)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(Obj));
            }
        }

        public void UpdateCurrentBox(Box updatedBox)
        {
            lock (this)
            {

                m_CurrentLayout.Boxes.ReplaceOrAdd(updatedBox);
            }
        }

        public Box createBoxFromFactory()
        {
            Box ret = new Box(m_IdCounter);
            ret.Name = m_IdCounter.ToString();
            ret.TriggerMessage = m_IdCounter + "";
            m_IdCounter++;
            ret.trigger += WorkflowManager.Instance.OnTriggered;
            return ret;
        }

        public void decreaseIDByOne()
        {
            if (m_IdCounter > 0)
            {
                m_IdCounter = m_IdCounter - 1;
            }
        }


        public void loadBoxLayoutFromFile()
        {
            SetNewLayout(BoxLayout.loadBoxLayoutFromFile());
        }


        internal void SetNewLayout(BoxLayout pBoxLayout)
        {
            if (pBoxLayout != null)
            {
                int highestID = 0;

                // workaround for databinding bug
                m_CurrentLayout.Boxes.Clear();
                foreach (Box b in pBoxLayout.Boxes)
                {
                    m_CurrentLayout.Boxes.Add(b);

                    if (b.Id > highestID)
                    {
                        highestID = b.Id;
                    }
                }
                m_CurrentLayout.LayoutName = pBoxLayout.LayoutName;

                m_IdCounter = highestID + 1;

            }
        }

        internal void saveBoxLayoutToFile()
        {
            BoxLayout.saveBoxLayoutToFile(m_CurrentLayout);
        }

        private void refreshTrigger(object pSource, Image<Bgra, byte> pColorImage, Image<Bgra, byte> pColorImageCropped, Image<Gray, int> pDepthImage, Image<Gray, int> pDepthImageCropped)
        {
            if (BoxManager.Instance.m_CurrentLayout == null)
                return;

            lock (this)
            {
                foreach (Box b in m_CurrentLayout.Boxes)
                {
                    double percentage = getPercentageWithinMeanBoundries(calculateCurrentMeanDepth(b), b, pDepthImage);

                    if (percentage > ((double)(SettingsManager.Instance.Settings.BoxesInputTriggerPercentage) + b.MatchPercentageOffset) / 100.0)
                    {
                        // box was hit by the user --> go and trigger the action
                        b.Trigger();
                    }
                }
            }
        }

        private double getPercentageWithinMeanBoundries(double depthmean, Box b, Image<Gray, int> pDepthPixel)
        {

            //int LowerThreshold = 60;
            //int UpperThreshold = 190; // was 160  (was 240 in production)

            double sum = 0;
            double within = 0;
            for (int x = b.X; x < b.Width + b.X; x++)
            {
                for (int y = b.Y; y < b.Height + b.Y; y++)
                {
                    sum = sum + 1;
                    int depthval = pDepthPixel.Data[y, x, 0];
                    int offsetZ = 0; // (int)((double)(b.Z / KinectManager.SCALE_FACTOR) - depthmean);
                    int real_depthval = depthval / KinectManager.SCALE_FACTOR;
                    if ((real_depthval > (offsetZ + b.Depthmean - b.UpperThreshold)) && (real_depthval < (offsetZ + b.Depthmean - b.LowerThreshold)))
                    {
                        within = within + 1;
                    }
                }
            }
            if (sum != 0)
            {
                return within / sum;
            }
            return 0.0;
        }

        private double calculateCurrentMeanDepth(Box b)
        {
            Image<Gray, int> depthPixel = KinectManager.Instance.GetCurrentDepthImage();
            long sum = 0;
            int count = 0;
            for (int x = b.X; x < b.Width + b.X; x++)
            {
                for (int y = b.Y; y < b.Height + b.Y; y++)
                {
                    int depthval = depthPixel.Data[y, x, 0];
                    int real_depthval = depthval / KinectManager.SCALE_FACTOR;
                    if (real_depthval != 0)
                    {
                        sum = sum + real_depthval;
                        count++;
                    }
                }
            }

            if (count == 0)
                return 0;
            else
                return (double)sum / (double)count;
        }

        public Scene.Scene drawErrorFeedback()
        {
            m_SceneOfAllBoxes.Clear();
            foreach (Box b in m_CurrentLayout.Boxes)
            {
                if (b.IsBoxErroneous)
                {
                    SceneItem s;
                    s = b.drawErrorFeedback(false);
                    if (s is Scene.Scene)
                    {
                        foreach (SceneItem i in ((Scene.Scene)s).Items)
                        {
                            m_SceneOfAllBoxes.Add(i);
                        }
                    }
                    else
                    {
                        m_SceneOfAllBoxes.Add(s);
                    }
                }
            }
            return m_SceneOfAllBoxes;
        }

        public Scene.Scene drawProjectorUI()
        {
            m_SceneOfAllBoxes.Clear();
            foreach (Box b in m_CurrentLayout.Boxes)
            {
                SceneItem s;
                s = b.getDrawable(false);
                if (s is Scene.Scene)
                {
                    foreach (SceneItem i in ((Scene.Scene)s).Items)
                    {
                        m_SceneOfAllBoxes.Add(i);
                    }
                }
                else
                {
                    m_SceneOfAllBoxes.Add(s);
                }

            }
            return m_SceneOfAllBoxes;
        }

        public Image<Bgra, byte> drawAdminUI(Image<Bgra, byte> pImage)
        {
            if (m_CurrentLayout == null)
                return null;

            foreach (Box b in BoxManager.Instance.m_CurrentLayout.Boxes)
            {
                // draw ID
                pImage.Draw(b.Id + "", ref UI_FONT, new System.Drawing.Point(b.X, b.Y), new Bgra(0, 0, 0, 0));

                // draw Frame
                if (b.wasRecentlyTriggered())
                    pImage.Draw(new Rectangle(b.X, b.Y, b.Width, b.Height), new Bgra(0, 255, 0, 0), 0);
                else
                    pImage.Draw(new Rectangle(b.X, b.Y, b.Width, b.Height), new Bgra(0, 0, 255, 0), 0);
            }

            return pImage;
        }

        #region getter / Setter
        public BoxLayout CurrentLayout
        {
            get { return m_CurrentLayout; }
            /*set
            {
                m_CurrentLayout = value;
                NotifyPropertyChanged("CurrentLayout");
            }*/
        }
        #endregion

    }
}
