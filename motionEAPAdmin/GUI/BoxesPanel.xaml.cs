// <copyright file=BoxesPanel.xaml.cs
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

using Emgu.CV;
using Emgu.CV.Structure;
using HciLab.Kinect;
using HciLab.motionEAP.InterfacesAndDataModel;
using HciLab.Utilities;
using motionEAPAdmin.Backend;
using motionEAPAdmin.Backend.Boxes;
using motionEAPAdmin.ContentProviders;
using motionEAPAdmin.GUI.Dialog;
using motionEAPAdmin.Scene;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace motionEAPAdmin.GUI
{
    /// <summary>
    /// Interaktionslogik für Boxes.xaml
    /// </summary>
    public partial class BoxesPanel : UserControl
    {
        public BoxesPanel()
        {
            InitializeComponent();

            m_TopBar.DataContext = SettingsManager.Instance.Settings;
            StateManager.Instance.StateChange += StateChange;
            m_LstBoxBoxes.DataContext = BoxManager.Instance.CurrentLayout;
            m_ButtomBar.DataContext = SettingsManager.Instance.Settings;
        }

        private void StateChange(object pSource, AllEnums.State pState)
        {
            
            if (pState == HciLab.motionEAP.InterfacesAndDataModel.AllEnums.State.IDLE)
            {
                //checkBoxProjectVisualFeedback.IsChecked = SettingsManager.Instance.Settings.BoxesVisualFeedbackProject;
            }
            else if (pState == HciLab.motionEAP.InterfacesAndDataModel.AllEnums.State.EDIT)
            {
                SceneManager.Instance.TemporaryBoxScene.Clear();
            }             
        }

        public static readonly System.Drawing.Pen BOX_GUI_FEEDBACK_TRIGGERT = new System.Drawing.Pen(System.Drawing.Color.Red, 3);
        public static readonly System.Drawing.Pen BOX_GUI_FEEDBACK_TRIGGERT_NOT = new System.Drawing.Pen(System.Drawing.Color.Green, 3);
        public static readonly System.Drawing.Font BOX_GUI_FEEDBACK_TEXT = new System.Drawing.Font("Arial", 16);
        public static readonly SolidBrush BOX_GUI_FEEDBACK_TEXT_COLOR = new SolidBrush(System.Drawing.Color.White);
        public static readonly int BOX_BORDERWIDTH = 5;
        public static readonly int BOX_MANUALY_INSERT_HEIGHT = 50;
        public static readonly int BOX_MANUALY_INSERT_WIDTH = 50;

        private Box m_DraggedObj = null;
        private bool m_DragEnabled = false;
        private AllEnums.Direction m_DragMode = AllEnums.Direction.NONE;

        /// <summary>
        /// Code to be called within the main OnLoaded Method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="er"></param>
        public void Boxes_OnLoaded(object sender, RoutedEventArgs er)
        {
        }

        /// <summary>
        /// Code to be called within a certain part of the main ProccessFrame Method
        /// </summary>
        private void Boxes_ProccessFrame_Draw(bool hasToUpdateUI, Image<Bgr, byte> pImage)
        {
            if (SettingsManager.Instance.Settings.BoxesVisualFeedbackProject)
                BoxManager.Instance.drawProjectorUI();
        }
        
        private void initializeDrag(System.Windows.Point p)
        {
            foreach (Box o in BoxManager.Instance.CurrentLayout.Boxes)
            {
                m_DragMode = isMouseOnObj(p, o);
                if (m_DragMode != AllEnums.Direction.NONE)
                {
                    m_DraggedObj = o;
                    m_DragEnabled = true;
                    if (m_DragMode == AllEnums.Direction.NORTH || m_DragMode == AllEnums.Direction.SOUTH)
                    {
                        Cursor = Cursors.SizeNS;
                    }
                    else if (m_DragMode == AllEnums.Direction.EAST || m_DragMode == AllEnums.Direction.WEST)
                    {
                        Cursor = Cursors.SizeWE;
                    }
                    return;
                }
            }
            m_DragEnabled = false;
        }

        private void createANewObj(object sender, MouseButtonEventArgs e)
        {
            Image<Gray, Int32> img = KinectManager.Instance.GetCurrentDepthImage();
            int boxX = (int)e.GetPosition(image).X;
            int boxY = (int)e.GetPosition(image).Y;

            // get the Z position from the Kinect
            if (img != null)
            {

                // define a new box
                Box b = BoxManager.Instance.createBoxFromFactory();
                b.X = boxX;
                b.Y = boxY;

                b.Z = img.Data[b.Y, b.X, 0];
                if (b.Z == 0)
                {
                    BoxManager.Instance.decreaseIDByOne();
                    return;
                }


                // check if box is bigger than depth frame
                if (BOX_MANUALY_INSERT_WIDTH + b.X < img.Width) { 
                    b.Width = BOX_MANUALY_INSERT_WIDTH;
                }
                else
                {
                    b.Width = img.Width - b.X;
                }

                if(BOX_MANUALY_INSERT_HEIGHT + b.Y < img.Height)
                { 
                    b.Height = BOX_MANUALY_INSERT_HEIGHT;
                }
                else
                {
                    b.Height = img.Height - b.Y;
                }
            



                // depthmean
                double depthmean = calculateCurrentMeanDepth(b);
                b.Depthmean = depthmean;

                // standard box values
                b.LowerThreshold = 60;
                b.UpperThreshold = 190;

                BoxManager.Instance.CurrentLayout.Boxes.Add(b);
            }
        }



        private double calculateCurrentMeanDepth(Box b)
        {

            Image<Gray, Int32> depthImg = KinectManager.Instance.GetCurrentDepthImage();

            int count = 0;
            double sum = 0;
            for (int x = b.X; x < b.Width + b.X; x++)
            {
                for (int y = b.Y; y < b.Height + b.Y; y++)
                {
                    // freaking img.Data is y,x,0
                    int depthval = depthImg.Data[y, x, 0];
                    int real_depthval = depthval / KinectManager.SCALE_FACTOR;
                    if (real_depthval != 0)
                    {
                        sum = sum + real_depthval;
                        count++;
                    }
                }
            }

            if (count == 0)
            {
                return 0;
            }
            else
            {
                return sum / (double)count;
            }

        }


        private void buttonSaveBoxLayout_Click(object sender, RoutedEventArgs e)
        {
            BoxManager.Instance.CurrentLayout.LayoutName = m_TextBoxLayoutName.Text;
            BoxManager.Instance.saveBoxLayoutToFile();
        }

        private void buttonLoadBoxLayout_Click(object sender, RoutedEventArgs e)
        {
            BoxManager.Instance.loadBoxLayoutFromFile();

            // also update name
            if (BoxManager.Instance.CurrentLayout != null)
            {
                m_TextBoxLayoutName.Text = BoxManager.Instance.CurrentLayout.LayoutName;
                //this.listBoxWuerfel.DataContext = BoxManager.Instance.CurrentLayout.Boxes;
            }
        }

        public void refreshLayoutName()
        {
            if (m_TextBoxLayoutName != null && (BoxManager.Instance.CurrentLayout != null))
            {
                m_TextBoxLayoutName.Text = BoxManager.Instance.CurrentLayout.LayoutName;
            }
        }

        public void Boxes_refreshDataContext()
        {
            this.m_LstBoxBoxes.DataContext = BoxManager.Instance.CurrentLayout;
        }

        private void image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point p = e.GetPosition(image);
            if (isMouseOnAnyObj(p) == AllEnums.Direction.NONE)
            {
                createANewObj(sender, e);
            }
            else
            {
                initializeDrag(p);
            }
        }

        private void image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (m_DragEnabled)
            {
                // update depth data
                m_DraggedObj.Depthmean = calculateCurrentMeanDepth(m_DraggedObj);
                BoxManager.Instance.UpdateCurrentBox(m_DraggedObj);
                m_DragEnabled = false;
            }

        }

        private void image_MouseMove(object sender, MouseEventArgs e)
        {
            System.Windows.Point p = e.GetPosition(image);
            if (m_DragEnabled == false)
            {
                AllEnums.Direction d = isMouseOnAnyObj(p);
                if (d == AllEnums.Direction.NORTH || d == AllEnums.Direction.SOUTH)
                {
                    Cursor = Cursors.SizeNS;
                }
                else if (d == AllEnums.Direction.EAST || d == AllEnums.Direction.WEST)
                {
                    Cursor = Cursors.SizeWE;
                }
                else
                {
                    this.Cursor = Cursors.Arrow;
                }
            }
            else
            {
                newRectSize(p);
            }
        }

        private void newRectSize(System.Windows.Point p)
        {
            if (m_DragEnabled)
            {
                if (m_DragMode == AllEnums.Direction.NORTH) // North
                {
                    if (p.Y > m_DraggedObj.Y && p.Y < m_DraggedObj.Y + m_DraggedObj.Height)
                    {
                        m_DraggedObj.Height = (m_DraggedObj.Y + m_DraggedObj.Height) - ((int)p.Y);
                        m_DraggedObj.Y = (int)p.Y;
                    }

                    else if (p.Y < m_DraggedObj.Y)
                    {
                        m_DraggedObj.Height = m_DraggedObj.Y + m_DraggedObj.Height - ((int)p.Y);
                        m_DraggedObj.Y = (int)p.Y;
                    }
                }
                else if (m_DragMode == AllEnums.Direction.EAST) // East
                {
                    if (p.X > m_DraggedObj.X && p.X < m_DraggedObj.X + m_DraggedObj.Width)
                    {
                        m_DraggedObj.Width = ((int)p.X) - m_DraggedObj.X;
                    }

                    else if (p.X > m_DraggedObj.X + m_DraggedObj.Width)
                    {
                        m_DraggedObj.Width = ((int)p.X) - m_DraggedObj.X;
                    }
                }

                else if (m_DragMode == AllEnums.Direction.SOUTH) // South
                {
                    if (p.Y < m_DraggedObj.Y + m_DraggedObj.Height && p.Y > m_DraggedObj.Y)
                    {
                        m_DraggedObj.Height = ((int)p.Y) - m_DraggedObj.Y;
                    }

                    else if (p.Y > m_DraggedObj.Y + m_DraggedObj.Height)
                    {
                        m_DraggedObj.Height = ((int)p.Y) - m_DraggedObj.Y;
                    }
                }
                else if (m_DragMode == AllEnums.Direction.WEST) // West
                {
                    if (p.X > m_DraggedObj.X && p.X < m_DraggedObj.X + m_DraggedObj.Width)
                    {
                        m_DraggedObj.Width = m_DraggedObj.X + m_DraggedObj.Width - ((int)p.X);
                        m_DraggedObj.X = ((int)p.X);
                    }

                    else if (p.X < m_DraggedObj.X)
                    {
                        m_DraggedObj.Width = m_DraggedObj.X + m_DraggedObj.Width - ((int)p.X);
                        m_DraggedObj.X = ((int)p.X);
                    }
                }
            }
        }

        private AllEnums.Direction isMouseOnAnyObj(System.Windows.Point p)
        {
            foreach (Box b in BoxManager.Instance.CurrentLayout.Boxes)
            {
                AllEnums.Direction d = isMouseOnObj(p, b);
                if (d != AllEnums.Direction.NONE)
                    return d;

            }
            return AllEnums.Direction.NONE;
        }

        private AllEnums.Direction isMouseOnObj(System.Windows.Point pPoint, Box pRect)
        {
            if (pPoint.X >= pRect.X && pPoint.X <= (pRect.X + pRect.Width)
                    && pPoint.Y >= pRect.Y - (BOX_BORDERWIDTH / 2)
                    && pPoint.Y <= pRect.Y + (BOX_BORDERWIDTH / 2))
            {
                return AllEnums.Direction.NORTH;
            }
            else if (pPoint.X >= pRect.X && pPoint.X <= (pRect.X + pRect.Width)
                    && pPoint.Y >= pRect.Y + pRect.Height - (BOX_BORDERWIDTH / 2)
                && pPoint.Y <= pRect.Y + (BOX_BORDERWIDTH / 2) + pRect.Height)
            {
                return AllEnums.Direction.SOUTH;
            }
            else if (pPoint.Y >= pRect.Y && pPoint.Y <= (pRect.Y + pRect.Height)
                    && pPoint.X >= pRect.X - (BOX_BORDERWIDTH / 2)
                    && pPoint.X <= pRect.X + (BOX_BORDERWIDTH / 2))
            {
                return AllEnums.Direction.WEST;
            }
            else if (pPoint.Y >= pRect.Y && pPoint.Y <= (pRect.Y + pRect.Height)
                    && pPoint.X >= pRect.X + pRect.Width - (BOX_BORDERWIDTH / 2)
                    && pPoint.X <= pRect.X + (BOX_BORDERWIDTH / 2) + pRect.Width)
            {
                return AllEnums.Direction.EAST;
            }

            return AllEnums.Direction.NONE;
        }




        private void MenuItem_EditSelectedBox(object sender, RoutedEventArgs e)
        {
            var selectedItem = m_LstBoxBoxes.SelectedItem;
            if (selectedItem is Box)
            {
                Box b = (Box)selectedItem;

                EditBoxDialog dlg = new EditBoxDialog(b);
                dlg.ShowDialog(); // blocking
                if (dlg.wasOkay())
                {
                    Box editedBox = dlg.EditedBox;
                    BoxManager.Instance.UpdateCurrentBox(editedBox);
                }

            }


        }

        private void MenuItem_DeleteSelectedBox(object sender, RoutedEventArgs e)
        {
            var selectedItem = m_LstBoxBoxes.SelectedItem;
            if (selectedItem is Box)
            {
                Box b = (Box)selectedItem;

                BoxManager.Instance.CurrentLayout.Boxes.Remove(b);

            }
        }

        internal void DrawColorFrame(Image<Bgra, byte> pColorImage)
        {
            image.Width = pColorImage.Width;
            image.Height = pColorImage.Height;

            UtilitiesImage.ToImage(image, pColorImage);
        }

        private void MenuItem_EditCustomScene(object sender, RoutedEventArgs e)
        {

            var selectedItem = m_LstBoxBoxes.SelectedItem;
            if (selectedItem is Box)
            {
                Box b = (Box)selectedItem;

                // call editor

                if(b!= null)
                {
                    SceneItem item;
                    if (b.CustomScene == null)
                    {
                        item = b.getDrawable(false);
                        item.Touchy = true;
                        SceneEditorDialog dlg;

                        // set Editor Mode
                        StateManager.Instance.SetNewState(this, HciLab.motionEAP.InterfacesAndDataModel.AllEnums.State.EDIT);

                        SceneManager.Instance.CurrentScene.Clear();
                        SceneManager.Instance.CurrentScene.Add(item);
                        dlg = new SceneEditorDialog(b);
                        dlg.Show();


                    }
                    else
                    {
                        SceneEditorDialog dlg;

                        // set Editor Mode
                        StateManager.Instance.SetNewState(this, HciLab.motionEAP.InterfacesAndDataModel.AllEnums.State.EDIT);

                        SceneManager.Instance.CurrentScene.Clear();
                        if (b.CustomScene is Scene.Scene)
                        { 

                            Scene.Scene scene =  (Scene.Scene) b.CustomScene;
                            foreach(SceneItem itemIter in scene.Items)
                            {
                                itemIter.Touchy = true;
                            }

                            SceneManager.Instance.CurrentScene = scene;
                            dlg = new SceneEditorDialog(b);
                            dlg.Show();
                        }

                    }

                   
                }                
            }
            
        }
    }
}
