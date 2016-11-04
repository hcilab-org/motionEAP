// <copyright file=Scene.cs
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
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Forms;


namespace motionEAPAdmin.Scene
{
    /// <summary>
    /// This class represents a whole scene.
    /// A scene is a collection of multiple sceneitems and their assembly
    /// </summary>
    /// 
    /// 
    [Serializable()]
    public class Scene : SceneItem, ISerializable
    {
        private int m_SerVersion = 1;

        private int m_LastSelectedItemIndex = -1;

        private CollectionWithItemNotify<SceneItem> m_Items = new CollectionWithItemNotify<SceneItem>();
        
        [Browsable(false)]
        public delegate void EditItemChangedHandler(SceneItem item);

        [Browsable(false)]
        public event EditItemChangedHandler EditItemChanged;

        public Scene()
            : base(0, 0, 1, 1, 0, 0, 0, 1)
        {
            init();
        }

        protected Scene(SerializationInfo pInfo, StreamingContext context)
            : base(pInfo, context)
        {
            int pSerVersion = pInfo.GetInt32("m_SerVersion");

            if (pSerVersion < 1)
                return;

            m_Items = (CollectionWithItemNotify<SceneItem>)pInfo.GetValue("m_Items", typeof(CollectionWithItemNotify<SceneItem>));

            init();
        }

        public static Scene GetFromClipboard()
        {
            Scene scene = null;
            String jsonString = "";
            IDataObject dataObject = Clipboard.GetDataObject();
            string format = typeof(Scene).FullName;

            if (dataObject.GetDataPresent(format))
            {
                jsonString = dataObject.GetData(format) as String;
                UtilitiesIO.GetObjectFromJsonString<Scene>(ref scene, jsonString);
            }
            scene.Id = new Scene().Id;
            return scene;
        }

        void m_Items_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged("ItemsChanged");
        }

        public new void GetObjectData(SerializationInfo pInfo, StreamingContext pContext)
        {
            base.GetObjectData(pInfo, pContext);

            pInfo.AddValue("m_SerVersion", m_SerVersion);

            pInfo.AddValue("m_Items", m_Items);

            reconstrctDrawable();
        }

        private void init()
        {
            m_Items.CollectionChanged += m_Items_CollectionChanged;
            reconstrctDrawable();
        }

        void m_Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("Items_CollectionChanged");
        }

        protected override void reconstrctDrawable()
        {
            this.Visual3DModel = null;
            isFlashing();
        }

        /// <summary>
        /// Add SceneItem to List
        /// </summary>
        /// <param name="item"></param>
        public void Add(SceneItem item)
        {
            m_Items.Add(item);
            NotifyPropertyChanged();
        }

        /// <summary>
        /// removes the given item from being rendered
        /// </summary>
        /// <param name="pItem">item to be removed</param>
        /// 
        public void Remove(SceneItem pItem)
        {
            m_Items.Remove(pItem);
            NotifyPropertyChanged("Remove");
        }
        

        public override string Name
        {
            get
            {
                return "Scene " + this.Id;
            }
        }

        public void Clear()
        {
            m_Items.Clear();
            NotifyPropertyChanged("Clear");
        }

        public delegate void SelectedItemChangedHandler(object pSource, SceneItem pScene);

        public event SelectedItemChangedHandler selectedItemChanged;

        public void OnSelectedItemChanged(object pSource, SceneItem pScene)
        {
            if (this.selectedItemChanged != null)
                selectedItemChanged(pSource, pScene);
        }


        #region getter / setter

        [Browsable(false)]
        public CollectionWithItemNotify<SceneItem> Items
        {
            get { return m_Items; }
            set
            {
                m_Items = value;
                NotifyPropertyChanged("Items");
            }
        }

        static readonly object _SelectedItemIndexLock = new object();

        public SceneItem SelectedItem
        {
            get {
                lock (_SelectedItemIndexLock)
                {
                    if (m_LastSelectedItemIndex >= 0 && m_LastSelectedItemIndex < Items.Count)
                    {

                        return Items[m_LastSelectedItemIndex];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            set
            {
                int i = Items.IndexOf(value);
                SelectedItemIndex = i;
            }
            
        }

        public int SelectedItemIndex
        {
            get { return m_LastSelectedItemIndex; }
            set
            {
                lock (_SelectedItemIndexLock)
                {
                    m_LastSelectedItemIndex = value;
                    OnSelectedItemChanged(this, SelectedItem);
                    NotifyPropertyChanged("EditItem");
                }
            }
        }
        #endregion


        internal void setShown(bool p_shown)
        {
            foreach (SceneItem item in m_Items)
            {
                item.IsShown = p_shown;
            }
        }
    }
}
