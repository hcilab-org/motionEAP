// <copyright file=PointCloud.cs
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
using System.Runtime.Serialization;

namespace HciLab.Utilities.Mathematics.Geometry3D
{
    [Serializable]
    //[TypeConverter(typeof(PointCloudConverter))]
    public class PointCloud : ISerializable, ICloneable
    {
        private int m_SerVersion = 1;

        private List<Vector3> m_Points = new List<Vector3>();

        public PointCloud() { }

        public PointCloud(List<Vector3> pPoints)
        {
            m_Points = pPoints;
        }

        public PointCloud (PointCloud pPointCloud)
        {
            this.m_Points = pPointCloud.Points;
        }


        protected PointCloud(SerializationInfo info, StreamingContext context)
        {
            // for version evaluation while deserializing
            int pSerVersionMobilitaet = info.GetInt32("m_SerVersion");
        }


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("m_SerVersion", m_SerVersion);
        }

        #region ICloneable Members
        /// <summary>
        /// Creates an exact copy of this <see cref="Polygon"/> object.
        /// </summary>
        /// <returns>The <see cref="Polygon"/> object this method creates, cast as an object.</returns>
        object ICloneable.Clone()
        {
            return new PointCloud(this);
        }
        /// <summary>
        /// Creates an exact copy of this <see cref="Polygon"/> object.
        /// </summary>
        /// <returns>The <see cref="Polygon"/> object this method creates.</returns>
        public PointCloud Clone()
        {
            return new PointCloud(this);
        }
        #endregion

        
        public List<Vector3> Points
        {
            get { return m_Points; }
            set { m_Points = value;}
        }

        public void AddPoints(List<Vector3> list)
        {
            foreach (Vector3 v in list)
                m_Points.Add(v);

            /*foreach (KeyValuePair<double, Dictionary<double, double>> x in dictionary)
            {
                foreach (KeyValuePair<double, double> y in x.Value)
                {
                    if (!m_Points.ContainsKey(x.Key))
                        m_Points.Add(x.Key, new Dictionary<double, double>());
                    if (!m_Points[x.Key].ContainsKey(y.Key))
                        m_Points[x.Key].Add(y.Key, 0);

                    m_Points[x.Key][y.Key] = y.Value;
                }
            }*/
        }


        public Plane GetFitPlane()
        {
            double[,] solvM = new double[3, 4];

            foreach (Vector3 v in m_Points)
            {
                solvM[0, 0] += v.X * v.X;
                solvM[0, 1] += v.X * v.Y;
                solvM[0, 2] += v.X;

                solvM[1, 0] += v.Y * v.X;
                solvM[1, 1] += v.Y * v.Y;
                solvM[1, 2] += v.Y;

                solvM[2, 0] += v.X;
                solvM[2, 1] += v.Y;
                solvM[2, 2] += 1;
            }

            foreach (Vector3 v in m_Points)
            {
                solvM[0, 3] += v.X * v.Z;
                solvM[1, 3] += v.Z * v.Z;
                solvM[2, 3] += v.Z;
            }

            List<double> resultSolv = new List<double>();
            Boolean okay = LinearEquationSolver.Solve(solvM, out resultSolv);

            if (okay == true)
                return Plane.CreateFormFunctionXYCoefficient(resultSolv[0], resultSolv[1], resultSolv[2]);
            else
                return null;
        }
            
    }
}
