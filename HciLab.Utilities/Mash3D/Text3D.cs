// <copyright file=Text3D.cs
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

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace HciLab.Utilities.Mash3D
{
    public static class Text3DGeo
    {
        /// <summary>
        /// simplifed version for use with SceneText
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <param name="pText"></param>
        /// <param name="pTextColor"></param>
        /// <param name="pFontSize"></param>
        /// <param name="pFontFamily"></param>
        /// <param name="pOffsetZ"></param>
        /// <returns></returns>
        public static GeometryModel3D CreateTextLabel(double pX, double pY, String pText, Color pTextColor, double pFontSize, FontFamily pFontFamily, double pOffsetZ = 0)
        {
            return CreateTextLabel(pText, pTextColor, pFontSize, new Point3D(pX, pY, pOffsetZ), new Vector3D(1, 0, pOffsetZ), new Vector3D(0, 1, pOffsetZ), pFontFamily);
        }


        /// <summary>
        /// DOSE NOT WORK => Use CreateTextLabel
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <param name="pText"></param>
        /// <param name="pColor"></param>
        /// <param name="pFontSize"></param>
        /// <param name="pFontFamily"></param>
        /// <param name="pOffsetZ"></param>
        /// <returns></returns>
        public static ModelVisual3D Text(double pX, double pY, String pText, Color pColor, double pFontSize, FontFamily pFontFamily, double pOffsetZ = 0)
        {
            HelixToolkit.Wpf.TextVisual3D origin = new HelixToolkit.Wpf.TextVisual3D();

            origin.Text = pText;
            origin.Position = new Point3D(pX, pY, pOffsetZ);
            origin.Foreground = new SolidColorBrush(pColor);
            origin.TextDirection = new Vector3D(0, 0, 1);
            origin.FontSize = 200;
            origin.Height = 200;

            /*GeometryModel3D rect = Rectangle3DGeo.Rect(pX, pY, pWidth, pHeight, Color.FromRgb(0, 0, 0));
            TextBlock t = new TextBlock();
            t.Text = pText;
            t.Foreground = new SolidColorBrush(pColor);
            rect.Material = new DiffuseMaterial(new VisualBrush(t));*/
            return origin;
        }

        /// <summary>
        /// Creates a GeometryModel3D text label. 
        /// (derived from: http://ericsink.com/wpf3d/4_Text.html)
        /// </summary>
        /// <param name="text">The string</param>
        /// <param name="textColor">The color of the text.</param>
        /// <param name="height">Height of the characters</param>
        /// <param name="center">The center of the label</param>
        /// <param name="over">Horizontal direction of the label</param>
        /// <param name="up">Vertical direction of the label</param>
        /// <returns>Suitable for adding to your Viewport3D</returns>
        public static GeometryModel3D CreateTextLabel(
            string text,
            Color textColor,
            double height,
            Point3D center,
            Vector3D over,
            Vector3D up,
            FontFamily pFontFamily)
        {
            // First we need a textblock containing the text of our label
            TextBlock tb = new TextBlock(new Run(text));
            tb.Foreground = new SolidColorBrush(textColor);
            tb.FontFamily = pFontFamily;

            // Now use that TextBlock as the brush for a material
            DiffuseMaterial mat = new DiffuseMaterial();
            mat.Brush = new VisualBrush(tb);

            // We just assume the characters are square
            double width = text.Length * height;

            // Since the parameter coming in was the center of the label,
            // we need to find the four corners
            // p0 is the lower left corner
            // p1 is the upper left
            // p2 is the lower right
            // p3 is the upper right
            Point3D p0 = center - width / 2 * over - height / 2 * up;
            Point3D p1 = p0 + up * 1 * height;
            Point3D p2 = p0 + over * width;
            Point3D p3 = p0 + up * 1 * height + over * width;

            // Now build the geometry for the sign.  It's just a
            // rectangle made of two triangles, on each side.

            MeshGeometry3D mg = new MeshGeometry3D();
            mg.Positions = new Point3DCollection();
            mg.Positions.Add(p0);    // 0
            mg.Positions.Add(p1);    // 1
            mg.Positions.Add(p2);    // 2
            mg.Positions.Add(p3);    // 3

            mg.TriangleIndices.Add(0);
            mg.TriangleIndices.Add(3);
            mg.TriangleIndices.Add(1);
            mg.TriangleIndices.Add(0);
            mg.TriangleIndices.Add(2);
            mg.TriangleIndices.Add(3);

            // These texture coordinates basically stretch the
            // TextBox brush to cover the full side of the label.

            mg.TextureCoordinates.Add(new Point(0, 1));
            mg.TextureCoordinates.Add(new Point(0, 0));
            mg.TextureCoordinates.Add(new Point(1, 1));
            mg.TextureCoordinates.Add(new Point(1, 0));

            // And that's all.  Return the result.

            var model = new GeometryModel3D(mg, mat);
            
            return model;
        }

    }
}
