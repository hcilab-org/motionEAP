// <copyright file=Settings.cs
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
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Linq.Expressions;

namespace HciLab.motionEAP.InterfacesAndDataModel.Data
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    [Serializable()]
    public class Settings : ISerializable, INotifyPropertyChanged
    {
        // Version
        private int m_SerVersion = 11;

        #region Settingsvariables
        // 2D touch
        private double m_TextFieldOffset;
        private double m_TextFieldHeight;
        private double m_TextFieldSmoothingSteps;
        private double m_TextFieldMovingThreshold;
        private double m_TextFieldStoppingThreshold;
        private bool m_CheckBoxInvertDisplay;
        // Video
        private bool m_CheckBoxFreeSpace;
        private bool m_CheckBoxObjectMapping;
        private bool m_CheckBoxTrackObject;
        private bool m_CheckBoxStartProjection;
        private int m_IntegerUpDownInnerBandThresholdInput;
        private int m_IntegerUpDownOuterBandThresholdInput;
        private int m_IntegerUpDownAverageFrameCountInput;
        // Settings
        private int m_IntegerUpDownMinDepth;
        private int m_IntegerUpDownMaxDepth;
        private int m_IntegerUpDownXBox;
        private int m_IntegerUpDownYBox;
        private bool m_CheckBoxSmoothing;
        // Gestures
        private bool m_CheckBoxGesturesEnabled;
        // Objects
        private bool m_CheckBoxObjectsDisplayVisualFeedback;
        private bool m_CheckBoxObjectsProjectVisualFeedback;
        private bool m_CheckBoxObjectsRecognizeObject;
        // Boxen
        private bool m_CheckBoxBoxesDisplayVisualFeedback;
        private bool m_CheckBoxBoxesProjectVisualFeedback;
        private int m_IntegerUpDownBoxes_InputTriggerPercentage;
        // Assembly zone
        private bool m_CheckBoxAssemblyZoneDisplayVisualFeedback;
        private bool m_CheckBoxAssemblyZoneProjectVisualFeedback;
        private int m_IntegerUpDownAssemblyZonezones_InputMatchPercentage;
        private int m_IntegerUpDownAssemblyZonezones_InputMatchTolerance;
        private int m_IntegerUpDownAssemblyZonezones_InputMinAreaAdding;
        private int m_IntegerUpDownAssemblyZonezones_InputMinValueChangeAdding;
        private int m_IntegerUpDownAssemblyZonezones_InputMinAreaRemoval;
        private int m_IntegerUpDownAssemblyZonezones_InputMinValueChangeRemoval;
        // Language
        private string m_Language;
        // Workflow
        private int m_adaptivityLevelId;
        #endregion

        //v2
        SettingsKinect m_SettingsKinect = new SettingsKinect();

        //v3
        SettingsTable m_SettingsTable = new SettingsTable();

        //v4
        Boolean m_BlobRadio = false;

        //v7 - Network
        private string m_NetworkTableName;
        private string m_NetworkAuthToken;
        private string m_ServerBaseAddress;

        //v8 - fault detection and box feedback
        private int m_TextFieldBoxFeedback;

        //v10
        private bool m_AdaptivityEnabled;

        // v11
        private bool m_UDPStreamingEnabled;
        private string m_UDPIPTarget;


        private double m_AdaptivityThresholdMedium;
        private double m_AdaptivityThresholdHard;

        public Settings()
        {
        }

        protected Settings(SerializationInfo info, StreamingContext context)
        {
            #region Get settings
            int SerVersion = info.GetInt32("m_SerVersion");
            // Check version of XML version
            if (SerVersion < 1)
            {
                return;
            }
            // 2D Touch
            m_TextFieldOffset = (double)info.GetDouble("m_TextFieldOffset");
            m_TextFieldHeight = (double)info.GetDouble("m_TextFieldHeight");
            m_TextFieldSmoothingSteps = (double)info.GetDouble("m_TextFieldSmoothingSteps");
            m_TextFieldMovingThreshold = (double)info.GetDouble("m_TextFieldMovingThreshold");
            m_TextFieldStoppingThreshold = (double)info.GetDouble("m_TextFieldStoppingThreshold");
            m_CheckBoxInvertDisplay = info.GetBoolean("m_CheckBoxInvertDisplay");
            // Video
            m_CheckBoxFreeSpace = info.GetBoolean("m_CheckBoxFreeSpace");
            m_CheckBoxObjectMapping = info.GetBoolean("m_CheckBoxObjectMapping");
            m_CheckBoxTrackObject = info.GetBoolean("m_CheckBoxTrackObject");
            m_CheckBoxStartProjection = info.GetBoolean("m_CheckBoxStartProjection");
            m_IntegerUpDownInnerBandThresholdInput = info.GetInt32("m_IntegerUpDownInnerBandThresholdInput");
            m_IntegerUpDownOuterBandThresholdInput = info.GetInt32("m_IntegerUpDownOuterBandThresholdInput");
            m_IntegerUpDownAverageFrameCountInput = info.GetInt32("m_IntegerUpDownAverageFrameCountInput");
            // Settings
            m_IntegerUpDownMinDepth = info.GetInt32("m_IntegerUpDownMinDepth");
            m_IntegerUpDownMaxDepth = info.GetInt32("m_IntegerUpDownMaxDepth");
            m_IntegerUpDownXBox = info.GetInt32("m_IntegerUpDownXBox");
            m_IntegerUpDownYBox = info.GetInt32("m_IntegerUpDownYBox");
            m_CheckBoxSmoothing = info.GetBoolean("m_CheckBoxSmoothing");
            // Objects
            m_CheckBoxObjectsDisplayVisualFeedback = info.GetBoolean("m_CheckBoxObjectsDisplayVisualFeedback");
            m_CheckBoxObjectsProjectVisualFeedback = info.GetBoolean("m_CheckBoxObjectsProjectVisualFeedback");
            m_CheckBoxObjectsRecognizeObject = info.GetBoolean("m_CheckBoxObjectsRecognizeObject");
            // Boxen
            m_CheckBoxBoxesDisplayVisualFeedback = info.GetBoolean("m_CheckBoxBoxenDisplayVisualFeedback");
            m_CheckBoxBoxesProjectVisualFeedback = info.GetBoolean("m_CheckBoxBoxenProjectVisualFeedback");
            // Assembly zone
            m_CheckBoxAssemblyZoneDisplayVisualFeedback = info.GetBoolean("m_CheckBoxMontagezoneDisplayVisualFeedback");
            m_CheckBoxAssemblyZoneProjectVisualFeedback = info.GetBoolean("m_CheckBoxMontagezoneProjectVisualFeedback");
            m_IntegerUpDownAssemblyZonezones_InputMatchPercentage = info.GetInt32("m_IntegerUpDownAssemblyZonezones_InputMatchPercentage");
            m_IntegerUpDownAssemblyZonezones_InputMatchTolerance = info.GetInt32("m_IntegerUpDownAssemblyZonezones_InputMatchTolerance");
            m_IntegerUpDownAssemblyZonezones_InputMinAreaAdding = info.GetInt32("m_IntegerUpDownAssemblyZonezones_InputMinAreaAdding");
            m_IntegerUpDownAssemblyZonezones_InputMinAreaRemoval = info.GetInt32("m_IntegerUpDownAssemblyZonezones_InputMinAreaRemoval");
            m_IntegerUpDownAssemblyZonezones_InputMinValueChangeAdding = info.GetInt32("m_IntegerUpDownAssemblyZonezones_InputMinValueChangeAdding");
            m_IntegerUpDownAssemblyZonezones_InputMinValueChangeRemoval = info.GetInt32("m_IntegerUpDownAssemblyZonezones_InputMinValueChangeRemoval");
            // Language
            m_Language = info.GetString("m_Language");
            #endregion

            if (SerVersion < 2)
                return;

            m_SettingsKinect = (SettingsKinect)info.GetValue("m_SettingsKinect", typeof(SettingsKinect));
            m_SettingsKinect.PropertyChanged += m_SettingsKinect_PropertyChanged;


            if (SerVersion < 3)
                return;

            m_SettingsTable = (SettingsTable)info.GetValue("m_SettingsTable", typeof(SettingsTable));
            m_SettingsTable.PropertyChanged += m_SettingsTable_PropertyChanged;

            if (SerVersion < 4)
                return;

            m_BlobRadio = info.GetBoolean("m_BlobRadio");

            if (SerVersion < 5)
                return;

            m_IntegerUpDownBoxes_InputTriggerPercentage = info.GetInt32("m_IntegerUpDownBoxes_InputTriggerPercentage");

            if (SerVersion < 6)
                return;

            m_adaptivityLevelId = info.GetInt32("m_adaptivityLevelId");

            if (SerVersion < 7)
                return;

            m_NetworkTableName = info.GetString("m_NetworkTableName");
            m_NetworkAuthToken = info.GetString("m_NetworkAuthToken");
            m_ServerBaseAddress = info.GetString("m_ServerBaseAddress");

            if (SerVersion < 8)
                return;


            if (SerVersion < 9)
                return;

            m_TextFieldBoxFeedback = info.GetInt32("m_TextFieldBoxFeedback");

            if (SerVersion < 10)
                return;

            m_AdaptivityEnabled = info.GetBoolean("m_AdaptivityEnabled");
            m_AdaptivityThresholdMedium = info.GetDouble("m_AdaptivityThresholdMedium");
            m_AdaptivityThresholdHard = info.GetDouble("m_AdaptivityThresholdHard");

            if (SerVersion < 11)
                return;

            m_UDPStreamingEnabled = info.GetBoolean("m_UDPStreamingEnabled");
            m_UDPIPTarget = info.GetString("m_UDPIPTarget");
        }

        void m_SettingsTable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged("SettingsTable");
        }

        void m_SettingsKinect_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged("SettingsKinect");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            #region Save values for settings
            info.AddValue("m_SerVersion", m_SerVersion);
            // 2D Touch
            info.AddValue("m_TextFieldOffset", m_TextFieldOffset);
            info.AddValue("m_TextFieldHeight", m_TextFieldHeight);
            info.AddValue("m_TextFieldSmoothingSteps", m_TextFieldSmoothingSteps);
            info.AddValue("m_TextFieldMovingThreshold", m_TextFieldMovingThreshold);
            info.AddValue("m_TextFieldStoppingThreshold", m_TextFieldStoppingThreshold);
            info.AddValue("m_CheckBoxInvertDisplay", m_CheckBoxInvertDisplay);
            // Video
            info.AddValue("m_CheckBoxFreeSpace", m_CheckBoxFreeSpace);
            info.AddValue("m_CheckBoxObjectMapping", m_CheckBoxObjectMapping);
            info.AddValue("m_CheckBoxTrackObject", m_CheckBoxTrackObject);
            info.AddValue("m_CheckBoxStartProjection", m_CheckBoxStartProjection);
            info.AddValue("m_IntegerUpDownInnerBandThresholdInput", m_IntegerUpDownInnerBandThresholdInput);
            info.AddValue("m_IntegerUpDownOuterBandThresholdInput", m_IntegerUpDownOuterBandThresholdInput);
            info.AddValue("m_IntegerUpDownAverageFrameCountInput", m_IntegerUpDownAverageFrameCountInput);
            // Settings
            info.AddValue("m_IntegerUpDownMinDepth", m_IntegerUpDownMinDepth);
            info.AddValue("m_IntegerUpDownMaxDepth", m_IntegerUpDownMaxDepth);
            info.AddValue("m_IntegerUpDownXBox", m_IntegerUpDownXBox);
            info.AddValue("m_IntegerUpDownYBox", m_IntegerUpDownYBox);
            info.AddValue("m_CheckBoxSmoothing", m_CheckBoxSmoothing);
            // Objects
            info.AddValue("m_CheckBoxObjectsDisplayVisualFeedback", m_CheckBoxObjectsDisplayVisualFeedback);
            info.AddValue("m_CheckBoxObjectsProjectVisualFeedback", m_CheckBoxObjectsProjectVisualFeedback);
            info.AddValue("m_CheckBoxObjectsRecognizeObject", m_CheckBoxObjectsRecognizeObject);
            // Boxen
            info.AddValue("m_CheckBoxBoxenDisplayVisualFeedback", m_CheckBoxBoxesDisplayVisualFeedback);
            info.AddValue("m_CheckBoxBoxenProjectVisualFeedback", m_CheckBoxBoxesProjectVisualFeedback);
            info.AddValue("m_IntegerUpDownBoxes_InputTriggerPercentage", m_IntegerUpDownBoxes_InputTriggerPercentage);
            // Assembly zone
            info.AddValue("m_CheckBoxMontagezoneDisplayVisualFeedback", m_CheckBoxAssemblyZoneDisplayVisualFeedback);
            info.AddValue("m_CheckBoxMontagezoneProjectVisualFeedback", m_CheckBoxAssemblyZoneProjectVisualFeedback);
            info.AddValue("m_IntegerUpDownAssemblyZonezones_InputMatchPercentage", m_IntegerUpDownAssemblyZonezones_InputMatchPercentage);
            info.AddValue("m_IntegerUpDownAssemblyZonezones_InputMatchTolerance", m_IntegerUpDownAssemblyZonezones_InputMatchTolerance);
            info.AddValue("m_IntegerUpDownAssemblyZonezones_InputMinAreaAdding", m_IntegerUpDownAssemblyZonezones_InputMinAreaAdding);
            info.AddValue("m_IntegerUpDownAssemblyZonezones_InputMinAreaRemoval", m_IntegerUpDownAssemblyZonezones_InputMinAreaRemoval);
            info.AddValue("m_IntegerUpDownAssemblyZonezones_InputMinValueChangeAdding", m_IntegerUpDownAssemblyZonezones_InputMinValueChangeAdding);
            info.AddValue("m_IntegerUpDownAssemblyZonezones_InputMinValueChangeRemoval", m_IntegerUpDownAssemblyZonezones_InputMinValueChangeRemoval);
            // Language
            info.AddValue("m_Language", m_Language);
            // Workflow
            info.AddValue("m_adaptivityLevelId", m_adaptivityLevelId);
            // Network
            info.AddValue("m_NetworkTableName", m_NetworkTableName);
            info.AddValue("m_NetworkAuthToken", m_NetworkAuthToken);
            info.AddValue("m_ServerBaseAddress", m_ServerBaseAddress);
            // Adaptivity
            info.AddValue("m_AdaptivityEnabled", m_AdaptivityEnabled);
            info.AddValue("m_AdaptivityThresholdHard", m_AdaptivityThresholdHard);
            info.AddValue("m_AdaptivityThresholdMedium", m_AdaptivityThresholdMedium);
            #endregion

            info.AddValue("m_SettingsKinect", m_SettingsKinect);
            info.AddValue("m_SettingsTable", m_SettingsTable);

            info.AddValue("m_BlobRadio", m_BlobRadio);

            info.AddValue("m_TextFieldBoxFeedback", m_TextFieldBoxFeedback);

            info.AddValue("m_UDPStreamingEnabled", m_UDPStreamingEnabled);
            info.AddValue("m_UDPIPTarget", m_UDPIPTarget);

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        static string GetVariableName<T>(Expression<Func<T>> expr)
        {
            var body = (MemberExpression)expr.Body;

            return body.Member.Name;
        }

        #region Getter and setter
        // Getter and Setter for each settings variable

        public SettingsKinect SettingsKinect
        {
            get
            {
                return m_SettingsKinect;
            }
            set
            {
                m_SettingsKinect = value;
                NotifyPropertyChanged("KinectSettings");
            }
        }

        public int SerVersion
        {
            get { return m_SerVersion; }
            set { m_SerVersion = value; }
        }

        public Boolean BlobRadio
        {
            get { return m_BlobRadio; }
            set
            {
                m_BlobRadio = value;
                NotifyPropertyChanged("BlobRadio");
            }
        }


        public double TouchOffset
        {
            get { return m_TextFieldOffset; }
            set
            {
                m_TextFieldOffset = value;
                NotifyPropertyChanged("TextFieldOffset");
            }
        }

        public double TouchFieldHeight
        {
            get { return m_TextFieldHeight; }
            set
            {
                m_TextFieldHeight = value;
                NotifyPropertyChanged("TextFieldHeight");
            }
        }

        public double TouchSmoothingSteps
        {
            get { return m_TextFieldSmoothingSteps; }
            set
            {
                m_TextFieldSmoothingSteps = value;
                NotifyPropertyChanged("TextFieldSmoothingSteps");
            }
        }

        public double TouchMovingThreshold
        {
            get { return m_TextFieldMovingThreshold; }
            set
            {
                m_TextFieldMovingThreshold = value;
                NotifyPropertyChanged("TextFieldMovingThreshold");
            }
        }

        public double TouchStoppingThreshold
        {
            get { return m_TextFieldStoppingThreshold; }
            set
            {
                m_TextFieldStoppingThreshold = value;
                NotifyPropertyChanged("TextFieldStoppingThreshold");
            }
        }

        public bool TouchInvertDisplay
        {
            get { return m_CheckBoxInvertDisplay; }
            set
            {
                m_CheckBoxInvertDisplay = value;
                NotifyPropertyChanged("CheckBoxInvertDisplay");
            }
        }

        public bool ShowFreeSpace
        {
            get { return m_CheckBoxFreeSpace; }
            set
            {
                m_CheckBoxFreeSpace = value;
                NotifyPropertyChanged("CheckBoxFreeSpace");
            }
        }

        public bool ShowObjectMapping
        {
            get { return m_CheckBoxObjectMapping; }
            set
            {
                m_CheckBoxObjectMapping = value;
                NotifyPropertyChanged("CheckBoxObjectMapping");
            }
        }

        public bool TrackObject
        {
            get { return m_CheckBoxTrackObject; }
            set
            {
                m_CheckBoxTrackObject = value;
                NotifyPropertyChanged("CheckBoxTrackObject");
            }
        }

        public bool CheckBoxStartProjection
        {
            get { return m_CheckBoxStartProjection; }
            set
            {
                m_CheckBoxStartProjection = value;
                NotifyPropertyChanged("CheckBoxStartProjection");
            }
        }

        public int IntegerUpDownInnerBandThresholdInput
        {
            get { return m_IntegerUpDownInnerBandThresholdInput; }
            set
            {
                m_IntegerUpDownInnerBandThresholdInput = value;
                NotifyPropertyChanged("IntegerUpDownInnerBandThresholdInput");
            }
        }

        public int IntegerUpDownOuterBandThresholdInput
        {
            get { return m_IntegerUpDownOuterBandThresholdInput; }
            set
            {
                m_IntegerUpDownOuterBandThresholdInput = value;
                NotifyPropertyChanged("IntegerUpDownOuterBandThresholdInput");
            }
        }

        public int IntegerUpDownAverageFrameCountInput
        {
            get { return m_IntegerUpDownAverageFrameCountInput; }
            set
            {
                m_IntegerUpDownAverageFrameCountInput = value;
                NotifyPropertyChanged("IntegerUpDownAverageFrameCountInput");
            }
        }

        public int IntegerUpDownMinDepth
        {
            get { return m_IntegerUpDownMinDepth; }

            set
            {
                m_IntegerUpDownMinDepth = value;
                NotifyPropertyChanged("IntegerUpDownMinDepth");
            }
        }

        public int IntegerUpDownMaxDepth
        {
            get { return m_IntegerUpDownMaxDepth; }
            set
            {
                m_IntegerUpDownMaxDepth = value;
                NotifyPropertyChanged("IntegerUpDownMaxDepth");
            }
        }

        public int IntegerUpDownXBox
        {
            get { return m_IntegerUpDownXBox; }
            set
            {
                m_IntegerUpDownXBox = value;
                NotifyPropertyChanged("IntegerUpDownXBox");
            }
        }

        public int IntegerUpDownYBox
        {
            get { return m_IntegerUpDownYBox; }
            set
            {
                m_IntegerUpDownYBox = value;
                NotifyPropertyChanged("IntegerUpDownYBox");
            }
        }

        public bool CheckBoxSmoothing
        {
            get { return m_CheckBoxSmoothing; }
            set
            {
                m_CheckBoxSmoothing = value;
                NotifyPropertyChanged("CheckBoxSmoothing");
            }
        }

        public bool ObjectsVisualFeedbackDisplay
        {
            get { return m_CheckBoxObjectsDisplayVisualFeedback; }
            set
            {
                m_CheckBoxObjectsDisplayVisualFeedback = value;
                NotifyPropertyChanged("CheckBoxObjectsDisplayVisualFeedback");
            }
        }

        public bool ObjectsVisualFeedbackProject
        {
            get { return m_CheckBoxObjectsProjectVisualFeedback; }
            set
            {
                m_CheckBoxObjectsProjectVisualFeedback = value;
                NotifyPropertyChanged("CheckBoxObjectsProjectVisualFeedback");
            }
        }

        public bool ObjectsRecognizeObject
        {
            get { return m_CheckBoxObjectsRecognizeObject; }
            set
            {
                m_CheckBoxObjectsRecognizeObject = value;
                NotifyPropertyChanged("CheckBoxObjectsRecognizeObject");
            }
        }

        public bool BoxesVisualFeedbackDisplay
        {
            get { return m_CheckBoxBoxesDisplayVisualFeedback; }
            set
            {
                m_CheckBoxBoxesDisplayVisualFeedback = value;
                NotifyPropertyChanged("CheckBoxBoxenDisplayVisualFeedback");
            }
        }

        public bool BoxesVisualFeedbackProject
        {
            get { return m_CheckBoxBoxesProjectVisualFeedback; }
            set
            {
                m_CheckBoxBoxesProjectVisualFeedback = value;
                NotifyPropertyChanged("CheckBoxBoxenProjectVisualFeedback");
            }
        }

        public int BoxesInputTriggerPercentage
        {
            get { return m_IntegerUpDownBoxes_InputTriggerPercentage; }
            set
            {
                m_IntegerUpDownBoxes_InputTriggerPercentage = value;
                NotifyPropertyChanged("BoxesInputTriggerPercentage");
            }
        }

        public bool AssemblyZoneVisualFeedbackDisplay
        {
            get { return m_CheckBoxAssemblyZoneDisplayVisualFeedback; }
            set
            {
                m_CheckBoxAssemblyZoneDisplayVisualFeedback = value;
                NotifyPropertyChanged("CheckBoxMontagezoneDisplayVisualFeedback");
            }
        }

        public bool AssemblyZoneVisualFeedbackProject
        {
            get { return m_CheckBoxAssemblyZoneProjectVisualFeedback; }
            set
            {
                m_CheckBoxAssemblyZoneProjectVisualFeedback = value;
                NotifyPropertyChanged("CheckBoxMontagezoneProjectVisualFeedback");
            }
        }

        public int AssemblyZonesInputMatchPercentage
        {
            get { return m_IntegerUpDownAssemblyZonezones_InputMatchPercentage; }
            set
            {
                m_IntegerUpDownAssemblyZonezones_InputMatchPercentage = value;
                NotifyPropertyChanged("IntegerUpDownAssemblyZonezones_InputMatchPercentage");
            }
        }

        public int AssemblyZonesInputMatchTolerance
        {
            get { return m_IntegerUpDownAssemblyZonezones_InputMatchTolerance; }
            set
            {
                m_IntegerUpDownAssemblyZonezones_InputMatchTolerance = value;
                NotifyPropertyChanged("IntegerUpDownAssemblyZonezones_InputMatchTolerance");
            }
        }

        public int AssemblyZonesInputMinAreaAdding
        {
            get { return m_IntegerUpDownAssemblyZonezones_InputMinAreaAdding; }
            set
            {
                m_IntegerUpDownAssemblyZonezones_InputMinAreaAdding = value;
                NotifyPropertyChanged("IntegerUpDownAssemblyZonezones_InputMinAreaAdding");
            }
        }

        public int AssemblyZonesInputMinValueChangeAdding
        {
            get { return m_IntegerUpDownAssemblyZonezones_InputMinValueChangeAdding; }
            set
            {
                m_IntegerUpDownAssemblyZonezones_InputMinValueChangeAdding = value;
                NotifyPropertyChanged("IntegerUpDownAssemblyZonezones_InputMinValueChangeAdding");
            }
        }

        public int AssemblyZonesInputMinAreaRemoval
        {
            get { return m_IntegerUpDownAssemblyZonezones_InputMinAreaRemoval; }
            set
            {
                m_IntegerUpDownAssemblyZonezones_InputMinAreaRemoval = value;
                NotifyPropertyChanged("IntegerUpDownAssemblyZonezones_InputMinAreaRemoval");
            }
        }

        public int AssemblyZonesInputMinValueChangeRemoval
        {
            get { return m_IntegerUpDownAssemblyZonezones_InputMinValueChangeRemoval; }
            set
            {
                m_IntegerUpDownAssemblyZonezones_InputMinValueChangeRemoval = value;
                NotifyPropertyChanged("IntegerUpDownAssemblyZonezones_InputMinValueChangeRemoval");
            }
        }

        public string Language
        {
            get { return m_Language; }
            set
            {
                m_Language = value;
                NotifyPropertyChanged("Language");
            }
        }

        public int AdaptivityLevelId
        {
            get { return m_adaptivityLevelId; }
            set
            {
                m_adaptivityLevelId = value;
                NotifyPropertyChanged("AdaptivityLevelId");
            }
        }

        public bool CheckBoxGesturesEnabled
        {
            get { return m_CheckBoxGesturesEnabled; }
            set
            {
                m_CheckBoxGesturesEnabled = value;
                NotifyPropertyChanged("CheckBoxGesturesEnabled");
            }
        }

        public string NetworkTableName
        {
            get { return m_NetworkTableName; }
            set
            {
                m_NetworkTableName = value;
                NotifyPropertyChanged("NetworkTableName");
            }
        }

        public string NetworkAuthToken
        {
            get { return m_NetworkAuthToken; }
            set
            {
                m_NetworkAuthToken = value;
                NotifyPropertyChanged("NetworkAuthToken");
            }
        }

        public string ServerBaseAddress
        {
            get { return m_ServerBaseAddress; }
            set
            {
                m_ServerBaseAddress = value;
                NotifyPropertyChanged("ServerBaseAddress");
            }
        }

        public int BoxFeedbackTimeout
        {
            get { return m_TextFieldBoxFeedback; }
            set
            {
                m_TextFieldBoxFeedback = value;
                NotifyPropertyChanged("BoxFeedbackTimeout");
            }
        }

        public bool AdaptivityEnabled
        {
            get { return m_AdaptivityEnabled; }
            set { m_AdaptivityEnabled = value; NotifyPropertyChanged("AdaptivityEnabled"); }
        }

        public double AdaptivityThresholdHard
        {
            get { return m_AdaptivityThresholdHard; }
            set
            {
                m_AdaptivityThresholdHard = value;
                NotifyPropertyChanged("AdaptivityThresholdHard");
            }
        }


        public double AdaptivityThresholdMedium
        {
            get { return m_AdaptivityThresholdMedium; }
            set
            {
                m_AdaptivityThresholdMedium = value;
                NotifyPropertyChanged("AdaptivityThresholdMedium");
            }
        }

        public bool UDPStreamingEnabled
        {
            get { return m_UDPStreamingEnabled; }
            set
            {
                m_UDPStreamingEnabled = value;
                NotifyPropertyChanged("UDPStreamingEnabled");
            }
        }

        public string UDPIPTarget
        {
            get { return m_UDPIPTarget; }
            set
            {
                m_UDPIPTarget = value;
                NotifyPropertyChanged("UDPIPTarget");
            }
        }


        public SettingsTable SettingsTable
        {
            get { return m_SettingsTable; }
            set
            {
                m_SettingsTable = value;
                NotifyPropertyChanged("SettingsTable");
            }
        }

        #endregion
    }
}
