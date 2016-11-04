// <copyright file=TableWindow3D.xaml.cs
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

using HciLab.Utilities.Mathematics.Core;
using HciLab.Utilities.Mathematics.Geometry2D;
using motionEAPAdmin.Backend;
using motionEAPAdmin.ContentProviders;
using motionEAPAdmin.Scene;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace motionEAPAdmin.Frontend
{
    /// <summary>
    /// Interaction logic for TableWindow.xaml
    /// </summary>
    public partial class TableWindow3D : Window
    {
        private static TableWindow3D m_Instance;

        private double m_XSpeed = 0;
        private double m_YSpeed = 0;
        private double m_Scalespeed = 0;
        private bool m_Zooming = false;

        private PerspectiveCamera m_Camera;

        public enum Mode
        {
            Normal = 0,
            Calibration = 1,
            PolygonEdit = 2
        }

        private Mode m_Mode = Mode.Normal;

        private ModelVisual3D m_CheckerBoardVisual;

        private ModelVisual3D m_BlackPlane = new ModelVisual3D();
        
        private Point3D m_HoveredPoint = new Point3D(0, 0, 0);

        private Point3D m_LastHoveredPoint = new Point3D(0, 0, 0);
        private System.Windows.Media.Imaging.BitmapSource m_CbImage;

        private bool isDragged = false;

        //For CameraCalibration via mouse
        private bool m_MouseMovesCamera = false;
        private bool m_MouseMovesCameraAngle = false;
        private bool m_MouseMovesCameraFOV = false;
        private System.Windows.Point m_LastHoveredPos = new System.Windows.Point(0, 0);


        public static TableWindow3D Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new TableWindow3D();
                }
                return m_Instance;
            }
        }

        public TableWindow3D()
        {
            InitializeComponent();
            
            //This serves as a transparent x,y Plane for Editor interaction
            //Geometry3D backDrop = new Geometry3D();
            //Plane is just a tiny bit set off below the z=0 plane to avoid rendering and interaction overlapping problems
            m_BlackPlane.Content = HciLab.Utilities.Mash3D.Rectangle3DGeo.Rect(-1000.0, -1000.0, 2000.0, 2000.0, System.Windows.Media.Colors.Black, -0.05);

            double w = CalibrationManager.Instance.GetProjectionArea().Width;
            double h = CalibrationManager.Instance.GetProjectionArea().Height;

            m_Camera = new PerspectiveCamera(
                SettingsManager.Instance.Settings.SettingsTable.ProjCamPosition,
                SettingsManager.Instance.Settings.SettingsTable.ProjCamLookDirection,
                new Vector3D(0, 1, 0),
                SettingsManager.Instance.Settings.SettingsTable.ProjCamFOV);
            m_Viewport.Camera = m_Camera;

            m_Projection.Positions.Clear();
            m_Projection.Positions.Add(new Point3D(0, 0, 0));
            m_Projection.Positions.Add(new Point3D(w, 0, 0));
            m_Projection.Positions.Add(new Point3D(w, h, 0));
            m_Projection.Positions.Add(new Point3D(w, h, 0));
            m_Projection.Positions.Add(new Point3D(0, h, 0));
            m_Projection.Positions.Add(new Point3D(0, 0, 0));
            
            if (ScreenManager.isSecondScreenConnected())
            {
                System.Drawing.Rectangle projectorResolution = ScreenManager.getProjectorResolution();
                Left = projectorResolution.Left;
                Top = projectorResolution.Top;
                Width = projectorResolution.Width;
                Height = projectorResolution.Height;
            }
            else
            {
                AllowsTransparency = false;
                Topmost = false;
                WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
            }

            AllowDrop = true;
            
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);

            m_CbImage = CalibrationManager.Instance.renderCheckerboard(32, 20, 1280, 800, 0, 0);
            m_CheckerBoardVisual = new ModelVisual3D();
            m_CheckerBoardVisual.Content = HciLab.Utilities.Mash3D.Image3DGeo.Image(0.0, 0.0, 690.0, 430.0, m_CbImage, -0.01);

            CalibrationManager.Instance.changedCalibrationMode += new CalibrationManager.ChangedCalibrationModeHandler(Instance_changedCalibrationMode);
            this.InvalidateVisual();
        }

        public PerspectiveCamera Camera { get; set; }

        private void ViewPort_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("SceneItem"))
            {
                string item = e.Data.GetData("SceneItem") as string;

                var pos = e.GetPosition(m_Viewport);
                var hitRes = VisualTreeHelper.HitTest(m_Viewport, pos);
                RayMeshGeometry3DHitTestResult rayMeshRes = hitRes as RayMeshGeometry3DHitTestResult;
                if (rayMeshRes != null)
                {
                    double x = (double)rayMeshRes.PointHit.X;
                    double y = (double)rayMeshRes.PointHit.Y;

                    Scene.SceneItem s = null;
                    if (item == typeof(SceneRect).ToString())
                        s = new Scene.SceneRect(x, y, 50, 50, System.Windows.Media.Color.FromRgb(0, 255, 0));       
                    else if (item == typeof(SceneText).ToString())
                        s = new Scene.SceneText(x, y, "Text", System.Windows.Media.Color.FromRgb(255, 255, 255), 10.0, new FontFamily("Arial"));
                    else if (item == typeof(SceneTextViewer).ToString())
                        s = new Scene.SceneTextViewer( x, y, 0.2, 0.2, "Text", new FontFamily("Arial"), 10.0, System.Windows.Media.Color.FromRgb(255, 255, 255));
                    else if (item == typeof(SceneCircle).ToString())
                        s = new Scene.SceneCircle(x, y, 10, 0.0, Math.PI * 2.0, System.Windows.Media.Color.FromRgb(0, 255, 0));
                    else if (item == typeof(SceneImage).ToString())
                        s = new Scene.SceneImage(x, y, 100, 100, null);
                    else if (item == typeof(SceneVideo).ToString())
                        s = new Scene.SceneVideo(x, y, 100, 100, null);
                    else if (item == typeof(ScenePolygon).ToString())
                        s = new Scene.ScenePolygon(new Polygon(new Vector2[] { new Vector2(0+x, 0+y), new Vector2(50+x, 50+y), new Vector2(50+x, y) }), System.Windows.Media.Color.FromRgb(0, 255, 0));
                    else if (item == typeof(SceneAudio).ToString())
                        s = new Scene.SceneAudio(null);

                    if (s != null)
                        SceneManager.Instance.CurrentScene.Add(s);
                    else
                        throw new NotImplementedException();
                }
            }
        }

        private void ViewPort_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("SceneItem") || sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        void Instance_changedCalibrationMode(object pSource, bool pIsInCalibrationMode, Boolean pSaveCalibration)
        {

            if (pIsInCalibrationMode == true)
            {
                //Temporarily disabled displaying of calibration Image
                //this.Content = m_CalibrattionModeImage;

                m_Mode = Mode.Calibration;
            }
            else
            {
                //TODO: Do this somewhere else (e.g. CalibrationManager)
                if (pSaveCalibration)
                {
                    SettingsManager.Instance.Settings.SettingsTable.ProjCamPosition = m_Camera.Position;
                    SettingsManager.Instance.Settings.SettingsTable.ProjCamLookDirection = m_Camera.LookDirection;
                    SettingsManager.Instance.Settings.SettingsTable.ProjCamFOV = m_Camera.FieldOfView;
                }
                m_Mode = Mode.Normal;
                this.Content = m_Viewport;
            }
            
            this.InvalidateVisual();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ScreenManager.isSecondScreenConnected())
                WindowState = WindowState.Maximized;
        }
        
        public void ShowCheckerboard() {
            m_Viewport.Children.Add(m_CheckerBoardVisual);
            m_Viewport.InvalidateVisual();
        }

        public void HideCheckerboard()
        {
            m_Viewport.Children.Remove(m_CheckerBoardVisual);
            m_Viewport.InvalidateVisual();
        }


        void CompositionTarget_Rendering(object sender, System.EventArgs e)
        {
            UpdateSceneManager();
            SceneManager.Instance.Update();

            if (CalibrationManager.Instance.IsInCalibrationMode == false)
            {
                m_Viewport.Children.Clear();
                m_Viewport.Children.Add(m_Light);

                m_Viewport.Children.Add(m_BlackPlane);

                foreach (Scene.Scene m in SceneManager.Instance.getAllScenes())
                {
                    m_Viewport.Children.Add(m);
                    foreach (Scene.SceneItem s in m.Items)
                    {
                        
                        if (s is Scene.Scene)
                            Debugger.Break();
                        if (s != null)
                            if (m_Viewport.Children.Contains(s))
                            {
                                m_Viewport.Children.Remove(s);
                            }
                            m_Viewport.Children.Add(s);

                        if (s is Scene.ScenePolygon && (s as Scene.ScenePolygon).IsInEditMode)
                        {
                            m_Viewport.Children.Add(HciLab.Utilities.Mash3D.PolygonMash3D.newPolygonToWireFrame((s as Scene.ScenePolygon).Polygon, (s as Scene.ScenePolygon).Z + 1));
                            m_Viewport.Children.Add(HciLab.Utilities.Mash3D.PolygonMash3D.newPolygonToPoints((s as Scene.ScenePolygon).Polygon, (s as Scene.ScenePolygon).Z +2));   
                        }
                    }
                }
            }
        }

        private void UpdateSceneManager()
        {
            if (SceneManager.Instance == null || SceneManager.Instance.CurrentScene == null || SceneManager.Instance.CurrentScene.SelectedItem == null)
                return;
            if (m_Zooming && m_Scalespeed != 0)
            {
                SceneManager.Instance.CurrentScene.SelectedItem.Scale += m_Scalespeed;
            }
            else if (m_XSpeed != 0 || m_YSpeed != 0)
            {
                SceneManager.Instance.CurrentScene.SelectedItem.Move(m_XSpeed, m_YSpeed);
            }
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            //UbidisplayManager.Instance.MouseDown((int)e.GetPosition(m_TableCanvas).X, (int)e.GetPosition(m_TableCanvas).Y);
        }

        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            //UbidisplayManager.Instance.MouseUp((int)e.GetPosition(m_TableCanvas).X, (int)e.GetPosition(m_TableCanvas).Y);
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            //UbidisplayManager.Instance.MouseMove((int)e.GetPosition(m_TableCanvas).X, (int)e.GetPosition(m_TableCanvas).Y);
        }

        protected override void OnTouchDown(System.Windows.Input.TouchEventArgs e)
        {
            base.OnTouchDown(e);
            //UbidisplayManager.Instance.TouchDown((int)e.GetTouchPoint(m_TableCanvas).Position.X, (int)e.GetTouchPoint(m_TableCanvas).Position.Y, e.TouchDevice.Id);
        }

        protected override void OnTouchMove(System.Windows.Input.TouchEventArgs e)
        {
            base.OnTouchMove(e);
            //UbidisplayManager.Instance.TouchMove((int)e.GetTouchPoint(m_TableCanvas).Position.X, (int)e.GetTouchPoint(m_TableCanvas).Position.Y, e.TouchDevice.Id);
        }

        protected override void OnTouchUp(System.Windows.Input.TouchEventArgs e)
        {
            base.OnTouchUp(e);
            //UbidisplayManager.Instance.TouchUp((int)e.GetTouchPoint(m_TableCanvas).Position.X, (int)e.GetTouchPoint(m_TableCanvas).Position.Y, e.TouchDevice.Id);
        }

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyDown(e);
            const double speed = 0.001f;
            const double sspeed = 0.002f;
            switch (e.Key)
            {
                case System.Windows.Input.Key.LeftShift:
                case System.Windows.Input.Key.RightShift:
                    m_Zooming = true;
                    break;
                case System.Windows.Input.Key.Up:
                    if (m_Zooming) m_Scalespeed = sspeed;
                    else m_YSpeed = -speed;
                    break;
                case System.Windows.Input.Key.Down:
                    if (m_Zooming) m_Scalespeed = -sspeed;
                    else m_YSpeed = speed;
                    break;
                case System.Windows.Input.Key.Left:
                    if (m_Zooming) m_Scalespeed = -sspeed;
                    else m_XSpeed = -speed;
                    break;
                case System.Windows.Input.Key.Right:
                    if (m_Zooming) m_Scalespeed = sspeed;
                    else m_XSpeed = speed;
                    break;
            }
        }

        protected override void OnKeyUp(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyUp(e);
            switch (e.Key)
            {
                case System.Windows.Input.Key.LeftShift:
                case System.Windows.Input.Key.RightShift:
                    m_Zooming = false;
                    break;
                case System.Windows.Input.Key.Up:
                case System.Windows.Input.Key.Down:
                    if (m_Zooming) m_Scalespeed = 0;
                    else m_YSpeed = 0;
                    break;
                case System.Windows.Input.Key.Left:
                case System.Windows.Input.Key.Right:
                    if (m_Zooming) m_Scalespeed = 0;
                    else m_XSpeed = 0;
                    break;
            }
        }

        private void OnViewportMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            System.Windows.Point pos = e.GetPosition(m_Viewport);
            var hitRes = VisualTreeHelper.HitTest(m_Viewport, pos);
            RayMeshGeometry3DHitTestResult rayMeshRes = hitRes as RayMeshGeometry3DHitTestResult;
            if (rayMeshRes != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    OnViewMove(rayMeshRes.VisualHit, new Vector2(rayMeshRes.PointHit.X, rayMeshRes.PointHit.Y), MouseButton.Left);
                else if (e.RightButton == MouseButtonState.Pressed)
                    OnViewMove(rayMeshRes.VisualHit, new Vector2(rayMeshRes.PointHit.X, rayMeshRes.PointHit.Y), MouseButton.Right);
                else if (e.MiddleButton == MouseButtonState.Pressed)
                    OnViewMove(rayMeshRes.VisualHit, new Vector2(rayMeshRes.PointHit.X, rayMeshRes.PointHit.Y), MouseButton.Middle);
            }

            double factor;

            if (m_Mode == Mode.Calibration)
            {
                double xDelta = pos.X - m_LastHoveredPos.X;
                double yDelta = pos.Y - m_LastHoveredPos.Y;
                m_LastHoveredPos = pos;

                if (m_MouseMovesCamera)
                {
                    factor = 0.1;
                    var cPos = m_Camera.Position;
                    var newCamPos = new Point3D(cPos.X - factor * xDelta, cPos.Y - factor * yDelta, cPos.Z);
                    m_Camera.Position = newCamPos;
                    SettingsManager.Instance.Settings.SettingsTable.ProjCamPosition = newCamPos;
                }
                if (m_MouseMovesCameraAngle)
                {
                    factor = 0.001;
                    var cAngle = m_Camera.LookDirection;
                    var newCamAngle = new Vector3D(cAngle.X, cAngle.Y - factor * yDelta, cAngle.Z);
                    m_Camera.LookDirection = newCamAngle;
                    SettingsManager.Instance.Settings.SettingsTable.ProjCamLookDirection = newCamAngle;
                }
                if (m_MouseMovesCameraFOV)
                {
                    factor = 0.01;
                    double newFOV = m_Camera.FieldOfView + factor * xDelta;
                    m_Camera.FieldOfView = newFOV;
                    SettingsManager.Instance.Settings.SettingsTable.ProjCamFOV = newFOV;
                }

            }
            else
            {
                if (rayMeshRes != null)
                    m_HoveredPoint = rayMeshRes.PointHit;
                if (hitRes != null)
                {
                    if (hitRes.VisualHit is SceneItem)
                    {
                        if ((hitRes.VisualHit as SceneItem).Touchy == true)
                            Mouse.OverrideCursor = Cursors.SizeAll;
                        else
                            Mouse.OverrideCursor = Cursors.No;
                    }
                    else
                    {
                        Mouse.OverrideCursor = null;
                    }
                }

                if (SceneManager.Instance.CurrentScene.SelectedItem != null  && isDragged == true)
                {
                    if (SceneManager.Instance.CurrentScene.SelectedItem is ScenePolygon)
                        (SceneManager.Instance.CurrentScene.SelectedItem as ScenePolygon).Move(m_HoveredPoint.X - m_LastHoveredPoint.X, m_HoveredPoint.Y - m_LastHoveredPoint.Y);
                    else
                        SceneManager.Instance.CurrentScene.SelectedItem.Move(m_HoveredPoint.X - m_LastHoveredPoint.X, m_HoveredPoint.Y - m_LastHoveredPoint.Y);
                    m_LastHoveredPoint = m_HoveredPoint;
                }
            }

        }

        private void OnViewportMouseDown(object pSender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Windows.Point pos = e.GetPosition(m_Viewport);
            var hitRes = VisualTreeHelper.HitTest(m_Viewport, pos);
            RayMeshGeometry3DHitTestResult rayMeshRes = hitRes as RayMeshGeometry3DHitTestResult;
            if (rayMeshRes != null)
            {
                OnViewDown(rayMeshRes.VisualHit, new Vector2(rayMeshRes.PointHit.X, rayMeshRes.PointHit.Y), e.ChangedButton);
            }
            

            if (m_Mode == Mode.Calibration)
            {
                m_LastHoveredPos = pos;
                if (e.LeftButton == MouseButtonState.Pressed) m_MouseMovesCamera = true;
                else if (e.RightButton == MouseButtonState.Pressed) m_MouseMovesCameraAngle = true;
                else if (e.MiddleButton == MouseButtonState.Pressed) m_MouseMovesCameraFOV = true;
            }
            else
            {
                if (hitRes != null)
                {
                    if (hitRes.VisualHit is SceneItem && (hitRes.VisualHit as SceneItem).Touchy == true)
                    {
                        SceneItem s = hitRes.VisualHit as SceneItem;
                        SceneManager.Instance.CurrentScene.SelectedItem = s;
                        isDragged = true;
                        BackendControl.Instance.refreshGUI();
                    }

                    m_LastHoveredPoint = m_HoveredPoint;
                }
            }

        }

        private void OnViewportMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            System.Windows.Point pos = e.GetPosition(m_Viewport);
            var hitRes = VisualTreeHelper.HitTest(m_Viewport, pos);
            RayMeshGeometry3DHitTestResult rayMeshRes = hitRes as RayMeshGeometry3DHitTestResult;
            if (rayMeshRes != null)
            {
                OnViewUp(rayMeshRes.VisualHit, new Vector2(rayMeshRes.PointHit.X, rayMeshRes.PointHit.Y), e.ChangedButton);
            }

            if (m_Mode == Mode.Calibration)
            {
                if (e.LeftButton == MouseButtonState.Released) m_MouseMovesCamera = false;
                if (e.RightButton == MouseButtonState.Released) m_MouseMovesCameraAngle = false;
                if (e.MiddleButton == MouseButtonState.Released) m_MouseMovesCameraFOV = false;
            }
            else
            {
                isDragged = false;
                //SceneManager.Instance.CurrentScene.EditItem = null;
            }
        }

        private void OnViewPortMouseEnter(object sender, MouseEventArgs e)
        {
            
        }

        private void OnViewPortMouseLeave(object sender, MouseEventArgs e)
        {
            //SceneManager.Instance.CurrentScene.EditItem = null;
            Mouse.OverrideCursor = null;
            isDragged = false;
        }

        private void OnViewportMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (m_Mode == Mode.Calibration)
            {
                var cPos = m_Camera.Position;
                var newCamPos = new Point3D(cPos.X, cPos.Y, cPos.Z - (e.Delta * 0.05));
                m_Camera.Position = newCamPos;
                SettingsManager.Instance.Settings.SettingsTable.ProjCamPosition = newCamPos;
            }
        }


        public delegate void ViewDownHandler(object pSource, Vector2 pPos, MouseButton pMouseButton);

        public event ViewDownHandler viewDown;

        public void OnViewDown(object pSource, Vector2 pPos, MouseButton pMouseButton)
        {
            if (this.viewDown != null)
                viewDown(pSource, pPos, pMouseButton);
        }

        public delegate void ViewUpHandler(object pSource, Vector2 pPos, MouseButton pMouseButton);

        public event ViewUpHandler viewUp;

        public void OnViewUp(object pSource, Vector2 pPos, MouseButton pMouseButton)
        {
            if (this.viewUp != null)
                viewUp(pSource, pPos, pMouseButton);
        }


        public delegate void ViewMoveHandler(object pSource, Vector2 pPos, MouseButton pMouseButton);

        public event ViewMoveHandler viewMove;

        public void OnViewMove(object pSource, Vector2 pPos, MouseButton pMouseButton)
        {
            if (this.viewMove != null)
                viewMove(pSource, pPos, pMouseButton);
        }
    }
}
