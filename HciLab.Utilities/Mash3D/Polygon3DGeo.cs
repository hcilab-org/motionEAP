// <copyright file=Polygon3DGeo.cs
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
// <date> 11/2/2016 12:25:57 PM</date>

using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace HciLab.Utilities.Mash3D
{
    public static class PolygonMash3D
    {
        public static GeometryModel3D newPolygon3D(HciLab.Utilities.Mathematics.Geometry2D.Polygon pPolygon, Color pColor, double pOffesetZ = 0)
        {
            GeometryModel3D model = new GeometryModel3D();
            if (pPolygon != null)
            {
                model.Geometry = FillPolygon(pPolygon.ToHelixToolkit(), pOffesetZ);
            }
            model.Material = new DiffuseMaterial(new SolidColorBrush(pColor)); 
            return model;
        }

        public static GeometryModel3D newPolygon3D(HelixToolkit.Wpf.Polygon pPolygon, Color pColor, double pOffesetZ = 0)
        {
            GeometryModel3D model = new GeometryModel3D();
            model.Geometry = FillPolygon(pPolygon, pOffesetZ);
            model.Material = new DiffuseMaterial(new SolidColorBrush(pColor));
            return model;
        }

        internal static MeshGeometry3D FillPolygon(HelixToolkit.Wpf.Polygon p, double pOffesetZ = 0)
        {
            List<Point3D> pts3D = new List<Point3D>();
            foreach (var point in p.Points)
            {
                pts3D.Add(new Point3D(point.X, point.Y, pOffesetZ));
            }
            HelixToolkit.Wpf.Polygon3D p3 = new HelixToolkit.Wpf.Polygon3D(pts3D);
            return FillPolygon(p3);
        }

        /// <summary>
        /// For 3D polygons
        /// </summary>
        /// <param name="pPolygon"></param>
        /// <returns></returns>
        internal static MeshGeometry3D FillPolygon(HelixToolkit.Wpf.Polygon3D pPolygon)
        {
            try
            {
                HelixToolkit.Wpf.MeshBuilder meshBuilder = new HelixToolkit.Wpf.MeshBuilder(false, false);

                HelixToolkit.Wpf.Polygon polygon = pPolygon.Flatten();

                Int32Collection triangleIndexes = HelixToolkit.Wpf.CuttingEarsTriangulator.Triangulate(polygon.Points);

                meshBuilder.Append(pPolygon.Points, triangleIndexes);
                return meshBuilder.ToMesh();
            }
            catch (System.InvalidOperationException exp)
            {
                return null;
            }
        }



        public static GeometryModel3D newPolygon3DOWN(HciLab.Utilities.Mathematics.Geometry2D.Polygon pPolygon, Color pColor, double pOffesetZ = 0)
        {
            GeometryModel3D model = new GeometryModel3D();
            if (pPolygon != null)
            {
                HelixToolkit.Wpf.MeshBuilder meshBuilder = new HelixToolkit.Wpf.MeshBuilder(false, false);
                
                Int32Collection triangleIndexes = CuttingEarsTriangulatorNEW.Triangulate(pPolygon.Points);

                meshBuilder.Append(pPolygon.ToHelixToolkit3D(pOffesetZ).Points, triangleIndexes);
                model.Geometry = meshBuilder.ToMesh();

            }
            model.Material = new DiffuseMaterial(new SolidColorBrush(pColor));
            return model;
        }


        public static HelixToolkit.Wpf.LinesVisual3D newPolygonToWireFrame(HciLab.Utilities.Mathematics.Geometry2D.Polygon pPolygon, double pZ)
        {
            
            HelixToolkit.Wpf.LinesVisual3D line = new HelixToolkit.Wpf.LinesVisual3D();
            line.Color = Colors.Gray;
            line.Thickness = 10;
            var points = pPolygon.ToHelixToolkit3D(pZ).Points;
            for (int i = 0; i < points.Count; i++)
            {
                line.Points.Add(points[i]);

                if (i == points.Count - 1)
                    line.Points.Add(points[0]);                        
                else
                    line.Points.Add(points[i + 1]);
            }

            return line;
        }

        public static HelixToolkit.Wpf.PointsVisual3D newPolygonToPoints(HciLab.Utilities.Mathematics.Geometry2D.Polygon pPolygon, double pZ)
        {
            HelixToolkit.Wpf.PointsVisual3D points = new HelixToolkit.Wpf.PointsVisual3D();
            points.Size = 20;
            points.Color = Colors.Aqua;
            points.Points = pPolygon.ToHelixToolkit3D(pZ).Points;
            return points;
        }

        
    }
}
