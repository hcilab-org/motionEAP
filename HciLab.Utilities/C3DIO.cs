// <copyright file=C3DIO.cs
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

using HciLab.Utilities.C3D;
using HciLab.Utilities.Mathematics.Core;
using System;
using System.Collections.Generic;

namespace HciLab.Utilities
{
    public class C3DIO
    {
        C3dReader m_Reader = null;
        String m_FileName = String.Empty;


        public C3DIO(String pFileName)
        {
            m_FileName = pFileName;
        }

        public void open (out Boolean pIsComplete, out String pError)
        {
            ArrayListSerializable list = new ArrayListSerializable();

            try
            {
                m_Reader = new C3dReader();

                if (!m_Reader.Open(m_FileName))
                {
                    pError = "Error: Unable to open file " + m_FileName;
                    pIsComplete = false;
                }
                else
                {
                    pError = "Okay";
                    pIsComplete = true;
                }
            }
            catch (Exception e)
            {
                pError = e.Message;
                pIsComplete = false;
            }
        }


        public Dictionary<TimeSpan, Vector3[]> read()
        {
            if (m_Reader == null)
                throw new NullReferenceException("m_Reader is Null");

            Dictionary<TimeSpan, Vector3[]> list = new Dictionary<TimeSpan, Vector3[]>();
            int i = 0;
            float lastProcent = -1;
            try
            {

                //for (int i = 0; i < Math.Abs(reader.GetParameter<Int16>("POINT:FRAMES")); i++)
                
                while (true)
                {
                    // returns an array of all points, it is necessary to call this method in each cycle

                    Vector3[] array = m_Reader.ReadFrame();

                    if (array == null)
                        break;
                    TimeSpan t = TimeSpan.FromMilliseconds(i*10);

                    list.Add(t, array);

                    //Vector3 e = reader["Spine"];
                    // we can ask for specific point - you can check labels in reader.Labels
                    //Vector3 spine = array[1];

                    // get analog data for this frame
                    //float value = reader.AnalogData["Fx1", 0 /* from 0 to reader.AnalogChannels*/];
                    // OR 
                    //value = reader.AnalogData[0, 0];

                    // OR
                    //float [,] analogData = reader.AnalogData.Data;


                    //Console.WriteLine("Frame " + i + ": Spine.X " + spine.X + ",  Spine.Y " + spine.Y + ": Spine.Z " + spine.Z);
                    if (m_Reader.GetProcent() > lastProcent)
                    {
                        lastProcent = m_Reader.GetProcent();
                        nextMesssage(this, "" + lastProcent.ToString() + "%");
                        
                    }
                    i++;
                }

                // Don't forget to close the reader
                // - it updates the frames count information in the c3d file header and parameters section
                return list;
            }
            catch
            {
                return null;
            }
        }

        public ArrayListSerializable getLabels()
        {
            if (m_Reader == null)
                throw new NullReferenceException("m_Reader is Null");

            ArrayListSerializable list = new ArrayListSerializable();

            foreach (String l in m_Reader.Labels)
                list.Add(l.Replace("-", String.Empty));


            return list;
        }

        public void close()
        {
            m_Reader.Close();
            m_Reader = null;
        }
        
        public event MyEventManager.Message nextMesssage;

        public void OnMessage(object sender, String text)
        {
            if (this.nextMesssage != null)
                nextMesssage(sender, text);
        }

    }
}
