// <copyright file=Utilities3D.cs
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

namespace HciLab.Utilities
{
    public class Utilities3D
    {

        /*public static MatrixSerialized ToMatrix(ArrayListSerializable pList)
        {
            MatrixSerialized m = new MatrixSerialized();

            foreach (Vector3D p1 in pList)
            {
                foreach (Vector3D p2 in pList)
                {
                    if (m[p1.Id, p2.Id] == null)
                    {
                        double dis = Utilities.Distance(p1, p2);
                        m[p1.Id, p2.Id] = dis;
                        m[p2.Id, p1.Id] = dis;
                    }
                }
            }

            return m;
        }

        public static Dictionary<AllEnums.Marker, Vector3D> CenterX(Dictionary<AllEnums.Marker, int> MarkerMap, PointingData pPointingData, Matrix4D pRotationMatrix)
        {

            Dictionary<int, Vector3D> dic = new Dictionary<int, Vector3D>();
            Dictionary<int, int> dicCount = new Dictionary<int, int>();

            foreach (FrameOfMocapDataSerialized f in pPointingData.AllFrames)
            {
                foreach (Vector3D v in f.LabeledMarkers)
                {
                    if (v != null)
                    {
                        if (!dic.ContainsKey(v.Id))
                        {
                            dic.Add(v.Id, new Vector3D(v.Id));
                        }

                        dic[v.Id].X += v.X;
                        dic[v.Id].Y += v.Y;
                        dic[v.Id].Z += v.Z;

                        if (!dicCount.ContainsKey(v.Id))
                            dicCount.Add(v.Id, 0);

                        dicCount[v.Id] += 1;
                    }
                }
            }

            foreach (KeyValuePair<int, Vector3D> pair in dic)
            {
                pair.Value.X = pair.Value.X / dicCount[pair.Key];
                pair.Value.Y = pair.Value.Y / dicCount[pair.Key];
                pair.Value.Z = pair.Value.Z / dicCount[pair.Key];
            }

            Dictionary<AllEnums.Marker, Vector3D> ret = new Dictionary<AllEnums.Marker, Vector3D>();
            foreach (KeyValuePair<int, Vector3D> pair in dic)
            {
                ret.Add(MarkerMap.FirstOrDefault(x => x.Value == pair.Key).Key, (Vector3D)(pRotationMatrix * (Vector4D)pair.Value));
            }
            
            return ret;
        }

        public static Matrix4D RotateCoordinateSystem(Plane pPlane)
        {
            return RotateCoordinateSystem(pPlane.Normal * pPlane.Constant,
                pPlane.Normal);
        }

        public static Matrix4D RotateCoordinateSystem(Ray pRay)
        {
            return RotateCoordinateSystem(pRay.Origin, pRay.Direction);
        }

        public static Matrix4D RotateCoordinateSystem(Vector3D pOrigin, Vector3D pDirection)
        {
            double bx = pDirection.X;
            double by = pDirection.Y;
            double bz = pDirection.Z;

            Matrix4D t1 = new Matrix4D();
            t1.M11 = 1.0;
            t1.M22 = 1.0;
            t1.M33 = 1.0;
            t1.M14 = -pOrigin.X;
            t1.M24 = -pOrigin.Y;
            t1.M34 = -pOrigin.Z;
            t1.M44 = 1.0;

            double d = System.Math.Sqrt(bx * bx + by * by);
            Matrix4D t2 = new Matrix4D();
            t2.M11 = bx;
            t2.M12 = by;
            t2.M21 = -by;
            t2.M22 = bx;
            t2.M33 = d;
            t2.M44 = d;
            t2 = t2 * (1.0 / d);

            double e = System.Math.Sqrt(bx * bx + by * by + bz * bz);
            Matrix4D t3 = new Matrix4D();
            t3.M11 = bz;
            t3.M13 = -d;
            t3.M22 = e;
            t3.M31 = d;
            t3.M33 = bz;
            t3.M44 = e;
            t3 = t3 * (1.0 / e);

            //Z Drehung
            Matrix4D t4 = new Matrix4D();
            t4[1, 1] = Math.Cos(Math.PI);
            t4[1, 2]= -Math.Sin(Math.PI);
            t4[2, 1] = Math.Sin(Math.PI);
            t4[2, 2] = Math.Cos(Math.PI);
            t4[3, 3] = 1;
            t4[4, 4] = 1;

            Matrix4D t5 = new Matrix4D();
            t5.M12 = 1;
            t5.M21 = 1;
            t5.M33 = 1;
            t5.M44 = 1;

            return t5 * t4 * t3 * t2 * t1;

            Matrix4D t6 = new Matrix4D();
            t6.M11 = -1;
            t6.M22 = 1;
            t6.M33 = 1;
            t6.M44 = 1;

            return t6 * t5 * t4 * t3 * t2 * t1;
        }

        public static Ray Orthogonality(Ray pRay, Vector3D pPoint)
        {
            Vector3D direction = new Vector3D();

            Plane p = new Plane(pRay.Direction, pPoint);

            IntersectionPair cross = IntersectionMethods.Intersects(pRay, p);

            if (cross.IntersectionOccurred == true)
            {
                return Ray.CreateUsingTowPoints(pPoint, cross.IntersectionPoint);
            }
            else
            {
                return null;
            }          

        }*/

        
    }
}
