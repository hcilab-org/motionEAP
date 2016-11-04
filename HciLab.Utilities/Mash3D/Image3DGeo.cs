// <copyright file=Image3DGeo.cs
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

using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace HciLab.Utilities.Mash3D
{
    public static class Image3DGeo
    {
        /// <summary>
        /// Using mash generator until with Image Mapping
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <param name="pWidth"></param>
        /// <param name="pHeight"></param>
        /// <param name="pImage"></param>
        /// <param name="zOffset"></param>
        /// <returns></returns>
        public static GeometryModel3D Image(double pX, double pY, double pWidth, double pHeight, ImageSource pImage, double zOffset = 0)
        {
            GeometryModel3D model = new GeometryModel3D();
            model.Geometry = MappingMash(pX, pY, pWidth, pHeight, zOffset);;
            model.Material = new DiffuseMaterial(new ImageBrush(pImage));

            return model;
        }

        public static MeshGeometry3D MappingMash(double pX, double pY, double pWidth, double pHeight, double zOffset = 0)
        {
            MeshGeometry3D mash = new MeshGeometry3D();

            mash.Positions.Add(new Point3D(pX, pY, zOffset));
            mash.Positions.Add(new Point3D(pX + pWidth, pY, zOffset));
            mash.Positions.Add(new Point3D(pX + pWidth, pY + pHeight, zOffset));
            mash.Positions.Add(new Point3D(pX + pWidth, pY + pHeight, zOffset));
            mash.Positions.Add(new Point3D(pX, pY + pHeight, zOffset));
            mash.Positions.Add(new Point3D(pX, pY, zOffset));

            mash.Normals.Add(new Vector3D(0, 0, 1));
            mash.Normals.Add(new Vector3D(0, 0, 1));
            mash.Normals.Add(new Vector3D(0, 0, 1));
            mash.Normals.Add(new Vector3D(0, 0, 1));
            mash.Normals.Add(new Vector3D(0, 0, 1));
            mash.Normals.Add(new Vector3D(0, 0, 1));

            mash.TextureCoordinates.Add(new System.Windows.Point(0, 1));
            mash.TextureCoordinates.Add(new System.Windows.Point(1, 1));
            mash.TextureCoordinates.Add(new System.Windows.Point(1, 0));
            mash.TextureCoordinates.Add(new System.Windows.Point(1, 0));
            mash.TextureCoordinates.Add(new System.Windows.Point(0, 0));
            mash.TextureCoordinates.Add(new System.Windows.Point(0, 1));
            
            mash.TriangleIndices.Add(0);
            mash.TriangleIndices.Add(1);
            mash.TriangleIndices.Add(2);
            mash.TriangleIndices.Add(3);
            mash.TriangleIndices.Add(4);
            mash.TriangleIndices.Add(5);

            return mash;
        }


    }
}
