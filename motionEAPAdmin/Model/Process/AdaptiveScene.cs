// <copyright file=AdaptiveScene.cs
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
// <date> 11/2/2016 12:25:59 PM</date>

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace motionEAPAdmin.Model.Process
{
    /// <summary>
    /// This class represents an adaptive scene.
    /// An adapive scene is a scene that is able to adapt itself to the current needs of the user.
    /// One could see this as a mapping between an adaptivity level and a scene
    /// </summary>
    /// 
    /// 
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    [Serializable()]
    public class AdaptiveScene : ISerializable, INotifyPropertyChanged
    {

        private string m_Name = "";

        /// <summary>
        /// the Adaptivity level belonging to this scene.
        /// </summary>
        private AdaptivityLevel m_Level; 

        /// <summary>
        /// the scene that belongs to the adative scene.
        /// </summary>
        private Scene.Scene m_Scene;

        public event PropertyChangedEventHandler PropertyChanged; // event for the databinding

        // constructor
        public AdaptiveScene(motionEAPAdmin.Scene.Scene pScene, AdaptivityLevel pLevel)
        {
            this.m_Scene = pScene;
            this.m_Level = pLevel;
        }

        protected AdaptiveScene(SerializationInfo info, StreamingContext context)
        {
            m_Name = info.GetString("m_Name");
            m_Level = (AdaptivityLevel)info.GetValue("m_Level", typeof(AdaptivityLevel));
            m_Scene = (Scene.Scene)info.GetValue("m_Scene", typeof(Scene.Scene));
        }


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("m_Name", m_Name);
            info.AddValue("m_Level", m_Level);
            info.AddValue("m_Scene", m_Scene);
        }

        #region getter/setter
        public string Name
        {
            get
            {
                string myName = "";
                if (m_Scene == null)
                {
                    myName = " - null";
                }
                else
                {
                    myName = " - " + m_Scene.Id;
                }
                m_Name = this.m_Level.Name + myName;
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }

        public Scene.Scene Scene
        {
            get
            {
                return m_Scene;
            }
            set
            {
                m_Scene = value;
                NotifyPropertyChanged("SCENE");
            }
        }

        public AdaptivityLevel Level
        {
            get
            {
                return m_Level;
            }
            set
            {
                m_Level = value;
                NotifyPropertyChanged("LEVEL");
            }
        }

        #endregion

        private void NotifyPropertyChanged(string Obj)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(Obj));
            }
        }
    }
}
