// <copyright file=MatrixD.cs
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

#region Sharp3D.Math, Copyright(C) 2003-2004 Eran Kampf, Licensed under LGPL.
//	Sharp3D.Math math library
//	Copyright (C) 2003-2004  
//	Eran Kampf
//	tentacle@zahav.net.il
//	http://www.ekampf.com/Sharp3D.Math/
//
//	This library is free software; you can redistribute it and/or
//	modify it under the terms of the GNU Lesser General Public
//	License as published by the Free Software Foundation; either
//	version 2.1 of the License, or (at your option) any later version.
//
//	This library is distributed in the hope that it will be useful,
//	but WITHOUT ANY WARRANTY; without even the implied warranty of
//	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//	Lesser General Public License for more details.
//
//	You should have received a copy of the GNU Lesser General Public
//	License along with this library; if not, write to the Free Software
//	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using System.Security.Permissions;

namespace HciLab.Utilities.Mathematics.Core
{
	/// <summary>
	/// Represents a n-dimentional double-precision floating point matrix.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class MatrixD : ISerializable, ICloneable
	{
		#region Private Fields
		private double[][] _data;
		private int _rows;
		private int _columns;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="MatrixD"/> class with the specified size.
		/// </summary>
		/// <param name="rows">Number of rows.</param>
		/// <param name="columns">Number of columns.</param>
		public MatrixD(int rows, int columns)
		{
			_rows = rows;
			_columns = columns;
			_data = new double[rows][];
			for (int i = 0; i < _rows; i++)
			{
				_data[i] = new double[_columns];
			}
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="MatrixD"/> class with the give data.
		/// </summary>
		/// <param name="data">An 2 dimentional array of double-precision floating point values.</param>
		public MatrixD(double[][] data)
		{
			_rows = data.Length;
			_columns = data[0].Length;

			for (int i = 0; i < _rows; i++)
			{
				if (data[i].Length != _columns)
				{
					// TODO: rephrase the error messeage.
					throw new ArgumentException("The data[i] arrays have differ in length.", "data");
				}
			}

			_data = data;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="MatrixD"/> using a given matrix.
		/// </summary>
		/// <param name="matrix">A <see cref="MatrixD"/> instance.</param>
		public MatrixD(MatrixD matrix)
		{
			_rows	= matrix._rows;
			_columns= matrix._columns;
			_data	= (double[][])matrix._data.Clone();
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="MatrixD"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		protected MatrixD(SerializationInfo info, StreamingContext context)
		{
			_rows	= info.GetInt32("Rows");
			_columns= info.GetInt32("Columns");
			_data   = (double[][])info.GetValue("Data", typeof(double[][]));

			Debug.Assert(_data.Length == _rows);
			Debug.Assert(_data[0].Length == _columns);
		}
		#endregion

		#region ISerializable Members
		/// <summary>
		/// Populates a <see cref="SerializationInfo"/> with the data needed to serialize this object.
		/// </summary>
		/// <param name="info">The <see cref="SerializationInfo"/> to populate with data. </param>
		/// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter=true)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Rows", _rows);
			info.AddValue("Columns", _columns);
			info.AddValue("Data", _data);
		}
		#endregion

		#region ICloneable Members
		/// <summary>
		/// Creates an exact copy of this <see cref="MatrixD"/> object.
		/// </summary>
		/// <returns>The <see cref="MatrixD"/> object this method creates, cast as an object.</returns>
		object ICloneable.Clone()
		{
			return new MatrixD(this);
		}
		/// <summary>
		/// Creates an exact copy of this <see cref="MatrixD"/> object.
		/// </summary>
		/// <returns>The <see cref="MatrixD"/> object this method creates.</returns>
		public MatrixD Clone()
		{
			return new MatrixD(this);
		}
		#endregion

		#region Public Static Matrix Arithmetics
		/// <summary>
		/// Adds two matrices.
		/// </summary>
		/// <param name="a">A <see cref="MatrixD"/> instance.</param>
		/// <param name="b">A <see cref="MatrixD"/> instance.</param>
		/// <returns>A new <see cref="MatrixD"/> instance containing the sum.</returns>
		public static MatrixD Add(MatrixD a, MatrixD b)
		{
			if (!MatrixD.EqualDimentions(a,b))
			{
				throw new ArgumentException("Matrix dimentions do no match.");
			}

			MatrixD result = new MatrixD(a.Rows, a.Columns);

			for(int r = 0; r < a.Rows; r++)
			{
				for(int c = 0; c < a.Columns; c++)
				{
					result._data[r][c] = a._data[r][c] + b._data[r][c];
				}
			}

			return result;
		}
		/// <summary>
		/// Adds two matrices and put the result in a third matrix.
		/// </summary>
		/// <param name="a">A <see cref="MatrixD"/> instance.</param>
		/// <param name="b">A <see cref="MatrixD"/> instance.</param>
		/// <param name="result">A <see cref="MatrixD"/> instance to hold the result.</param>
		public static void Add(MatrixD a, MatrixD b, MatrixD result)
		{
			if ((!MatrixD.EqualDimentions(a,b)) || (!MatrixD.EqualDimentions(a,result)))
			{
				throw new ArgumentException("Matrix dimentions do no match.");
			}

			for(int r = 0; r < a.Rows; r++)
			{
				for(int c = 0; c < a.Columns; c++)
				{
					result._data[r][c] = a._data[r][c] + b._data[r][c];
				}
			}
		}
		/// <summary>
		/// Subtracts a matrix from a matrix.
		/// </summary>
		/// <param name="a">A <see cref="MatrixD"/> instance to subtract from.</param>
		/// <param name="b">A <see cref="MatrixD"/> instance to subtract.</param>
		/// <returns>A new <see cref="MatrixD"/> instance containing the difference.</returns>
		/// <remarks>result[x][y] = a[x][y] - b[x][y]</remarks>
		/// <exception cref="System.ArgumentException">Matrix dimentions do not match.</exception>
		public static MatrixD Subtract(MatrixD a, MatrixD b)
		{
			if (!MatrixD.EqualDimentions(a,b))
			{
				throw new ArgumentException("Matrix dimentions do not match.");
			}

			MatrixD result = new MatrixD(a.Rows, a.Columns);

			for(int r = 0; r < a.Rows; r++)
			{
				for(int c = 0; c < a.Columns; c++)
				{
					result._data[r][c] = a._data[r][c] - b._data[r][c];
				}
			}

			return result;		
		}
		/// <summary>
		/// Subtracts a matrix from a matrix and put the result in a third matrix.
		/// </summary>
		/// <param name="a">A <see cref="MatrixD"/> instance to subtract from.</param>
		/// <param name="b">A <see cref="MatrixD"/> instance to subtract.</param>
		/// <param name="result">A <see cref="MatrixD"/> instance to hold the result.</param>
		/// <remarks>result[x][y] = a[x][y] - b[x][y]</remarks>
		/// <exception cref="System.ArgumentException">Matrix dimentions do not match.</exception>
		public static void Subtract(MatrixD a, MatrixD b, MatrixD result)
		{
			if ((!MatrixD.EqualDimentions(a,b)) && (!MatrixD.EqualDimentions(a,result)))
			{
				throw new ArgumentException("Matrix dimentions do not match.");
			}

			for(int r = 0; r < a.Rows; r++)
			{
				for(int c = 0; c < a.Columns; c++)
				{
					result._data[r][c] = a._data[r][c] - b._data[r][c];
				}
			}
		}
		/// <summary>
		/// Multiplies a matrix by a scalar.
		/// </summary>
		/// <param name="m">A <see cref="MatrixD"/> instance.</param>
		/// <param name="s">A double-precision floating point value.</param>
		/// <returns>A new <see cref="MatrixD"/> instance containing the result.</returns>
		public static MatrixD Multiply(MatrixD m, double s)
		{
			MatrixD result = new MatrixD(m);
			for (int i = 0; i < result.Rows; i++)
			{
				for (int j = 0; j < result.Columns; j++)
				{
					result._data[i][j] *= s;
				}
			}

			return result;
		}
		/// <summary>
		/// Multiplies a matrix by a scalar and put the result in a given matrix instance.
		/// </summary>
		/// <param name="m">A <see cref="MatrixD"/> instance.</param>
		/// <param name="s">A double-precision floating point value.</param>
		/// <param name="result">A <see cref="MatrixD"/> instance to hold the result.</param>
		public static void Multiply(MatrixD m, double s, MatrixD result)
		{
			Debug.Assert(result.Rows == m.Rows);
			Debug.Assert(result.Columns == m.Columns);

			for (int i = 0; i < result.Rows; i++)
			{
				for (int j = 0; j < result.Columns; j++)
				{
					result._data[i][j] = m._data[i][j] * s;
				}
			}
		}
		/// <summary>
		/// Negates a matrix.
		/// </summary>
		/// <param name="m">A <see cref="MatrixD"/> instance.</param>
		/// <returns>A new <see cref="MatrixD"/> instance containing the result.</returns>
		public static MatrixD Negate(MatrixD m)
		{
			MatrixD result = new MatrixD(m.Rows, m.Columns);
			for(int r = 0; r < result.Rows; r++)
			{
				for(int c = 0; c < result.Columns; c++)
				{
					result._data[r][c] = -m._data[r][c];
				}
			}

			return result;
		}
		/// <summary>
		/// Transposes a matrix.
		/// </summary>
		/// <param name="m">A <see cref="MatrixD"/> instance.</param>
		/// <returns>A new <see cref="MatrixD"/> instance transposed matrix.</returns>
		public static MatrixD Transpose(MatrixD m)
		{
			MatrixD result = new MatrixD(m.Columns, m.Rows);

			for(int r = 0; r < m.Rows; r++)
			{
				for(int c = 0; c < m.Columns; c++)
				{
					result._data[c][r] = m._data[r][c];
				}
			}

			return result;
		}

		/// <summary>
		/// Tests whether the dimentions of two given matrices are equal.
		/// </summary>
		/// <param name="a">A <see cref="MatrixD"/> instance.</param>
		/// <param name="b">A <see cref="MatrixD"/> instance.</param>
		/// <returns><see langword="true"/> if the dimentions of the two matrices are equal; otherwise, <see langword="false"/>.</returns>
		public static bool EqualDimentions(MatrixD a, MatrixD b)
		{
			if (a.Rows != b.Rows)
				return false;
            
			if (a.Columns != b.Columns)
				return false;

			return true;

		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets the number of rows in the matrix.
		/// </summary>
		public int Rows
		{
			get { return _rows; }
		}
		/// <summary>
		/// Gets the number of columns in the matrix.
		/// </summary>
		public int Columns
		{
			get { return _columns; }
		}
		/// <summary>
		/// Gets a value indicating whether the matrix is square.
		/// </summary>
		/// <value>True if the matrix is square; otherwise, <see langword="false"/>.</value>
		public bool IsSquare
		{
			get { return (_rows == _columns); }
		}
		/// <summary>
		/// Gets a value indicating whether the matrix is symmetric.
		/// </summary>
		/// <value>True if the matrix is symmetric; otherwise, <see langword="false"/>.</value>
		public bool IsSymmetric
		{
			get
			{
				if (this.IsSquare)
				{
					for (int i=0; i < _rows; i++)
					{
						for (int j=0; j <= i; j++)
						{
							if (_data[i][j] != _data[j][i])
							{
								return false;
							}
						}
					}

					return true;
				}

				return false;
			}
		}
		/// <summary>
		/// Gets the value of the matrix's trace.
		/// </summary>
		/// <value>A double-precision floating point number.</value>
		/// <remarks>The matrix trace is the sum of its diagonal elements.</remarks>
		public double Trace
		{
			get
			{
				double trace = 0;
				for (int i = 0; i < System.Math.Min(_rows, _columns); i++)
				{
					trace += _data[i][i];
				}

				return trace;
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Returns the hashcode for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return _rows.GetHashCode() ^ _columns.GetHashCode() ^ _data.GetHashCode();
		}
		/// <summary>
		/// Returns a value indicating whether this instance is equal to
		/// the specified object.
		/// </summary>
		/// <param name="obj">An object to compare to this instance.</param>
		/// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="MatrixD"/> and has the same values as this instance; otherwise, <see langword="false"/>.</returns>
		public override bool Equals(object obj)
		{
			if (obj is MatrixD)
			{
				MatrixD m = (MatrixD)obj;
				for (int i = 0; i < m.Rows; i++)
				{
					for (int j = 0; j < m.Columns; j++)
					{
						if (_data[i][j] != m._data[i][j])
							return false;
					}
				}

				return true;
			}
			return false;
		}

		/// <summary>
		/// Returns a string representation of this object.
		/// </summary>
		/// <returns>A string representation of this object.</returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < _rows; i++)
			{
				for (int j = 0; j < _columns; j++)
					builder.Append(_data[i][j] + " ");

				builder.Append(Environment.NewLine);
			}

			return builder.ToString();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Extracts a sub matrix from the current matrix.
		/// </summary>
		/// <param name="rowStart">Row index to start extraction from.</param>
		/// <param name="rowEnd">Row index to end extraction at.</param>
		/// <param name="colStart">Column index to start extraction from.</param>
		/// <param name="colEnd">Column index to end extraction at.</param>
		/// <returns>A new <see cref="MatrixD"/> instance containing the extracted sub matrix.</returns>
		public MatrixD Submatrix(int rowStart, int rowEnd, int colStart, int colEnd)
		{
			MatrixD result = new MatrixD( (rowEnd-rowStart)+1 , (colEnd-colStart)+1 );

			for (int r = rowStart; r <= rowEnd; r++)
			{
				for (int c = colStart; c <= colEnd; c++)
				{
					result._data[r - rowStart][c - colStart] = _data[r][c];
				}
			}
			
			return result;
		}
		/// <summary>
		/// Extracts a sub matrix from the current matrix.
		/// </summary>
		/// <param name="rowIndices">An array of row indices.</param>
		/// <param name="colIndices">An array of column indices.</param>
		/// <returns>A new <see cref="MatrixD"/> instance containing the extracted sub matrix.</returns>
		public MatrixD Submatrix(int[] rowIndices, int[] colIndices)
		{
			MatrixD result = new MatrixD(rowIndices.Length, colIndices.Length);

			for(int r = 0; r < rowIndices.Length; r++)
			{
				for(int c = 0; c < colIndices.Length; c++)
				{
					result._data[r][c] = _data[rowIndices[r]][colIndices[c]];
				}
			}

			return result;
		}
		/// <summary>
		/// Extracts a sub matrix from the current matrix.
		/// </summary>
		/// <param name="rowStart">Row index to start extraction from.</param>
		/// <param name="rowEnd">Row index to end extraction at.</param>
		/// <param name="colIndices">An array of column indices.</param>
		/// <returns>A new <see cref="MatrixD"/> instance containing the extracted sub matrix.</returns>
		public MatrixD Submatrix(int rowStart, int rowEnd, int[] colIndices)
		{
			MatrixD result = new MatrixD( (rowEnd-rowStart)+1, colIndices.Length );

			for(int r = rowStart; r <= rowEnd; r++)
			{
				for(int c = 0; c < colIndices.Length; c++)
				{
					result._data[r - rowStart][c] = _data[r][colIndices[c]];
				}
			}

			return result;
		}
		/// <summary>
		/// Extracts a sub matrix from the current matrix.
		/// </summary>
		/// <param name="rowIndices">An array of row indices.</param>
		/// <param name="colStart">Column index to start extraction from.</param>
		/// <param name="colEnd">Column index to end extraction at.</param>
		/// <returns>A new <see cref="MatrixD"/> instance containing the extracted sub matrix.</returns>
		public MatrixD Submatrix(int[] rowIndices, int colStart, int colEnd)
		{
			MatrixD result = new MatrixD( rowIndices.Length, (colEnd-colStart)+1 );

			for(int r = 0; r < rowIndices.Length; r++)
			{
				for(int c = colStart; c <= colEnd; c++)
				{
					result._data[r][c - colStart] = _data[rowIndices[r]][c];
				}
			}

			return result;
		}

		#endregion

		#region Comparison Operators
		/// <summary>
		/// Tests whether two specified matrices are equal.
		/// </summary>
		/// <param name="a">The left-hand matrix.</param>
		/// <param name="b">The right-hand matrix.</param>
		/// <returns><see langword="true"/> if the two matrices are equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator==(MatrixD a, MatrixD b)
		{
			return Object.Equals(a,b);
		}
		/// <summary>
		/// Tests whether two specified matrices are not equal.
		/// </summary>
		/// <param name="a">The left-hand matrix.</param>
		/// <param name="b">The right-hand matrix.</param>
		/// <returns><see langword="true"/> if the two matrices are not equal; otherwise, <see langword="false"/>.</returns>
		public static bool operator!=(MatrixD a, MatrixD b)
		{
			return !Object.Equals(a,b);
		}
		#endregion

		#region Unary Operators
		/// <summary>
		/// Negates the values of a matrix.
		/// </summary>
		/// <param name="m">A <see cref="MatrixD"/> instance.</param>
		/// <returns>A new <see cref="MatrixD"/> instance containing the result.</returns>
		public static MatrixD operator-(MatrixD m)
		{
			return MatrixD.Negate(m);
		}
		#endregion

		#region Binary Operators
		/// <summary>
		/// Adds two matrices.
		/// </summary>
		/// <param name="a">A <see cref="MatrixD"/> instance.</param>
		/// <param name="b">A <see cref="MatrixD"/> instance.</param>
		/// <returns>A new <see cref="MatrixD"/> instance containing the sum.</returns>
		public static MatrixD operator+(MatrixD a, MatrixD b)
		{
			return MatrixD.Add(a,b);
		}
		/// <summary>
		/// Multiplies a matrix by a scalar.
		/// </summary>
		/// <param name="m">A <see cref="MatrixD"/> instance.</param>
		/// <param name="s">A double-precision floating point value.</param>
		/// <returns>A new <see cref="MatrixD"/> instance containing the result.</returns>
		public static MatrixD operator*(MatrixD m, double s)
		{
			return MatrixD.Multiply(m,s);
		}
		/// <summary>
		/// Multiplies a matrix by a scalar.
		/// </summary>
		/// <param name="m">A <see cref="MatrixD"/> instance.</param>
		/// <param name="s">A double-precision floating point value.</param>
		/// <returns>A new <see cref="MatrixD"/> instance containing the result.</returns>
		public static MatrixD operator*(double s, MatrixD m)
		{
			return MatrixD.Multiply(m,s);
		}
		#endregion

		#region Indexing Operators
		/// <summary>
		/// Indexer allowing to access the matrix elements by row and column.
		/// </summary>
		public double this[int row, int col]
		{
			set { _data[row][col] = value;}
			get { return _data[row][col]; }
		}

		#endregion
	}
}
