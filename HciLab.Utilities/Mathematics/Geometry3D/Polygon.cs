// <copyright file=Polygon.cs
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
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace HciLab.Utilities.Mathematics.Geometry3D
{
	/// <summary>
	/// Represents a polygon in 3 dimentional space.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class Polygon : ICloneable, ISerializable
	{
		#region Private fields
		private Vector3DArrayList _points = new Vector3DArrayList();
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Polygon"/> class.
		/// </summary>
		public Polygon()
		{
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Polygon"/> class using an array of coordinates.
		/// </summary>
		/// <param name="points">An <see cref="Vector3DArrayList"/> instance.</param>
		public Polygon(Vector3DArrayList points)
		{
			_points.AddRange(points);
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Polygon"/> class using an array of coordinates.
		/// </summary>
		/// <param name="points">An array of <see cref="Vector3"/> coordniates.</param>
		public Polygon(Vector3[] points)
		{
			_points.AddRange(points);
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Polygon"/> class using coordinates from another instance.
		/// </summary>
		/// <param name="polygon">A <see cref="Polygon"/> instance.</param>
		public Polygon(Polygon polygon)
		{
			_points = (Vector3DArrayList)polygon._points.Clone();
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Polygon"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		protected Polygon(SerializationInfo info, StreamingContext context)
		{
			_points = (Vector3DArrayList)info.GetValue("Points", typeof(Vector3DArrayList));
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets the polygon's list of points.
		/// </summary>
		[XmlArrayItem(Type = typeof(Vector3))]
		public Vector3DArrayList Points
		{
			get { return _points; }
		}
		#endregion

		#region ICloneable Members
		/// <summary>
		/// Creates an exact copy of this <see cref="Polygon"/> object.
		/// </summary>
		/// <returns>The <see cref="Polygon"/> object this method creates, cast as an object.</returns>
		object ICloneable.Clone()
		{
			return new Polygon(this);
		}
		/// <summary>
		/// Creates an exact copy of this <see cref="Polygon"/> object.
		/// </summary>
		/// <returns>The <see cref="Polygon"/> object this method creates.</returns>
		public Polygon Clone()
		{
			return new Polygon(this);
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
			info.AddValue("Points", _points, typeof(Vector3DArrayList));
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Flips the polygon.
		/// </summary>
		public void Flip()
		{
			_points.Reverse();
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets the polygon's vertex count.
		/// </summary>
		public int VertexCount
		{
			get { return _points.Count; }
		}
		#endregion
	}
}
