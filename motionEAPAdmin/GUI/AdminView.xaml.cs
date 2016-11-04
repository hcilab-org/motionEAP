// <copyright file=AdminView.xaml.cs
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
using HelixToolkit.Wpf;
using motionEAPAdmin.Backend;
using motionEAPAdmin.Backend.AssembleyZones;
using motionEAPAdmin.Backend.Boxes;
using motionEAPAdmin.Backend.CameraManager;
using motionEAPAdmin.ContentProviders;
using motionEAPAdmin.GUI.Dialog;
using motionEAPAdmin.Localization;
using motionEAPAdmin.Model.Process;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace motionEAPAdmin.GUI
{
    /// <summary>
    /// Interaction logic for AdminView.xaml
    /// </summary>
    public partial class AdminView : Window
    {
        private static AdminView m_Instance;

        private object m_ProcessFrameLock = new object();

        private bool m_IsSendingUPDData;

        /// <summary>
        /// Singleton Constructor
        /// </summary>
        public static AdminView Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new AdminView();
                }
                return m_Instance;
            }
        }

        //do start the record of 3D gesture template with the return key
        //
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            /*if (e.Key == Key.Return)
            {
                m_GUI_GestureDebugPanel.GestureTabRecording3D();
            }*/
        }


        public AdminView()
        {
            InitializeComponent();
            this.DataContext = new MainScreenViewModel();
            // set handles and initialize databinding
            Logger.Instance.ViewHandle = this;
            m_Instance = this;

            // Instantiate settings
            ResourceManagerService.ChangeLocale(SettingsManager.Instance.Settings.Language);

            Closing += AdminView_Closing;
            this.Loaded += new RoutedEventHandler(OnLoaded);
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);

            //Refresh Workflow UI if Workingstep gets changed automatically
            WorkflowManager.WorkingstepChanged += AdminView.Instance.refreshWorkflowUI;

            DebugInformationManager.Instance.start();

            //KinectManager.Instance.allFramesReady += new KinectManager.AllFramesReadyHandler(Instance_allFramesReady);
            CameraManager.Instance.OnAllFramesReady += Instance_allFramesReady;
            USBCameraDetector.UpdateConnectedUSBCameras();
        }


        void CompositionTarget_Rendering(object sender, System.EventArgs e)
        {
            InvalidateVisual();
        }


        void AdminView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            BackendControl.Instance.shoutDownApplication();
        }


        /// <summary>
        /// Displays the ConfigurationDialog
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">RoutedEventArgs</param>
        /// 
        /// 
        private void configureTableItem_Click(object sender, RoutedEventArgs e)
        {
            CalibrationDialog dlg = new CalibrationDialog();
            dlg.Show(); // non blocking
        }

        /// <summary>
        /// Displays the ConfigurationDialog
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">RoutedEventArgs</param>
        /// 
        /// 
        private void configureKinectItem_Click(object sender, RoutedEventArgs e)
        {
            CalibrateKinectDialog dlg = new CalibrateKinectDialog(SettingsManager.Instance.Settings.SettingsKinect);
            if (dlg.ShowDialog() == true)
            {
                SettingsManager.Instance.Settings.SettingsKinect = dlg.getSettings();
            }
        }

        private void drawTableBoundaries()
        {
            MeshBuilder b = new MeshBuilder();

            // the bottom plate
            b.AddBox(new Point3D(5, 0, -10.25), 10.5, 0.5, 0.5);
            b.AddBox(new Point3D(5, 0, 0.25), 10.5, 0.5, 0.5);
            b.AddBox(new Point3D(0, 0, -5), 0.5, 0.5, 10.5);

            // table neck
            b.AddBox(new Point3D(0, 5, -5), 0.5, 10, 0.5);

            MeshGeometry3D geo = b.ToMesh();
            Material material = new DiffuseMaterial(new SolidColorBrush(Colors.Gray));
            GeometryModel3D triangleModel = new GeometryModel3D(
                geo,
                material);
            ModelVisual3D model = new ModelVisual3D();
            ModelVisual3D model2 = new ModelVisual3D();
            model.Content = triangleModel;
            model2.Content = triangleModel;

            m_GUI_Visualization.view1.Children.Add(model);
            //m_GUI_GestureDebugPanel.view2.Children.Add(model2);

            //Test
            MeshBuilder c = new MeshBuilder();

            // camera
            var camPos = SettingsManager.Instance.Settings.SettingsTable.ProjCamPosition.Multiply(0.03);
            c.AddBox(new Point3D(camPos.Y, camPos.Z, -camPos.X), 0.5, 0.5, 0.5);

            MeshGeometry3D cGeo = c.ToMesh();
            Material cMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Red));
            ModelVisual3D cModel = new ModelVisual3D();
            GeometryModel3D cTriangleModel = new GeometryModel3D(cGeo, cMaterial);
            cModel.Content = cTriangleModel;
            m_GUI_Visualization.view1.Children.Add(cModel);


        }

        /// <summary>
        /// This is what happens when the window is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="er"></param>
        private void OnLoaded(object sender, RoutedEventArgs er)
        {
            drawTableBoundaries();
            m_GUI_BoxesPanel.Boxes_OnLoaded(sender, er);
            m_GUI_ObjectsPanel.Boxes_OnLoadedObject(sender, er);
            m_GUI_AssemblyPanel.AssemblyZones_OnLoaded(sender, er);
        }

        void Instance_allFramesReady(object pSource,
            Image<Bgra, byte> pColorImage, Image<Bgra, byte> pColorImageCropped,
            Image<Gray, Int32> pDepthImage, Image<Gray, Int32> pDepthImageCropped)
        {
            Dispatcher.Invoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    new Action(
                        () =>
                        {
                            ProcessFrame(pColorImage, pColorImageCropped, pDepthImage, pDepthImageCropped);
                        }
                    )
                );
        }

        public void ProcessFrame(
            Image<Bgra, byte> pColorImage, Image<Bgra, byte> pColorImageCropped,
            Image<Gray, Int32> pDepthImage, Image<Gray, Int32> pDepthImageCropped)
        {
            if (tabControl1.SelectedItem.Equals(VideoItem))
                m_GUI_Video.ProcessFrame(pColorImage, pColorImageCropped, pDepthImage, pDepthImageCropped);

            m_GUI_ObjectsPanel.Boxes_ProccessFrameObject();
            m_GUI_AssemblyPanel.AssemblyZones_ProccessFrame();


            if (tabControl1.SelectedItem.Equals(tabItemAssemblyZones))
            {
                if (SettingsManager.Instance.Settings.AssemblyZoneVisualFeedbackDisplay) //m_GUI_AssemblyPanel.zones_checkBoxVisualFeedback.IsChecked.Value)
                    pColorImage = AssemblyZoneManager.Instance.drawAdminUI(pColorImage);


                m_GUI_AssemblyPanel.DrawColorFrame(pColorImage);

            }

            if (tabControl1.SelectedItem.Equals(tabItemBoxes))
            {
                if (SettingsManager.Instance.Settings.BoxesVisualFeedbackDisplay)
                    pColorImage = BoxManager.Instance.drawAdminUI(pColorImage);

                m_GUI_BoxesPanel.DrawColorFrame(pColorImage);
            }

            m_GUI_ObjectsPanel.Object_ProccessFrame_Draw(tabControl1.SelectedItem.Equals(tabItemObjects), pColorImage);
            m_GUI_PBDPanel.checkIfUserIsWorking(pColorImageCropped);

            QRDetectManager.Instance.SimpleScan(pColorImageCropped);
        }

        private static T FindAnchestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }



        private void editWorkFlowItem_Click(object sender, RoutedEventArgs e)
        {
            WorkflowEditor edit = new WorkflowEditor();
            edit.Show();

        }

        public void refreshDataContext()
        {
            m_GUI_BoxesPanel.Boxes_refreshDataContext();
            m_GUI_AssemblyPanel.refreshDataContext();
            m_GUI_ObjectsPanel.refreshDataContext();
            m_GUI_PBDPanel.refreshDataContext();
            //m_GUI_Editor.refreshDataBinding();

            if (EditWorkflowManager.Instance.EditorGUIHandle != null)
            {
                EditWorkflowManager.Instance.EditorGUIHandle.refreshDataBinding();
            }
        }




        /// <summary>
        /// Displays the Load workflow dialog
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">RoutedEventArgs</param>
        /// 
        /// 
        private void loadWorkFlowItem_Click(object sender, RoutedEventArgs e)
        {
            if (WorkflowManager.Instance.loadWorkflow())
            {
                SettingsManager.Instance.Settings.BoxesVisualFeedbackDisplay = false;
                //WorkflowManager.Instance.startWorkflow();
                refreshWorkflowUI();
            }
        }

        /// <summary>
        /// This method refreshes the Workflow UI according to the currently loaded workflow
        /// </summary>
        /// 
        /// 
        public void refreshWorkflowUI()
        {
            if (WorkflowManager.Instance.LoadedWorkflow != null)
            {
                // clear previous items
                this.m_GUI_Workflow.listViewWorkingStepCarrier.Items.Clear();
                // add new items
                int i = 0;
                foreach (WorkingStep step in WorkflowManager.Instance.LoadedWorkflow.WorkingSteps)
                {
                    ListViewItem item = new ListViewItem();
                    string fullName = step.Name;
                    item.Content = fullName;

                    if (i < WorkflowManager.Instance.CurrentWorkingStepNumber)
                    {
                        item.Background = System.Windows.Media.Brushes.Green;
                    }
                    else
                    {
                        item.Background = System.Windows.Media.Brushes.Red;
                    }

                    this.m_GUI_Workflow.listViewWorkingStepCarrier.Items.Add(item);
                    i++;
                }
            }
        }

        /// <summary>
        /// Listener for language switching
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void buttonSwitchLanguageEnglish(object sender, RoutedEventArgs e)
        {
            ResourceManagerService.ChangeLocale("");
            SettingsManager.Instance.Settings.Language = "";
        }

        public bool IsSendingUPDData
        {
            get { return m_IsSendingUPDData; }
            set { m_IsSendingUPDData = value; }
        }

        /// <summary>
        /// Listener for language switching
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void buttonSwitchLanguageGerman(object sender, RoutedEventArgs e)
        {
            ResourceManagerService.ChangeLocale("de-DE");
            SettingsManager.Instance.Settings.Language = "de-DE";
        }

        public Point3D GetVisualizationCamaraPosition()
        {
            return m_GUI_Visualization.view1.Camera.Position;
        }

        public Vector3D GetVisualizationCamaraLookDirection()
        {
            return m_GUI_Visualization.view1.Camera.LookDirection;
        }

        internal void ObjectsListViewRefresh()
        {
            m_GUI_ObjectsPanel.Refresh();
        }

        internal void refreshLayoutName()
        {
            m_GUI_BoxesPanel.refreshLayoutName();
            m_GUI_AssemblyPanel.refreshLayoutName();
            m_GUI_ObjectsPanel.refreshLayoutName();
        }

        private void buttonSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsDialog dlg = new SettingsDialog();
            dlg.ShowDialog();
        }

        private void about_Click(object sender, RoutedEventArgs e)
        {
            AboutDialog dlg = new AboutDialog();
            dlg.Left = this.Left + (this.Width / 2) - (dlg.Width / 2);
            dlg.Top = this.Top + (this.Height / 2) - (dlg.Height / 2);
            dlg.ShowDialog();
        }

        /// <summary>
        /// Do cleanup of everything what can not be easily handled in sub windows or classes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }

        public bool IsEnsensoActive
        {
            get
            {
                if (m_GUI_Video.m_ComboBoxSelectedCameras.SelectedItem.ToString().Equals(ProjectConstants.ENSENSON10DESCRIPTION))
                {
                    return true;
                }
                return false;
            }
        }

        public bool IsKinectActive
        {
            get
            {
                if (m_GUI_Video.m_ComboBoxSelectedCameras.SelectedItem.ToString().Equals(ProjectConstants.KINECTV2DESCRIPTION))
                {
                    return true;
                }
                return false;
            }
        }

        public bool IsStructureSensorActive
        {
            get
            {
                if (m_GUI_Video.m_ComboBoxSelectedCameras.SelectedItem.ToString().Equals(ProjectConstants.STRUCTURESENSORDESCRIPTION))
                {
                    return true;
                }
                return false;
            }
        }
    }
}
    
