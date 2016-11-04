// <copyright file=SceneItem.cs
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
// <date> 11/2/2016 12:26:00 PM</date>

using HciLab.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace motionEAPAdmin.Scene
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    [Serializable()]
    public abstract class SceneItem : UIElement3D, ISerializable, INotifyPropertyChanged
    {
        [Browsable(false)]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Scene Id Count
        /// </summary>
        private static int SCENE_COUNT = 0;

        private int m_SerVersionSceneItem = 4;

        protected int m_Id = -1;

        private double m_X = 0;

        private double m_Y = 0;

        private double m_Z = 0;

        private double m_Width = 1;

        private double m_Height = 1;

        private double m_Scale;

        private bool m_Touchy = true;
        
        /// <summary>
        /// Rotation
        /// </summary>
        private double m_Rotation;

        /// <summary>
        /// rotation center X
        /// </summary>
        private double m_Rx;

        /// <summary>
        /// rotation center Y
        /// </summary>
        private double m_Ry;

        /// <summary>
        /// Object is Flashing with m_FlschTimeOn and m_FlschTimeOff
        /// </summary>
        private Boolean m_Flash = false;

        /// <summary>
        /// in milliseconds
        /// </summary>
        private int m_FlashTimeOn = 1000;

        /// <summary>
        /// in milliseconds
        /// </summary>
        private int m_FlashTimeOff = 1000;

        

        #region Animation Stuff

            /// <summary>
            /// 
            /// </summary>
            private Boolean m_IsAnimated = false;

            /// <summary>
            /// 
            /// </summary>
            private System.Timers.Timer m_AnimationTimer = new System.Timers.Timer();

            /// <summary>
            /// 
            /// </summary>
            private int m_StartX = 0;

            /// <summary>
            /// 
            /// </summary>
            private int m_StartY = 0;

            /// <summary>
            /// 
            /// </summary>
            private int m_TargetX = 0;

            /// <summary>
            /// 
            /// </summary>
            private int m_TargetY = 0;

            /// <summary>
            /// 
            /// </summary>
            private double m_SpeedX = 0;

            /// <summary>
            /// 
            /// </summary>
            private double m_SpeedY = 0;

            /// <summary>
            /// 
            /// </summary>
            private int m_AnimationTriggered = 0;

            /// <summary>
            /// 
            /// </summary>
            private readonly int m_UpdateMillis = 10;

            /// <summary>
            /// 
            /// </summary>
            private readonly double m_Duration = 5000;    

        #endregion


        #region No Serializableization
        
        /// <summary>
        /// Flash Timer
        /// </summary>
        private System.Timers.Timer m_Timer = new System.Timers.Timer();

        private bool m_FlashStatOn = true;

        private bool m_IsInEditMode = false;

        /// <summary>
        /// 
        /// </summary>
        protected Boolean m_isShown = false;

        #endregion

        public SceneItem()
        {
            m_Id = SCENE_COUNT;
            SCENE_COUNT++;
            init();
        }

        
        public SceneItem(double pX, double pY, double pWidth, double pHeight,
            double pRotation = 0, double pRotationCenterX = 0, double pRotationCenterY = 0, double pScale = 1.0)
        {
            m_Id = SCENE_COUNT;
            SCENE_COUNT++;

            this.m_X = pX;
            this.m_Y = pY;
            this.m_Width = pWidth;
            this.m_Height = pHeight;
            this.m_Rotation = pRotation;
            this.m_Rx = pRotationCenterX;
            this.m_Ry = pRotationCenterY;
            this.m_Scale = pScale;

            if (pRotationCenterX == 0 && pRotationCenterY == 0)
            {
                m_Rx = pWidth / 2;
                m_Ry = pHeight / 2;
            }

            #region Animation Stuff
            this.m_StartX = (int)pX;
            this.m_StartY = (int)pY;
            if (this.m_Duration / this.m_UpdateMillis != 0)
            { 
                this.m_SpeedX = (this.m_TargetX - pX) / (this.m_Duration / this.m_UpdateMillis);
                this.m_SpeedY = (this.m_TargetY - pY) / (this.m_Duration / this.m_UpdateMillis);
            }
            else
            {
                m_IsAnimated = false;
            }
            this.m_AnimationTimer.Interval = this.m_UpdateMillis;
            this.m_AnimationTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEventAnimation);
            if (m_IsAnimated)
                this.m_AnimationTimer.Start();
            #endregion

            init();
        }

        protected SceneItem(SerializationInfo pInfo, StreamingContext pContext)
        {
            

            int pSerVersion = pInfo.GetInt32("m_SerVersionSceneItem");

            if (pSerVersion > 0)
            {
                m_Id = pInfo.GetInt32("m_Id");

                m_X = pInfo.GetDouble("m_X");
                m_Y = pInfo.GetDouble("m_Y");
                m_Height = pInfo.GetDouble("m_Height");
                m_Width = pInfo.GetDouble("m_Width");
                m_Rotation = pInfo.GetDouble("m_Rotation");
                m_Rx = pInfo.GetDouble("m_Rx");
                m_Ry = pInfo.GetDouble("m_Ry");
                m_Scale = pInfo.GetDouble("m_Scale");
                m_Touchy = pInfo.GetBoolean("m_Touchy");
            }
            if (pSerVersion > 1)
            {
                m_Flash = pInfo.GetBoolean("m_Flash");
                m_FlashTimeOn = pInfo.GetInt32("m_FlashTimeOn");
                m_FlashTimeOff = pInfo.GetInt32("m_FlashTimeOff"); 
            }
            if (pSerVersion > 2)
            {
                m_IsAnimated = pInfo.GetBoolean("m_IsAnimated");
                m_TargetX = pInfo.GetInt32("m_TargetX");
                m_TargetY = pInfo.GetInt32("m_TargetY");
                m_StartX = pInfo.GetInt32("m_StartX");
                m_StartY = pInfo.GetInt32("m_StartY");
            }
            if (pSerVersion > 3)
            {
                m_Z = pInfo.GetDouble("m_Z");
            }
            init();
        }

        public void CopyToClipboard()
        {
            var format = System.Windows.Forms.DataFormats.GetFormat(this.GetType().FullName);

            System.Windows.Forms.IDataObject dataObject = new System.Windows.Forms.DataObject();
            dataObject.SetData(format.Name, false, UtilitiesIO.SaveObjectToJsonString<SceneItem>(this));
            Clipboard.SetDataObject(dataObject, false);
        }

        public static SceneItem GetFromClipboard(Type itemType)
        {
            String jsonString = "";
            IDataObject dataObject = Clipboard.GetDataObject();
            string format = itemType.FullName;

            if (dataObject.GetDataPresent(format))
            {
                jsonString = dataObject.GetData(format) as String;
                MethodInfo mi = typeof(UtilitiesIO).GetMethod("GetObjectFromJsonString").MakeGenericMethod(itemType);
                object[] args = {null, jsonString};
                mi.Invoke(null, args);
                SceneItem item = args[0] as SceneItem;
                item.Id = SCENE_COUNT;
                SCENE_COUNT++;
                return item;
            }
            else
            {
                return null;
            }
            
        }

        //public static abstract SceneItem GetObjectFromJsonString(String jsonString);

        public new void GetObjectData(SerializationInfo pInfo, StreamingContext pContext)
        {

            pInfo.AddValue("m_SerVersionSceneItem", m_SerVersionSceneItem);

            pInfo.AddValue("m_Id", m_Id);

            
            pInfo.AddValue("m_X", m_X);
            pInfo.AddValue("m_Y", m_Y);
            pInfo.AddValue("m_Z", m_Z);
            pInfo.AddValue("m_Height", m_Height);
            pInfo.AddValue("m_Width", m_Width);
            pInfo.AddValue("m_Rotation", m_Rotation);
            pInfo.AddValue("m_Rx", m_Rx);
            pInfo.AddValue("m_Ry", m_Ry);
            pInfo.AddValue("m_Scale", m_Scale);
            pInfo.AddValue("m_Touchy", m_Touchy);

            pInfo.AddValue("m_Flash", m_Flash);
            pInfo.AddValue("m_FlashTimeOn", m_FlashTimeOn);
            pInfo.AddValue("m_FlashTimeOff", m_FlashTimeOff);

            pInfo.AddValue("m_IsAnimated", m_IsAnimated);
            pInfo.AddValue("m_TargetX", m_TargetX);
            pInfo.AddValue("m_TargetY", m_TargetY);
            pInfo.AddValue("m_StartX", m_StartX);
            pInfo.AddValue("m_StartY", m_StartY);

            
        }

        private void init() {

            base.Visibility = System.Windows.Visibility.Visible;

            var element = new ModelUIElement3D();

            this.Visual3DModel = new GeometryModel3D();

            this.MouseDown += this.SceneItemMouseDownEvent;
            this.MouseUp += this.SceneItemMouseUpEvent;
            this.MouseMove += this.SceneItemMouseMove;

            this.m_Timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
        }

        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

            reconstrctDrawable();    
        }

        /*protected void NotifyPropertyChanged(string pInfo)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(pInfo));
            }

            reconstrctDrawable();      
        }*/

        protected abstract void reconstrctDrawable();
        
        #region [Browsable(false)]

        [Browsable(false)]
        public new Transform3D Transform { get { return base.Transform; } set { base.Transform = value; } }

        [Browsable(false)]
        public new  CommandBindingCollection CommandBindings { get { return base.CommandBindings; } }

        [Browsable(false)]
        public new  DependencyObjectType DependencyObjectType { get { return base.DependencyObjectType; } }

        [Browsable(false)]
        public new  Dispatcher Dispatcher { get { return base.Dispatcher; } }

        [Browsable(false)]
        public new  InputBindingCollection InputBindings { get { return base.InputBindings; } }

        [Browsable(false)]
        public new  IEnumerable<TouchDevice> TouchesCaptured { get { return base.TouchesCaptured; } }
        
        [Browsable(false)]
        public new  IEnumerable<TouchDevice> TouchesCapturedWithin { get { return base.TouchesCapturedWithin; } }

        [Browsable(false)]
        public new  IEnumerable<TouchDevice> TouchesDirectlyOver { get { return base.TouchesDirectlyOver; } }

        [Browsable(false)]
        public new  IEnumerable<TouchDevice> TouchesOver { get { return base.TouchesOver; } }

        [Browsable(false)]
        public new bool IsEnabled { get { return base.IsEnabled; } set { base.IsEnabled = value; } }

        [Browsable(false)]
        protected new virtual bool IsEnabledCore { get { return base.IsEnabledCore; } }

        [Browsable(false)]
        public new bool IsFocused { get { return base.IsFocused; } }

        [Browsable(false)]
        public new bool IsInputMethodEnabled { get { return base.IsInputMethodEnabled; } }

        [Browsable(false)]
        public new bool IsKeyboardFocused { get { return base.IsKeyboardFocused; } }

        [Browsable(false)]
        public new bool IsKeyboardFocusWithin { get { return base.IsKeyboardFocusWithin; } }

        [Browsable(false)]
        public new bool IsMouseCaptured { get { return base.IsMouseCaptured; } }

        [Browsable(false)]
        public new bool IsMouseCaptureWithin { get { return base.IsMouseCaptureWithin; } }

        [Browsable(false)]
        public new bool IsMouseDirectlyOver { get { return base.IsMouseDirectlyOver; } }

        [Browsable(false)]
        public new bool IsMouseOver { get { return base.IsMouseOver; } }

        [Browsable(false)]
        public new bool IsStylusCaptured { get { return base.IsStylusCaptured; } }
        
        [Browsable(false)]
        public new bool IsStylusCaptureWithin { get { return base.IsStylusCaptureWithin; } }
        
        [Browsable(false)]
        public new bool IsStylusDirectlyOver { get { return base.IsStylusDirectlyOver; } }
        
        [Browsable(false)]
        public new bool IsStylusOver { get { return base.IsStylusOver; } }
     
        [Browsable(false)]
        public new bool IsVisible { get { return base.IsVisible; } }

        [Browsable(false)]
        public new bool AllowDrop { get { return base.AllowDrop; } set { base.AllowDrop = value; } }

        [Browsable(false)]
        public new bool AreAnyTouchesCaptured { get { return base.AreAnyTouchesCaptured; } }

        [Browsable(false)]
        public new bool AreAnyTouchesCapturedWithin { get { return base.AreAnyTouchesCapturedWithin; } }

        [Browsable(false)]
        public new bool AreAnyTouchesDirectlyOver { get { return base.AreAnyTouchesDirectlyOver; } }

        [Browsable(false)]
        public new bool AreAnyTouchesOver { get { return base.AreAnyTouchesOver; } }

        [Browsable(false)]
        public new bool Focusable { get { return base.Focusable; } set { base.Focusable = value; } }

        [Browsable(false)]
        public new bool HasAnimatedProperties { get { return base.HasAnimatedProperties; } }

        [Browsable(false)]
        public new bool IsHitTestVisible { get { return base.IsHitTestVisible; } set { base.IsHitTestVisible = value; } }

        [Browsable(false)]
        public new bool IsSealed { get { return base.IsSealed; } }


        #endregion


        [Browsable(false)]
        public abstract string Name
        {
            get;
        }

        [Browsable(false)]
        protected void isFlashing()
        {
            if (m_Timer.Enabled != m_Flash)
                m_Timer.Enabled = m_Flash;
        }

        private void OnTimedEventAnimation(object sender, System.Timers.ElapsedEventArgs e)
        {

            m_AnimationTriggered++;

            Dispatcher.Invoke(
               System.Windows.Threading.DispatcherPriority.Normal,
               new Action(
                   () =>
                   {
                       this.X = m_SpeedX + X;
                       this.Y = m_SpeedY + Y;
                   }
               )
           );



            if (m_AnimationTriggered * m_UpdateMillis >= m_Duration)
            {
                // trigger animation done
                this.m_AnimationTimer.Stop();
                //m_TimerAnimation.Enabled = false;

                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => {
                        X = m_StartX;
                        Y = m_StartY;
                    }));
                m_AnimationTriggered = 0;
                this.m_AnimationTimer.Start();
            }


        }

        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {

            if (m_FlashStatOn)
            {
                m_FlashStatOn = false;
                m_Timer.Interval = m_FlashTimeOn;
            }
            else if (!m_FlashStatOn)
            {
                m_FlashStatOn = true;
                m_Timer.Interval = m_FlashTimeOff;
            }

            
            Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Normal,
                new Action(
                    () =>
                    {
                        if (m_FlashStatOn)
                            this.Visibility = System.Windows.Visibility.Visible;
                        else
                            this.Visibility = System.Windows.Visibility.Hidden;
                    }
                )
            );
                
        }

        /// <summary>
        /// Move of a Oject 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        internal new void Move(double pDeltaX, double pDeltaY)
        {
            X += pDeltaX;
            Y += pDeltaY;

            m_StartX = (int)X;
            m_StartY = (int)Y;
            this.m_SpeedX = (m_TargetX - X) / (this.m_Duration / this.m_UpdateMillis);
            this.m_SpeedY = (m_TargetY - Y) / (this.m_Duration / this.m_UpdateMillis);
        }

        protected void SceneItemMouseDownEvent(object sender, MouseButtonEventArgs e)
        {
        }

        protected void SceneItemMouseUpEvent(object sender, MouseButtonEventArgs e)
        {
        }

        protected void SceneItemMouseMove(object sender, MouseEventArgs e)
        {
        }


        #region getter / setter
        [Browsable(false)]
        public int Id
        {
            get
            {
                return m_Id;
            }
            set
            {
                m_Id = value;
                NotifyPropertyChanged("Id");
            }
        }

        [Category("Position")]
        [DisplayName("X")]
        [Description("The X position of the Scene")]
        [Editor(typeof(double), typeof(double))]
        public double X
        {
            get { return m_X; }
            set
            {
                m_X = value;
                NotifyPropertyChanged("X");
            }
        }

        [Category("Position")]
        [DisplayName("Y")]
        [Description("The Y position of the Scene")]
        [Editor(typeof(double), typeof(double))]
        public double Y
        {
            get { return m_Y; }
            set
            {
                m_Y = value;
                NotifyPropertyChanged("Y");
            }
        }

        [Category("Position")]
        [DisplayName("Z")]
        [Description("The Z position of the Scene")]
        [Editor(typeof(double), typeof(double))]
        public double Z
        {
            get { return m_Z; }
            set
            {
                m_Z = value;
                NotifyPropertyChanged("Z");
            }
        }

        [Category("Animation")]
        [DisplayName("Is Animated")]
        [Description("If the scene is animated")]
        [Editor(typeof(bool), typeof(bool))]
        public bool IsAnimated
        {
            get { return m_IsAnimated; }
            set
            {
                m_IsAnimated = value;
                NotifyPropertyChanged("Animation");
            }
        }

        [Category("Animation")]
        [DisplayName("Target X")]
        [Description("The X position of the scene target")]
        [Editor(typeof(double), typeof(double))]
        public int TargetX
        {
            get { return m_TargetX; }
            set
            {
                m_TargetX = value;
                NotifyPropertyChanged("TargetX");
            }
        }

        [Category("Animation")]
        [DisplayName("Target Y")]
        [Description("The Y position of the scene target")]
        [Editor(typeof(double), typeof(double))]
        public int TargetY
        {
            get { return m_TargetY; }
            set
            {
                m_TargetY = value;
                NotifyPropertyChanged("TargetY");
            }
        }


        [Category("Layout")]
        [DisplayName("Width")]
        [Description("The width of the scene target")]
        [Editor(typeof(double), typeof(double))]
        public double Width
        {
            get { return m_Width; }
            set
            {
                m_Width = value;
                NotifyPropertyChanged("WIDTH");
            }
        }


        [Category("Layout")]
        [DisplayName("Height")]
        [Description("The height of the scene")]
        [Editor(typeof(double), typeof(double))]
        public double Height
        {
            get { return m_Height; }
            set
            {
                m_Height = value;
                NotifyPropertyChanged("HEIGHT");
            }
        }

        [Category("Representation")]
        [DisplayName("Touchable")]
        [Description("Is the scene touchable or not")]
        [Editor(typeof(bool), typeof(bool))]
        public bool Touchy
        {
            get { return m_Touchy; }
            set
            {
                m_Touchy = value;
                NotifyPropertyChanged("Touchy");
            }
        }

        [Category("Layout")]
        [DisplayName("Rotation")]
        [Description("The rotation of the scene")]
        [Editor(typeof(double), typeof(double))]
        public double Rotation // radians
        {
            get
            {
                return m_Rotation;
            }
            set
            {
                m_Rotation = value;
                NotifyPropertyChanged("Rotation");
            }
        }

        [Category("Layout")]
        [DisplayName("Scale")]
        [Description("The scale of the scene")]
        [Editor(typeof(double), typeof(double))]
        public double Scale
        {
            get
            {
                return m_Scale;
            }
            set
            {
                m_Scale = value;
                NotifyPropertyChanged("Scale");
            }
        }

        /// <summary>
        /// The rotaion center X of the scene
        /// </summary>
        [Category("Layout")]
        [DisplayName("Rotaion Center X")]
        [Description("The rotaion center X of the scene")]
        [Editor(typeof(double), typeof(double))]
        public double Rx
        {
            get
            {
                return m_Rx;
            }
            set
            {
                m_Rx = value;
                NotifyPropertyChanged("Rx");
            }
        }

        /// <summary>
        /// The rotaion center Y of the scene
        /// </summary>
        [Category("Layout")]
        [DisplayName("Rotaion Center Y")]
        [Description("The rotaion center Y of the scene")]
        [Editor(typeof(double), typeof(double))]
        public double Ry
        {
            get
            {
                return m_Ry;
            }
            set
            {
                m_Ry = value;
                NotifyPropertyChanged("Ry");
            }
        }

        [Category("Representation")]
        [DisplayName("Visibility")]
        [Description("The visible of the scene")]
        [Editor(typeof(bool), typeof(bool))]
        public new Visibility Visibility
        {
            get
            {
                return base.Visibility;
            }
            set
            {
                base.Visibility = value;
                NotifyPropertyChanged("Visible");
            }

        }

        /// <summary>
        /// Object is Flashing with FlashTimeOn and FlashTimeOff
        /// </summary>
        [Category("Flash")]
        [DisplayName("Flash")]
        [Description("This scene is flashing or not")]
        [Editor(typeof(bool), typeof(bool))]
        public bool Flash
        {
            get
            {
                return m_Flash;
            }
            set
            {
                m_Flash = value;
                if (!m_Flash)
                    this.Visibility = System.Windows.Visibility.Visible;
                NotifyPropertyChanged("Flash");
            }
        }
        
        /// <summary>
        /// in milliseconds
        /// </summary>
        [Category("Flash")]
        [DisplayName("Flash Time On")]
        [Description("How long the time is when visible while flashing, in milli seconds")]
        [Editor(typeof(int), typeof(int))]
        public int FlashTimeOn
        {
            get
            {
                return m_FlashTimeOn;
            }
            set
            {
                m_FlashTimeOn = value;
                NotifyPropertyChanged("FlashTimeOn");
            }
        }

        /// <summary>
        /// in milliseconds
        /// </summary>
        [Category("Flash")]
        [DisplayName("Flash Time Off")]
        [Description("How long the time is when invisible while flashing, in milli seconds")]
        [Editor(typeof(int), typeof(int))]
        public int FlashTimeOff
        {
            get
            {
                return m_FlashTimeOff;
            }
            set
            {
                m_FlashTimeOff = value;
                NotifyPropertyChanged("FlashTimeOff");
            }

        }

        /// <summary>
        /// If the scene is currently the scene to edit
        /// </summary>
        [Browsable(false)]
        public bool IsInEditMode
        {
            get
            {
                return m_IsInEditMode;
            }
            set
            {
                m_IsInEditMode = value;
                NotifyPropertyChanged("IsInEditMode");
            }
        }


        [Browsable(false)]
        public Boolean IsShown {
            get { return m_isShown; }
            set
            {
                m_isShown = value;
                NotifyPropertyChanged();
            }
            
        }
        #endregion
    }
}
