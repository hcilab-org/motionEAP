// <copyright file=ProjectConstants.cs
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
    public static class ProjectConstants
    {
        public static readonly string WORKFLOW_FILE_ENDING = ".work";
        public static readonly string SCENE_FILE_ENDING = ".scene";
        public static readonly string BOX_FILE_ENDING = ".box";
        public static readonly string ASSEMBLYZONE_FILE_ENDING = ".zone";
        public static readonly string OBJECTDETECTIONZONES_FILE_ENDING = ".ozone";
        public static readonly string WORKFLOW_DIR = "workflows";
        public static readonly string SCENES_DIR = "scenes";
        public static readonly string BOXES_DIR = "boxes";
        public static readonly string OBJECTDETECTIONZONES_DIR = "ozones";
        public static readonly string ASSEMBLYZONES_DIR = "assemblyzones";
        public static readonly string STUDY_DIR = "Study";
        
        public static readonly string OBJECT_DIR = "objects";

        // Destination of settings.xml
        public static readonly string SETTINGS_FILE = "settings.json";

        // vendor Id of supported cameras
        public static readonly string ENSENSON10VENDOR = "0x1409";
        public static readonly string KINECTV1VENDOR = "0x045E";
        public static readonly string KINECTV2VENDOR = "0x045E";
        public static readonly string STRUCTURESENSORVENDOR = "0x1D27";

        // product Id of supported cameras
        public static readonly string ENSENSON10PRODUCT = "0x1225";
        public static readonly string KINECTV1PRODUCT = "0x02BF";
        public static readonly string KINECTV2PRODUCT = "0x02C4";
        public static readonly string KINECTV2FWPRODUCT = "0x02D8"; //For Windows
        public static readonly string STRUCTURESENSORPRODUCT = "0x0600";

        // description of supported cameras
        public static readonly string KINECTV1DESCRIPTION = "Microsoft Kinect v1";
        public static readonly string KINECTV2DESCRIPTION = "Microsoft Kinect v2";
        public static readonly string ENSENSON10DESCRIPTION = "Ensenso N10";
        public static readonly string STRUCTURESENSORDESCRIPTION = "Structure Sensor";

    }
}
