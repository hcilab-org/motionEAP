// <copyright file=EditPolygon.xaml.cs
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

using HciLab.Utilities.Mathematics.Core;
using HciLab.Utilities.Mathematics.Geometry2D;
using motionEAPAdmin.Frontend;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace motionEAPAdmin.GUI.Dialog
{
    /// <summary>
    /// Interaktionslogik für EditPolgyon.xaml
    /// </summary>
    public partial class EditPolgyon : Window
    {     

        public static readonly DependencyProperty PolygonProperty = DependencyProperty.Register("Polygon", typeof(Polygon), typeof(EditPolgyon));

        enum Mode
        {
            None = 0,
            MovePoint = 1
        }

        private Mode m_Mode = Mode.None;

        private int m_ModePointId = -1;

        public EditPolgyon()
        {
            InitializeComponent();

            TableWindow3D.Instance.viewDown += TableWindow3D_viewDown;
            TableWindow3D.Instance.viewUp += TableWindow3D_viewUp;
            TableWindow3D.Instance.viewMove += TableWindow3D_viewMove;
            m_ListPoints.DataContext = Polygon;
        }

        void TableWindow3D_viewDown(object pSource, Vector2 pPos, MouseButton pMouseButton)
        {
            if (Polygon == null)
                return;

            if (m_Mode == Mode.None)
            {
                if (pMouseButton == MouseButton.Left)
                {
                    if (pSource is HelixToolkit.Wpf.LinesVisual3D)
                    {
                        m_Mode = Mode.MovePoint;

                        double delta = double.MaxValue;
                        for (int i = 0; i < Polygon.Points.Count; i++)
                        {
                            Ray r;
                            if (i == Polygon.Points.Count - 1)
                                r = Ray.createByStartAndEnd(Polygon.Points[i], Polygon.Points[0]);
                            else
                                r = Ray.createByStartAndEnd(Polygon.Points[i], Polygon.Points[i + 1]);
                            double distance = DistanceMethods.Distance(pPos, r);
                            if (distance < delta)
                            {
                                delta = distance;
                                m_ModePointId = i;
                            }
                        }
                        m_ModePointId++;
                        Polygon.Points.Insert(m_ModePointId, pPos);

                    }
                    else if (pSource is HelixToolkit.Wpf.PointsVisual3D)
                    {
                        m_Mode = Mode.MovePoint;
                        double delta = double.MaxValue;
                        for (int i = 0; i < Polygon.Points.Count; i++)
                        {
                            double distance = (Polygon.Points[i] - pPos).GetLength();
                            if (distance < delta)
                            {
                                delta = distance;
                                m_ModePointId = i;
                            }
                        }
                    }
                }
            }
        }


        private void TableWindow3D_viewMove(object pSource, Vector2 pPos, MouseButton pMouseButton)
        {
            if (Polygon == null)
                return;

            if (m_Mode == Mode.MovePoint)
            {
                Polygon.Points[m_ModePointId].X = pPos.X;
                Polygon.Points[m_ModePointId].Y = pPos.Y;
            }
        }

        private void TableWindow3D_viewUp(object pSource, Vector2 pPos, MouseButton pMouseButton)
        {
            if (Polygon == null)
                return;

            if (m_Mode == Mode.MovePoint)
            {
                Polygon.Points[m_ModePointId].X = pPos.X;
                Polygon.Points[m_ModePointId].Y = pPos.Y;
                m_Mode = Mode.None;
            }
            else
            {
                if (pMouseButton == MouseButton.Right && pSource is HelixToolkit.Wpf.PointsVisual3D)
                {
                    if (Polygon.Points.Count > 3)
                    {
                        double delta = double.MaxValue;
                        for (int i = 0; i < Polygon.Points.Count; i++)
                        {
                            double distance = (Polygon.Points[i] - pPos).GetLength();
                            if (distance < delta)
                            {
                                delta = distance;
                                m_ModePointId = i;
                            }
                        }

                        Polygon.Points.RemoveAt(m_ModePointId);
                    }
                }
            }
        }
        
        public void refreshDataContext()
        {
            m_ListPoints.DataContext = Polygon;
        }

        private void MenuItem_DeletePointScene(object sender, RoutedEventArgs e)
        {
            Polygon.Points.RemoveAt(m_ListPoints.SelectedIndex);
        }

        private void m_ListPoints_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public Polygon Polygon {

            get { return (Polygon)GetValue(PolygonProperty); }
            set { SetValue(PolygonProperty, value); }
        }


        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            TableWindow3D.Instance.viewDown -= TableWindow3D_viewDown;
            TableWindow3D.Instance.viewUp -= TableWindow3D_viewUp;
            TableWindow3D.Instance.viewMove -= TableWindow3D_viewMove;
        }
    }
}
