// <copyright file=MathFunctions.cs
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
	/// Provides standard mathematical functions for the library types.
	/// </summary>
    public static class MathFunctions
	{
		#region Delegates
		#region Delegates - Boolean Functions
		/// <summary>
		/// Functor that takes no arguments and returns a boolean.
		/// </summary>
		public delegate bool BooleanFunction();
		/// <summary>
		/// Functor that takes one booleans and returns a boolean. 
		/// </summary>
		public delegate bool BooleanUnaryFunction(bool b);
		/// <summary>
		/// Functor that takes two booleans and returns a boolean. 
		/// </summary>
		public delegate bool BooleanBinaryFunction(bool b1, bool b2);
		#endregion

		#region Delegates - Double Functions
		/// <summary>
		/// Functor that takes no arguments and returns a double.
		/// </summary>
		public delegate double DoubleFunction();
		/// <summary>
		/// Functor that takes one double and returns a double. 
		/// </summary>
		public delegate double DoubleUnaryFunction(double x);
		/// <summary>
		/// Functor that takes two doubles and returns a double. 
		/// </summary>
		public delegate double DoubleBinaryFunction(double x, double y);
		/// <summary>
		/// Functor that takes three doubles and returns a double. 
		/// </summary>
		public delegate double DoubleTernaryFunction(double x, double y, double z);
		#endregion

		#region Delegates - double Functions
		/// <summary>
		/// Functor that takes no arguments and returns a double.
		/// </summary>
		public delegate double doubleFunction();
		/// <summary>
		/// Functor that takes one double and returns a double. 
		/// </summary>
		public delegate double doubleUnaryFunction(double x);
		/// <summary>
		/// Functor that takes two doubles and returns a double. 
		/// </summary>
		public delegate double doubleBinaryFunction(double x, double y);
		/// <summary>
		/// Functor that takes three doubles and returns a double. 
		/// </summary>
		public delegate double doubleTernaryFunction(double x, double y, double z);
		#endregion

		#region Delegates - Integet Functions
		/// <summary>
		/// Functor that takes no arguments and returns an integer.
		/// </summary>
		public delegate int IntFunction();
		/// <summary>
		/// Functor that takes one integer and returns an integer.
		/// </summary>
		public delegate int IntUnaryFunction(int x);
		/// <summary>
		/// Functor that takes two integers and returns an integer.
		/// </summary>
		public delegate int IntBinaryFunction(int x, int y);
		/// <summary>
		/// Functor that takes three integers and returns an integer.
		/// </summary>
		public delegate int IntTernaryFunction(int x, int y, int z);
		#endregion

		#region Delegates - Object Functions
		/// <summary>
		/// Functor that takes no arguments and returns an object.
		/// </summary>
		public delegate object ObjectFunction();
		/// <summary>
		/// Functor that takes one object and returns an object.
		/// </summary>
		public delegate object ObjectUnaryFunction(object obj);
		/// <summary>
		/// Functor that takes one objects and returns an object.
		/// </summary>
		public delegate object ObjectBinaryFunction(object obj1, object obj2);
		/// <summary>
		/// Functor that takes three objects and returns an object.
		/// </summary>
		public delegate object ObjectTernaryFunction(object obj1, object obj2, object obj3);
		#endregion

		#region Delegates - String Functions
		/// <summary>
		/// Functor that takes no arguments and returns a string.
		/// </summary>
		public delegate string StringFunction();
		/// <summary>
		/// Functor that takes one object and returns a string.
		/// </summary>
		public delegate string StringUnaryFunction(string s);
		/// <summary>
		/// Functor that takes two objects and returns a string.
		/// </summary>
		public delegate string StringBinaryFunction(string s1, string s2);
		/// <summary>
		/// Functor that takes three objects and returns a string.
		/// </summary>
		public delegate string StringTernaryFunction(string s1, string s2, string s3);
		#endregion
		#endregion

		#region Enums
		/// <summary>
		/// Represents a value's sign (positive, negative or zero).
		/// </summary>
		public enum Sign
		{
			/// <summary>
			/// Negative sign
			/// </summary>
			Negative = -1,
			/// <summary>
			/// Zero
			/// </summary>
			Zero = 0,
			/// <summary>
			/// Positive sign
			/// </summary>
			Positive = 1
		}
		#endregion

		#region Constants
		/// <summary>
		/// The value of PI.
		/// </summary>
		public const double PI = System.Math.PI;
		/// <summary>
		/// The value of (2 * PI).
		/// </summary>
		public const double TwoPI = 2*System.Math.PI;
		/// <summary>
		/// The value of (PI*PI).
		/// </summary>
		public const double SquaredPI = System.Math.PI * System.Math.PI;
		/// <summary>
		/// The value of PI/2.
		/// </summary>
		public const double HalfPI = System.Math.PI / 2.0;

		/// <summary>
		/// Epsilon, a fairly small value for a single precision doubleing point
		/// </summary>
		public const double EpsilonF = 4.76837158203125E-7f;
		/// <summary>
		/// Epsilon, a fairly small value for a double precision doubleing point
		/// </summary>
		public const double EpsilonD = 8.8817841970012523233891E-16;
		#endregion

		#region Functions
		#region Abs Functions
		/// <summary>
		/// Absolute value function for single-precision doubleing point numbers.
		/// </summary>
		public static readonly doubleUnaryFunction	doubleAbsFunction	= new doubleUnaryFunction(System.Math.Abs);
		/// <summary>
		/// Absolute value function for double-precision doubleing point numbers.
		/// </summary>
		public static readonly DoubleUnaryFunction	DoubleAbsFunction	= new DoubleUnaryFunction(System.Math.Abs);
		/// <summary>
		/// Absolute value function for integers.
		/// </summary>
		public static readonly IntUnaryFunction		IntAbsFunction		= new IntUnaryFunction(System.Math.Abs);
		#endregion

		#region Sqrt Functions
		/// <summary>
		/// Square root function for single-precision doubleing point numbers.
		/// </summary>
		public static readonly doubleUnaryFunction	doubleSqrtFunction	= new doubleUnaryFunction(MathFunctions.Sqrt);
		/// <summary>
		/// Square root function for double-precision doubleing point numbers.
		/// </summary>
		public static readonly DoubleUnaryFunction	DoubleSqrtFunction	= new DoubleUnaryFunction(System.Math.Sqrt);
		/// <summary>
		/// Square root function for integers.
		/// </summary>
		public static readonly IntUnaryFunction		IntSqrtFunction		= new IntUnaryFunction(MathFunctions.Sqrt);
		#endregion

		#region Interpolation Functions
		/// <summary>
		/// Linear interpolation function  for double-precision doubleing point numbers.
		/// </summary>
		public static readonly DoubleTernaryFunction DoubleLinearInterpolationFunction = new DoubleTernaryFunction(LinearInterpolation);
		/// <summary>
		/// Cosine interpolation function  for double-precision doubleing point numbers.
		/// </summary>
		public static readonly DoubleTernaryFunction DoubleCosineInterpolationFunction = new DoubleTernaryFunction(CosineInterpolation);
		/// <summary>
		/// Cubic interpolation function  for double-precision doubleing point numbers.
		/// </summary>
		public static readonly DoubleTernaryFunction DoubleCubicInterpolationFunction = new DoubleTernaryFunction(CubicInterpolation);
		#endregion
		#endregion

		#region Abs
		/// <summary>
		/// Creates a new <see cref="IntArrayList"/> whose element values are the
		/// result of applying the absolute function on the elements of the
		/// given integers array.
		/// </summary>
		/// <param name="array">An array of integers.</param>
		/// <returns>A new <see cref="IntArrayList"/> whose values are the result of applying the absolute function to each element in <paramref name="array"/></returns>
		public static IntArrayList Abs(IntArrayList array)
		{
			IntArrayList result = new IntArrayList(array.Count);
			for (int i = 0; i < array.Count; i++)
			{
				result[i] = Math.Abs(array[i]);
			}
			return result;
		}
		/// <summary>
		/// Creates a new <see cref="DoubleArrayList"/> whose element values are the
		/// result of applying the absolute function on the elements of the
		/// given doubles array.
		/// </summary>
		/// <param name="array">An array of double-precision doubleing point values.</param>
		/// <returns>A new <see cref="DoubleArrayList"/> whose values are the result of applying the absolute function to each element in <paramref name="array"/></returns>
		public static DoubleArrayList Abs(DoubleArrayList array)
		{
			DoubleArrayList result = new DoubleArrayList(array.Count);
			for (int i = 0; i < array.Count; i++)
			{
                result[i] = Math.Abs(array[i]);
			}
			return result;
		}
		/// <summary>
		/// Creates a new <see cref="IntArrayList"/> whose element values are the
		/// result of applying the absolute function on the elements of the
		/// given integers array.
		/// </summary>
		/// <param name="array">An array of integers.</param>
		/// <returns>A new <see cref="IntArrayList"/> whose values are the result of applying the absolute function to each element in <paramref name="array"/></returns>
		public static int[] Abs(int[] array)
		{
			int[] result = new int[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
                result[i] = Math.Abs(array[i]);
			}
			return result;
		}		/// <summary>
		/// Creates a new <see cref="doubleArrayList"/> whose element values are the
		/// result of applying the absolute function on the elements of the
		/// given doubles array.
		/// </summary>
		/// <param name="array">An array of single-precision doubleing point values.</param>
		/// <returns>A new <see cref="doubleArrayList"/> whose values are the result of applying the absolute function to each element in <paramref name="array"/></returns>
		public static double[] Abs(double[] array)
		{
			double[] result = new double[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
                result[i] = Math.Abs(array[i]);
			}
			return result;
		}
		#endregion

		#region AbsSum
		/// <summary>
		/// Calculates the sum of the absolute value of a given array's elements.
		/// </summary>
		/// <param name="array">An array of integers.</param>
		/// <returns>The sum of the absolute values of the elements in <paramref name="array"/>.</returns>
		/// <remarks>sum = abs(array[0]) + abs(array[1])...</remarks>
		public static int AbsSum(int[] array)
		{
			int sum = 0;
			foreach (int i in array)
                sum += Math.Abs(i);

			return sum;
		}
		/// <summary>
		/// Calculates the sum of the absolute value of a given array's elements.
		/// </summary>
		/// <param name="array">An array of integers.</param>
		/// <returns>The sum of the absolute values of the elements in <paramref name="array"/>.</returns>
		/// <remarks>sum = abs(array[0]) + abs(array[1])...</remarks>
		public static int AbsSum(IntArrayList array)
		{
			int sum = 0;
			foreach (int i in array)
                sum += Math.Abs(i);

			return sum;
		}
		/// <summary>
		/// Calculates the sum of the absolute value of a given array's elements.
		/// </summary>
		/// <param name="array">An array of double-precision doubleing point values.</param>
		/// <returns>The sum of the absolute values of the elements in <paramref name="array"/>.</returns>
		/// <remarks>sum = abs(array[0]) + abs(array[1])...</remarks>
		public static double AbsSum(double[] array)
		{
			double sum = 0;
			foreach (double d in array)
                sum += Math.Abs(d);

			return sum;
		}
		/// <summary>
		/// Calculates the sum of the absolute value of a given array's elements.
		/// </summary>
		/// <param name="array">An array of double-precision doubleing point values.</param>
		/// <returns>The sum of the absolute values of the elements in <paramref name="array"/>.</returns>
		/// <remarks>sum = abs(array[0]) + abs(array[1])...</remarks>
		public static double AbsSum(DoubleArrayList array)
		{
			double sum = 0;
			foreach (double d in array)
				sum += Math.Abs(d);

			return sum;
		}
		#endregion

		#region Sum
		/// <summary>
		/// Calculates the sum of a given array's elements.
		/// </summary>
		/// <param name="array">An array of integers.</param>
		/// <returns>The sum of the array's elements.</returns>
		/// <remarks>sum = array[0] + array[1]...</remarks>
		public static int Sum(int[] array)
		{
			int sum = 0;
			foreach (int i in array)
				sum += i;

			return sum;
		}
		/// <summary>
		/// Calculates the sum of a given array's elements.
		/// </summary>
		/// <param name="array">An array of integers.</param>
		/// <returns>The sum of the array's elements.</returns>
		/// <remarks>sum = array[0] + array[1]...</remarks>
		public static int Sum(IntArrayList array)
		{
			int sum = 0;
			foreach (int i in array)
				sum += i;

			return sum;
		}
		/// <summary>
		/// Calculates the sum of a given array's elements.
		/// </summary>
		/// <param name="array">An array of double-precision doubleing point values.</param>
		/// <returns>The sum of the array's elements.</returns>
		/// <remarks>sum = array[0] + array[1]...</remarks>
		public static double Sum(double[] array)
		{
			double sum = 0;
			foreach (double d in array)
				sum += d;

			return sum;
		}
		/// <summary>
		/// Calculates the sum of a given array's elements.
		/// </summary>
		/// <param name="array">An array of double-precision doubleing point values.</param>
		/// <returns>The sum of the array's elements.</returns>
		/// <remarks>sum = array[0] + array[1]...</remarks>
		public static double Sum(DoubleArrayList array)
		{
			double sum = 0;
			foreach (double d in array)
				sum += d;

			return sum;
		}
		#endregion

		#region SumOfSquares
		/// <summary>
		/// Calculates the sum of a given array's elements square values.
		/// </summary>
		/// <param name="array">An array of integers.</param>
		/// <returns>The sum of the array's elements square value.</returns>
		/// <remarks>sum = array[0]^2 + array[1]^2 ...</remarks>
		public static int SumOfSquares(int[] array)
		{
			int SumOfSquares = 0;
			foreach (int i in array)
				SumOfSquares += i*i;

			return SumOfSquares;
		}
		/// <summary>
		/// Calculates the sum of a given array's elements square values.
		/// </summary>
		/// <param name="array">An array of integers.</param>
		/// <returns>The sum of the array's elements square value.</returns>
		/// <remarks>sum = array[0]^2 + array[1]^2 ...</remarks>
		public static int SumOfSquares(IntArrayList array)
		{
			int SumOfSquares = 0;
			foreach (int i in array)
				SumOfSquares += i*i;

			return SumOfSquares;
		}
		/// <summary>
		/// Calculates the sum of a given array's elements square values.
		/// </summary>
		/// <param name="array">An array of double-precision doubleing point numbers.</param>
		/// <returns>The sum of the array's elements square value.</returns>
		/// <remarks>sum = array[0]^2 + array[1]^2 ...</remarks>
		public static double SumOfSquares(double[] array)
		{
			double SumOfSquares = 0;
			foreach (double d in array)
				SumOfSquares += d*d;

			return SumOfSquares;
		}
		/// <summary>
		/// Calculates the sum of a given array's elements square values.
		/// </summary>
		/// <param name="array">An array of double-precision doubleing point numbers.</param>
		/// <returns>The sum of the array's elements square value.</returns>
		/// <remarks>sum = array[0]^2 + array[1]^2 ...</remarks>
		public static double SumOfSquares(DoubleArrayList array)
		{
			double SumOfSquares = 0;
			foreach (double d in array)
				SumOfSquares += d*d;

			return SumOfSquares;
		}
		#endregion

		#region Sqrt
		/// <summary>
		/// Returns the square root of a specified number.
		/// </summary>
		/// <param name="value">A double-precision doubleing point number.</param>
		/// <returns>The square root of a specified number.</returns>
		public static double Sqrt(double value)
		{
			return System.Math.Sqrt(value);
		}
		/// <summary>
		/// Returns the square root of a specified number.
		/// </summary>
		/// <param name="value">An integer.</param>
		/// <returns>The square root of a specified number.</returns>
		public static int Sqrt(int value)
		{
			return (int)System.Math.Sqrt(value);
		}
		/// <summary>
		/// Returns the square root of a specified number.
		/// </summary>
		/// <param name="value">A <see cref="ComplexD"/> instance.</param>
		/// <returns>The square root of a specified number.</returns>
		public static ComplexD Sqrt(ComplexD value)
		{
			return ComplexD.Sqrt(value);
		}
		#endregion

		#region Log
		/// <summary>
		/// Calculates the natural (base e) logarithm of a specified number.
		/// </summary>
		/// <param name="value">A double-precision real number whose logarithm is to be found.</param>
		/// <returns>The natural logarithm of <paramref name="value"/>; that is, ln(value).</returns>
		public static double Log(double value)
		{
			return System.Math.Log(value);
		}
		public static ComplexD Log(ComplexD value)
		{
			return ComplexD.Log(value);
		}
		#endregion

		#region Exp
		/// <summary>
		/// Returns <b>e</b> raised to the specified power.
		/// </summary>
		/// <param name="value">A double-precision real number specifying the power.</param>
		/// <returns>The number e raised to the power <paramref name="value"/>.</returns>
		public static double Exp(double value)
		{
			return System.Math.Exp(value);
		}
		public static ComplexD Exp(ComplexD value)
		{
			return ComplexD.Exp(value);
		}
		#endregion

		#region Sin
		public static ComplexD Sin(ComplexD value)
		{
			return ComplexD.Sin(value);
		}
		#endregion

		#region Cos
		/// <summary>
		/// Returns the cosine of the specified angle.
		/// </summary>
		/// <param name="value">A single-precision real number representing the angle, measured in radians.</param>
		/// <returns>
		/// The cosine of <paramref name="value"/>. 
		/// If <paramref name="value"/> is equal to NaN, NegativeInfinity, or PositiveInfinity, this method returns NaN.
		/// </returns>
		public static double Cos(double value)
		{
			return (double)System.Math.Cos(value);
		}
		public static ComplexD Cos(ComplexD value)
		{
			return ComplexD.Cos(value);
		}
		#endregion

		#region Tan
		/// <summary>
		/// Returns the tangent of the specified angle.
		/// </summary>
		/// <param name="value">A double-precision real number representing the angle, measured in radians.</param>
		/// <returns>
		/// The cosine of <paramref name="value"/>. 
		/// If <paramref name="value"/> is equal to NaN, NegativeInfinity, or PositiveInfinity, this method returns NaN.
		/// </returns>
		public static double Tan(double value)
		{
			return System.Math.Tan(value);
		}
		public static ComplexD Tan(ComplexD value)
		{
		    return ComplexD.Tan(value);
		}
		#endregion

		#region Clamp
		/// <summary>
		/// Clamp a <paramref name="value"/> to <paramref name="destValue"/> if it is withon the <paramref name="tolerance"/> range.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="destValue">The clamped value.</param>
		/// <param name="tolerance">The tolerance value.</param>
		/// <returns>
		/// Returns the clamped value.
		/// result = (tolerance > Abs(value-destValue)) ? destValue : value;
		/// </returns>
		public static double Clamp(double value, double destValue, double tolerance)
		{
            return (tolerance > Math.Abs(value - destValue)) ? destValue : value;
		}
		/// <summary>
		/// Clamp a <paramref name="value"/> to <paramref name="destValue"/> using the default tolerance value.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="destValue">The clamped value.</param>
		/// <returns>
		/// Returns the clamped value.
		/// result = (EpsilonD > Abs(value-destValue)) ? destValue : value;
		/// </returns>
		/// <remarks><see cref="MathFunctions.EpsilonD"/> is used for tolerance.</remarks>
		public static double Clamp(double value, double destValue)
		{
            return (EpsilonD > Math.Abs(value - destValue)) ? destValue : value;
		}
		#endregion

		#region MinValue
		/// <summary>
		/// Calculates the minimum value of a given array's elements.
		/// </summary>
		/// <param name="array">An array of integers.</param>
		/// <returns>The minimum value.</returns>
		/// <exception cref="ArgumentException">The if the given array's length is zero.</exception>
		public static int MinValue(int[] array)
		{
			if (array.Length == 0)
				throw new ArgumentException("Array has zero elements.", "array");

			int value = array[0];
			foreach(int i in array)
			{
				if (i < value)
					value = i;
			}

			return value;
		}
		/// <summary>
		/// Calculates the minimum value of a given array's elements.
		/// </summary>
		/// <param name="array">An array of integers.</param>
		/// <returns>The minimum value.</returns>
		/// <exception cref="ArgumentException">The if the given array's length is zero.</exception>
		public static int MinValue(IntArrayList array)
		{
			if (array.Count == 0)
				throw new ArgumentException("Array has zero elements.", "array");

			int value = array[0];
			foreach(int i in array)
			{
				if (i < value)
					value = i;
			}

			return value;
		}
		/// <summary>
		/// Calculates the minimum value of a given array's elements.
		/// </summary>
		/// <param name="array">An array of double-precision doubleing point numbers.</param>
		/// <returns>The minimum value.</returns>
		/// <exception cref="ArgumentException">The if the given array's length is zero.</exception>
		public static double MinValue(double[] array)
		{
			if (array.Length == 0)
				throw new ArgumentException("Array has zero elements.", "array");

			double value = array[0];
			foreach(double d in array)
			{
				if (d < value)
					value = d;
			}

			return value;
		}
		/// <summary>
		/// Calculates the minimum value of a given array's elements.
		/// </summary>
		/// <param name="array">An array of double-precision doubleing point numbers.</param>
		/// <returns>The minimum value.</returns>
		/// <exception cref="ArgumentException">The if the given array's length is zero.</exception>
		public static double MinValue(DoubleArrayList array)
		{
			if (array.Count == 0)
				throw new ArgumentException("Array has zero elements.", "array");

			double value = array[0];
			foreach(double d in array)
			{
				if (d < value)
					value = d;
			}

			return value;
		}

		/// <summary>
		/// Calculates the minimum value of a given vector's elements.
		/// </summary>
		/// <param name="vector">A <see cref="Vector2"/> instance.</param>
		/// <returns>The minimum value.</returns>
        public static double MinValue(Vector2 vector)
		{
			return System.Math.Min(vector.X, vector.Y);
		}
		/// <summary>
		/// Calculates the minimum value of a given vector's elements.
		/// </summary>
		/// <param name="vector">A <see cref="Vector3"/> instance.</param>
		/// <returns>The minimum value.</returns>
        public static double MinValue(Vector3 vector)
		{
			return System.Math.Min(vector.X, System.Math.Min(vector.Y, vector.Z));
		}
		/// <summary>
		/// Calculates the minimum value of a given vector's elements.
		/// </summary>
		/// <param name="vector">A <see cref="Vector4"/> instance.</param>
		/// <returns>The minimum value.</returns>
        public static double MinValue(Vector4 vector)
		{
			return System.Math.Min(vector.X, System.Math.Min(vector.Y, System.Math.Min(vector.Z, vector.W)));
		}
		#endregion

		#region MaxValue
		/// <summary>
		/// Calculates the maximum value of a given array's elements.
		/// </summary>
		/// <param name="array">An array of integers.</param>
		/// <returns>The maximum value.</returns>
		/// <exception cref="ArgumentException">The if the given array's length is zero.</exception>
		public static int MaxValue(int[] array)
		{
			if (array.Length == 0)
				throw new ArgumentException("Array has zero elements.", "array");

			int value = array[0];
			foreach(int i in array)
			{
				if (i > value)
					value = i;
			}

			return value;
		}
		/// <summary>
		/// Calculates the maximum value of a given array's elements.
		/// </summary>
		/// <param name="array">An array of integers.</param>
		/// <returns>The maximum value.</returns>
		/// <exception cref="ArgumentException">The if the given array's length is zero.</exception>
		public static int MaxValue(IntArrayList array)
		{
			if (array.Count == 0)
				throw new ArgumentException("Array has zero elements.", "array");

			int value = array[0];
			foreach(int i in array)
			{
				if (i > value)
					value = i;
			}

			return value;
		}
		/// <summary>
		/// Calculates the maximum value of a given array's elements.
		/// </summary>
		/// <param name="array">An array of double-precision doubleing point numbers.</param>
		/// <returns>The maximum value.</returns>
		/// <exception cref="ArgumentException">The if the given array's length is zero.</exception>
		public static double MaxValue(double[] array)
		{
			if (array.Length == 0)
				throw new ArgumentException("Array has zero elements.", "array");

			double value = array[0];
			foreach(double d in array)
			{
				if (d > value)
					value = d;
			}

			return value;
		}
		/// <summary>
		/// Calculates the maximum value of a given array's elements.
		/// </summary>
		/// <param name="array">An array of double-precision doubleing point numbers.</param>
		/// <returns>The maximum value.</returns>
		/// <exception cref="ArgumentException">The if the given array's length is zero.</exception>
		public static double MaxValue(DoubleArrayList array)
		{
			if (array.Count == 0)
				throw new ArgumentException("Array has zero elements.", "array");

			double value = array[0];
			foreach(double d in array)
			{
				if (d > value)
					value = d;
			}

			return value;
		}

		/// <summary>
		/// Calculates the maximum value of a given vector's elements.
		/// </summary>
		/// <param name="vector">A <see cref="Vector2"/> instance.</param>
		/// <returns>The maximum value.</returns>
        public static double MaxValue(Vector2 vector)
		{
			return System.Math.Max(vector.X, vector.Y);
		}
		/// <summary>
		/// Calculates the maximum value of a given vector's elements.
		/// </summary>
		/// <param name="vector">A <see cref="Vector3"/> instance.</param>
		/// <returns>The maximum value.</returns>
        public static double MaxValue(Vector3 vector)
		{
			return System.Math.Max(vector.X, System.Math.Max(vector.Y, vector.Z));
		}
		/// <summary>
		/// Calculates the maximum value of a given vector's elements.
		/// </summary>
		/// <param name="vector">A <see cref="Vector4"/> instance.</param>
		/// <returns>The maximum value.</returns>
		public static double MaxValue(Vector4 vector)
		{
			return System.Math.Max(vector.X, System.Math.Max(vector.Y, System.Math.Max(vector.Z, vector.W)));
		}
		#endregion

		#region MinAbsValue
		/// <summary>
		/// Calculates the minimum value of a given array's elements absolute values.
		/// </summary>
		/// <param name="array">An array of integers.</param>
		/// <returns>The minimum value.</returns>
		/// <exception cref="ArgumentException">The if the given array's length is zero.</exception>
		public static int MinAbsValue(int[] array)
		{
			//if (array.Length == 0)
			//	throw new InvalidArgumentException();

			int value = array[0];
			foreach(int i in array)
			{
                if (Math.Abs(i) < value)
					value = i;
			}

			return value;
		}
		/// <summary>
		/// Calculates the minimum value of a given array's elements absolute values.
		/// </summary>
		/// <param name="array">An array of integers.</param>
		/// <returns>The minimum value.</returns>
		/// <exception cref="ArgumentException">The if the given array's length is zero.</exception>
		public static int MinAbsValue(IntArrayList array)
		{
			//if (IntArrayList.Count == 0)
			//	throw new InvalidArgumentException();

			int value = array[0];
			foreach(int i in array)
			{
                if (Math.Abs(i) < value)
					value = i;
			}

			return value;
		}
		/// <summary>
		/// Calculates the minimum value of a given array's elements absolute values.
		/// </summary>
		/// <param name="array">An array of double-precision doubleing point numbers.</param>
		/// <returns>The minimum value.</returns>
		/// <exception cref="ArgumentException">The if the given array's length is zero.</exception>
		public static double MinAbsValue(double[] array)
		{
			//if (array.Length == 0)
			//	throw new InvalidArgumentException();

			double value = array[0];
			foreach(double d in array)
			{
                if (Math.Abs(d) < value)
					value = d;
			}

			return value;
		}
		/// <summary>
		/// Calculates the minimum value of a given array's elements absolute values.
		/// </summary>
		/// <param name="array">An array of double-precision doubleing point numbers.</param>
		/// <returns>The minimum value.</returns>
		/// <exception cref="ArgumentException">The if the given array's length is zero.</exception>
		public static double MinAbsValue(DoubleArrayList array)
		{
			//if (IntArrayList.Count == 0)
			//	throw new InvalidArgumentException();

			double value = array[0];
			foreach(double d in array)
			{
                if (Math.Abs(d) < value)
					value = d;
			}

			return value;
		}
		#endregion

		#region MaxAbsValue
		/// <summary>
		/// Calculates the maximum value of a given array's elements absolute values.
		/// </summary>
		/// <param name="array">An array of integers.</param>
		/// <returns>The maximum value.</returns>
		/// <exception cref="ArgumentException">The if the given array's length is zero.</exception>
		public static int MaxAbsValue(int[] array)
		{
			//if (array.Length == 0)
			//	throw new InvalidArgumentException();

			int value = array[0];
			foreach(int i in array)
			{
                if (Math.Abs(i) > value)
					value = i;
			}

			return value;
		}
		/// <summary>
		/// Calculates the maximum value of a given array's elements absolute values.
		/// </summary>
		/// <param name="array">An array of integers.</param>
		/// <returns>The maximum value.</returns>
		/// <exception cref="ArgumentException">The if the given array's length is zero.</exception>
		public static int MaxAbsValue(IntArrayList array)
		{
			//if (IntArrayList.Count == 0)
			//	throw new InvalidArgumentException();

			int value = array[0];
			foreach(int i in array)
			{
                if (Math.Abs(i) > value)
					value = i;
			}

			return value;
		}
		/// <summary>
		/// Calculates the maximum value of a given array's elements absolute values.
		/// </summary>
		/// <param name="array">An array of double-precision doubleing point numbers.</param>
		/// <returns>The maximum value.</returns>
		/// <exception cref="ArgumentException">The if the given array's length is zero.</exception>
		public static double MaxAbsValue(double[] array)
		{
			//if (array.Length == 0)
			//	throw new InvalidArgumentException();

			double value = array[0];
			foreach(double d in array)
			{
                if (Math.Abs(d) > value)
					value = d;
			}

			return value;
		}
		/// <summary>
		/// Calculates the maximum value of a given array's elements absolute values.
		/// </summary>
		/// <param name="array">An array of double-precision doubleing point numbers.</param>
		/// <returns>The maximum value.</returns>
		/// <exception cref="ArgumentException">The if the given array's length is zero.</exception>
		public static double MaxAbsValue(DoubleArrayList array)
		{
			//if (IntArrayList.Count == 0)
			//	throw new InvalidArgumentException();

			double value = array[0];
			foreach(double d in array)
			{
                if (Math.Abs(d) > value)
					value = d;
			}

			return value;
		}
		#endregion

		#region Mean
		/// <summary>
		/// Calculates the mean of a given array's elements.
		/// </summary>
		/// <param name="array">An array of integers.</param>
		/// <returns>The mean of the elements in <paramref name="array"/>.</returns>
		public static double Mean(int[] array)
		{
			return Sum(array) / array.Length;
		}
		/// <summary>
		/// Calculates the mean of a given array's elements.
		/// </summary>
		/// <param name="array">An array of integers.</param>
		/// <returns>The mean of the elements in <paramref name="array"/>.</returns>
		public static double Mean(IntArrayList array)
		{
			return Sum(array) / array.Count;
		}
		/// <summary>
		/// Calculates the mean of a given array's elements.
		/// </summary>
		/// <param name="array">An array of double-precision doubleing point values.</param>
		/// <returns>The mean of the elements in <paramref name="array"/>.</returns>
		public static double Mean(double[] array)
		{
			return Sum(array) / array.Length;
		}
		/// <summary>
		/// Calculates the mean of a given array's elements.
		/// </summary>
		/// <param name="array">An array of double-precision doubleing point values.</param>
		/// <returns>The mean of the elements in <paramref name="array"/>.</returns>
		public static double Mean(DoubleArrayList array)
		{
			return Sum(array) / array.Count;
		}
		#endregion

		#region Variance
		/// <summary>
		/// Calculates the variance of the given array.
		/// </summary>
		/// <param name="array">An array of double-precision doubleing point values.</param>
		/// <returns>The variance of the array elements.</returns>
		public static double Variance(int[] array)
		{
			double variance = 0;
			double delta = 0;
			double mean = Mean(array);

			for (int i = 0; i < array.Length; i++)
			{
				delta = array[i] - mean;
				variance += (delta*delta - variance) / (i+1);
			}

			return variance;
		}

		/// <summary>
		/// Calculates the variance of the given array.
		/// </summary>
		/// <param name="array">An array of double-precision doubleing point values.</param>
		/// <returns>The variance of the array elements.</returns>
		public static double Variance(IntArrayList array)
		{
			double variance = 0;
			double delta = 0;
			double mean = Mean(array);

			for (int i = 0; i < array.Count; i++)
			{
				delta = array[i] - mean;
				variance += (delta*delta - variance) / (i+1);
			}

			return variance;
		}
		/// <summary>
		/// Calculates the variance of the given array.
		/// </summary>
		/// <param name="array">An array of double-precision doubleing point values.</param>
		/// <returns>The variance of the array elements.</returns>
		public static double Variance(double[] array)
		{
			double variance = 0;
			double delta = 0;
			double mean = Mean(array);

			for (int i = 0; i < array.Length; i++)
			{
				delta = array[i] - mean;
				variance += (delta*delta - variance) / (i+1);
			}

			return variance;
		}

		/// <summary>
		/// Calculates the variance of the given array.
		/// </summary>
		/// <param name="array">An array of double-precision doubleing point values.</param>
		/// <returns>The variance of the array elements.</returns>
		public static double Variance(DoubleArrayList array)
		{
			double variance = 0;
			double delta = 0;
			double mean = Mean(array);

			for (int i = 0; i < array.Count; i++)
			{
				delta = array[i] - mean;
				variance += (delta*delta - variance) / (i+1);
			}

			return variance;
		}

		#endregion

		#region CountPositives
		/// <summary>
		/// Calculates the number of positive values in the given array.
		/// </summary>
		/// <param name="array">An array of integers.</param>
		/// <returns>The number of positive values in the array</returns>
		public static int CountPositives(int[] array)
		{
			int count = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] > 0)
					count++;
			}
			return count;
		}
		/// <summary>
		/// Calculates the number of positive values in the given array.
		/// </summary>
		/// <param name="array">An array of integers.</param>
		/// <returns>The number of positive values in the array</returns>
		public static int CountPositives(IntArrayList array)
		{
			int count = 0;
			for (int i = 0; i < array.Count; i++)
			{
				if (array[i] > 0)
					count++;
			}
			return count;
		}
		/// <summary>
		/// Calculates the number of positive values in the given array.
		/// </summary>
		/// <param name="array">An array of double-precision doubleing point numbers.</param>
		/// <returns>The number of positive values in the array</returns>
		public static int CountPositives(double[] array)
		{
			int count = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] > 0)
					count++;
			}
			return count;
		}
		/// <summary>
		/// Calculates the number of positive values in the given array.
		/// </summary>
		/// <param name="array">An array of double-precision doubleing point numbers.</param>
		/// <returns>The number of positive values in the array</returns>
		public static int CountPositives(DoubleArrayList array)
		{
			int count = 0;
			for (int i = 0; i < array.Count; i++)
			{
				if (array[i] > 0)
					count++;
			}
			return count;
		}
		#endregion

		#region CountNegatives
		/// <summary>
		/// Calculates the number of negative values in the given array.
		/// </summary>
		/// <param name="array">An array of integers.</param>
		/// <returns>The number of positive values in the array</returns>
		public static int CountNegatives(int[] array)
		{
			int count = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] < 0)
					count++;
			}
			return count;
		}
		/// <summary>
		/// Calculates the number of negative values in the given array.
		/// </summary>
		/// <param name="array">An array of integers.</param>
		/// <returns>The number of positive values in the array</returns>
		public static int CountNegatives(IntArrayList array)
		{
			int count = 0;
			for (int i = 0; i < array.Count; i++)
			{
				if (array[i] < 0)
					count++;
			}
			return count;
		}
		/// <summary>
		/// Calculates the number of negative values in the given array.
		/// </summary>
		/// <param name="array">An array of double-precision doubleing point numbers.</param>
		/// <returns>The number of positive values in the array</returns>
		public static int CountNegatives(double[] array)
		{
			int count = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] < 0)
					count++;
			}
			return count;
		}
		/// <summary>
		/// Calculates the number of negative values in the given array.
		/// </summary>
		/// <param name="array">An array of double-precision doubleing point numbers.</param>
		/// <returns>The number of positive values in the array</returns>
		public static int CountNegatives(DoubleArrayList array)
		{
			int count = 0;
			for (int i = 0; i < array.Count; i++)
			{
				if (array[i] < 0)
					count++;
			}
			return count;
		}
		#endregion

		#region GetSign
		/// <summary>
		/// Returns a value indicating the sign of a given integer.
		/// </summary>
		/// <param name="value">A signed number.</param>
		/// <returns>A <see cref="MathFunctions.Sign"/> value.</returns>
		public static Sign GetSign(int value)
		{
			return (Sign)System.Math.Sign(value);
		}
		/// <summary>
		/// Returns a value indicating the sign of a given double.
		/// </summary>
		/// <param name="value">A single precision doubleing point number.</param>
		/// <returns>A <see cref="MathFunctions.Sign"/> value.</returns>
		public static Sign GetSign(double value)
		{
			return (Sign)System.Math.Sign(value);
		}
		/// <summary>
		/// Returns a value indicating the sign of a given integer.
		/// </summary>
		/// <param name="value">A single precision doubleing point number.</param>
		/// <param name="tolerance">The tolerance value to use.</param>
		/// <returns>A <see cref="MathFunctions.Sign"/> value.</returns>
		/// <remarks>
		/// If the value if larger than the tolerance value, the result is positive.
		/// If the value if smaller than the negative tolerance value (-tolerance), the result is negative.
		/// if value if in the [-tolerance, tolerance] range the result is zero.
		/// </remarks>
		public static Sign GetSign(double value, double tolerance)
		{
			if( value > tolerance )	
			{					
				return	Sign.Positive;
			}
			if( value < -tolerance )	
			{
				return	Sign.Negative;
			}
			return	Sign.Zero;
		}
		#endregion

		#region ApproxEquals
		/// <summary>
		/// Tests whether two single-precision doubleing point numbers are approximately equal using default tolerance value.
		/// </summary>
		/// <param name="a">A single-precision doubleing point number.</param>
		/// <param name="b">A single-precision doubleing point number.</param>
		/// <returns><see langword="true"/> if the two vectors are approximately equal; otherwise, <see langword="false"/>.</returns>
		public static bool ApproxEquals(double a, double b)
		{
			return (System.Math.Abs(a-b) <= EpsilonF);
		}
		/// <summary>
		/// Tests whether two single-precision doubleing point numbers are approximately equal given a tolerance value.
		/// </summary>
		/// <param name="a">A single-precision doubleing point number.</param>
		/// <param name="b">A single-precision doubleing point number.</param>
		/// <param name="tolerance">The tolerance value used to test approximate equality.</param>
		/// <returns><see langword="true"/> if the two vectors are approximately equal; otherwise, <see langword="false"/>.</returns>
		public static bool ApproxEquals(double a, double b, double tolerance)
		{
			return (System.Math.Abs(a-b) <= tolerance);
		}
		#endregion

		#region Swap
		/// <summary>
		/// Swaps two values.
		/// </summary>
		/// <param name="a">A double-precision doubleing point number.</param>
		/// <param name="b">A double-precision doubleing point number.</param>
		public static void Swap(ref double a, ref double b) 
		{
			double c = a;
			a = b;
			b = c;
		}
		/// <summary>
		/// Swaps two values.
		/// </summary>
		/// <param name="a">A <see cref="decimal"/> value.</param>
		/// <param name="b">A <see cref="decimal"/> value.</param>
		public static void Swap(ref decimal a, ref decimal b) 
		{
			decimal c = a;
			a = b;
			b = c;
		}
		/// <summary>
		/// Swaps two values.
		/// </summary>
		/// <param name="a">A <see cref="sbyte"/> value.</param>
		/// <param name="b">A <see cref="sbyte"/> value.</param>
		public static void Swap(ref sbyte a, ref sbyte b) 
		{
			sbyte c = a;
			a = b;
			b = c;
		}
		/// <summary>
		/// Swaps two values.
		/// </summary>
		/// <param name="a">A <see cref="byte"/> value.</param>
		/// <param name="b">A <see cref="byte"/> value.</param>
		public static void Swap(ref byte a, ref byte b) 
		{
			byte c = a;
			a = b;
			b = c;
		}
		/// <summary>
		/// Swaps two values.
		/// </summary>
		/// <param name="a">A <see cref="char"/> value.</param>
		/// <param name="b">A <see cref="char"/> value.</param>
		public static void Swap(ref char a, ref char b) 
		{
			char c = a;
			a = b;
			b = c;
		}
		/// <summary>
		/// Swaps two values.
		/// </summary>
		/// <param name="a">A <see cref="short"/> value.</param>
		/// <param name="b">A <see cref="short"/> value.</param>
		public static void Swap(ref short a, ref short b) 
		{
			short c = a;
			a = b;
			b = c;
		}
		/// <summary>
		/// Swaps two values.
		/// </summary>
		/// <param name="a">A <see cref="ushort"/> value.</param>
		/// <param name="b">A <see cref="ushort"/> value.</param>
		public static void Swap(ref ushort a, ref ushort b) 
		{
			ushort c = a;
			a = b;
			b = c;
		}
		/// <summary>
		/// Swaps two values.
		/// </summary>
		/// <param name="a">A <see cref="int"/> value.</param>
		/// <param name="b">A <see cref="int"/> value.</param>
		public static void Swap(ref int a, ref int b) 
		{
			int c = a;
			a = b;
			b = c;
		}
		/// <summary>
		/// Swaps two values.
		/// </summary>
		/// <param name="a">A <see cref="uint"/> value.</param>
		/// <param name="b">A <see cref="uint"/> value.</param>
		public static void Swap(ref uint a, ref uint b) 
		{
			uint c = a;
			a = b;
			b = c;
		}

		/// <summary>
		/// Swaps two values.
		/// </summary>
		/// <param name="a">A <see cref="long"/> value.</param>
		/// <param name="b">A <see cref="long"/> value.</param>
		public static void Swap(ref long a, ref long b) 
		{
			long c = a;
			a = b;
			b = c;
		}
		/// <summary>
		/// Swaps two values.
		/// </summary>
		/// <param name="a">A <see cref="ulong"/> value.</param>
		/// <param name="b">A <see cref="ulong"/> value.</param>
		public static void Swap(ref ulong a, ref ulong b) 
		{
			ulong c = a;
			a = b;
			b = c;
		}

		#endregion

		#region Linear Interpolation
		/// <summary>
		/// Interpolate two values using linear interpolation.
		/// </summary>
		/// <param name="a">A double-precision doubleing point number representing the first point.</param>
		/// <param name="b">A double-precision doubleing point number representing the second point.</param>
		/// <param name="x">A double-precision doubleing point number between 0 and 1 ( [0,1] ).</param>
		/// <returns>The interpolated value.</returns>
		public static double LinearInterpolation(double a, double b, double x)
		{
			return a*(1-x) + b*x;
		}
		#endregion

		#region Cosine Interpolation
		/// <summary>
		/// Interpolate two values using cosine interpolation.
		/// </summary>
		/// <param name="a">A double-precision doubleing point number representing the first point.</param>
		/// <param name="b">A double-precision doubleing point number representing the second point.</param>
		/// <param name="x">A double-precision doubleing point number between 0 and 1 ( [0,1] ).</param>
		/// <returns></returns>
		public static double CosineInterpolation(double a, double b, double x)
		{
			double ft = (double)(x * PI);
			double f = (1 - System.Math.Cos(ft)) * 0.5;
			return a*(1-f) + b*f;
		}
		#endregion

		#region Cubic Interpolation
		/// <summary>
		/// Interpolate two values using cubic interpolation.
		/// </summary>
		/// <param name="a">A double-precision doubleing point number representing the first point.</param>
		/// <param name="b">A double-precision doubleing point number representing the second point.</param>
		/// <param name="x">A double-precision doubleing point number between 0 and 1 ( [0,1] ).</param>
		/// <returns></returns>
		public static double CubicInterpolation(double a, double b, double x) 
		{
			double fac1 = 3*System.Math.Pow(1-x, 2) - 2*System.Math.Pow(1-x,3);
			double fac2 = 3*System.Math.Pow(x, 2) - 2*System.Math.Pow(x, 3);

			return a*fac1 + b*fac2; //add the weighted factors
		}
		#endregion

		#region Primes
		/// <summary>
		/// Checks if the given value is a prime number.
		/// </summary>
		/// <param name="value">The number to check.</param>
		/// <returns><c>True</c> if the number is a prime; otherwise, <c>False</c>.</returns>
		public static bool IsPrime(long value)
		{
			int sqrtValue = (int)System.Math.Sqrt(value);

			for (int i = 2; i <= sqrtValue; i++)
			{
				if ((value % i) == 0)
					return false;
			}

			return true;
		}
		#endregion

        public static Vector3 Pow2(Vector3 v)
        {
            return new Vector3(v.X * v.X, v.Y * v.Y, v.Z * v.Z);
        }
        
        public static Vector3 Sqrt(Vector3 v)
        {
            return new Vector3(Sqrt(v.X), Sqrt(v.Y), Sqrt(v.Z));
        }
    }
}
