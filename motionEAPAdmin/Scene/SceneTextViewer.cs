// <copyright file=SceneTextViewer.cs
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

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace motionEAPAdmin.Scene
{
    [Serializable()]
    class SceneTextViewer : SceneColor, ISerializable
    {
        private int m_SerVersion = 1;

        public string m_Text;
        public FontFamily m_FontFamily;
        public double m_FontSize;

        public SceneTextViewer(double x, double y, double width, double height, string text, FontFamily pFontFamily, double pFontSize, Color pColor, double rotation = 0, double rotationCenterX = 0, double rotationCenterY = 0, double scale = 1.0f)
            : base(x, y, width, height, pColor, rotation, rotationCenterX, rotationCenterY, scale)
        {
            m_Text = text;
            m_FontFamily = pFontFamily;
            m_FontSize = pFontSize;
            Touchy = true;

            reconstrctDrawable();
        }

        public SceneTextViewer()
            : base()
        {
        }

        protected SceneTextViewer(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            int pSerVersion = info.GetInt32("m_SerVersion");

            if (pSerVersion < 1)
                return;

            m_Text = info.GetString("m_Text");
            m_FontFamily = (FontFamily)info.GetValue("m_Font", typeof(FontFamily));
            m_FontSize = info.GetDouble("m_FontSize");

            reconstrctDrawable();
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("m_SerVersion", m_SerVersion);

            info.AddValue("m_Text", m_Text);
            info.AddValue("m_FontFamily", m_FontFamily);
            info.AddValue("m_FontSize", m_FontSize);
        }

        protected override void reconstrctDrawable()
        {
            this.Visual3DModel = null;

            isFlashing();
        }

        public override string Name
        {
            get
            {
                return "TextViewer " + this.Id;
            }
        }

        [Category("Text")]
        [DisplayName("Text")]
        [Description("The text")]
        [Editor(typeof(string), typeof(string))]
        public string Text
        {
            get { return m_Text; }
            set
            {
                m_Text = value;
                NotifyPropertyChanged("Text");
            }
        }
        
        [Category("Text")]
        [DisplayName("Font Size")]
        [Description("The font size of the text")]
        [Editor(typeof(double), typeof(double))]
        public double FontSize
        {
            get { return m_FontSize; }
            set
            {
                m_FontSize = value;
                NotifyPropertyChanged("FontSize");
            }
        }

        [Category("Text")]
        [DisplayName("Font Family")]
        [Description("The fonnt family of the text")]
        [Editor(typeof(FontFamily), typeof(FontFamily))]
        public FontFamily FontFamily
        {
            get { return m_FontFamily; }
            set
            {
                m_FontFamily = value;
                NotifyPropertyChanged("FontFamily");
            }
        }
    }
}
