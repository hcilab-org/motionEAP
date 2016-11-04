// <copyright file=DebugInformationManager.cs
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

using System.Diagnostics;
using motionEAPAdmin.Scene;

namespace motionEAPAdmin.ContentProviders
{
    /// <summary>
    /// This class manages the debug information and provides them to others.
    /// </summary>
    /// 
    /// 
    class DebugInformationManager
    {
        private static DebugInformationManager m_Instance;

        

        private Stopwatch stopWatchFPS = new Stopwatch();

        double timeInMillis = -1;  // current time in millis
        public double fps = 0;     // current FPS

        private DebugInformationManager() {
            // display GUI FPS
            SceneText fpsInfo = new Scene.SceneText(0.43, 0.013, "DebugFPS:", System.Windows.Media.Color.FromRgb(255, 0, 0), 10, new System.Windows.Media.FontFamily("Arial"));
            SceneManager.Instance.temporarylyDisplaySceneItem(fpsInfo);

            // display Leap FPS
            SceneText leapFpsInfo = new Scene.SceneText(0.43, 0.013, "LeapFPS:", System.Windows.Media.Color.FromRgb(255, 0, 0), 10, new System.Windows.Media.FontFamily("Arial"));
            SceneManager.Instance.temporarylyDisplaySceneItem(leapFpsInfo);
        }


        // get the singleton instance
        public static DebugInformationManager Instance
        {
            get 
            {
                if (m_Instance == null)
                {
                    m_Instance = new DebugInformationManager();
                }
                return m_Instance;
            }
        }

        /// <summary>
        /// Triggers the next tick of the FPS.
        /// 
        /// THIS SHOULD BE CALLED FROM SOME GUI ELEMENT.
        /// Call this only once per tick.
        /// </summary>
        /// 
        /// 
        public void nextTick()
        {
            if (stopWatchFPS.IsRunning)
            {
                stopWatchFPS.Stop();
                long elapsed = stopWatchFPS.ElapsedMilliseconds;

                if (timeInMillis == -1)
                {
                    timeInMillis = elapsed;
                }

                timeInMillis = timeInMillis * 0.9 + elapsed * 0.1;

                fps = 1000 / timeInMillis;

                stopWatchFPS.Restart();                
            }

            stopWatchFPS.Start();

        }

        internal void start()
        {
        }
    }
}
