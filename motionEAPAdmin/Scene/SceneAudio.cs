// <copyright file=SceneAudio.cs
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
    class SceneAudio : SceneItem, ISerializable
    {
        private int m_SerVersion = 1;
        private string m_Filename;
        private MediaPlayer m_MediaPlayer = new MediaPlayer();


        /// <summary>
        /// Constructor called when creating new audio scene object
        /// </summary>
        /// <param name="pFile"></param>
        public SceneAudio(string pFile)
        {
            if (pFile != null)
            {
                this.m_Filename = pFile;
            }
            else
            {
                m_Filename = "C:\\richtig.wav";
            }
            //reconstrctDrawable();
        }


        /// <summary>
        /// Constructor called when loading workflow containing audio objects
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected SceneAudio(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            int pSerVersion = info.GetInt32("m_SerVersion");

            if (pSerVersion > 0)
            {
                m_Filename = info.GetString("m_Filename");

            }
            //reconstrctDrawable();
        }


        /// <summary>
        /// Called when loading old workflow
        /// </summary>
        /// <param name="pInfo"></param>
        /// <param name="pContext"></param>
        public new void GetObjectData(SerializationInfo pInfo, StreamingContext pContext)
        {
            base.GetObjectData(pInfo, pContext);

            pInfo.AddValue("m_SerVersion", m_SerVersion);
            pInfo.AddValue("m_Filename", m_Filename);
        }

        /// <summary>
        /// plays the audiofile
        /// </summary>
        protected override void reconstrctDrawable()
        {
            playAudio(m_Filename);
        }


        /// <summary>
        /// Helper to play audio file
        /// </summary>
        /// <param name="pSource"></param>
        private void playAudio(string pSource)
        {
            m_MediaPlayer.Open(new Uri(pSource));
            m_MediaPlayer.Play();
        }


        /// <summary>
        /// Return uniqe name to audio object
        /// </summary>
        public override string Name
        {
            get
            {
                return "Audio " + this.Id;
            }
        }


        /// <summary>
        /// Audio file destination
        /// </summary>
        [Category("Source")]
        [DisplayName("File Path")]
        [Description("The path to the audio file")]
        public String FileName
        {
            get { return m_Filename; }
            set
            {
                m_Filename = value;
                playAudio(m_Filename);
                NotifyPropertyChanged();
            }
        }
    }
}
