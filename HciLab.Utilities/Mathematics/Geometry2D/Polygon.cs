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
// <date> 11/2/2016 12:25:57 PM</date>

using HciLab.Utilities.Mathematics.Core;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace HciLab.Utilities.Mathematics.Geometry2D
{
	/// <summary>
	/// Represents a polygon in 2 dimentional space.
	/// </summary>
    [Serializable()]
    public class Polygon : DataBaseClass, ISerializable, ICloneable
	{

        /// <summary>
        /// ISerialization Version
        /// </summary>
        private int m_SerVersion = 1;

		#region Private fields
		//private Vector2DArrayList m_Points = new Vector2DArrayList();
        private CollectionWithItemNotify<Vector2> m_Points = new CollectionWithItemNotify<Vector2>();
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Polygon"/> class.
		/// </summary>
		public Polygon()
            : base()
		{
            initListener();
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Polygon"/> class using an array of coordinates.
		/// </summary>
		/// <param name="pPoints">An <see cref="Vector2DArrayList"/> instance.</param>
		public Polygon(Vector2DArrayList pPoints)
            : base()
		{
            foreach(Vector2 v in pPoints.ToArray())
			    m_Points.Add(v);
            initListener();
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Polygon"/> class using an array of coordinates.
		/// </summary>
		/// <param name="pPoints">An array of <see cref="Vector2"/> coordniates.</param>
		public Polygon(Vector2[] pPoints)
            : base()
		{
            foreach (Vector2 v in pPoints)
                m_Points.Add(v);
            initListener();
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Polygon"/> class using coordinates from another instance.
		/// </summary>
		/// <param name="polygon">A <see cref="Polygon"/> instance.</param>
		public Polygon(Polygon polygon)
            : base()
		{
            foreach (Vector2 v in polygon.Points)
                m_Points.Add(new Vector2(v.X, v.Y));
            initListener();
		}

        public Polygon(System.Collections.Generic.List<Vector2> pList)
            : base()
        {
            foreach (Vector2 v in pList)
                m_Points.Add(v);
            initListener();
        }

        private void initListener()
        {
            if (m_Points == null)
                return;

            m_Points.ItemPropertyChanged += Points_PropertyChanged;
            m_Points.CollectionChanged += Points_CollectionChanged;
            //m_Points.ItemsPropertyChanged += Points_ItemsPropertyChanged;
        }

        

		/// <summary>
		/// Initializes a new instance of the <see cref="Polygon"/> class with serialized data.
		/// </summary>
		/// <param name="pInfo">The object that holds the serialized object data.</param>
		/// <param name="pContext">The contextual information about the source or destination.</param>
		protected Polygon(SerializationInfo pInfo, StreamingContext pContext)
            : base(pInfo, pContext)
		{
            int pSerVersion = pInfo.GetInt32("m_SerVersion");

            if (m_SerVersion >= 1)
            {
                m_Points = (CollectionWithItemNotify<Vector2>)pInfo.GetValue("m_Points", typeof(CollectionWithItemNotify<Vector2>));
            }
            initListener();
		}

		#endregion

		#region Public Properties
        public CollectionWithItemNotify<Vector2> Points
		{
			get {
                return m_Points;
            }
            set {
                m_Points = value;
                initListener();
            }
		}

        void Points_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged();
        }

        void Points_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged();
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
		/// <param name="pInfo">The <see cref="SerializationInfo"/> to populate with data. </param>
		/// <param name="pContext">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
		//[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter=true)]
		public void GetObjectData(SerializationInfo pInfo, StreamingContext pContext)
		{
            base.GetObjectData(pInfo, pContext);

            pInfo.AddValue("m_SerVersion", m_SerVersion);
            pInfo.AddValue("m_Points", m_Points);
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Flips the polygon.
		/// </summary>
		public void Flip()
		{
            m_Points = new CollectionWithItemNotify<Vector2>(m_Points.Reverse());
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets the polygon's vertex count.
		/// </summary>
		public int VertexCount
		{
			get { return m_Points.Count; }
		}
		#endregion

        public HelixToolkit.Wpf.Polygon ToHelixToolkit()
        {
            HelixToolkit.Wpf.Polygon p = new HelixToolkit.Wpf.Polygon();
            foreach (HciLab.Utilities.Mathematics.Core.Vector2 v in Points)
                p.Points.Add(new System.Windows.Point(v.X, v.Y));

            return p;
        }

        public HelixToolkit.Wpf.Polygon3D ToHelixToolkit3D(double pOffset = 0)
        {
            HelixToolkit.Wpf.Polygon3D p = new HelixToolkit.Wpf.Polygon3D();
            foreach (HciLab.Utilities.Mathematics.Core.Vector2 v in Points)
                p.Points.Add(new System.Windows.Media.Media3D.Point3D(v.X, v.Y, pOffset));

            return p;
        }
        
    }
}
