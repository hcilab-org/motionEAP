// <copyright file=AllEnums.cs
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

namespace HciLab.motionEAP.InterfacesAndDataModel
{
    public static class AllEnums
    {

        public enum State
        {
            IDLE = 0,
            WORKFLOW_LOADED = 1,
            WORKFLOW_PLAYING = 2,
            EDIT = 3,
            RECORD = 4,
            RECORD_PAUSED = 5
        }


        public enum Direction
        {
            NONE = -1,
            NORTH = 0,
            EAST = 1,
            SOUTH = 2,
            WEST = 3,
        }

        public enum PBD_Mode
        {
            BOX_WITHDRAWEL = 0,
            ASSEMBLY_DONE = 1,
            OBJECT_RECOGNIZED = 2,
            ACTIVITY_RECOGNIZED = 3,
            END_CONDITION = 4,
            NETWORK_TABLE_DONE = 5
        }

        public enum Corners
        {
            LowerLeft = 0,
            LowerRights = 1,
            UpperLeft = 2,
            UpperRight = 3,
        }

        public enum WorkingStepEndConditionTrigger
        {
            BOX = 0,
            ASSEMBLY_ZONE = 1,
            WORKFLOWPANEL_BUTTON = 2,
            FOOTPEDAL = 3,
            TIMEOUT = 4,
            NETWORK_TABLE = 5,
            TRACKABLE_OBJECT = 6,
            BOXBUTTON = 7,
            HACK = 666
        }

    }
}
