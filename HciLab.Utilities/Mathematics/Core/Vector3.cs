// <copyright file=Vector3.cs
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
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;

namespace HciLab.Utilities.Mathematics.Core
{
	/// <summary>
	/// Represents 3-Dimentional vector of single-precision doubleing point numbers.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(Vector3DConverter))]
	public class Vector3 : DataBaseClass, ISerializable, ICloneable
	{
		#region Private fields
        private double m_X;
        private double m_Y;
        private double m_Z;
		#endregion

		#region Constructors

        public Vector3()
            : base ()
        {
            m_X = 0;
            m_Y = 0;
            m_Z = 0;
        }

        public Vector3(int pId)
            : base (pId)
        {
            m_X = 0;
            m_Y = 0;
            m_Z = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3"/> class with the specified coordinates.
        /// </summary>
        /// /// <param name="pId">Ideftifier of thre Vector</param>
        /// <param name="x">The vector's X coordinate.</param>
        /// <param name="y">The vector's Y coordinate.</param>
        /// <param name="z">The vector's Z coordinate.</param>
        public Vector3(int pId, double x, double y, double z)
            : base (pId)
        {
            m_X = x;
            m_Y = y;
            m_Z = z;
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="Vector3"/> class with the specified coordinates.
		/// </summary>
		/// <param name="x">The vector's X coordinate.</param>
		/// <param name="y">The vector's Y coordinate.</param>
		/// <param name="z">The vector's Z coordinate.</param>
		public Vector3(double x, double y, double z)
            : base ()
		{
			m_X = x;
			m_Y = y;
			m_Z = z;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Vector3"/> class with the specified coordinates.
		/// </summary>
		/// <param name="coordinates">An array containing the coordinate parameters.</param>
		public Vector3(double[] coordinates)
            :base ()
		{
			Debug.Assert(coordinates != null);
			Debug.Assert(coordinates.Length >= 3);
			m_X = coordinates[0];
			m_Y = coordinates[1];
			m_Z = coordinates[2];
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Vector3"/> class with the specified coordinates.
		/// </summary>
		/// <param name="coordinates">An array containing the coordinate parameters.</param>
        public Vector3(DoubleArrayList coordinates)
            : base ()
		{
			Debug.Assert(coordinates != null);
			Debug.Assert(coordinates.Count >= 3);
			m_X = coordinates[0];
			m_Y = coordinates[1];
			m_Z = coordinates[2];
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Vector3"/> class using coordinates from a given <see cref="Vector3"/> instance.
		/// </summary>
		/// <param name="vector">A <see cref="Vector3"/> to get the coordinates from.</param>
		public Vector3(Vector3 vector)
            :base (vector.Id)
		{
			m_X = vector.X;
			m_Y = vector.Y;
			m_Z = vector.Z;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Vector3"/> class with serialized data.
		/// </summary>
		/// <param name="pInfo">The object that holds the serialized object data.</param>
		/// <param name="pContext">The contextual information about the source or destination.</param>
		private Vector3(SerializationInfo pInfo, StreamingContext pContext)
            : base(pInfo, pContext)
		{
			m_X = pInfo.GetSingle("m_X");
			m_Y = pInfo.GetSingle("m_Y");
			m_Z = pInfo.GetSingle("m_Z");
		}

       
		#endregion

		#region Constants
		/// <summary>
		/// 3-Dimentional single-precision doubleing point zero vector.
		/// </summary>
        public static Vector3 Zero()
        {
            return new Vector3(0.0f, 0.0f, 0.0f);
        }
		/// <summary>
		/// 3-Dimentional single-precision doubleing point X-Axis vector.
		/// </summary>
        public static Vector3 XAxis()
        {
            return new Vector3(1.0f, 0.0f, 0.0f);
        }
		/// <summary>
		/// 3-Dimentional single-precision doubleing point Y-Axis vector.
		/// </summary>
        public static Vector3 YAxis()
        {
            return new Vector3(0.0f, 1.0f, 0.0f);
        }
		/// <summary>
		/// 3-Dimentional single-precision doubleing point Y-Axis vector.
		/// </summary>
        public static Vector3 ZAxis()
        {
            return new Vector3(0.0f, 0.0f, 1.0f);
        }
		#endregion

		#region Public properties
        /// <summary>
		/// Gets or sets the x-coordinate of this vector.
		/// </summary>
		/// <value>The x-coordinate of this vector.</value>
		public double X
		{
			get { return m_X; }
			set { m_X = value;}
		}
		/// <summary>
		/// Gets or sets the y-coordinate of this vector.
		/// </summary>
		/// <value>The y-coordinate of this vector.</value>
		public double Y
		{
			get { return m_Y; }
			set { m_Y = value;}
		}
		/// <summary>
		/// Gets or sets the z-coordinate of this vector.
		/// </summary>
		/// <value>The z-coordinate of this vector.</value>
		public double Z
		{
			get { return m_Z; }
			set { m_Z = value;}
		}
		#endregion

		#region ICloneable Members
		/// <summary>
		/// Creates an exact copy of this <see cref="Vector3"/> object.
		/// </summary>
		/// <returns>The <see cref="Vector3"/> object this method creates, cast as an object.</returns>
		object ICloneable.Clone()
		{
			return new Vector3(this);
		}
		/// <summary>
		/// Creates an exact copy of this <see cref="Vector3"/> object.
		/// </summary>
		/// <returns>The <see cref="Vector3"/> object this method creates.</returns>
		public Vector3 Clone()
		{
			return new Vector3(this);
		}
		#endregion

		#region ISerializable Members
		/// <summary>
		/// Populates a <see cref="SerializationInfo"/> with the data needed to serialize this object.
		/// </summary>
		/// <param name="pInfo">The <see cref="SerializationInfo"/> to populate with data. </param>
		/// <param name="pContext">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
		//[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter=true)]
		public void GetObjectData(SerializationInfo pInfo, StreamingContext pContext)
		{
            base.GetObjectData(pInfo, pContext);
			pInfo.AddValue("m_X", m_X);
			pInfo.AddValue("m_Y", m_Y);
			pInfo.AddValue("m_Z", m_Z);
		}
		#endregion

		#region Public Static Parse Methods
		/// <summary>
		/// Converts the specified string to its <see cref="Vector3"/> equivalent.
		/// </summary>
		/// <param name="s">A string representation of a <see cref="Vector3"/></param>
		/// <returns>A <see cref="Vector3"/> that represents the vector specified by the <paramref name="s"/> parameter.</returns>
		public static Vector3 Parse(string s)
		{
			Regex r = new Regex(@"\((?<x>.*),(?<y>.*),(?<z>.*)\)", RegexOptions.None);
			Match m = r.Match(s);
			if (m.Success)
			{
				return new Vector3(
					double.Parse(m.Result("${x}")),
					double.Parse(m.Result("${y}")),
					double.Parse(m.Result("${z}"))
					);
			}
			else
			{
				throw new ParseException("Unsuccessful Match.");
			}
		}
		#endregion

		#region Public Static Vector Arithmetics
		/// <summary>
		/// Adds two vectors.
		/// </summary>
		/// <param name="v">A <see cref="Vector3"/> instance.</param>
		/// <param name="w">A <see cref="Vector3"/> instance.</param>
		/// <returns>A new <see cref="Vector3"/> instance containing the sum.</returns>
		public static Vector3 Add(Vector3 v, Vector3 w)
		{
			return new Vector3(v.X + w.X, v.Y + w.Y, v.Z + w.Z);
		}
		/// <summary>
		/// Adds a vector and a scalar.
		/// </summary>
		/// <param name="v">A <see cref="Vector3"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Vector3"/> instance containing the sum.</returns>
		public static Vector3 Add(Vector3 v, double s)
		{
			return new Vector3(v.X + s, v.Y + s, v.Z + s);
		}
		/// <summary>
		/// Adds two vectors and put the result in the third vector.
		/// </summary>
		/// <param name="u">A <see cref="Vector3"/> instance.</param>
		/// <param name="v">A <see cref="Vector3"/> instance</param>
		/// <param name="w">A <see cref="Vector3"/> instance to hold the result.</param>
		public static void Add(Vector3 u, Vector3 v, ref Vector3 w)
		{
			w.X = u.X + v.X;
			w.Y = u.Y + v.Y;
			w.Z = u.Z + v.Z;
		}
		/// <summary>
		/// Adds a vector and a scalar and put the result into another vector.
		/// </summary>
		/// <param name="u">A <see cref="Vector3"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <param name="v">A <see cref="Vector3"/> instance to hold the result.</param>
		public static void Add(Vector3 u, double s, ref Vector3 v)
		{
			v.X = u.X + s;
			v.Y = u.Y + s;
			v.Z = u.Z + s;
		}
		/// <summary>
		/// Subtracts a vector from a vector.
		/// </summary>
		/// <param name="v">A <see cref="Vector3"/> instance.</param>
		/// <param name="w">A <see cref="Vector3"/> instance.</param>
		/// <returns>A new <see cref="Vector3"/> instance containing the difference.</returns>
		/// <remarks>
		///	result[i] = v[i] - w[i].
		/// </remarks>
		public static Vector3 Subtract(Vector3 v, Vector3 w)
		{
			return new Vector3(v.X - w.X,
                v.Y - w.Y,
                v.Z - w.Z);
		}
		/// <summary>
		/// Subtracts a scalar from a vector.
		/// </summary>
		/// <param name="v">A <see cref="Vector3"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Vector3"/> instance containing the difference.</returns>
		/// <remarks>
		/// result[i] = v[i] - s
		/// </remarks>
		public static Vector3 Subtract(Vector3 v, double s)
		{
			return new Vector3(v.X - s, v.Y - s, v.Z - s);
		}
		/// <summary>
		/// Subtracts a vector from a scalar.
		/// </summary>
		/// <param name="v">A <see cref="Vector3"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Vector3"/> instance containing the difference.</returns>
		/// <remarks>
		/// result[i] = s - v[i]
		/// </remarks>
		public static Vector3 Subtract(double s, Vector3 v)
		{
			return new Vector3(s - v.X, s - v.Y, s - v.Z);
		}
		/// <summary>
		/// Subtracts a vector from a second vector and puts the result into a third vector.
		/// </summary>
		/// <param name="u">A <see cref="Vector3"/> instance.</param>
		/// <param name="v">A <see cref="Vector3"/> instance</param>
		/// <param name="w">A <see cref="Vector3"/> instance to hold the result.</param>
		/// <remarks>
		///	w[i] = v[i] - w[i].
		/// </remarks>
		public static void Subtract(Vector3 u, Vector3 v, ref Vector3 w)
		{
			w.X = u.X - v.X;
			w.Y = u.Y - v.Y;
			w.Z = u.Z - v.Z;
		}
		/// <summary>
		/// Subtracts a vector from a scalar and put the result into another vector.
		/// </summary>
		/// <param name="u">A <see cref="Vector3"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <param name="v">A <see cref="Vector3"/> instance to hold the result.</param>
		/// <remarks>
		/// v[i] = u[i] - s
		/// </remarks>
		public static void Subtract(Vector3 u, double s, ref Vector3 v)
		{
			v.X = u.X - s;
			v.Y = u.Y - s;
			v.Z = u.Z - s;
		}
		/// <summary>
		/// Subtracts a scalar from a vector and put the result into another vector.
		/// </summary>
		/// <param name="u">A <see cref="Vector3"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <param name="v">A <see cref="Vector3"/> instance to hold the result.</param>
		/// <remarks>
		/// v[i] = s - u[i]
		/// </remarks>
		public static void Subtract(double s, Vector3 u, ref Vector3 v)
		{
			v.X = s - u.X;
			v.Y = s - u.Y;
			v.Z = s - u.Z;
		}
		/// <summary>
		/// Divides a vector by another vector.
		/// </summary>
		/// <param name="u">A <see cref="Vector3"/> instance.</param>
		/// <param name="v">A <see cref="Vector3"/> instance.</param>
		/// <returns>A new <see cref="Vector3"/> containing the quotient.</returns>
		/// <remarks>
		///	result[i] = u[i] / v[i].
		/// </remarks>
		public static Vector3 Divide(Vector3 u, Vector3 v)
		{
			return new Vector3(u.X / v.X, u.Y / v.Y, u.Z / v.Z);
		}
		/// <summary>
		/// Divides a vector by a scalar.
		/// </summary>
		/// <param name="v">A <see cref="Vector3"/> instance.</param>
		/// <param name="s">A scalar</param>
		/// <returns>A new <see cref="Vector3"/> containing the quotient.</returns>
		/// <remarks>
		/// result[i] = v[i] / s;
		/// </remarks>
		public static Vector3 Divide(Vector3 v, double s)
		{
			return new Vector3(v.X / s, v.Y / s, v.Z / s);
		}
		/// <summary>
		/// Divides a scalar by a vector.
		/// </summary>
		/// <param name="v">A <see cref="Vector3"/> instance.</param>
		/// <param name="s">A scalar</param>
		/// <returns>A new <see cref="Vector3"/> containing the quotient.</returns>
		/// <remarks>
		/// result[i] = s / v[i]
		/// </remarks>
		public static Vector3 Divide(double s, Vector3 v)
		{
			return new Vector3(s / v.X, s/ v.Y, s / v.Z);
		}
		/// <summary>
		/// Divides a vector by another vector.
		/// </summary>
		/// <param name="u">A <see cref="Vector3"/> instance.</param>
		/// <param name="v">A <see cref="Vector3"/> instance.</param>
		/// <param name="w">A <see cref="Vector3"/> instance to hold the result.</param>
		/// <remarks>
		/// w[i] = u[i] / v[i]
		/// </remarks>
		public static void Divide(Vector3 u, Vector3 v, ref Vector3 w)
		{
			w.X = u.X / v.X;
			w.Y = u.Y / v.Y;
			w.Z = u.Z / v.Z;
		}
		/// <summary>
		/// Divides a vector by a scalar.
		/// </summary>
		/// <param name="u">A <see cref="Vector3"/> instance.</param>
		/// <param name="s">A scalar</param>
		/// <param name="v">A <see cref="Vector3"/> instance to hold the result.</param>
		/// <remarks>
		/// v[i] = u[i] / s
		/// </remarks>
		public static void Divide(Vector3 u, double s, ref Vector3 v)
		{
			v.X = u.X / s;
			v.Y = u.Y / s;
			v.Z = u.Z / s;
		}
		/// <summary>
		/// Divides a scalar by a vector.
		/// </summary>
		/// <param name="u">A <see cref="Vector3"/> instance.</param>
		/// <param name="s">A scalar</param>
		/// <param name="v">A <see cref="Vector3"/> instance to hold the result.</param>
		/// <remarks>
		/// v[i] = s / u[i]
		/// </remarks>
		public static void Divide(double s, Vector3 u, ref Vector3 v)
		{
			v.X = s / u.X;
			v.Y = s / u.Y;
			v.Z = s / u.Z;
		}
		/// <summary>
		/// Multiplies a vector by a scalar.
		/// </summary>
		/// <param name="u">A <see cref="Vector3"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Vector3"/> containing the result.</returns>
		public static Vector3 Multiply(Vector3 u, double s)
		{
			return new Vector3(u.X * s, u.Y * s, u.Z * s);
		}
		/// <summary>
		/// Multiplies a vector by a scalar and put the result in another vector.
		/// </summary>
		/// <param name="u">A <see cref="Vector3"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <param name="v">A <see cref="Vector3"/> instance to hold the result.</param>
		public static void Multiply(Vector3 u, double s, ref Vector3 v)
		{
			v.X = u.X * s;
			v.Y = u.Y * s;
			v.Z = u.Z * s;
		}
		/// <summary>
		/// Calculates the dot product of two vectors.
		/// </summary>
		/// <param name="u">A <see cref="Vector3"/> instance.</param>
		/// <param name="v">A <see cref="Vector3"/> instance.</param>
		/// <returns>The dot product value.</returns>
		public static double DotProduct(Vector3 u, Vector3 v)
		{
			return (u.X * v.X) + (u.Y * v.Y) + (u.Z * v.Z);
		}
		/// <summary>
		/// Calculates the cross product of two vectors.
		/// </summary>
		/// <param name="u">A <see cref="Vector3"/> instance.</param>
		/// <param name="v">A <see cref="Vector3"/> instance.</param>
		/// <returns>A new <see cref="Vector3"/> containing the cross product result.</returns>
		public static Vector3 CrossProduct(Vector3 u, Vector3 v)
		{
			return new Vector3( 
				u.Y*v.Z - u.Z*v.Y, 
				u.Z*v.X - u.X*v.Z, 
				u.X*v.Y - u.Y*v.X );
		}
		/// <summary>
		/// Calculates the cross product of two vectors.
		/// </summary>
		/// <param name="u">A <see cref="Vector3"/> instance.</param>
		/// <param name="v">A <see cref="Vector3"/> instance.</param>
		/// <param name="w">A <see cref="Vector3"/> instance to hold the cross product result.</param>
		public static void CrossProduct(Vector3 u, Vector3 v, ref Vector3 w)
		{
			w.X = u.Y*v.Z - u.Z*v.Y;
			w.Y = u.Z*v.X - u.X*v.Z;
			w.Z = u.X*v.Y - u.Y*v.X;
		}
		/// <summary>
		/// Negates a vector.
		/// </summary>
		/// <param name="v">A <see cref="Vector3"/> instance.</param>
		/// <returns>A new <see cref="Vector3"/> instance containing the negated values.</returns>
		public static Vector3 Negate(Vector3 v)
		{
			return new Vector3(-v.X, -v.Y, -v.Z);
		}
		/// <summary>
		/// Tests whether two vectors are approximately equal using default tolerance value.
		/// </summary>
		/// <param name="v">A <see cref="Vector3"/> instance.</param>
		/// <param name="u">A <see cref="Vector3"/> instance.</param>
		/// <returns><see langword="true"/> if the two vectors are approximately equal; otherwise, <see langword="false"/>.</returns>
		public static bool ApproxEqual(Vector3 v, Vector3 u)
		{
			return ApproxEqual(v,u, MathFunctions.EpsilonF);
		}
		/// <summary>
		/// Tests whether two vectors are approximately equal given a tolerance value.
		/// </summary>
		/// <param name="v">A <see cref="Vector3"/> instance.</param>
		/// <param name="u">A <see cref="Vector3"/> instance.</param>
		/// <param name="tolerance">The tolerance value used to test approximate equality.</param>
		/// <returns><see langword="true"/> if the two vectors are approximately equal; otherwise, <see langword="false"/>.</returns>
		public static bool ApproxEqual(Vector3 v, Vector3 u, double tolerance)
		{
			return
				(
				(System.Math.Abs(v.X - u.X) <= tolerance) &&
				(System.Math.Abs(v.Y - u.Y) <= tolerance)
				);
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Scale the vector so that its length is 1.
		/// </summary>
		public void Normalize()
		{
			double length = GetLength();
			if (length == 0)
			{
				throw new DivideByZeroException("Trying to normalize a vector with length of zero.");
			}

			m_X /= length;
			m_Y /= length;
			m_Z /= length;
		}
		/// <summary>
		/// Returns a new <see cref="Vector3"/> instance representing the unit vector of the current vector.
		/// </summary>
		/// <returns>A <see cref="Vector3"/> instance.</returns>
		public Vector3 GetUnit()
		{
			Vector3 result = new Vector3(this);
			result.Normalize();
			return result;
		}
		/// <summary>
		/// Returns the length of the vector.
		/// </summary>
		/// <returns>The length of the vector. (Sqrt(X*X + Y*Y + Z*Z))</returns>
		public double GetLength()
		{
			return System.Math.Sqrt(m_X*m_X + m_Y*m_Y + m_Z*m_Z);
		}
		/// <summary>
		/// Returns the squared length of the vector.
		/// </summary>
		/// <returns>The squared length of the vector. (X*X + Y*Y + Z*Z)</returns>
		public double GetLengthSquared()
		{
			return (m_X*m_X + m_Y*m_Y + m_Z*m_Z);
		}
		/// <summary>
		/// Clamps vector values to zero using a given tolerance value.
		/// </summary>
		/// <param name="tolerance">The tolerance to use.</param>
		/// <remarks>
		/// The vector values that are close to zero within the given tolerance are set to zero.
		/// </remarks>
		public void ClampZero(double tolerance)
		{
			m_X = MathFunctions.Clamp(m_X, 0.0f, tolerance);
			m_Y = MathFunctions.Clamp(m_Y, 0.0f, tolerance);
			m_Z = MathFunctions.Clamp(m_Z, 0.0f, tolerance);
		}
		/// <summary>
		/// Clamps vector values to zero using the default tolerance value.
		/// </summary>
		/// <remarks>
		/// The vector values that are close to zero within the given tolerance are set to zero.
		/// The tolerance value used is <see cref="MathFunctions.EpsilonF"/>
		/// </remarks>
		public void ClampZero()
		{
			m_X = MathFunctions.Clamp(m_X, 0.0f);
			m_Y = MathFunctions.Clamp(m_Y, 0.0f);
			m_Z = MathFunctions.Clamp(m_Z, 0.0f);
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Returns the hashcode for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return m_X.GetHashCode() ^ m_Y.GetHashCode() ^ m_Z.GetHashCode();
		}
		/// <summary>
		/// Returns a value indicating whether this instance is equal to
		/// the specified object.
		/// </summary>
		/// <param name="obj">An object to compare to this instance.</param>
		/// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="Vector3"/> and has the same values as this instance; otherwise, <see langword="false"/>.</returns>
		public override bool Equals(object obj)
		{
			if (obj is Vector3)
			{
				Vector3 v = (Vector3)obj;
				return (m_X == v.X) && (m_Y == v.Y) && (m_Z == v.Z);
			}
			return false;
		}

		/// <summary>
		/// Returns a string representation of this object.
		/// </summary>
		/// <returns>A string representation of this object.</returns>
		public override string ToString()
		{
			return string.Format("({0}, {1}, {2})", m_X, m_Y, m_Z);
		}
		#endregion
		
		#region Comparison Operators
		/// <summary>
		/// Tests whether two specified vectors are equal.
		/// </summary>
		/// <param name="u">The left-hand vector.</param>
		/// <param name="v">The right-hand vector.</param>
		/// <returns><see langword="true"/> if the two vectors are equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator==(Vector3 u, Vector3 v)
		{
			return ValueType.Equals(u,v);
		}
		/// <summary>
		/// Tests whether two specified vectors are not equal.
		/// </summary>
		/// <param name="u">The left-hand vector.</param>
		/// <param name="v">The right-hand vector.</param>
		/// <returns><see langword="true"/> if the two vectors are not equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator!=(Vector3 u, Vector3 v)
		{
			return !ValueType.Equals(u,v);
		}
		/// <summary>
		/// Tests if a vector's components are greater than another vector's components.
		/// </summary>
		/// <param name="u">The left-hand vector.</param>
		/// <param name="v">The right-hand vector.</param>
		/// <returns><see langword="true"/> if the left-hand vector's components are greater than the right-hand vector's component; otherwise, <see langword="false"/>.</returns>
		public static bool operator>(Vector3 u, Vector3 v)
		{
			return (
				(u.m_X > v.m_X) && 
				(u.m_Y > v.m_Y) && 
				(u.m_Z > v.m_Z));
		}
		/// <summary>
		/// Tests if a vector's components are smaller than another vector's components.
		/// </summary>
		/// <param name="u">The left-hand vector.</param>
		/// <param name="v">The right-hand vector.</param>
		/// <returns><see langword="true"/> if the left-hand vector's components are smaller than the right-hand vector's component; otherwise, <see langword="false"/>.</returns>
		public static bool operator<(Vector3 u, Vector3 v)
		{
			return (
				(u.m_X < v.m_X) && 
				(u.m_Y < v.m_Y) && 
				(u.m_Z < v.m_Z));
		}
		/// <summary>
		/// Tests if a vector's components are greater or equal than another vector's components.
		/// </summary>
		/// <param name="u">The left-hand vector.</param>
		/// <param name="v">The right-hand vector.</param>
		/// <returns><see langword="true"/> if the left-hand vector's components are greater or equal than the right-hand vector's component; otherwise, <see langword="false"/>.</returns>
		public static bool operator>=(Vector3 u, Vector3 v)
		{
			return (
				(u.m_X >= v.m_X) && 
				(u.m_Y >= v.m_Y) && 
				(u.m_Z >= v.m_Z));
		}
		/// <summary>
		/// Tests if a vector's components are smaller or equal than another vector's components.
		/// </summary>
		/// <param name="u">The left-hand vector.</param>
		/// <param name="v">The right-hand vector.</param>
		/// <returns><see langword="true"/> if the left-hand vector's components are smaller or equal than the right-hand vector's component; otherwise, <see langword="false"/>.</returns>
		public static bool operator<=(Vector3 u, Vector3 v)
		{
			return (
				(u.m_X <= v.m_X) && 
				(u.m_Y <= v.m_Y) && 
				(u.m_Z <= v.m_Z));
		}
		#endregion

		#region Unary Operators
		/// <summary>
		/// Negates the values of the vector.
		/// </summary>
		/// <param name="v">A <see cref="Vector3"/> instance.</param>
		/// <returns>A new <see cref="Vector3"/> instance containing the negated values.</returns>
		public static Vector3 operator-(Vector3 v)
		{
			return Vector3.Negate(v);
		}
		#endregion

		#region Binary Operators
		/// <summary>
		/// Adds two vectors.
		/// </summary>
		/// <param name="u">A <see cref="Vector3"/> instance.</param>
		/// <param name="v">A <see cref="Vector3"/> instance.</param>
		/// <returns>A new <see cref="Vector3"/> instance containing the sum.</returns>
		public static Vector3 operator+(Vector3 u, Vector3 v)
		{
			return Vector3.Add(u,v);
		}
		/// <summary>
		/// Adds a vector and a scalar.
		/// </summary>
		/// <param name="v">A <see cref="Vector3"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Vector3"/> instance containing the sum.</returns>
		public static Vector3 operator+(Vector3 v, double s)
		{
			return Vector3.Add(v,s);
		}
		/// <summary>
		/// Adds a vector and a scalar.
		/// </summary>
		/// <param name="v">A <see cref="Vector3"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Vector3"/> instance containing the sum.</returns>
		public static Vector3 operator+(double s, Vector3 v)
		{
			return Vector3.Add(v,s);
		}
		/// <summary>
		/// Subtracts a vector from a vector.
		/// </summary>
		/// <param name="u">A <see cref="Vector3"/> instance.</param>
		/// <param name="v">A <see cref="Vector3"/> instance.</param>
		/// <returns>A new <see cref="Vector3"/> instance containing the difference.</returns>
		/// <remarks>
		///	result[i] = v[i] - w[i].
		/// </remarks>
		public static Vector3 operator-(Vector3 u, Vector3 v)
		{
			return Vector3.Subtract(u,v);
		}
		/// <summary>
		/// Subtracts a scalar from a vector.
		/// </summary>
		/// <param name="v">A <see cref="Vector3"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Vector3"/> instance containing the difference.</returns>
		/// <remarks>
		/// result[i] = v[i] - s
		/// </remarks>
		public static Vector3 operator-(Vector3 v, double s)
		{
			return Vector3.Subtract(v, s);
		}
		/// <summary>
		/// Subtracts a vector from a scalar.
		/// </summary>
		/// <param name="v">A <see cref="Vector3"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Vector3"/> instance containing the difference.</returns>
		/// <remarks>
		/// result[i] = s - v[i]
		/// </remarks>
		public static Vector3 operator-(double s, Vector3 v)
		{
			return Vector3.Subtract(s, v);
		}

		/// <summary>
		/// Multiplies a vector by a scalar.
		/// </summary>
		/// <param name="v">A <see cref="Vector3"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Vector3"/> containing the result.</returns>
		public static Vector3 operator*(Vector3 v, double s)
		{
			return Vector3.Multiply(v,s);
		}
		/// <summary>
		/// Multiplies a vector by a scalar.
		/// </summary>
		/// <param name="v">A <see cref="Vector3"/> instance.</param>
		/// <param name="s">A scalar.</param>
		/// <returns>A new <see cref="Vector3"/> containing the result.</returns>
		public static Vector3 operator*(double s, Vector3 v)
		{
			return Vector3.Multiply(v,s);
		}
		/// <summary>
		/// Divides a vector by a scalar.
		/// </summary>
		/// <param name="v">A <see cref="Vector3"/> instance.</param>
		/// <param name="s">A scalar</param>
		/// <returns>A new <see cref="Vector3"/> containing the quotient.</returns>
		/// <remarks>
		/// result[i] = v[i] / s;
		/// </remarks>
		public static Vector3 operator/(Vector3 v, double s)
		{
			return Vector3.Divide(v,s);
		}
		/// <summary>
		/// Divides a scalar by a vector.
		/// </summary>
		/// <param name="v">A <see cref="Vector3"/> instance.</param>
		/// <param name="s">A scalar</param>
		/// <returns>A new <see cref="Vector3"/> containing the quotient.</returns>
		/// <remarks>
		/// result[i] = s / v[i]
		/// </remarks>
		public static Vector3 operator/(double s, Vector3 v)
		{
			return Vector3.Divide(s,v);
		}
		#endregion

		#region Array Indexing Operator
		/// <summary>
		/// Indexer ( [x, y] ).
		/// </summary>
		public double this[int index]
		{
			get	
			{
				switch( index ) 
				{
					case 0:
						return m_X;
					case 1:
						return m_Y;
					case 2:
						return m_Z;
					default:
						throw new IndexOutOfRangeException();
				}
			}
			set 
			{
				switch( index ) 
				{
					case 0:
						m_X = value;
						break;
					case 1:
						m_Y = value;
						break;
					case 2:
						m_Z = value;
						break;
					default:
						throw new IndexOutOfRangeException();
				}
			}

		}

		#endregion

		#region Conversion Operators
		/// <summary>
		/// Converts the vector to an array of single-precision doubleing point values.
		/// </summary>
		/// <param name="v">A <see cref="Vector3"/> instance.</param>
		/// <returns>An array of single-precision doubleing point values.</returns>
		public static explicit operator double[](Vector3 v)
		{
			double[] array = new double[3];
			array[0] = v.X;
			array[1] = v.Y;
			array[2] = v.Z;
			return array;
		}
		/// <summary>
		/// Converts the vector to an array of single-precision doubleing point values.
		/// </summary>
		/// <param name="v">A <see cref="Vector3"/> instance.</param>
		/// <returns>An array of single-precision doubleing point values.</returns>
        public static explicit operator DoubleArrayList(Vector3 v)
		{
            DoubleArrayList array = new DoubleArrayList(3);
			array.Add(v.X);
			array.Add(v.Y);
			array.Add(v.Z);
			return array;
		}

        public static explicit operator Vector4(Vector3 v)
        {
            return new Vector4(v.X, v.Y, v.Z, 0);
        }

		#endregion

        /*public System.Windows.Media.Media3D.Point3D getPoint3D()
        {
            return new System.Windows.Media.Media3D.Point3D(_x, _y, _z);
        }*/

        public double[,] GetOnPlaneDoubleMartix()
        {
            return new double[, ] { 
                {m_X, m_Y, m_Z }
            };
        }
    }

	#region Vector3DConverter class
	/// <summary>
	/// Converts a <see cref="Vector3"/> to and from string representation.
	/// </summary>
	public class Vector3DConverter : ExpandableObjectConverter
	{
		/// <summary>
		/// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="sourceType">A <see cref="Type"/> that represents the type you want to convert from.</param>
		/// <returns><b>true</b> if this converter can perform the conversion; otherwise, <b>false</b>.</returns>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;

			return base.CanConvertFrom (context, sourceType);
		}
		/// <summary>
		/// Returns whether this converter can convert the object to the specified type, using the specified context.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="destinationType">A <see cref="Type"/> that represents the type you want to convert to.</param>
		/// <returns><b>true</b> if this converter can perform the conversion; otherwise, <b>false</b>.</returns>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
				return true;

			return base.CanConvertTo (context, destinationType);
		}
		/// <summary>
		/// Converts the given value object to the specified type, using the specified context and culture information.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="culture">A <see cref="System.Globalization.CultureInfo"/> object. If a null reference (Nothing in Visual Basic) is passed, the current culture is assumed.</param>
		/// <param name="value">The <see cref="Object"/> to convert.</param>
		/// <param name="destinationType">The Type to convert the <paramref name="value"/> parameter to.</param>
		/// <returns>An <see cref="Object"/> that represents the converted value.</returns>
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if ((destinationType == typeof(string)) && (value is Vector3))
			{
				Vector3 v = (Vector3)value;
				return v.ToString();
			}

			return base.ConvertTo (context, culture, value, destinationType);
		}
		/// <summary>
		/// Converts the given object to the type of this converter, using the specified context and culture information.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="culture">The <see cref="System.Globalization.CultureInfo"/> to use as the current culture. </param>
		/// <param name="value">The <see cref="Object"/> to convert.</param>
		/// <returns>An <see cref="Object"/> that represents the converted value.</returns>
		/// <exception cref="ParseException">Failed parsing from string.</exception>
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value.GetType() == typeof(string))
			{
				return Vector3.Parse((string)value);
			}

			return base.ConvertFrom (context, culture, value);
		}

		/// <summary>
		/// Returns whether this object supports a standard set of values that can be picked from a list.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
		/// <returns><b>true</b> if <see cref="GetStandardValues"/> should be called to find a common set of values the object supports; otherwise, <b>false</b>.</returns>
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		/// <summary>
		/// Returns a collection of standard values for the data type this type converter is designed for when provided with a format context.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context that can be used to extract additional information about the environment from which this converter is invoked. This parameter or properties of this parameter can be a null reference.</param>
		/// <returns>A <see cref="TypeConverter.StandardValuesCollection"/> that holds a standard set of valid values, or a null reference (Nothing in Visual Basic) if the data type does not support a standard set of values.</returns>
		public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			StandardValuesCollection svc =
                new StandardValuesCollection(new object[4] { Vector3.Zero(), Vector3.XAxis(), Vector3.YAxis(), Vector3.ZAxis() });

			return svc;
		}
	}
	#endregion



}
