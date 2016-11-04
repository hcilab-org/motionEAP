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
// <date> 11/2/2016 12:25:57 PM</date>

using HciLab.Utilities.Mathematics.Core;

namespace HciLab.Utilities.Mathematics.Geometry2D
{
	/// <summary>
	/// Provides various distance computation methods.
	/// </summary>
	public sealed class DistanceMethods
	{
		#region Point-Point
		/// <summary>
		/// Calculates the squared distance between two points.
		/// </summary>
		/// <param name="point1">A <see cref="Vector2"/> instance.</param>
		/// <param name="point2">A <see cref="Vector2"/> instance.</param>
		/// <returns>The squared distance between the two points.</returns>
		public static double SquaredDistance(Vector2 point1, Vector2 point2)
		{
			Vector2 diff = point1-point2;
			return diff.GetLengthSquared();
		}

		/// <summary>
		/// Calculates the distance between two points.
		/// </summary>
		/// <param name="point1">A <see cref="Vector2"/> instance.</param>
		/// <param name="point2">A <see cref="Vector2"/> instance.</param>
		/// <returns>The distance between the two points.</returns>
		public static double Distance(Vector2 point1, Vector2 point2)
		{
			return (double)System.Math.Sqrt(SquaredDistance(point1, point2));
		}
		#endregion

		#region Point-Ray
		/// <summary>
		/// Calculates the squared distance between a given point and a given ray.
		/// </summary>
		/// <param name="point">A <see cref="Vector2"/> instance.</param>
		/// <param name="ray">A <see cref="Ray"/> instance.</param>
		/// <returns>The squared distance between the point and the ray.</returns>
        public static double SquaredDistance(Vector2 point, Ray ray)
		{
			Vector2 diff = point - ray.Origin;
            double t = Vector2.DotProduct(diff, ray.Direction);

			if (t <= 0.0f)
			{
				t = 0.0f;
			}
			else
			{
				t	/= ray.Direction.GetLengthSquared();
				diff-= t * ray.Direction;
			}

			return diff.GetLengthSquared();
		}

		/// <summary>
		/// Calculates the distance between a given point and a given ray.
		/// </summary>
		/// <param name="point">A <see cref="Vector2"/> instance.</param>
		/// <param name="ray">A <see cref="Ray"/> instance.</param>
		/// <returns>The distance between the point and the ray.</returns>
		public static double Distance(Vector2 point, Ray ray)
		{
			return (double)System.Math.Sqrt(SquaredDistance(point, ray));
		}
		#endregion

		#region Point-AABB
		/// <summary>
		/// Calculates the squared distance between a point and a solid axis-aligned box.
		/// </summary>
		/// <param name="point">A <see cref="Vector2"/> instance.</param>
		/// <param name="aabb">An <see cref="AxisAlignedBox"/> instance.</param>
		/// <returns>The squared distance between a point and a solid axis-aligned box.</returns>
		/// <remarks>
		/// Treating the box as solid means that any point inside the box has
		/// distance zero from the box.
		/// </remarks>
		public static double SquaredDistance(Vector2 point, AxisAlignedBox aabb)
		{
			double sqrDistance = 0.0f;
			double delta;

			if (point.X < aabb.Min.X)
			{
				delta = point.X - aabb.Min.X;
				sqrDistance += delta*delta;
			}
			else if (point.X > aabb.Max.X)
			{
				delta = point.X - aabb.Max.X;
				sqrDistance += delta*delta;
			}

			if (point.Y < aabb.Min.Y)
			{
				delta = point.Y - aabb.Min.Y;
				sqrDistance += delta*delta;
			}
			else if (point.Y > aabb.Max.Y)
			{
				delta = point.Y - aabb.Max.Y;
				sqrDistance += delta*delta;
			}

			return sqrDistance;
		}

		/// <summary>
		/// Calculates the distance between a point and a solid axis-aligned box.
		/// </summary>
		/// <param name="point">A <see cref="Vector2"/> instance.</param>
		/// <param name="aabb">An <see cref="AxisAlignedBox"/> instance.</param>
		/// <returns>The distance between a point and a solid axis-aligned box.</returns>
		/// <remarks>
		/// Treating the box as solid means that any point inside the box has
		/// distance zero from the box.
		/// </remarks>
		public static double Distance(Vector2 point, AxisAlignedBox aabb)
		{
			return (double)System.Math.Sqrt(SquaredDistance(point, aabb));
		}
		#endregion

		#region Point-OBB
		/// <summary>
		/// Calculates the squared distance between a point and a solid oriented box.
		/// </summary>
		/// <param name="point">A <see cref="Vector2"/> instance.</param>
		/// <param name="obb">An <see cref="OrientedBox"/> instance.</param>
		/// <param name="closestPoint">The closest point in box coordinates.</param>
		/// <returns>The squared distance between a point and a solid oriented box.</returns>
		/// <remarks>
		/// Treating the oriented box as solid means that any point inside the box has
		/// distance zero from the box.
		/// </remarks>
		public static double SquaredDistancePointSolidOrientedBox(Vector2 point, OrientedBox obb, out Vector2 closestPoint)
		{
			Vector2 diff = point - obb.Center;
			Vector2 closest = new Vector2(
				Vector2.DotProduct(diff, obb.Axis1),
				Vector2.DotProduct(diff, obb.Axis2));

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

			closestPoint = closest;

			return sqrDist;
		}
		/// <summary>
		/// Calculates the squared distance between a point and a solid oriented box.
		/// </summary>
		/// <param name="point">A <see cref="Vector2"/> instance.</param>
		/// <param name="obb">An <see cref="OrientedBox"/> instance.</param>
		/// <returns>The squared distance between a point and a solid oriented box.</returns>
		/// <remarks>
		/// Treating the oriented box as solid means that any point inside the box has
		/// distance zero from the box.
		/// </remarks>
		public static double SquaredDistance(Vector2 point, OrientedBox obb)
		{
			Vector2 temp;
			return SquaredDistancePointSolidOrientedBox(point, obb, out temp);
		}

		/// <summary>
		/// Calculates the distance between a point and a solid oriented box.
		/// </summary>
		/// <param name="point">A <see cref="Vector2"/> instance.</param>
		/// <param name="obb">An <see cref="OrientedBox"/> instance.</param>
		/// <returns>The distance between a point and a solid oriented box.</returns>
		/// <remarks>
		/// Treating the oriented box as solid means that any point inside the box has
		/// distance zero from the box.
		/// </remarks>
		public static double Distance(Vector2 point, OrientedBox obb)
		{
			return (double)System.Math.Sqrt(SquaredDistance(point, obb));
		}
		#endregion

		#region Private Constructor
		private DistanceMethods()
		{
		}
		#endregion
	}	
}
