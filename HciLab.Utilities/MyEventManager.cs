// <copyright file=MyEventManager.cs
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

using System;


namespace HciLab.Utilities
{
    public class MyEventManager
    {
        private static MyEventManager eventManager;

        public static MyEventManager getInstance()
        {
            if (eventManager == null)
            {
                eventManager = new MyEventManager();
            }
            return eventManager;
        }

        public delegate void Message(object sender, String text);

        

        /*public delegate void updatePointHandler(object pSource, int pId, Point3D pPoint);

        public event updatePointHandler updatePoint;

        public void OnUpdatePoint(object pSource, int pId, Point3D pPoint)
        {
            if (this.updatePoint != null)
                updatePoint(pSource, pId, pPoint);
        }

        public delegate void newFrameHandler(object pSource, FrameOfMocapDataSerialized pFrame);

        public event newFrameHandler newFrame;

        public void OnFrameHandler(object pSource, FrameOfMocapDataSerialized pFrame)
        {
            if (this.newFrame != null)
                newFrame(pSource, pFrame);
        }



        public delegate void UpdateUIHandler(object pSource, FrameOfMocapDataSerialized pFrame);

        public event UpdateUIHandler updateUI;

        public void OnUpdateUIHandler(object pSource, FrameOfMocapDataSerialized pFrame)
        {
            if (this.updateUI != null)
                updateUI(pSource, pFrame);
        }


        public delegate void SelectedMarkerChangedHandler(object pSource, List<int> pSelectedMarkerChangedList);

        public event SelectedMarkerChangedHandler selectedMarkerChanged;

        public void OnSelectedMarkerChangedHandler(object pSource, List<int> pSelectedMarkerChangedList)
        {
            if (this.selectedMarkerChanged != null)
                selectedMarkerChanged(pSource, pSelectedMarkerChangedList);
        }

        public delegate void SelectedPatternChangedHandler(object pSource, List<int> pSelectedPatternChangedList);

        public event SelectedPatternChangedHandler selectedPatternChanged;

        public void OnSelectedPatternChangedHandler(object pSource, List<int> pSelectedPatternChangedList)
        {
            if (this.selectedPatternChanged != null)
                selectedPatternChanged(pSource, pSelectedPatternChangedList);
        }*/
    }
}
