// <copyright file=AdaptivityLevel.cs
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
// <date> 11/2/2016 12:25:59 PM</date>

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using HciLab.Utilities;

namespace motionEAPAdmin.Model.Process
{
    /// <summary>
    /// This class represents an adaptivity level.
    /// </summary>
    /// 
    /// 
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    [Serializable()]
    public class AdaptivityLevel : DataBaseClass, ISerializable
    {
        private string m_Name; // the name of this level
        
        public static List<AdaptivityLevel> AdaptivityLevels = new List<AdaptivityLevel>
        {
            new AdaptivityLevel(1, "Easy"),
            new AdaptivityLevel(2, "Medium"),
            new AdaptivityLevel(3, "Hard")
        };


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pId"></param>
        /// <param name="pName"></param>
        public AdaptivityLevel(int pId, string pName)
            : base(pId)
        {
            m_Name = pName;
        }
        
        protected AdaptivityLevel(SerializationInfo pInfo, StreamingContext pContext)
            : base(pInfo, pContext)
        {
            m_Name = pInfo.GetString("m_Name");
        }


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("m_Name", m_Name);
            
        }

        #region getter/setter
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }
        #endregion
    }
}
