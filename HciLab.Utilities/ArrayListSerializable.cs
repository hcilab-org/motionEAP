// <copyright file=ArrayListSerializable.cs
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
// <date> 11/2/2016 12:25:56 PM</date>

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace HciLab.Utilities
{
    /// <summary>
	/// TPMArrayList is a Wrapperclass for the .Net ArrayList Class.
	/// This Class makes an ArrayList accessible via COM.
	/// </summary>
	[ClassInterface(ClassInterfaceType.AutoDual)]
	[ComVisible(true)]
	[Serializable()]
	public class ArrayListSerializable : ISerializable
	{

        public static void Shuffle(ArrayListSerializable list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                object value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

		/// <summary>
		/// Kontrollvariable, die die Klassenversion wiederspiegelt, in der zuletzt eine Änderung durchgeführt wurde.
		/// </summary>
		private int m_SerVersionArrayList = 1;
		
		/// <summary>
		/// The objects are stored in the inner ArrayList.
		/// </summary>
		private ArrayList m_Inner;

		
		/// <summary>
		/// Standard constructor.
		/// </summary>
		public ArrayListSerializable()
		{
			// initializing the inner ArrayList.
			m_Inner = new ArrayList();
		}

		/// <summary>
		/// Number of objects in the ArrayList.
		/// </summary>
		public int Count 
		{
			get
			{
                return m_Inner.Count;
            }
		}
		

		/// <summary>
		/// Adds an element to the ArrayList.
		/// </summary>
		/// <param name="pObject">Object to add</param>
		/// <returns>True, if successfull</returns>
		public bool Add (object pObject)
		{
			/*if (pObject == null)
			{
                return false;
			}
			if (m_Inner.Contains(pObject))
			{
				
				return false;
			}*/
			m_Inner.Add(pObject);
			return true;
		}

		/// <summary>
		/// Removes an object.
		/// </summary>
		/// <param name="pObject">Object.</param>
		/// <returns>True, if successfull.</returns>
		public bool Remove(
			object pObject
			)
		{
			if (!m_Inner.Contains(pObject))
			{
				return false;
			}
			m_Inner.Remove(pObject);
			return true;
		}

		/// <summary>
		/// Removes an object by its index
		/// </summary>
		/// <param name="pIndex">Index of the object.</param>
		/// <returns>True, if successfull.</returns>
		public bool RemoveAt(
			int pIndex
			)
		{
			if (pIndex < 0 ) {return false;}
			if (pIndex > m_Inner.Count) {return false;}
			m_Inner.RemoveAt(pIndex);
			return true;
		}

        public bool Insert(int pIndex, object pObject)
        {
            if (pIndex < 0) { return false; }
            if (pObject == null){return false;}
            m_Inner.Insert(pIndex, pObject);
            return true;
        }

		/// <summary>
		/// Returns the Enumerator of the ArrayList.
		/// </summary>
		/// <returns>Enumerator</returns>
		public IEnumerator GetEnumerator()
		{
			return m_Inner.GetEnumerator();
		}



		/// <summary>
		/// Access the objects.
		/// </summary>
		[XmlIgnore]
		public object this[int pIndex]
		{
			get
			{
				if (pIndex < 0 ) {return null;}
				if (pIndex >= m_Inner.Count) {return null;}
				return m_Inner[pIndex];
			}
            set
            {
                m_Inner[pIndex] = value;
            }

		}



		/// <summary>
		/// Checks, if the ArrayList contains an object.
		/// </summary>
		/// <param name="pObject">Object</param>
		/// <returns>True, if contained</returns>
		public bool Contains(object pObject)
		{
			return m_Inner.Contains(pObject);
		}
		
		
		/// <summary>
		/// Deletes all elements in the ArrayList
		/// </summary>
		public void Clear()
		{
			m_Inner.Clear();
		}


		/// <summary>
		/// Returns as information the type of the first element
		/// in this collection.
		/// As a hashtable is not typesafe, other types can be accidently included!
		/// </summary>
		/// <returns>Typeinformation of first element.</returns>
		public string GetTypeOfFirstElement()
		{
			if (m_Inner.Count < 1)
			{
				return "";
			}
			return m_Inner[0].GetType().ToString();
		}

		/// <summary>
		/// Array of objects for XML Serialization
		/// </summary>
		[XmlArray ("ContainedObjects"),
		XmlArrayItem("object", typeof(object)),
		XmlArrayItem("string", typeof(string)),
		]
		[ComVisible(false)]
		public object[] PropertyForXMLSerialization
		{
			get
			{
				object[] pObjs= new object[this.m_Inner.Count];
				int i = 0;
				foreach (object pObj in this.m_Inner)
				{
					pObjs[i] = pObj;
					i++;
				}
				return pObjs;
			}
			set
			{
				// empty, but necessary for XML Serialization!
			}
		}

		/// <summary>			
		/// Deserialization constructor			
		/// </summary>			
		/// <param name="info"></param>			
		/// <param name="context"></param>			
        protected ArrayListSerializable(SerializationInfo info, 			
			StreamingContext context)  
		{
			// serialization of objects, that remained the same		
			m_Inner = (ArrayList)info.GetValue("m_Inner", typeof(ArrayList));		
		} 			
					
		public void GetObjectData(SerializationInfo info, StreamingContext context) 
		{
			info.AddValue("m_SerVersionArrayList", m_SerVersionArrayList);		
			info.AddValue("m_Inner", m_Inner);		
		}


        public Boolean ReplaceOrAdd(Object pObject)
        {
            if (pObject is DataBaseClass)
            {
                DataBaseClass obj = (DataBaseClass)pObject;
                for (int i = 0; i < m_Inner.Count; i++)
                {
                    if (m_Inner[i] is DataBaseClass && ((DataBaseClass)m_Inner[i]).Id == obj.Id)
                    {
                        m_Inner[i] = pObject;
                        return true;
                    }
                }
            }
            else
            {
                throw new NotImplementedException();
            }
            return Add(pObject);
        }

        public ArrayList ToArrayList()
        {
            return m_Inner;
        }

        public int IndexOf(Object pValue)
        {
            return m_Inner.IndexOf(pValue);
            
        }

        public void Sort(IComparer comparer)
        {
            m_Inner.Sort(comparer);
        }

        public object LastElement()
        {
            return m_Inner[this.Count-1];
        }

        public Object GetObjectById(int pId)
        {
            foreach (object pObj in this.m_Inner)
            {
                if (pObj is DataBaseClass)
                {
                    if ((pObj as DataBaseClass).Id == pId)
                        return pObj;
                }
            }
            return null;
        }

        public Boolean RemoveObjectById (int pId)
        {
            for (int i = 0; i < this.m_Inner.Count; i++)
            {
                object pObj = this.m_Inner[i];
                if (pObj is DataBaseClass)
                {
                    if ((pObj as DataBaseClass).Id == pId)
                    {
                        RemoveAt(i);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
