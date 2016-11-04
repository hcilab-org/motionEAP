// <copyright file=AngleMethods.cs
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

using HciLab.Utilities.Mathematics.Core;
using System;
using System.Diagnostics;

namespace HciLab.Utilities.Mathematics.Geometry3D
{
    public sealed class AngleMethods
    {

        public enum AngleOrientation : short
        {
            XPlane = 0,
            YPlane = 1,
            ZPlane = 2
        };

        public static double Angle(Vector3 pDirection, Plane pPlane)
        {
            return System.Math.Asin(
                System.Math.Abs(Vector3.DotProduct(pDirection, pPlane.Normal)) /
                (pDirection.GetLength() * pPlane.Normal.GetLength())
                );
        }


        #region Ray-Plane
        public static double Angle(Ray pRay, Plane pPlane)
        {
            return System.Math.Acos(Vector3.DotProduct(pRay.Direction, pPlane.Normal) / 
                (pRay.Direction.GetLength() * pPlane.Normal.GetLength()));
        }

        public static double Angle(Ray pRay, Plane pPlane, AngleOrientation pOrientation)
        {
            Ray senkrechte = Ray.CreateUsingOrginAndDirection(pRay.Origin, pPlane.Normal);

            IntersectionPair schnittEbeneSenkrechte = IntersectionMethods.Intersects(senkrechte, pPlane);
            IntersectionPair schnittEbeneGerade = IntersectionMethods.Intersects(pRay, pPlane);

            if (schnittEbeneSenkrechte.IntersectionOccurred == false ||schnittEbeneGerade.IntersectionOccurred == false)
                throw new NotImplementedException();

            Double a = 0, c = 0, b = 0;
            Vector2 A, B, C;
            
            if (pOrientation == AngleOrientation.XPlane)
            {
                A = new Vector2(schnittEbeneGerade.IntersectionPoint.Z, schnittEbeneGerade.IntersectionPoint.Y);
                B = new Vector2(pRay.Origin.Z, pRay.Origin.Y);
                C = new Vector2(schnittEbeneSenkrechte.IntersectionPoint.Z, schnittEbeneSenkrechte.IntersectionPoint.Y);
            }
            else if (pOrientation == AngleOrientation.YPlane)
            {
                A = new Vector2(schnittEbeneGerade.IntersectionPoint.Z, schnittEbeneGerade.IntersectionPoint.X);
                B = new Vector2(pRay.Origin.Z, pRay.Origin.X);
                C = new Vector2(schnittEbeneSenkrechte.IntersectionPoint.Z, schnittEbeneSenkrechte.IntersectionPoint.X); 
            }
            else if (pOrientation == AngleOrientation.ZPlane)
            {   
                A = new Vector2(schnittEbeneGerade.IntersectionPoint.Y, schnittEbeneGerade.IntersectionPoint.X);
                B = new Vector2(pRay.Origin.Y, pRay.Origin.X);
                C = new Vector2(schnittEbeneSenkrechte.IntersectionPoint.Y, schnittEbeneSenkrechte.IntersectionPoint.X);
            }
            else
            {
                throw new Exception();
            }

            a = Geometry2D.DistanceMethods.Distance(C, B);
            b = Geometry2D.DistanceMethods.Distance(A, C);
            c = Geometry2D.DistanceMethods.Distance(A, B);

            double ret;

            if (A.Y < C.Y && B.X >= C.X) // 0-90
            {
                ret = System.Math.Asin(a / c);
                
            }
            else if (A.Y >= C.Y && B.X >= C.X) // 90-180
            {
                ret = (System.Math.PI - System.Math.Asin(a / c));
                
            }
            else if (A.Y >= C.Y && B.X < C.X) //180-270
            {
                ret = (System.Math.PI + System.Math.Asin(a / c)); //k
            }
            else if (A.Y < C.Y && B.X < C.X) //270-360
            {
                ret = ((2 * System.Math.PI) - System.Math.Asin(a / c));
            }
            else
                throw new Exception();

            return (ret - (Math.PI / 2))*-1;
        }

        #endregion

        #region Plane-Plane 
        public static double Angle(Plane e1, Plane e2)
        {
            return System.Math.Acos(Vector3.DotProduct(e1.Normal, e2.Normal) / (e1.Normal.GetLength() * e2.Normal.GetLength()));
        }
        #endregion

        public static double ToDegree(double rad)
        {
            return ((180 / System.Math.PI) * rad);
        }

        /*public static double MapToPi(double rad)
        {
            return rad % Math.PI;

        }*/

        #region Private Constructor
        private AngleMethods()
		{
		}
		#endregion

        public static double Angle(Ray pRay1, Ray pRay2, AngleOrientation pOrientation)
        {
            Plane p = null;
            if (pOrientation == AngleOrientation.XPlane)
            {
                p = Plane.CreateZPlane();
            }
            else if (pOrientation == AngleOrientation.YPlane)
            {
                p = Plane.CreateXPlane();

            }
            else if (pOrientation == AngleOrientation.ZPlane)
            {
                p = Plane.CreateYPlane();
            }
            else
            {
                throw new Exception();
            }

            if (pOrientation == AngleOrientation.YPlane)
                Debugger.Break();

            Double a1 = Angle(pRay1, p, pOrientation);
            Double a2 = Angle(pRay2, p, pOrientation);

            return a2 - a1;
        }

        /// <summary>
        /// Angle in B with Ray to A and C
        /// 
        /// calc Beta in ABC
        /// </summary>
        /// <param name="pA"></param>
        /// <param name="pB"></param>
        /// <param name="pC"></param>
        /// <param name="v"></param>
        /// <returns></returns>

        public static double Angle(Vector3 pA, Vector3 pB, Vector3 pC, AngleOrientation pOrientation)
        {
            Double a = 0, c = 0, b = 0;
            Vector2 A, B, C;

            if (pOrientation == AngleOrientation.XPlane)
            {
                A = new Vector2(pA.Z, pA.Y);
                B = new Vector2(pB.Z, pB.Y);
                C = new Vector2(pC.Z, pC.Y);
            }
            else if (pOrientation == AngleOrientation.YPlane)
            {
                A = new Vector2(pA.Z, pA.X);
                B = new Vector2(pB.Z, pB.X);
                C = new Vector2(pC.Z, pC.X);
            }
            else if (pOrientation == AngleOrientation.ZPlane)
            {
                A = new Vector2(pA.Y, pA.X);
                B = new Vector2(pB.Y, pB.X);
                C = new Vector2(pC.Y, pC.X);
            }
            else
            {
                throw new Exception();
            }

            a = Geometry2D.DistanceMethods.Distance(C, B);
            b = Geometry2D.DistanceMethods.Distance(A, C);
            c = Geometry2D.DistanceMethods.Distance(A, B);

            if (a == 0 || c == 0)
                throw new ArgumentOutOfRangeException();

            if (C.Y < A.Y)
                return - Math.Acos((a * a - b * b + c * c) / (2 * a * c));
            else
                return Math.Acos((a * a - b * b + c * c) / (2 * a * c));
        }
    }
}
