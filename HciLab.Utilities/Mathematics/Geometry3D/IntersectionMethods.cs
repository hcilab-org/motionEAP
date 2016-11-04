// <copyright file=IntersectionMethods.cs
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

using System;
using HciLab.Utilities.Mathematics.Core;

namespace HciLab.Utilities.Mathematics.Geometry3D
{
	/// <summary>
	/// Stores an intersection result pair (if intersection occurred and where).
	/// </summary>
	public struct IntersectionPair
	{
		private bool _occurred;
		private Vector3 _point;

		#region Constructors
		/// <summary>
		/// Initialize a new instance of the <see cref="IntersectionPair"/> class.
		/// </summary>
		/// <param name="intersectionOccurred">A boolean value.</param>
		/// <param name="intersectionPoint">A <see cref="Vector3"/> instance.</param>
		public IntersectionPair(bool intersectionOccurred, Vector3 intersectionPoint)
		{
			_occurred = intersectionOccurred;
			_point = intersectionPoint;
		}

		#endregion

		#region Public Properties
		/// <summary>
		/// Gets a value indicating if an intersection ahs occurred.
		/// </summary>
		/// <value>A boolean value.</value>
		public bool IntersectionOccurred
		{
			get { return _occurred; }
		}

		/// <summary>
		/// Gets the intersection point if intersection has occurred.
		/// </summary>
		/// <value>A <see cref="Vector3"/> instance.</value>
		public Vector3 IntersectionPoint
		{
			get { return _point; }
		}
		#endregion
	}

	/// <summary>
	/// Represents the possible interction types.
	/// </summary>
	public enum IntersectionType
	{
		/// <summary>
		/// Objects do not intersect
		/// </summary>
		None,
		/// <summary>
		/// Objects parially intersect each other.
		/// </summary>
		Partial,
		/// <summary>
		/// An object is fully contained in another object.
		/// </summary>
		Contained
	}

	/// <summary>
	/// Provides method for testing intersections between objects.
	/// </summary>
	public sealed class IntersectionMethods
	{
		#region Ray\Line to plane,AABB,OBB, Triangle and Sphere intersection methods
		/// <summary>
		/// Tests for intersection between a <see cref="Ray"/> and a <see cref="Plane"/>.
		/// </summary>
		/// <param name="ray">A <see cref="Ray"/> instance.</param>
		/// <param name="plane">A <see cref="Plane"/> instance.</param>
		/// <returns>An <see cref="IntersectionPair"/> instance containing the intersection information.</returns>
		public static IntersectionPair Intersects(Ray ray, Plane plane)
		{
			bool intersect = false;
            Vector3 hitPoint = Vector3.Zero();
			double denominator = Vector3.DotProduct(plane.Normal, ray.Direction);

			// Check if the ray is parrallel to the plane
			if (MathFunctions.ApproxEquals(denominator, 0.0f))
			{
				// If the line is parallel to the plane it only intersects the plane if it is on the plane.
				intersect = (plane.GetSign(ray.Origin) == MathFunctions.Sign.Zero);
				if (intersect)
					hitPoint = ray.Origin;
			}
			else
			{
				double t = (-plane.Constant - Vector3.DotProduct(plane.Normal, ray.Origin)) / denominator;
				hitPoint = ray.Origin + ray.Direction * t;
				intersect = true;
			}

			return new IntersectionPair(intersect, hitPoint);
		}

		/// <summary>
		/// Tests for intersection between a <see cref="Ray"/> and an <see cref="AxisAlignedBox">axis aligned box</see>.
		/// </summary>
		/// <param name="ray">A <see cref="Ray"/> instance.</param>
		/// <param name="aabb">A <see cref="AxisAlignedBox"/> instance.</param>
		/// <returns>An <see cref="IntersectionPair"/> instance containing the intersection information.</returns>
		public static IntersectionPair Intersects(Ray ray, AxisAlignedBox aabb)
		{
			// TODO: Implement this.
			throw new NotImplementedException();
		}
		/// <summary>
		/// Tests for intersection between a <see cref="Ray"/> and an <see cref="OrientedBox">oriented box</see>.
		/// </summary>
		/// <param name="ray">A <see cref="Ray"/> instance.</param>
		/// <param name="obb">A <see cref="OrientedBox"/> instance.</param>
		/// <returns>An <see cref="IntersectionPair"/> instance containing the intersection information.</returns>
		public static IntersectionPair Intersects(Ray ray, OrientedBox obb)
		{
			// TODO: Implement this.
			throw new NotImplementedException();
		}
		/// <summary>
		/// Tests for intersection between a <see cref="Ray"/> and a <see cref="Sphere"/>.
		/// </summary>
		/// <param name="ray">A <see cref="Ray"/> instance.</param>
		/// <param name="sphere">A <see cref="Sphere"/> instance.</param>
		/// <returns>An <see cref="IntersectionPair"/> instance containing the intersection information.</returns>
		public static IntersectionPair Intersects(Ray ray, Sphere sphere)
		{
			// TODO: Implement this.
			throw new NotImplementedException();
		}

		/// <summary>
		/// Tests for intersection between a <see cref="Ray"/> and a <see cref="Triangle"/>.
		/// </summary>
		/// <param name="ray">A <see cref="Ray"/> instance.</param>
		/// <param name="triangle">A <see cref="Triangle"/> instance.</param>
		/// <returns>An <see cref="IntersectionPair"/> instance containing the intersection information.</returns>
		/// <remarks>
		/// For information about the algorithm visit http://www.acm.org/jgt/papers/MollerTrumbore97/ 
		/// </remarks>
		public static IntersectionPair Intersects(Ray ray, Triangle triangle)
		{
			// Find the vectors for the 2 edges sharing trangle.Point0
			Vector3 edge1 = triangle.Point1 - triangle.Point0;
			Vector3 edge2 = triangle.Point2 - triangle.Point0;

			// Begin calculating determinant - also used to calc the U parameter.
			Vector3 pvec = Vector3.CrossProduct(ray.Direction, edge2);

			double det = Vector3.DotProduct(edge1, pvec);

			// If determinant is zero the ray lies in plane of the triangle
			if (MathFunctions.ApproxEquals(det, 0.0f))
			{
                return new IntersectionPair(false, Vector3.Zero());
			}

			double invDet = 1.0f / det;

			// Calculate the distance from triangle.Point0 to the ray origin
			Vector3 tvec = ray.Origin - triangle.Point0;

			// Calculate U parameter and test bounds
			double u = Vector3.DotProduct(tvec, pvec) * invDet;
			if ((u < 0.0f) || u > 1.0f)
                return new IntersectionPair(false, Vector3.Zero());

			// Prepare to test the V parameter
			Vector3 qvec  = Vector3.CrossProduct(tvec, edge1);

			// Calculate V parameter and test bounds
			double v = Vector3.DotProduct(ray.Direction, qvec) * invDet;
			if ((v < 0.0f) || v > 1.0f)
                return new IntersectionPair(false, Vector3.Zero());

			// The ray intersects the triangle
			// Calculate the distance from  the ray origin to the intersection point.
			//t = Vector3F.DotProduct(edge2, qvec) * invDet;

			return new IntersectionPair(true, triangle.FromBarycentric(u,v));
		}


		/// <summary>
		/// Tests for intersection between a <see cref="Segment"/> and a <see cref="Plane"/>
		/// </summary>
		/// <param name="line">A <see cref="Segment"/> instance.</param>
		/// <param name="plane">A <see cref="Plane"/> instance.</param>
		/// <returns>An <see cref="IntersectionPair"/> instance containing the intersection information.</returns>
		public static IntersectionPair Intersects(Segment segment, Plane plane)
		{
			if (TestIntersection(segment, plane) == false)
                return new IntersectionPair(false, Vector3.Zero());

			Vector3 dir = segment.P1 - segment.P0;
			double d  = Vector3.DotProduct(plane.Normal, dir);
			double t  = (plane.Constant - Vector3.DotProduct(plane.Normal, segment.P0)) / d;

			return new IntersectionPair(true, segment.P0 + dir*t);
		}
		#endregion

		/// <summary>
		/// Tests for intersection between two <see cref="AxisAlignedBox"/> instances.
		/// </summary>
		/// <param name="box1">An <see cref="AxisAlignedBox"/> instance.</param>
		/// <param name="box2">An <see cref="AxisAlignedBox"/> instance.</param>
		/// <returns>An <see cref="IntersectionType"/> value.</returns>
		public static IntersectionType Intersects(AxisAlignedBox box1, AxisAlignedBox box2)
		{
			Vector3 min1 = box1.Min; 
			Vector3 max1 = box1.Max; 
			Vector3 min2 = box2.Min; 
			Vector3 max2 = box2.Max; 

			if ((min2.X > min1.X) && 
				(max2.X < max1.X) && 
				(min2.Y > min1.Y) && 
				(max2.Y < max1.Y) && 
				(min2.Z > min1.Z) && 
				(max2.Z < max1.Z)) 
			{
				// box2 contains box1
				return IntersectionType.Contained; 
			}

			if ((min2.X > max2.X) || 
				(min2.Y > max2.Y) || 
				(min2.Z > max2.Z) || 
				(max2.X < min1.X) || 
				(max2.Y < min1.Y) || 
				(max2.Z < min1.Z)) 
			{

				// The two boxes are not intersecting.
				return IntersectionType.None; 
			}

			// if we got this far, they are partially intersecting
			return IntersectionType.Partial; 
		}
		/// <summary>
		/// Tests for intersection between an <see cref="AxisAlignedBox"/> and an <see cref="OrientedBox"/> .
		/// </summary>
		/// <param name="box1">An <see cref="AxisAlignedBox"/> instance.</param>
		/// <param name="box2">An <see cref="OrientedBox"/> instance.</param>
		/// <returns>An <see cref="IntersectionType"/> value.</returns>
		public static IntersectionType Intersects(AxisAlignedBox box1, OrientedBox box2)
		{
			// TODO: Implement this.
			throw new NotImplementedException();
		}
		/// <summary>
		/// Tests for intersection between two <see cref="OrientedBox"/> instances.
		/// </summary>
		/// <param name="box1">An <see cref="OrientedBox"/> instance.</param>
		/// <param name="box2">An <see cref="OrientedBox"/> instance.</param>
		/// <returns>An <see cref="IntersectionType"/> value.</returns>
		public static IntersectionType Intersects(OrientedBox box1, OrientedBox box2)
		{
			// TODO: Implement this.
			throw new NotImplementedException();
		}
		/// <summary>
		/// Tests for intersection between two <see cref="Sphere"/> instances.
		/// </summary>
		/// <param name="sphere1">A <see cref="Sphere"/> instance.</param>
		/// <param name="sphere2">A <see cref="Sphere"/> instance.</param>
		/// <returns>An <see cref="IntersectionType"/> value.</returns>
		public static IntersectionType Intersects(Sphere sphere1, Sphere sphere2)
		{
			// TODO: Implement this.
			throw new NotImplementedException();
		}
		/// <summary>
		/// Tests for intersection between a <see cref="Sphere"/> and an <see cref="AxisAlignedBox"/> .
		/// </summary>
		/// <param name="sphere">A <see cref="Sphere"/> instance.</param>
		/// <param name="box">An <see cref="AxisAlignedBox"/> instance.</param>
		/// <returns>An <see cref="IntersectionType"/> value.</returns>
		public static IntersectionType Intersects(Sphere sphere, AxisAlignedBox box)
		{
			// TODO: Implement this.
			throw new NotImplementedException();
		}
		/// <summary>
		/// Tests for intersection between a <see cref="Sphere"/> and an <see cref="OrientedBox"/> .
		/// </summary>
		/// <param name="sphere">A <see cref="Sphere"/> instance.</param>
		/// <param name="box">An <see cref="OrientedBox"/> instance.</param>
		/// <returns>An <see cref="IntersectionType"/> value.</returns>
		public static IntersectionType Intersects(Sphere sphere, OrientedBox box)
		{
			// TODO: Implement this.
			throw new NotImplementedException();
		}


		/// <summary>
		/// Tests for intersection between a <see cref="Segment"/> and a <see cref="Plane"/>.
		/// </summary>
		/// <param name="line">A <see cref="Segment"/> instance.</param>
		/// <param name="plane">A <see cref="Plane"/> instance.</param>
		/// <returns><see langword="true"/> if intersection occurs; otherwise, <see langword="false"/>.</returns>
		public static bool TestIntersection(Segment segment, Plane plane)
		{
			// Get the position of the line's end point relative to the plane.
			int sign0 = (int)plane.GetSign(segment.P0);
			int sign1 = (int)plane.GetSign(segment.P1);

			// Intersection occurs if the 2 endpoints are at oposite sides of the plane.
			return ( ((sign0 > 0) && (sign1 < 0)) || ((sign0 < 0) && (sign0 > 0)) );
		}


		#region Private Constructor
		private IntersectionMethods()
		{
		}
		#endregion
	}
}
