// <copyright file=SceneVideo.cs
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

using motionEAPAdmin.GUI.TypeEditor;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace motionEAPAdmin.Scene
{
    [Serializable()]
    class SceneVideo : SceneItem, ISerializable
    {
        private int m_SerVersion = 1;

        private String m_Filename;
        
        private VideoDrawing m_VideoDrawing = new VideoDrawing();
        
        /// <summary>
        /// Creates a new instance of SceneVideo. Does not play the video automatically (call Play).
        /// </summary>
        public SceneVideo(double x, double y, double w, double h, String file, double rotation = 0, double rotationCenterX = 0, double rotationCenterY = 0, double scale = 1.0f)
            : base(x, y, w, h, rotation, rotationCenterX, rotationCenterY, scale)
        {
            if (file != null)
            {
                this.m_Filename = file;
            }
            else
            {
                m_Filename = "C:\\Wildlife.wmv";
            }
            init();
        }
        
        /// <summary>
        /// For Deserialisazion
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected SceneVideo(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            int pSerVersion = info.GetInt32("m_SerVersion");

            if (pSerVersion > 0)
            {
                m_Filename = info.GetString("m_Filename");
                
            }
            
            init();
        }

        private void init()
        {
            MediaTimeline mTimeline = new MediaTimeline(new Uri(m_Filename, UriKind.Absolute));
            mTimeline.RepeatBehavior = RepeatBehavior.Forever;
            MediaClock mClock = mTimeline.CreateClock();
            MediaPlayer repeatingVideoDrawingPlayer = new MediaPlayer();
            repeatingVideoDrawingPlayer.Clock = mClock;
            m_VideoDrawing.Player = repeatingVideoDrawingPlayer;
            Z = 1;

            reconstrctDrawable();
        }

        public new void GetObjectData(SerializationInfo pInfo, StreamingContext pContext)
        {
            base.GetObjectData(pInfo, pContext);

            pInfo.AddValue("m_SerVersion", m_SerVersion);
            pInfo.AddValue("m_Filename", m_Filename);
        }


        protected override void reconstrctDrawable()
        {
            
            if (!(this.Visual3DModel is GeometryModel3D))
                throw new NotSupportedException();
                        
            m_VideoDrawing.Rect = new Rect(0, 0, this.Width, this.Height);

            if (((GeometryModel3D)this.Visual3DModel).Material is DiffuseMaterial && ((DiffuseMaterial)((GeometryModel3D)this.Visual3DModel).Material).Brush is DrawingBrush)
            {
            }
            else
            {
                ((GeometryModel3D)this.Visual3DModel).Material = new DiffuseMaterial(new DrawingBrush(m_VideoDrawing));
            }
            
            ((GeometryModel3D)this.Visual3DModel).Geometry = HciLab.Utilities.Mash3D.Image3DGeo.MappingMash(X, Y, this.Width, this.Height, Z);            

            isFlashing();
            handleShown();
        }

        public override string Name
        {
            get
            {
                return "Video " + this.Id;
            }
        }

        void m_Player_MediaEnded(object sender, EventArgs e)
        {
            m_VideoDrawing.Player.Position = TimeSpan.Zero;
            m_VideoDrawing.Player.Clock.Controller.Begin();
        }


        private void handleShown()
        {
            if (IsInEditMode)
            {
                m_VideoDrawing.Player.Clock.Controller.Begin();
            }
            else
            {
                m_VideoDrawing.Player.Clock.Controller.Stop();
            }
        }

        /// <summary>
        /// Video File Destination
        /// </summary>
        [Category("Source")]
        [DisplayName("File Path")]
        [Description("The path to the video file")]
        [Editor(typeof(VideoBrowseTypeEditor), typeof(VideoBrowseTypeEditor))]
        public String FileName
        {
            get { return m_Filename; }
            set
            {
                m_Filename = value;
                init();
                NotifyPropertyChanged();
            }
        }

    }
}
