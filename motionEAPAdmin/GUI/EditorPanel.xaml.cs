// <copyright file=EditorPanel.xaml.cs
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
// <date> 11/2/2016 12:25:58 PM</date>

using motionEAPAdmin.Scene;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace motionEAPAdmin.GUI
{
    /// <summary>
    /// Interaktionslogik für Editor.xaml
    /// </summary>
    public partial class EditorPanel : UserControl
    {

        public EditorPanel()
        {
            InitializeComponent();
            refreshDataBinding();
            if (((Scene.Scene)SceneManager.Instance.CurrentScene).Items.Count > 0)
                m_ListScenes.SelectedIndex = 0;
        }

        public void refreshDataBinding()
        {
                m_ListScenes.DataContext = SceneManager.Instance.CurrentScene;

                SceneManager.Instance.CurrentScene.selectedItemChanged += CurrentScene_selectedItemChanged;
                //m_propertyGrid.DataContext = ((Scene.Scene)SceneManager.Instance.CurrentScene).Items;  

        }

        void CurrentScene_selectedItemChanged(object pSource, SceneItem pScene)
        {
            m_ListScenes.SelectedItem = pScene;
            m_propertyGrid.DataContext = pScene;
        }
        
        private void MenuItem_DeleteSelectedScene(object sender, RoutedEventArgs e)
        {
            SceneManager.Instance.CurrentScene.Remove(m_ListScenes.SelectedItem as SceneItem);
        }

        private void m_ListScenes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResetEditMode();

            if (m_ListScenes.SelectedItem is SceneItem)
            {
                m_propertyGrid.DataContext = m_ListScenes.SelectedItem as SceneItem;
                (m_ListScenes.SelectedItem as SceneItem).IsInEditMode = true;
            }
        }


        public void ResetEditMode()
        {
            foreach (SceneItem s in m_ListScenes.Items)
            {
                s.IsInEditMode = false;
            }
        }
        
        private void buttonRectangle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataObject dragData = new DataObject("SceneItem", typeof(SceneRect).ToString());
            DragDrop.DoDragDrop((Button)sender, dragData, DragDropEffects.Move);
        }

        private void buttonCircle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataObject dragData = new DataObject("SceneItem", typeof(SceneCircle).ToString());
            DragDrop.DoDragDrop((Button)sender, dragData, DragDropEffects.Move);
        }

        private void buttonText_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataObject dragData = new DataObject("SceneItem", typeof(SceneText).ToString());
            DragDrop.DoDragDrop((Button)sender, dragData, DragDropEffects.Move);
        }

        private void buttonTextViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataObject dragData = new DataObject("SceneItem", typeof(SceneTextViewer).ToString());
            DragDrop.DoDragDrop((Button)sender, dragData, DragDropEffects.Move);
        }

        private void buttonImage_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataObject dragData = new DataObject("SceneItem", typeof(SceneImage).ToString());
            DragDrop.DoDragDrop((Button)sender, dragData, DragDropEffects.Move);
        }

        private void buttonVideo_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataObject dragData = new DataObject("SceneItem", typeof(SceneVideo).ToString());
            DragDrop.DoDragDrop((Button)sender, dragData, DragDropEffects.Move);
        }

        private void buttonPolygon_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataObject dragData = new DataObject("SceneItem", typeof(ScenePolygon).ToString());
            DragDrop.DoDragDrop((Button)sender, dragData, DragDropEffects.Move);
        }
        private void buttonAudio_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataObject dragData = new DataObject("SceneItem", typeof(SceneAudio).ToString());
            DragDrop.DoDragDrop((Button)sender, dragData, DragDropEffects.Move);
        }

        public SceneItem getCurrentSelected ()
        {
            return m_ListScenes.SelectedItem as SceneItem;
        }

        private void MenuItem_CopyItemClick(object sender, RoutedEventArgs e)
        {
            SceneItem item = m_ListScenes.SelectedItem as SceneItem;

            item.CopyToClipboard();
        }

        private void m_ListScenes_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            FrameworkElement fe = e.Source as FrameworkElement;
            ContextMenu cm = fe.ContextMenu;
            foreach (MenuItem mi in cm.Items)
            {
                if ((String)mi.Name == "PasteMenuItem")
                {
                    mi.IsEnabled = false;
                    if (Clipboard.GetDataObject().GetFormats().Length > 0)
                    {
                        String typeName = (string)Clipboard.GetDataObject().GetFormats().GetValue(0);
                        if (typeName.Contains("motionEAPAdmin.Scene"))
                        {
                            if (Type.GetType(typeName).IsSubclassOf(typeof(SceneItem))) mi.IsEnabled = true;
                        }
                    }
                }
            }
        }

        private void MenuItem_PasteItemClick(object sender, RoutedEventArgs e)
        {
            String typeName = (string)Clipboard.GetDataObject().GetFormats().GetValue(0);
            var itemType = Type.GetType(typeName);
            dynamic item = SceneItem.GetFromClipboard(itemType);
            Convert.ChangeType(item, itemType);
            if (item != null)
            {
                SceneManager.Instance.CurrentScene.Add(item);
            }
        }
    }
}
