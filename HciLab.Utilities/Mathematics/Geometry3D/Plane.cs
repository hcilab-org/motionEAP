// <copyright file=Plane.cs
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace HciLab.Utilities.Mathematics.Geometry3D
{
	/// <summary>
	/// Represents a plane in 3D space.
	/// </summary>
	/// <remarks>
	/// The plane is described by a normal and a constant (N,D) which 
	/// denotes that the plane is consisting of points Q that
	/// satisfies (N dot Q)+D = 0.
	/// </remarks>
	[Serializable]
	[TypeConverter(typeof(PlaneConverter))]
	public class Plane : ISerializable, ICloneable
	{
		#region Private Fields

        //N0
        private Vector3 _normal;
        //D
        private double _const;

		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Plane"/> class using given normal and constant values.
		/// </summary>
		/// <param name="normal">The plane's normal vector.</param>
		/// <param name="constant">The plane's constant value.</param>
        public Plane(Vector3 normal, double constant)
		{
			_normal	= normal;
			_const	= constant;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Plane"/> class using given normal and a point.
		/// </summary>
		/// <param name="normal">The plane's normal vector.</param>
		/// <param name="point">A point on the plane in 3D space.</param>
        public Plane(Vector3 normal, Vector3 point)
		{
			_normal = normal;
            _const = -Vector3.DotProduct(normal, point);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Plane"/> class using 3 given points.
		/// </summary>
		/// <param name="p0">A point on the plane in 3D space.</param>
		/// <param name="p1">A point on the plane in 3D space.</param>
		/// <param name="p2">A point on the plane in 3D space.</param>
        public Plane(Vector3 p0, Vector3 p1, Vector3 p2)
		{
            _normal = Vector3.CrossProduct(p1 - p0, p2 - p0);
		    _normal.Normalize();
            _const = -Vector3.DotProduct(_normal, p0);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Plane"/> class using given a plane to assign values from.
		/// </summary>
		/// <param name="p">A 3D plane to assign values from.</param>
		public Plane(Plane p)
		{
			_normal	= p.Normal;
			_const	= p.Constant;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Plane"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		private Plane(SerializationInfo info, StreamingContext context)
		{
			_normal = (Vector3)info.GetValue("Normal", typeof(Vector3));
			_const	= info.GetSingle("Constant");
		}
		#endregion

		#region Constants

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pPlane"></param>
        /// <param name="pPoint"></param>
        /// <returns></returns>
        public static Plane CreateParallelPlane(Plane pPlane, Vector3 pPoint)
        {
            return new Plane(pPlane.Normal.Clone(), pPoint.Clone());
        }


        /// <summary>
        /// f(x,y) = a *x + b*y + c
        /// </summary>
        /// <param name="pA"></param>
        /// <param name="pB"></param>
        /// <param name="pC"></param>
        /// <returns></returns>
        public static Plane CreateFormFunctionXYCoefficient(double pA, double pB, double pC)
        {
            Vector3 v1 = new Vector3(1, 1, pA + pB + pC);
            Vector3 v2 = new Vector3(2, 1, pA * 2 + pB * 1 + pC);
            Vector3 v3 = new Vector3(1, 2, pA * 1 + pB * 2 + pC);
            return new Plane(v1, v2, v3);
        }

		
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets or sets the plane's normal vector.
		/// </summary>
		public Vector3 Normal
		{
			get { return _normal; }
			set { _normal = value;}
		}
		/// <summary>
		/// Gets or sets the plane's constant value.
		/// </summary>
		public double Constant
		{
			get { return _const; }
			set { _const = value;}
		}
		#endregion

		#region ICloneable Members
		/// <summary>
		/// Creates an exact copy of this <see cref="Plane"/> object.
		/// </summary>
		/// <returns>The <see cref="Plane"/> object this method creates, cast as an object.</returns>
		object ICloneable.Clone()
		{
			return new Plane(this);
		}
		/// <summary>
		/// Creates an exact copy of this <see cref="Plane"/> object.
		/// </summary>
		/// <returns>The <see cref="Plane"/> object this method creates.</returns>
		public Plane Clone()
		{
			return new Plane(this);
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
			info.AddValue("Normal", _normal, typeof(Vector3));
			info.AddValue("Constant", _const);
		}
		#endregion

		#region Public Static Parse Methods
		/// <summary>
		/// Converts the specified string to its <see cref="Plane"/> equivalent.
		/// </summary>
		/// <param name="s">A string representation of a <see cref="Plane"/></param>
		/// <returns>A <see cref="Plane"/> that represents the vector specified by the <paramref name="s"/> parameter.</returns>
		public static Plane Parse(string s)
		{
			Regex r = new Regex(@"Plane\(n=(?<normal>\([^\)]*\)), c=(?<const>.*)\)", RegexOptions.None);
			Match m = r.Match(s);
			if (m.Success)
			{
				return new Plane(
					Vector3.Parse(m.Result("${normal}")),
					double.Parse(m.Result("${const}"))
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
		/// Flip the plane.
		/// </summary>
		public void Flip()
		{
			_normal = -_normal;
		}
		/// <summary>
		/// Creates a new flipped plane (-normal, constant).
		/// </summary>
		/// <returns>A new <see cref="Plane"/> instance.</returns>
		public Plane GetFlipped()
		{
			return new Plane(-_normal, _const);
		}
		/// <summary>
		/// Returns the points's position relative to the plane itself (i.e Front/Back/On)
		/// </summary>
		/// <param name="p">A point in 3D space.</param>
		/// <returns>A <see cref="MathFunctions.Sign"/>.</returns>
		public MathFunctions.Sign GetSign(Vector3 p) 
		{
			return MathFunctions.GetSign(DistanceMethods.Distance(p,this));
		}
		/// <summary>
		/// Returns the points's position relative to the plane itself (i.e Front/Back/On)
		/// </summary>
		/// <param name="p">A point in 3D space.</param>
		/// <param name="tolerance">The tolerance value to use.</param>
		/// <returns>A <see cref="MathFunctions.Sign"/>.</returns>
		/// <remarks>
		/// If the point's distance from the plane is withon the [-tolerance, tolerance] range, the point is considered to be on the plane.
		/// </remarks>
		public MathFunctions.Sign GetSign(Vector3 p, double tolerance) 
		{
			return MathFunctions.GetSign(DistanceMethods.Distance(p,this), tolerance);
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Returns the hashcode for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return _normal.GetHashCode() ^ _const.GetHashCode();
		}
		/// <summary>
		/// Returns a value indicating whether this instance is equal to
		/// the specified object.
		/// </summary>
		/// <param name="obj">An object to compare to this instance.</param>
		/// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="Vector2"/> and has the same values as this instance; otherwise, <see langword="false"/>.</returns>
		public override bool Equals(object obj)
		{
			if(obj is Plane) 
			{
				Plane p = (Plane)obj;
				return (_normal == p.Normal) && (_const == p.Constant);
			}
			return false;
		}

		/// <summary>
		/// Returns a string representation of this object.
		/// </summary>
		/// <returns>A string representation of this object.</returns>
		public override string ToString()
		{
			return string.Format("Plane[n={0}, c={1}]", _normal, _const);
		}
		#endregion

		#region Comparison Operators
		/// <summary>
		/// Tests whether two specified planes are equal.
		/// </summary>
		/// <param name="a">The left-hand plane.</param>
		/// <param name="b">The right-hand plane.</param>
		/// <returns><see langword="true"/> if the two planes are equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator==(Plane a, Plane b)
		{
			return ValueType.Equals(a,b);
		}
		/// <summary>
		/// Tests whether two specified planes are not equal.
		/// </summary>
		/// <param name="a">The left-hand plane.</param>
		/// <param name="b">The right-hand plane.</param>
		/// <returns><see langword="true"/> if the two planes are not equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator!=(Plane a, Plane b)
		{
			return !ValueType.Equals(a,b);
		}
		#endregion

        /// <summary>
        /// Koordinatengleichung
        /// ax1 + bx2 + cx3 + d = 0
        /// </summary>
        public double[] equation()
        {
            return new double[4] { _normal.X, _normal.Y, _normal.Z, _const };
        }

        /*public Vector3D[] getParameterform()
        {
            double a = _normal.X;
            double b = _normal.Y;
            double c = _normal.Z;
            double d = _const;
            Vector3D orts, r1, r2;
            if (a != 0)
            {
                orts = new Vector3D(d / a, 0, 0);
                r1 = new Vector3D(-b / a, 1, 0);
                r2 = new Vector3D(-c / a, 0, 1);
            }
            else if (b != 0)
            {
                orts = new Vector3D(0, d / b, 0);
                r1 = new Vector3D(1, -a / b, 0);
                r2 = new Vector3D(0, -c / b, 1);
            }
            else if (c != 0)
            {
                orts = new Vector3D(0, 0, d / c);
                r1 = new Vector3D(1, 0, -a / c);
                r2 = new Vector3D(0, 1, -b / c);
            }
            else
            {
                throw new DivideByZeroException();
            }
            return new Vector3D[] { orts, r1, r2 };
        }*/
        
        public List<Vector3> GetPointsOnPlane()
        {
            double a = _normal.X;
            double b = _normal.Y;
            double c = _normal.Z;
            double d = _const;

            Vector3 v1, v2, v3;

            if (a != 0.0)
            {
                v1 = new Vector3(-(b * 0+c *0+d)/a, 0, 0 ); 
                v2 = new Vector3( -(b * 1+c *1+d)/a, 1, 1); 
                v3 = new Vector3( -(b * 1+c *0+d)/a, 1, 0); 
            }
            else if (b != 0.0)
            {
                v1 = new Vector3( 0, -(a * 0+c * 0+d)/b, 0); 
                v2 = new Vector3( 1, -(a * 1+c * 1+d)/b, 1); 
                v3 = new Vector3( 1, -(a * 1+c * 0+d)/b, 0); 
            }

            else if (c != 0.0)
            {
                v1 = new Vector3(0, 0, -(d + a * 0 + b * 0) / c);
                v2 = new Vector3(1, 1, -(d + a * 1 + b * 1) / c);
                v3 = new Vector3(0, 1, -(d + a * 0 + b * 1) / c);
            }
            else
                throw new Exception();

            return new List<Vector3>() {v1, v2, v3};
        }

        /// <summary>
        /// X Plane based on ZY axis directions.
        /// </summary>
        public static Plane CreateXPlane()
        {
            return new Plane(Vector3.Zero(), Vector3.ZAxis(), Vector3.YAxis());
        }
        /// <summary>
        /// Y Plane based on XZ axis directions.
        /// </summary>
        public static Plane CreateYPlane()
        {
            return new Plane(Vector3.Zero(), Vector3.XAxis(), Vector3.ZAxis());
        }
        /// <summary>
        /// Z Plane based on XY axis drections.
        /// </summary>
        public static Plane CreateZPlane()
        {
            return new Plane(Vector3.Zero(), Vector3.XAxis(), Vector3.YAxis());
        }

        /// <summary>
        /// Plane throgut pPoint
        /// </summary>
        /// <param name="pPoint"></param>
        /// <returns></returns>
        public static Plane CreateXPlane(Vector3 pPoint)
        {
            return new Plane(new Vector3(pPoint.X, 0, 0), Vector3.ZAxis(), Vector3.YAxis());
        }

        /// <summary>
        /// Plane throgut pPoint
        /// </summary>
        /// <param name="pPoint"></param>
        /// <returns></returns>
        public static Plane CreateYPlane(Vector3 pPoint)
        {
            return new Plane(new Vector3(0, pPoint.Y, 0), Vector3.XAxis(), Vector3.ZAxis());
        }

        /// <summary>
        /// Plane throgut pPoint
        /// </summary>
        /// <param name="pPoint"></param>
        /// <returns></returns>
        public static Plane CreateZPlane(Vector3 pPoint)
        {
            return new Plane(new Vector3(0, 0, pPoint.Z), Vector3.XAxis(), Vector3.ZAxis());
        }
    }

	#region PlaneConverter class
	/// <summary>
	/// Converts a <see cref="Plane"/> to and from string representation.
	/// </summary>
	public class PlaneConverter : ExpandableObjectConverter
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
			if ((destinationType == typeof(string)) && (value is Plane))
			{
				Plane c = (Plane)value;
				return c.ToString();
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
				return Plane.Parse((string)value);
			}

			return base.ConvertFrom (context, culture, value);
		}
	}
	#endregion
}
