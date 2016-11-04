// <copyright file=AssemblyZonesPanel.xaml.cs
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
using motionEAPAdmin.Backend.AssembleyZones;
using motionEAPAdmin.ContentProviders;
using motionEAPAdmin.GUI.Dialog;
using motionEAPAdmin.Scene;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace motionEAPAdmin.GUI
{
    /// <summary>
    /// Interaktionslogik für AssemblyZonesPanel.xaml
    /// </summary>
    public partial class AssemblyZonesPanel : UserControl
    {
        public static readonly int BOX_BORDERWIDTH = 15;
        public static readonly int ASSEMBLYZONE_BORDERWIDTH = 5;
        public static readonly int MANUAL_ZONE_HEIGHT = 50;
        public static readonly int MANUAL_ZONE_WIDTH = 50;

        /// <summary>
        /// 
        /// </summary>
        private AssemblyZone m_DraggedObj = null;

        /// <summary>
        /// 
        /// </summary>
        private bool m_DragEnabled = false;

        /// <summary>
        /// 0 = north / 1 = East / 2 = south / 3 = west
        /// </summary>
        private AllEnums.Direction m_DragMode = AllEnums.Direction.NONE;

        public AssemblyZonesPanel()
        {
            InitializeComponent();
            this.m_ListBoxAssemblyZoneWuerfel.DataContext = AssemblyZoneManager.Instance.CurrentLayout;

            m_TopBar.DataContext = SettingsManager.Instance.Settings;
            m_ButtomBar.DataContext = SettingsManager.Instance.Settings;
        }

        /// <summary>
        /// Code to be called within the main OnLoaded Method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="er"></param>
        public void AssemblyZones_OnLoaded(object sender, RoutedEventArgs er)
        {
        }

        /// <summary>
        /// Code to be called within the main ProccessFrame Method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AssemblyZones_ProccessFrame()
        {
            if (AssemblyZoneManager.Instance.CurrentLayout != null)
            {
                foreach (AssemblyZone z in AssemblyZoneManager.Instance.CurrentLayout.AssemblyZones)
                {
                    double percentage = getPercentageMatchingDepthArray(z);
                    z.LastPercentageMatched = percentage;

                    double matchPercentage = z.MatchPercentage / 100.0;
                    if (percentage > matchPercentage) // arbitrary Percentage value
                    {
                        z.Trigger();
                    }
                    else
                    {
                        z.TriggerNoMatch();
                    }
                }
            }
        }


        private void initializeDrag(System.Windows.Point p)
        {
            foreach (AssemblyZone b in AssemblyZoneManager.Instance.CurrentLayout.AssemblyZones)
            {
                if(b.IsSelected)
                { 
                    m_DragMode = isMouseOnObj(p, b);
                    if (m_DragMode != AllEnums.Direction.NONE)
                    {
                        m_DraggedObj = b;
                        m_DragEnabled = true;
                        return;
                    }
                }
            }
            m_DragEnabled = false;
        }

        /// <summary>
        /// Manual create a new AssemblyZone by clicking on to the GUI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createANewObj(object sender, MouseButtonEventArgs e)
        {
            Image<Gray, Int32> img = HciLab.Kinect.CameraManager.Instance.DepthImage;
            if (img != null)
            {
                // define a new zone
                AssemblyZone z = AssemblyZoneManager.Instance.createAssemblyZoneFromFactory();
                z.X = (int)e.GetPosition(m_Image).X;
                z.Y = (int)e.GetPosition(m_Image).Y;

                z.Z = img.Data[z.Y,z.X,0];
                if (z.Z == 0)
                {
                    AssemblyZoneManager.Instance.decreaseIDByOne();
                    return;
                }

                // check if box is bigger than depth frame
                if (MANUAL_ZONE_WIDTH + z.X < img.Width)
                {
                    z.Width = MANUAL_ZONE_WIDTH;
                }
                else
                {
                    z.Width = img.Width - z.X;
                }

                if (MANUAL_ZONE_HEIGHT + z.Y < img.Height)
                {
                    z.Height = MANUAL_ZONE_HEIGHT;
                }
                else
                {
                    z.Height = img.Height - z.Y;
                }
                
                z.DepthArray = AssemblyZoneManager.getDepthArrayFromAssemblyZone(z);
                Resources["checkBoxCollection"] = null;
                AssemblyZoneManager.Instance.CurrentLayout.AssemblyZones.Add(z);
                Resources["checkBoxCollection"] = AssemblyZoneManager.Instance.CurrentLayout.AssemblyZones;
            }
        }

        private float getPercentageMatchingDepthArray(AssemblyZone pZone)
        {
            int LowerTolerance = SettingsManager.Instance.Settings.AssemblyZonesInputMatchTolerance;
            int UpperTolerance = SettingsManager.Instance.Settings.AssemblyZonesInputMatchTolerance;

            Image<Gray, Int32> img = HciLab.Kinect.CameraManager.Instance.DepthImage;

            if (pZone.DepthArray.GetLength(0) != pZone.Width || pZone.DepthArray.GetLength(1) != pZone.Height)
                return -1.0f;

            int sum = 0;
            int within = 0;
            for (int x = pZone.X; x < pZone.X + pZone.Width; x++)
            {
                for (int y = pZone.Y; y < pZone.Y + pZone.Height; y++)
                {
                    int depthval = img.Data[y, x, 0];
                    int matchval = pZone.DepthArray[x - pZone.X, y - pZone.Y];

                    int real_depth = depthval;
                    int real_matchval = matchval;

                    if (AdminView.Instance.IsKinectActive)
                    {
                        real_depth = depthval / KinectManager.SCALE_FACTOR;
                        real_matchval = matchval / KinectManager.SCALE_FACTOR;
                    }

                    if (real_matchval != 0 && real_depth != 0 && (!pZone.UseDepthmask || pZone.DepthMask[x - pZone.X, y - pZone.Y]))
                    //Skip zero values (== invalid values) // skip depth mask values
                    {
                        sum = sum + 1;
                        if ((real_depth < (real_matchval + UpperTolerance)) && (real_depth > (real_matchval - LowerTolerance)))
                        {
                            within = within + 1;
                        }
                    }
                }
            }
            float percentage = 0.0f;
            if (sum != 0)
            {
                percentage = ((float)within) / ((float)sum);
            }
            return percentage;
        }



        private float calculateCurrentMeanDepth(AssemblyZone pZone)
        {
            Image<Gray, Int32> img = HciLab.Kinect.CameraManager.Instance.DepthImage;
            int count = 0;
            long sum = 0;
            for (int x = pZone.X; x < pZone.Width; x++)
            {
                for (int y = pZone.Y; y < pZone.Height; y++)
                {
                    int depthval = img.Data[y, x, 0];
                    int real_depthval = depthval;

                    if (AdminView.Instance.IsKinectActive)
                    {
                        real_depthval = depthval / KinectManager.SCALE_FACTOR;
                    }

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
                return sum / count;
            }
        }

        private void buttonSaveAssemblyZoneLayout_Click(object sender, RoutedEventArgs e)
        {
            AssemblyZoneManager.Instance.CurrentLayout.LayoutName = this.m_TextBoxLayoutName.Text;
            AssemblyZoneManager.Instance.saveAssemblyZoneLayoutToFile();
        }

        private void buttonLoadAssemblyZoneLayout_Click(object sender, RoutedEventArgs e)
        {
            AssemblyZoneManager.Instance.loadAssemblyZoneLayoutFromFile();

            // also update name
            if (AssemblyZoneManager.Instance.CurrentLayout != null)
            {
                m_TextBoxLayoutName.Text = AssemblyZoneManager.Instance.CurrentLayout.LayoutName;
            }
        }

        private void buttonAssemblyZoneTakeSnapshot_Click(object sender, RoutedEventArgs e)
        {
            AssemblyZoneManager.Instance.createDepthSnapshot();
        }

        private void buttonAssemblyZoneDetectZone_Click(object sender, RoutedEventArgs e)
        {
            AssemblyZone z = AssemblyZoneManager.Instance.createAssemblyZoneFromChanges();
            if (z != null)
            {
                // Set collection to null to force refresh
                Resources["checkBoxCollection"] = null;
                AssemblyZoneManager.Instance.CurrentLayout.AssemblyZones.Add(z);
                // Refresh checkBoxCollection in XAML
                Resources["checkBoxCollection"] = AssemblyZoneManager.Instance.CurrentLayout.AssemblyZones;
            }
        }

        public void refreshLayoutName()
        {
            if (AssemblyZoneManager.Instance.CurrentLayout != null)
            {
                m_TextBoxLayoutName.Text = AssemblyZoneManager.Instance.CurrentLayout.LayoutName;
            }
        }

        public void refreshDataContext()
        {
            this.m_ListBoxAssemblyZoneWuerfel.DataContext = AssemblyZoneManager.Instance.CurrentLayout;
            Resources["checkBoxCollection"] = AssemblyZoneManager.Instance.CurrentLayout.AssemblyZones;
        }

        private void image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point p = e.GetPosition(m_Image);
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
                m_DraggedObj.DepthArray = AssemblyZoneManager.getDepthArrayFromAssemblyZone(m_DraggedObj);
                AssemblyZoneManager.Instance.updateCurrentAssemblyZone(m_DraggedObj);
                m_DragEnabled = false;
            }
        }

        private void image_MouseMove(object sender, MouseEventArgs e)
        {
            System.Windows.Point p = e.GetPosition(m_Image);
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
            foreach (AssemblyZone b in AssemblyZoneManager.Instance.CurrentLayout.AssemblyZones)
            {
                if(b.IsSelected)
                { 
                    AllEnums.Direction d = isMouseOnObj(p, b);
                    if (d != AllEnums.Direction.NONE)
                        return d;
                }
            }
            return AllEnums.Direction.NONE;
        }

        private AllEnums.Direction isMouseOnObj(System.Windows.Point pPoint, AssemblyZone pRect)
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

        private void MenuItem_EditSelectedAssemblyZone(object sender, RoutedEventArgs e)
        {
            var selectedItem = m_ListBoxAssemblyZoneWuerfel.SelectedItem;
            if (selectedItem is AssemblyZone)
            {
                AssemblyZone b = (AssemblyZone)selectedItem;

                EditAssemblyZoneDialog dlg = new EditAssemblyZoneDialog(b);
                dlg.ShowDialog(); // blocking
                if (dlg.wasOkay())
                {
                    Resources["checkBoxCollection"] = null;
                    AssemblyZone editedAssemblyZone = dlg.EditedAssemblyZone;
                    AssemblyZoneManager.Instance.updateCurrentAssemblyZone(editedAssemblyZone);
                    Resources["checkBoxCollection"] = AssemblyZoneManager.Instance.CurrentLayout.AssemblyZones;
                }
            }
        }

        /// <summary>
        /// Listener which handles the deletion of one or multiple selected assemblyzones
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_DeleteSelectedAssemblyZone(object sender, RoutedEventArgs e)
        {
            // Get list with selected items. This list contains only elements which are selected
            IList selectedItems = m_ListBoxAssemblyZoneWuerfel.SelectedItems;
            int amountOfSelectedItems = selectedItems.Count;
            for (int i = 0; i < amountOfSelectedItems; i++)
            {
                if (selectedItems.Count > 0)
                {
                    if (m_ListBoxAssemblyZoneWuerfel.Items.Contains(selectedItems[0]))
                    {
                        AssemblyZone b = (AssemblyZone)selectedItems[0];
                        AssemblyZoneManager.Instance.CurrentLayout.AssemblyZones.Remove(b);
                    }
                }
            }
        }
        
        internal void DrawColorFrame(Image<Bgra, byte> pColorImage)
        {
            m_Image.Width = pColorImage.Width;
            m_Image.Height = pColorImage.Height;

            UtilitiesImage.ToImage(m_Image, pColorImage);
        }

        /// <summary>
        /// Sets all checkboxes to true
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSelectAll_Clicked(object sender, RoutedEventArgs e)
        {
            Resources["checkBoxCollection"] = null;
            foreach (AssemblyZone zone in AssemblyZoneManager.Instance.CurrentLayout.AssemblyZones)
            {
                zone.IsSelected = true;
            }
            Resources["checkBoxCollection"] = AssemblyZoneManager.Instance.CurrentLayout.AssemblyZones;
        }

        /// <summary>
        /// Sets all checkboxes to false
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonDeselectAll_Clicked(object sender, RoutedEventArgs e)
        {
            Resources["checkBoxCollection"] = null;
            foreach (AssemblyZone zone in AssemblyZoneManager.Instance.CurrentLayout.AssemblyZones)
            {
                zone.IsSelected = false;
            }
            Resources["checkBoxCollection"] = AssemblyZoneManager.Instance.CurrentLayout.AssemblyZones;
        }

        private void MenuItem_EditCustomScene(object sender, RoutedEventArgs e)
        {

            var selectedItem = m_ListBoxAssemblyZoneWuerfel.SelectedItem;
            if (selectedItem is AssemblyZone)
            {
                AssemblyZone b = (AssemblyZone)selectedItem;

                // call editor

                if (b != null)
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

                            Scene.Scene scene = (Scene.Scene)b.CustomScene;
                            foreach (SceneItem itemIter in scene.Items)
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
