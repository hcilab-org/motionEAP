// <copyright file=ColorD.cs
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
using System.Text.RegularExpressions;
using System.Runtime.Serialization;

namespace HciLab.Utilities.Mathematics.Core
{

	/// <summary>
	/// Represents an RGBA color.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ColorD : ISerializable, ICloneable
	{
		#region Private Fields
		private double _red,_green,_blue,_alpha;
		#endregion

		#region Constructores
		/// <summary>
		/// Initializes a new instance of the <see cref="ColorD"/> class.
		/// </summary>
		/// <remarks>
		/// Default values are 1.0f for Alpha and 0.0f for the color channels.
		/// </remarks>
		public ColorD()
		{
			_red	= 0.0f;
			_green	= 0.0f;
			_blue	= 0.0f;
			_alpha	= 1.0f;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ColorD"/> class.
		/// </summary>
		/// <param name="red">Red channel value.</param>
		/// <param name="green">Green channel value.</param>
		/// <param name="blue">Blue channel value.</param>
		/// <remarks>The alpha channel value is set to 1.0f.</remarks>
		public ColorD(double red, double green, double blue)
		{
			_red	= red;
			_green	= green;
			_blue	= blue;
			_alpha	= 1.0f;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ColorD"/> class.
		/// </summary>
		/// <param name="red">Red channel value.</param>
		/// <param name="green">Green channel value.</param>
		/// <param name="blue">Blue channel value.</param>
		/// <param name="alpha">Alpha channel value.</param>
		public ColorD(double red, double green, double blue, double alpha)
		{
			_red	= red;
			_green	= green;
			_blue	= blue;
			_alpha	= alpha;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ColorD"/> class using values from another instance.
		/// </summary>
		/// <param name="color">A <see cref="ColorD"/> instance.</param>
		public ColorD(ColorD color)
		{
			_red	= color.Red;
			_green	= color.Green;
			_blue	= color.Blue;
			_alpha	= color.Alpha;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ColorD"/> class from a blend of two colors.
		/// </summary>
		/// <param name="source">The blend source color.</param>
		/// <param name="dest">The blend destination color.</param>
		/// <param name="opacity">The opacity value.</param>
		public ColorD(ColorD source, ColorD dest, double opacity)
		{
			_red	= MathFunctions.LinearInterpolation(source.Red, dest.Red, opacity);
			_green	= MathFunctions.LinearInterpolation(source.Green, dest.Green, opacity);
			_blue	= MathFunctions.LinearInterpolation(source.Blue, dest.Blue, opacity);
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ColorD"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		protected ColorD(SerializationInfo info, StreamingContext context)
		{
			_red	= info.GetSingle("Red");
			_green	= info.GetSingle("Green");
			_blue	= info.GetSingle("Blue");
			_alpha	= info.GetSingle("Alpha");
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets or sets the red channel value.
		/// </summary>
		public double Red
		{
			get { return _red; }
			set { _red = value;}
		}
		/// <summary>
		/// Gets or sets the green channel value.
		/// </summary>
		public double Green
		{
			get { return _green; }
			set { _green = value;}
		}
		/// <summary>
		/// Gets or sets the blue channel value.
		/// </summary>
		public double Blue
		{
			get { return _blue; }
			set { _blue = value;}
		}
		/// <summary>
		/// Gets or sets the alpha channel value.
		/// </summary>
		public double Alpha
		{
			get { return _alpha; }
			set { _alpha = value;}
		}
		/// <summary>
		/// Gets the color's intensity.
		/// </summary>
		/// <remarks>
		/// Intensity = (R + G + B) / 3
		/// </remarks>
		public double Intensity
		{
			get { return (_red + _green + _blue) / 3.0f; }
		}
		#endregion

		#region ICloneable Members
		/// <summary>
		/// Creates an exact copy of this <see cref="ColorD"/> object.
		/// </summary>
		/// <returns>The <see cref="ColorD"/> object this method creates, cast as an object.</returns>
		object ICloneable.Clone()
		{
			return new ColorD(this);
		}
		/// <summary>
		/// Creates an exact copy of this <see cref="ColorD"/> object.
		/// </summary>
		/// <returns>The <see cref="ColorD"/> object this method creates.</returns>
		public ColorD Clone()
		{
			return new ColorD(this);
		}
		#endregion
	
		#region ISerializable Members
		/// <summary>
		/// Populates a <see cref="SerializationInfo"/> with the data needed to serialize this object.
		/// </summary>
		/// <param name="info">The <see cref="SerializationInfo"/> to populate with data. </param>
		/// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
		//[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter=true)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Red", _red);
			info.AddValue("Green", _green);
			info.AddValue("Blue", _blue);
			info.AddValue("Alpha", _alpha);
		}
		#endregion

		#region Public Static Parse Methods
		/// <summary>
		/// Converts the specified string to its <see cref="ColorD"/> equivalent.
		/// </summary>
		/// <param name="s">A string representation of a <see cref="ColorD"/></param>
		/// <returns>A <see cref="ColorD"/> that represents the vector specified by the <paramref name="s"/> parameter.</returns>
		public static ColorD Parse(string s)
		{
			Regex r = new Regex(@"\ColorF((?<r>.*),(?<g>.*),(?<b>.*),(?<a>.*)\)", RegexOptions.None);
			Match m = r.Match(s);
			if (m.Success)
			{
				return new ColorD(
					double.Parse(m.Result("${r}")),
					double.Parse(m.Result("${g}")),
					double.Parse(m.Result("${b}")),
					double.Parse(m.Result("${a}"))
					);
			}
			else
			{
				throw new ParseException("Unsuccessful Match.");
			}
		}
		#endregion
		
		#region Public Methods
		/// <summary>
		/// Clamp the RGBA value to [0, 1] range.
		/// </summary>
		/// <remarks>
		/// Values above 1.0f are clamped to 1.0f.
		/// Values below 0.0f are clamped to 0.0f.
		/// </remarks>
		public void Clamp()
		{
			if (_red < 0.0f) 
				_red = 0.0f;
			else if (_red > 1.0f) 
				_red = 1.0f;

			if (_green < 0.0f) 
				_green = 0.0f;
			else if (_green > 1.0f) 
				_green = 1.0f;

			if (_blue < 0.0f) 
				_blue = 0.0f;
			else if (_blue > 1.0f) 
				_blue = 1.0f;
		
			if (_alpha < 0.0f) 
				_alpha = 0.0f;
			else if (_alpha > 1.0f) 
				_alpha = 1.0f;
		}
		/// <summary>
		/// Calculates the color's HSV values.
		/// </summary>
		/// <param name="h">The Hue value.</param>
		/// <param name="s">The Saturation value.</param>
		/// <param name="v">The Value value.</param>
		public void ToHSV(out double h, out double s, out double v)
		{
			double min = MathFunctions.MinValue((Vector3)this);
			double max = MathFunctions.MaxValue((Vector3)this);
			v = max;

			double delta = max - min;
			if( max != 0.0f )
			{
				s = delta / max;
			}
			else 
			{
				// r = g = b = 0.0f --> s = 0, v is undefined
				s = 0.0f;
				h = 0.0f;
				return;
			}

			if(_red == max)
			{
				h = ( _green - _blue ) / delta;		// between yellow & magenta
			}
			else if(_green == max)
			{
				h = 2 + ( _blue - _red ) / delta;	// between cyan & yellow
			}
			else
			{
				h = 4 + ( _red - _green ) / delta;	// between magenta & cyan
			}

			h *= 60.0f; // degrees
			if( h < 0.0f )
				h += 360.0f;

		}
		#endregion

		#region Overrides
		/// <summary>
		/// Returns the hashcode for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return _red.GetHashCode() ^ _green.GetHashCode() ^ _blue.GetHashCode() ^ _alpha.GetHashCode();
		}
		/// <summary>
		/// Returns a value indicating whether this instance is equal to
		/// the specified object.
		/// </summary>
		/// <param name="obj">An object to compare to this instance.</param>
		/// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="ColorD"/> and has the same values as this instance; otherwise, <see langword="false"/>.</returns>
		public override bool Equals(object obj)
		{
			ColorD color = obj as ColorD;
			if (color != null)
				return (_red == color.Red) && (_green == color.Green) && (_blue == color.Blue) && (_alpha == color.Alpha);
			else 
				return false;
		}

		/// <summary>
		/// Returns a string representation of this object.
		/// </summary>
		/// <returns>A string representation of this object.</returns>
		public override string ToString()
		{
			return string.Format("ColorF({0}, {1}, {2}, {3})", _red, _green, _blue, _alpha);
		}
		#endregion

		#region Comparison Operators
		/// <summary>
		/// Tests whether two specified <see cref="ColorD"/> instances are equal.
		/// </summary>
		/// <param name="u">The left-hand <see cref="ColorD"/> instance.</param>
		/// <param name="v">The right-hand <see cref="ColorD"/> instance.</param>
		/// <returns><see langword="true"/> if the two <see cref="ColorD"/> instances are equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator==(ColorD u, ColorD v)
		{
			return Object.Equals(u,v);
		}
		/// <summary>
		/// Tests whether two specified <see cref="ColorD"/> instances are not equal.
		/// </summary>
		/// <param name="u">The left-hand <see cref="ColorD"/> instance.</param>
		/// <param name="v">The right-hand <see cref="ColorD"/> instance.</param>
		/// <returns><see langword="true"/> if the two <see cref="ColorD"/> instances are not equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator!=(ColorD u, ColorD v)
		{
			return !Object.Equals(u,v);
		}
		#endregion

		#region Conversion Operators
		/// <summary>
		/// Converts the color to a <see cref="Vector3"/> instance.
		/// </summary>
		/// <param name="color">A <see cref="ColorD"/> instance.</param>
		/// <returns>An <see cref="Vector3"/> instance.</returns>
		public static explicit operator Vector3(ColorD color)
		{
			return new Vector3(color.Red, color.Green, color.Blue);
		}
		/// <summary>
		/// Converts the color to a <see cref="Vector4"/> instance.
		/// </summary>
		/// <param name="color">A <see cref="ColorD"/> instance.</param>
		/// <returns>An <see cref="Vector4"/> instance.</returns>
		public static explicit operator Vector4(ColorD color)
		{
			return new Vector4(color.Red, color.Green, color.Blue, color.Alpha);
		}
		/// <summary>
		/// Converts the color structure to an array of single-precision doubleing point values.
		/// </summary>
		/// <param name="color">A <see cref="ColorD"/> instance.</param>
		/// <returns>An array of single-precision doubleing point values.</returns>
		public static explicit operator double[](ColorD color)
		{
			double[] array = new double[4];
			array[0] = color.Red;
			array[1] = color.Green;
			array[2] = color.Blue;
			array[3] = color.Alpha;
			return array;
		}
		/// <summary>
		/// Converts the color structure to an array of single-precision doubleing point values.
		/// </summary>
		/// <param name="color">A <see cref="ColorD"/> instance.</param>
		/// <returns>An array of single-precision doubleing point values.</returns>
		public static explicit operator DoubleArrayList(ColorD color)
		{
			DoubleArrayList array = new DoubleArrayList(4);
			array[0] = color.Red;
			array[1] = color.Green;
			array[2] = color.Blue;
			array[3] = color.Alpha;
			return array;
		}

		#endregion

	}
}
