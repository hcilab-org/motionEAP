// <copyright file=ObjectsPanel.xaml.cs
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
using HciLab.motionEAP.InterfacesAndDataModel;
using HciLab.motionEAP.InterfacesAndDataModel.Data;
using HciLab.Utilities;
using motionEAPAdmin.Backend;
using motionEAPAdmin.Backend.ObjectDetection;
using motionEAPAdmin.ContentProviders;
using motionEAPAdmin.Database;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace motionEAPAdmin.GUI
{
    /// <summary>
    /// Interaktionslogik für ObjectsPanel.xaml
    /// </summary>
    public partial class ObjectsPanel : UserControl
    {
        public static readonly int BOX_BORDERWIDTH = 5;
        public static readonly int BOX_MANUALY_INSERT_HEIGHT = 50;
        public static readonly int BOX_MANUALY_INSERT_WIDTH = 50;

        private ObjectDetectionZone m_DraggedObj = null;

        private bool m_DragEnabled = false;

        private AllEnums.Direction m_DragMode = AllEnums.Direction.NONE;

        private const int m_BORDERWIDTHObject = 5;

        private bool m_TakeScreenShotFromZone = false;

        private ObjectDetectionZone m_SelectedZone = null;

        private long m_ScreenshotTakenTimestamp = 0;
        
        public ObjectsPanel()
        {
            InitializeComponent();
            m_TopBar.DataContext = SettingsManager.Instance.Settings;
            m_ObjectsListView.DataContext = DatabaseManager.Instance;
            m_ListBoxObjects.DataContext = ObjectDetectionManager.Instance.CurrentLayout;
        }

        /// <summary>
        /// Delete object callback
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        /// 
        private void Menuitem_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (m_ObjectsListView.SelectedIndex != -1)
            {
                TrackableObject obj = (TrackableObject)m_ObjectsListView.SelectedItem;
                DatabaseManager.Instance.deleteTrackableObject(obj);
                DatabaseManager.Instance.listTrackableObject(); // refresh
                m_ObjectsListView.Items.Refresh();
            }
        }

        private void ObjectsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // check if an object is selected
            if (m_ObjectsListView.SelectedItems.Count == 1)
            {
                foreach (var item in m_ObjectsListView.SelectedItems)
                {
                    TrackableObject obj = item as TrackableObject;
                    ObjectDetectionManager.Instance.DisplayObjectEditDialog(obj);
                }
            }
        }

        //Code to be called within the main OnLoaded Method
        public void Boxes_OnLoadedObject(object sender, RoutedEventArgs er)
        {
        }

        //Code to be called within the main ProccessFrame Method
        public void Boxes_ProccessFrameObject()
        {

            if (ObjectDetectionManager.Instance.CurrentLayout != null)
            {
                if (StateManager.Instance.State == AllEnums.State.RECORD)
                {
                    ObjectDetectionManager.Instance.runPBDObjectThereCheck();
                }
            }
        }


        //Code to be called within a certain part of the main ProccessFrame Method
        public void Object_ProccessFrame_Draw(bool hasToUpdateUI, Image<Bgra, Byte> pImage)
        {
            if (m_TakeScreenShotFromZone)
            {
                if (m_SelectedZone != null)
                {
                    long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    if (now - m_ScreenshotTakenTimestamp > 200) // take picture after 200ms - frame should be gone by then
                    {
                        // crop image
                        Rectangle boundingBox = new Rectangle(m_SelectedZone.X, m_SelectedZone.Y, m_SelectedZone.Width, m_SelectedZone.Height);
                        pImage.ROI = boundingBox;

                        ObjectDetectionManager.Instance.SaveAndAddObjectToDatabase(pImage);

                        SceneManager.Instance.DisableObjectScenes = false;
                        m_TakeScreenShotFromZone = false;
                        m_SelectedZone = null;

                        CvInvoke.cvResetImageROI(pImage);
                    }
                }
            }

            // first clear all the feedback from previous frame
            SceneManager.Instance.TemporaryObjectsTextScene.Clear();

            // should we check for objects?
            if (SettingsManager.Instance.Settings.ObjectsRecognizeObject &&
                ObjectDetectionManager.Instance.CurrentLayout != null &&
                hasToUpdateUI)
            {
                // walk over all zones
                foreach (ObjectDetectionZone zone in ObjectDetectionManager.Instance.CurrentLayout.ObjectDetectionZones)
                {
                    // crop image
                    Rectangle boundingBox = new Rectangle(zone.X, zone.Y, zone.Width, zone.Height);
                    pImage.ROI = boundingBox;
                    Image<Gray, byte> grayscaleImage = pImage.Copy().Convert<Gray, byte>();
                    CvInvoke.cvResetImageROI(pImage);

                    // walk over all objects
                    foreach (TrackableObject obj in Database.DatabaseManager.Instance.Objects)
                    {
                        if (ObjectDetectionManager.Instance.RecognizeObject(obj.EmguImage, grayscaleImage))
                        {
                            // YAY we found an object
                            // trigger stuff
                            WorkflowManager.Instance.OnObjectRecognized(obj);

                            // update last seen timestamp
                            obj.LastSeenTimeStamp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                            obj.LastSeenZoneId = zone.Id;

                            // display visual feedback
                            Scene.SceneText textItem = ObjectDetectionManager.Instance.createSceneTextHeadingObjectDetectionZone(zone, obj.Name);
                            SceneManager.Instance.TemporaryObjectsTextScene.Add(textItem);

                        }
                    }
                    
                }
            }

            CvInvoke.cvResetImageROI(pImage);
            if (ObjectDetectionManager.Instance.CurrentLayout != null)
            {
                if (SettingsManager.Instance.Settings.ObjectsVisualFeedbackDisplay && (ObjectDetectionManager.Instance.CurrentLayout.ObjectDetectionZones.Count != 0))
                {
                    //write boxes
                    MCvFont font = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_SIMPLEX, 0.5, 0.5);
                    foreach (ObjectDetectionZone z in ObjectDetectionManager.Instance.CurrentLayout.ObjectDetectionZones)
                    {
                        // draw ID
                        pImage.Draw(z.Id + "", ref font, new System.Drawing.Point(z.X, z.Y), new Bgra(0,0,0,0));
                        // draw Frame
                        if (z.wasRecentlyTriggered())
                            pImage.Draw(new Rectangle(z.X, z.Y, z.Width, z.Height), new Bgra(0, 255, 255, 0), 0);
                        else
                            pImage.Draw(new Rectangle(z.X, z.Y, z.Width, z.Height), new Bgra(255, 255, 255, 0), 0);
                    }
                }

                UtilitiesImage.ToImage(image, pImage);

                if (SettingsManager.Instance.Settings.ObjectsVisualFeedbackProject)
                {
                    SceneManager.Instance.TemporaryObjectsScene.Clear();
                    // add a temporary scene for each box
                    foreach (ObjectDetectionZone z in ObjectDetectionManager.Instance.CurrentLayout.ObjectDetectionZones)
                    {
                        // false = call from the display loop
                        SceneManager.Instance.TemporaryObjectsScene.Add(ObjectDetectionManager.Instance.createSceneBoxForObjectDetectionZone(z, false));
                    }
                }
                else
                {
                    SceneManager.Instance.TemporaryObjectsScene.Clear();
                }
            }
        }

        private void initializeDrag(System.Windows.Point p)
        {

            foreach (ObjectDetectionZone o in ObjectDetectionManager.Instance.CurrentLayout.ObjectDetectionZones)
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

        private void buttonSaveBoxObjectLayout_Click(object sender, RoutedEventArgs e)
        {
            ObjectDetectionManager.Instance.CurrentLayout.LayoutName = textBoxName.Text;
            ObjectDetectionManager.Instance.saveObjectDetectionZoneLayoutToFile();
        }

        private void buttonLoadBoxObjectLayout_Click(object sender, RoutedEventArgs e)
        {

            ObjectDetectionManager.Instance.loadObjectDetectionZoneLayoutFromFile();

            // also update name
            if (ObjectDetectionManager.Instance.CurrentLayout != null)
            {
                textBoxName.Text = ObjectDetectionManager.Instance.CurrentLayout.LayoutName;
                this.m_ListBoxObjects.DataContext = ObjectDetectionManager.Instance.CurrentLayout.ObjectDetectionZones;
            }

        }

        public void refreshLayoutName()
        {
            if (textBoxName != null && (ObjectDetectionManager.Instance.CurrentLayout != null))
            {
                textBoxName.Text = ObjectDetectionManager.Instance.CurrentLayout.LayoutName;
            }
        }

        public void refreshDataContext()
        {
            this.m_ListBoxObjects.DataContext = ObjectDetectionManager.Instance.CurrentLayout;
        }

        private void image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point p = e.GetPosition(image);
            if (isMouseOnAnyObj(p) == AllEnums.Direction.NONE)
            {

                int width = 0;
                int height = 0;
                double x = e.GetPosition(image).X;
                double y = e.GetPosition(image).Y;

                if (BOX_MANUALY_INSERT_WIDTH + x < image.ActualWidth)
                {
                    width = BOX_MANUALY_INSERT_WIDTH;
                }
                else
                {
                    width = (int)(image.ActualWidth - x);
                }

                if (BOX_MANUALY_INSERT_HEIGHT + y < image.ActualHeight)
                {
                    height = BOX_MANUALY_INSERT_HEIGHT;
                }
                else
                {
                    height = (int)(image.ActualHeight - y);
                }

                ObjectDetectionManager.Instance.createObjectDetectionZoneFromFactory((int)x, (int)y, width, height);
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
                ObjectDetectionManager.Instance.UpdateCurrentObjectDetectionZone(m_DraggedObj);
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
            foreach (ObjectDetectionZone b in ObjectDetectionManager.Instance.CurrentLayout.ObjectDetectionZones)
            {
                AllEnums.Direction d = isMouseOnObj(p, b);
                if (d != AllEnums.Direction.NONE)
                    return d;

            }
            return AllEnums.Direction.NONE;
        }

        private AllEnums.Direction isMouseOnObj(System.Windows.Point pPoint, ObjectDetectionZone pRect)
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

      
        private void MenuItem_EditSelectedBoxObject(object sender, RoutedEventArgs e)
        {
            var selectedItem = m_ListBoxObjects.SelectedItem;
            if (selectedItem is ObjectDetectionZone)
            {
                ObjectDetectionZone z = (ObjectDetectionZone)selectedItem;

                EditObjectDetectionZoneDialog dlg = new EditObjectDetectionZoneDialog(z);
                dlg.ShowDialog(); // blocking
                if (dlg.wasOkay())
                {
                    ObjectDetectionZone editedZone = dlg.EditedObjectDetectionZone;
                    ObjectDetectionManager.Instance.UpdateCurrentObjectDetectionZone(editedZone);
                }

            }


        }

        private void MenuItem_DeleteSelectedObjectDetectionZone(object sender, RoutedEventArgs e)
        {
            var selectedItem = m_ListBoxObjects.SelectedItem;
            if (selectedItem is ObjectDetectionZone)
            {
                ObjectDetectionZone z = (ObjectDetectionZone)selectedItem;

                ObjectDetectionManager.Instance.CurrentLayout.ObjectDetectionZones.Remove(z);

            }
        }

        private void buttonObjectScreenShot_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = m_ListBoxObjects.SelectedItem;
            if (selectedItem is ObjectDetectionZone)
            {
                ObjectDetectionZone z = (ObjectDetectionZone)selectedItem;

                m_SelectedZone = z;
                m_TakeScreenShotFromZone = true;
                m_ScreenshotTakenTimestamp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                SceneManager.Instance.DisableObjectScenes = true;

            }
        }

        internal void Refresh()
        {
            m_ObjectsListView.Items.Refresh();
        }
    }
}
