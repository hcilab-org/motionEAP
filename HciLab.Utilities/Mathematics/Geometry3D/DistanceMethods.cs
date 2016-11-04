// <copyright file=DistanceMethods.cs
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
using System;

namespace HciLab.Utilities.Mathematics.Geometry3D
{
	/// <summary>
	/// Provides various distance computation methods.
	/// </summary>
	public sealed class DistanceMethods
	{
        public enum DistanceOrientation : short
        {
            XAxis = 0,
            YAxis = 1,
            ZAxis = 2
        };

		#region Point-Point
		/// <summary>
		/// Calculates the squared distance between two points.
		/// </summary>
		/// <param name="point1">A <see cref="Vector3"/> instance.</param>
		/// <param name="point2">A <see cref="Vector3"/> instance.</param>
		/// <returns>The squared distance between between two points.</returns>
		public static double SquaredDistance(Vector3 point1, Vector3 point2)
		{
			Vector3 delta = point2 - point1;
            return delta.GetLengthSquared();
		}
		/// <summary>
		/// Calculates the distance between two points.
		/// </summary>
		/// <param name="point1">A <see cref="Vector3"/> instance.</param>
		/// <param name="point2">A <see cref="Vector3"/> instance.</param>
		/// <returns>The distance between between two points.</returns>
        public static double Distance(Vector3 point1, Vector3 point2)
		{
			return System.Math.Sqrt(SquaredDistance(point1, point2));
		}

        public static double Distance(Vector3 point1, Vector3 point2, DistanceOrientation pOrientation)
        {
            if (pOrientation == DistanceOrientation.XAxis)
                return point2.X - point1.X;
            else if (pOrientation == DistanceOrientation.YAxis)
                return point2.Y - point1.Y;
            else if (pOrientation == DistanceOrientation.ZAxis)
                return point2.Z - point1.Z;

            throw new Exception();
        }

		#endregion

		#region Point-OBB
		/// <summary>
		/// Calculates the squared distance between a point and a solid oriented box.
		/// </summary>
		/// <param name="point">A <see cref="Vector3"/> instance.</param>
		/// <param name="obb">An <see cref="OrientedBox"/> instance.</param>
		/// <param name="closestPoint">The closest point in box coordinates.</param>
		/// <returns>The squared distance between a point and a solid oriented box.</returns>
		/// <remarks>
		/// Treating the oriented box as solid means that any point inside the box has
		/// distance zero from the box.
		/// </remarks>
		public static double SquaredDistancePointSolidOrientedBox(Vector3 point, OrientedBox obb, out Vector3 closestPoint)
		{
			Vector3 diff = point - obb.Center;
			Vector3 closest = new Vector3(
				Vector3.DotProduct(diff, obb.Axis1),
				Vector3.DotProduct(diff, obb.Axis2),
				Vector3.DotProduct(diff, obb.Axis3));

			double sqrDist = 0.0f;
			double delta	  = 0.0f;

			if (closest.X < -obb.Extent1)
			{
				delta = closest.X + obb.Extent1;
				sqrDist += delta*delta;
				closest.X = -obb.Extent1;
			}
			else if (closest.X > obb.Extent1)
			{
				delta = closest.X - obb.Extent1;
				sqrDist += delta*delta;
				closest.X = obb.Extent1;
			}

			if (closest.Y < -obb.Extent2)
			{
				delta = closest.Y + obb.Extent2;
				sqrDist += delta*delta;
				closest.Y = -obb.Extent2;
			}
			else if (closest.Y > obb.Extent2)
			{
				delta = closest.Y - obb.Extent2;
				sqrDist += delta*delta;
				closest.Y = obb.Extent2;
			}

			if (closest.Z < -obb.Extent3)
			{
				delta = closest.Z + obb.Extent3;
				sqrDist += delta*delta;
				closest.Z = -obb.Extent3;
			}
			else if (closest.Z > obb.Extent3)
			{
				delta = closest.Z - obb.Extent3;
				sqrDist += delta*delta;
				closest.Z = obb.Extent3;
			}

			closestPoint = closest;

			return sqrDist;
		}
		/// <summary>
		/// Calculates the squared distance between a point and a solid oriented box.
		/// </summary>
		/// <param name="point">A <see cref="Vector3"/> instance.</param>
		/// <param name="obb">An <see cref="OrientedBox"/> instance.</param>
		/// <returns>The squared distance between a point and a solid oriented box.</returns>
		/// <remarks>
		/// Treating the oriented box as solid means that any point inside the box has
		/// distance zero from the box.
		/// </remarks>
		public static double SquaredDistance(Vector3 point, OrientedBox obb)
		{
			Vector3 temp;
			return SquaredDistancePointSolidOrientedBox(point, obb, out temp);
		}

		/// <summary>
		/// Calculates the distance between a point and a solid oriented box.
		/// </summary>
		/// <param name="point">A <see cref="Vector3"/> instance.</param>
		/// <param name="obb">An <see cref="OrientedBox"/> instance.</param>
		/// <returns>The distance between a point and a solid oriented box.</returns>
		/// <remarks>
		/// Treating the oriented box as solid means that any point inside the box has
		/// distance zero from the box.
		/// </remarks>
		public static double Distance(Vector3 point, OrientedBox obb)
		{
			return (double)System.Math.Sqrt(SquaredDistance(point, obb));
		}
		#endregion

		#region Point-Plane
		/// <summary>
		/// Calculates the distance between a point and a plane.
		/// </summary>
		/// <param name="point">A <see cref="Vector3"/> instance.</param>
		/// <param name="plane">A <see cref="Plane"/> instance.</param>
		/// <returns>The distance between a point and a plane.</returns>
		/// <remarks>
		/// <p>
		///  A positive return value means teh point is on the positive side of the plane.
		///  A negative return value means teh point is on the negative side of the plane.
		///  A zero return value means the point is on the plane.
		/// </p>
		/// <p>
		///  The absolute value of the return value is the true distance only when the plane normal is
		///  a unit length vector. 
		/// </p>
		/// </remarks>
		public static double Distance(Vector3 point, Plane plane)
		{
			return Vector3.DotProduct(plane.Normal, point) + plane.Constant;
		}
		#endregion
        

		#region Private Constructor
		private DistanceMethods()
		{
		}
		#endregion
	}
}
