// <copyright file=SceneEditorDialog.xaml.cs
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

using motionEAPAdmin.Backend;
using motionEAPAdmin.Scene;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace motionEAPAdmin.GUI.Dialog
{
    /// <summary>
    /// Interaktionslogik für SceneEditorDialog.xaml
    /// </summary>
    public partial class SceneEditorDialog : Window
    {

        private WorkflowBase baseClassToEdit = null;
        private Scene.Scene sceneToEdit = null;

        public SceneEditorDialog(WorkflowBase pToEdit)
        {
            InitializeComponent();
            baseClassToEdit = pToEdit;            
            EditWorkflowManager.Instance.EditorGUIHandle = this;
        }


        public SceneEditorDialog(Scene.Scene pToEdit)
        {
            InitializeComponent();
            sceneToEdit = pToEdit;    
            EditWorkflowManager.Instance.EditorGUIHandle = this;

        }


        protected override void OnClosed(EventArgs e)
        {

            m_EditorPanel.ResetEditMode();

            if(baseClassToEdit != null)
            { 

            StateManager.Instance.SetNewState(this, HciLab.motionEAP.InterfacesAndDataModel.AllEnums.State.IDLE);
            baseClassToEdit.CustomScene = new Scene.Scene();
             
            foreach(SceneItem item in SceneManager.Instance.CurrentScene.Items)
            {
                item.Touchy = false;
                baseClassToEdit.CustomScene.Add(item);
            }

            // return to idle mode
            SceneManager.Instance.CurrentScene.Clear();

            }


            base.OnClosed(e);
        }

        public void refreshDataBinding()
        {
            m_EditorPanel.refreshDataBinding();
        }

        private void buttonNewScene_Click(object sender, RoutedEventArgs e)
        {
            // TODO: really want to do a new scene - this will delete the current scene
            SceneManager.Instance.CurrentScene.Clear();
        }

        private void buttonSaveScene_Click(object sender, RoutedEventArgs e)
        {
            SceneManager.Instance.SaveSceneToFile();
        }

        private void buttonLoadScene_Click(object sender, RoutedEventArgs e)
        {
            SceneManager.Instance.LoadScene();
        }


        private void MenuItem_DeleteSelectedScene(object sender, RoutedEventArgs e)
        {
            SceneManager.Instance.CurrentScene.Remove(m_EditorPanel.getCurrentSelected());
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
    }
}
