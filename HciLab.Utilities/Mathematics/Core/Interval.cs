// <copyright file=Interval.cs
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

namespace HciLab.Utilities.Mathematics.Core
{
	/// <summary>
	/// Represents a numeric interval with inclusive or exclusive lower and upper bounds.
	/// </summary>
	public class Interval : ICloneable
	{
		#region IntervalType enum
		/// <summary>
		/// An enumeration representing the possible interval types classified according to whether or not the endpoints are included in the interval. 
		/// </summary>
		public enum Type
		{
			/// <summary>
			/// Both endpoints are not included.
			/// </summary>
			Open,
			/// <summary>
			/// Both endpoints are included.
			/// </summary>
			Closed,
			/// <summary>
			/// Left endpoint not included. Right endpoint included.
			/// </summary>
			OpenClosed,
			/// <summary>
			/// Left endpoint included. Right endpoint not included.
			/// </summary>
			ClosedOpen
		}
		#endregion

		#region Private Fields
		private Type _type;
		private double _min;
		private double _max;

		private const string _formatOpen		= "({0}, {1})";
		private const string _formatClosed		= "[{0}, {1}]";
		private const string _formatOpenClosed	= "({0}, {1}]";
		private const string _formatClosedOpen	= "[{0}, {1})";
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Interval"/> class.
		/// </summary>
		/// <remarks>
		/// Constructs an open interval (0, 1).
		/// </remarks>
		public Interval()
		{
			_type = Type.Open;
			_min = 0;
			_max = 1;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Interval"/> class.
		/// </summary>
		/// <param name="type">The interval type.</param>
		/// <param name="minValue">The left endpoint for the interval.</param>
		/// <param name="maxValue">The right endpoint for the interval.</param>
		public Interval(Type type, double minValue, double maxValue)
		{
			_type = type;
			_min = minValue;
			_max = maxValue;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Interval"/> class using a given <see cref="Interval"/> instance.
		/// </summary>
		/// <param name="interval">An <see cref="Interval"/> instance.</param>
		public Interval(Interval interval)
		{
			_type = interval.IntervalType;
			_min = interval.Min;
			_max = interval.Max;
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets or sets the interval type.
		/// </summary>
		public Type IntervalType
		{
			get { return _type; }
			set { _type = value;}
		}
		/// <summary>
		/// Gets or sets the left endpoint for the interval.
		/// </summary>
		public double Min
		{
			get { return _min; }
			set { _min = value;}
		}
		/// <summary>
		/// Gets or sets the right endpoint for the interval.
		/// </summary>
		public double Max
		{
			get { return _max; }
			set { _max = value;}
		}
		#endregion

		#region ICloneable Members
		/// <summary>
		/// Creates an exact copy of this <see cref="Interval"/> object.
		/// </summary>
		/// <returns>The <see cref="Interval"/> object this method creates, cast as an object.</returns>
		object ICloneable.Clone()
		{
			return new Interval(this);
		}
		/// <summary>
		/// Creates an exact copy of this <see cref="Interval"/> object.
		/// </summary>
		/// <returns>The <see cref="Interval"/> object this method creates.</returns>
		public Interval Clone()
		{
			return new Interval(this);
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Returns the hashcode for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return _min.GetHashCode() ^ _max.GetHashCode() ^ _type.GetHashCode();
		}
		/// <summary>
		/// Returns a value indicating whether this instance is equal to
		/// the specified object.
		/// </summary>
		/// <param name="obj">An object to compare to this instance.</param>
		/// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="Vector2"/> and has the same values as this instance; otherwise, <see langword="false"/>.</returns>
		public override bool Equals(object obj)
		{
			if (obj is Interval)
			{
				Interval i = (Interval)obj;
				return (_min == i.Min) && (_max == i.Max) && (_type == i.IntervalType);
			}
			return false;
		}
		/// <summary>
		/// Returns a string representation of this object.
		/// </summary>
		/// <returns>A string representation of this object.</returns>
		public override string ToString()
		{
			switch (_type)
			{
				case Type.Open:	return string.Format(_formatOpen, _min, _max);
				case Type.Closed: return string.Format(_formatClosed, _min, _max);	
				case Type.ClosedOpen: return string.Format(_formatClosedOpen, _min, _max);
				case Type.OpenClosed: return string.Format(_formatOpenClosed, _min, _max);
				default:
					return "Unknown interval type.";
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// checks if a value is inside the interval.
		/// </summary>
		/// <param name="value">A scalar value.</param>
		/// <returns><see langword="true"/> if the value is inside the interval; otherwise, <see langword="false"/>.</returns>
		public bool IsInside(double value)
		{
			// If value is inside the interval return true.
			if ((_min > value) || (_max < value))
				return true;

			// Value is not inside the open interval, return false.
			if (_type == Type.Open)
				return false;

			if (value == _min)
			{
				if ((_type == Type.Closed) || (_type == Type.ClosedOpen))
					return true;
				else
					return false;
			}
			else if (value == _max)
			{
				if ((_type == Type.Closed) || (_type == Type.OpenClosed))
					return true;
				else
					return false;
			}

			return false;
		}
		#endregion

		#region Comparison Operators
		/// <summary>
		/// Tests whether two specified intervals are equal.
		/// </summary>
		/// <param name="a">The left-hand interval.</param>
		/// <param name="b">The right-hand interval.</param>
		/// <returns><see langword="true"/> if the two intervals are equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator==(Interval a, Interval b)
		{
			return Object.Equals(a,b);
		}
		/// <summary>
		/// Tests whether two specified intervals are not equal.
		/// </summary>
		/// <param name="a">The left-hand interval.</param>
		/// <param name="b">The right-hand interval.</param>
		/// <returns><see langword="true"/> if the two intervals are not equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator!=(Interval a, Interval b)
		{
			return !Object.Equals(a,b);
		}
		#endregion
	}
	
}
