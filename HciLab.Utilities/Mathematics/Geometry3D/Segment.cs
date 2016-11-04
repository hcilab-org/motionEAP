// <copyright file=Segment.cs
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
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace HciLab.Utilities.Mathematics.Geometry3D
{
	/// <summary>
	/// Represents a segment in 3D space.
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	[TypeConverter(typeof(SegmentConverter))]
	public struct Segment : ICloneable, ISerializable
	{
		#region Private Fields
		private Vector3 _p0;
		private Vector3 _p1;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Segment"/> class.
		/// </summary>
		/// <param name="p0">A <see cref="Vector3"/> instance marking the segment's starting point.</param>
		/// <param name="p1">A <see cref="Vector3"/> instance marking the segment's ending point.</param>
		public Segment(Vector3 p0, Vector3 p1)
		{
			_p0 = p0;
			_p1 = p1;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Segment"/> class using an existing <see cref="Segment"/> instance.
		/// </summary>
		/// <param name="l">A <see cref="Segment"/> instance.</param>
		public Segment(Segment l)
		{
			_p0 = l.P0;
			_p1 = l.P1;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Segment"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		private Segment(SerializationInfo info, StreamingContext context)
		{
			_p0	= (Vector3)info.GetValue("P0", typeof(Vector3));
			_p1	= (Vector3)info.GetValue("P1", typeof(Vector3));
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets or sets the line's first point.
		/// </summary>
		public Vector3 P0
		{
			get { return _p0; }
			set { _p0 = value;}
		}
		/// <summary>
		/// Gets or sets the line's second point.
		/// </summary>
		public Vector3 P1
		{
			get { return _p1; }
			set { _p1 = value;}
		}
		#endregion

		#region ICloneable Members
		/// <summary>
		/// Creates an exact copy of this <see cref="Segment"/> object.
		/// </summary>
		/// <returns>The <see cref="Segment"/> object this method creates, cast as an object.</returns>
		object ICloneable.Clone()
		{
			return new Segment(this);
		}
		/// <summary>
		/// Creates an exact copy of this <see cref="Segment"/> object.
		/// </summary>
		/// <returns>The <see cref="Segment"/> object this method creates.</returns>
		public Segment Clone()
		{
			return new Segment(this);
		}
		#endregion

		#region ISerializable Members
		/// <summary>
		/// Populates a <see cref="SerializationInfo"/> with the data needed to serialize the target object.
		/// </summary>
		/// <param name="info">The <see cref="SerializationInfo"/> to populate with data. </param>
		/// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
		//[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter=true)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("P0", _p0, typeof(Vector3));
			info.AddValue("P1", _p1, typeof(Vector3));
		}
		#endregion

		#region Public Static Parse Methods
		/// <summary>
		/// Converts the specified string to its <see cref="Segment"/> equivalent.
		/// </summary>
		/// <param name="s">A string representation of a <see cref="Segment"/></param>
		/// <returns>A <see cref="Segment"/> that represents the vector specified by the <paramref name="s"/> parameter.</returns>
		public static Segment Parse(string s)
		{
			Regex r = new Regex(@"Segment\(P0=(?<p0>\([^\)]*\)), P1=(?<p1>\([^\)]*\))\)", RegexOptions.None);
			Match m = r.Match(s);
			if (m.Success)
			{
				return new Segment(
					Vector3.Parse(m.Result("${p0}")),
					Vector3.Parse(m.Result("${p1}"))
					);
			}
			else
			{
				throw new ParseException("Unsuccessful Match.");
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Get the hashcode for this vector instance.
		/// </summary>
		/// <returns>Returns the hash code for this vector instance.</returns>
		public override int	GetHashCode() 
		{
			return	_p0.GetHashCode() ^ _p1.GetHashCode();
		}
		/// <summary>
		/// Returns a value indicating whether this instance is equal to
		/// the specified object.
		/// </summary>
		/// <param name="obj">An object to compare to this instance.</param>
		/// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="Vector3"/> and has the same values as this instance; otherwise, <see langword="false"/>.</returns>
		public override bool Equals(object obj) 
		{
			if(obj is Segment) 
			{
				Segment l = (Segment)obj;
				return ((_p0 == l.P0) && (_p1 == l.P1));
			}
			return false;
		}
		/// <summary>
		/// Returns a string representation of this object.
		/// </summary>
		/// <returns>A string representation of this object.</returns>
		public override string ToString() 
		{
			return string.Format("Segment(P0={0}, P1={1})", _p0, _p1);
		}
		#endregion

		#region Comparison Operators
		/// <summary>
		/// Tests whether two specified lines are equal.
		/// </summary>
		/// <param name="a">The left-hand line.</param>
		/// <param name="b">The right-hand line.</param>
		/// <returns><see langword="true"/> if the two lines are equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator==(Segment a, Segment b)
		{
			return ValueType.Equals(a,b);
		}
		/// <summary>
		/// Tests whether two specified lines are not equal.
		/// </summary>
		/// <param name="a">The left-hand line.</param>
		/// <param name="b">The right-hand line.</param>
		/// <returns><see langword="true"/> if the two lines are not equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator!=(Segment a, Segment b)
		{
			return !ValueType.Equals(a,b);
		}
		#endregion
	}

	#region SegmentConverter class
	/// <summary>
	/// Converts a <see cref="Segment"/> to and from string representation.
	/// </summary>
	public class SegmentConverter : ExpandableObjectConverter
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
			if ((destinationType == typeof(string)) && (value is Segment))
			{
				Segment l = (Segment)value;
				return l.ToString();
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
				return Segment.Parse((string)value);
			}

			return base.ConvertFrom (context, culture, value);
		}
	}
	#endregion
}
