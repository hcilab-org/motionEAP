// <copyright file=UtilitiesIO.cs
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

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace HciLab.Utilities
{
    public static class UtilitiesIO
    {
        public static String ReadFile(string pStrFileName, out Boolean pIsComplete, out String pError)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                using (StreamReader sr = new StreamReader(pStrFileName))
                {
                    String line;
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        sb.AppendLine(line);
                    }
                }
                pIsComplete = true;
                pError = "...Daten geladen.";
                return sb.ToString();
            }
            catch (FileNotFoundException ex)
            {
                pError = "FileNotFoundException";
                pIsComplete = false;
                return "";
            }
            catch (Exception ex)
            {
                pError = ex.ToString();
                pIsComplete = false;
                return "";
            }
        }

        /// <summary>
        /// Dumps an object as xml flat file.
        /// </summary>
        /// <param name="strFileName">Filename of xml file.</param>
        public static string WriteXmlToString(Object pObject)
        {

            XmlSerializer serializer = new XmlSerializer(pObject.GetType());

            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, pObject);

                return writer.ToString();
            }
        }


        /// <summary>
        /// Dumps an object as xml flat file.
        /// </summary>
        /// <param name="strFileName">Filename of xml file.</param>
        public static string WriteXmlToIndentString(Object pObject)
        {

            XmlSerializer serializer = new XmlSerializer(pObject.GetType());
            string pXML = "";
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, pObject);

                pXML = writer.ToString();
            }

            return IndentXMLString(pXML);
        }
        
        private static string IndentXMLString(string xml)
        {
            string outXml = string.Empty;
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            // Create a XMLTextWriter that will send its output to a memory stream (file)
            System.Xml.XmlTextWriter xtw = new System.Xml.XmlTextWriter(ms, System.Text.Encoding.Unicode);
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

            try
            {
                // Load the unformatted XML text string into an instance
                // of the XML Document Object Model (DOM)
                doc.LoadXml(xml);

                // Set the formatting property of the XML Text Writer to indented
                // the text writer is where the indenting will be performed
                xtw.Formatting = System.Xml.Formatting.Indented;
                xtw.Indentation = 4;

                // write dom xml to the xmltextwriter
                doc.WriteContentTo(xtw);
                // Flush the contents of the text writer
                // to the memory stream, which is simply a memory file
                xtw.Flush();

                // set to start of the memory stream (file)
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                // create a reader to read the contents of
                // the memory stream (file)
                System.IO.StreamReader sr = new System.IO.StreamReader(ms);
                // return the formatted string to caller
                return sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        /// <summary>
        /// Writes a object to a file.
        /// </summary>
        /// <param name="pFileName">Name of file.</param>
        /// <param name="pHaushaltsbefragungsDaten">Object to save</param>
        /// <param name="pBinary">Should be written to file as soap-xml or as binary.</param>
        /// <returns>True if write was successfull.</returns>
        public static bool WriteToFile(String pFileName, Object pObject)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(pFileName, FileMode.Create);
                SoapFormatter sf = new SoapFormatter();
                sf.AssemblyFormat = FormatterAssemblyStyle.Simple;
                sf.Serialize(fs, pObject);
                fs.Close();
            }
            catch
            {
                if (fs != null) fs.Close();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Writes a object to a memory stream.
        /// </summary>
        /// <param name="pHaushaltsbefragungsDaten">Object to save</param>
        /// <param name="pBinary">Should be written to file as soap-xml or as binary.</param>
        /// <returns>Serialized object as string.</returns>
        public static string WriteToString(Object pObject)
        {
            string pReturnString = "";
            MemoryStream pMS = new MemoryStream();
            SoapFormatter sf = new SoapFormatter();
            sf.AssemblyFormat = FormatterAssemblyStyle.Simple;
            sf.Serialize(pMS, pObject);
            pReturnString = UTF8ByteArrayToString(pMS.ToArray());
            return pReturnString;
        }

        /// <summary>
        /// Reads a object from a string. 
        /// </summary>
        /// <param name="pSerializedObject">Serialized object.</param>
        /// <param name="pBinary">File was written as soap-xml or as binary?</param>
        /// <returns>Object. null if failed.</returns>
        public static Object ReadFromString(String pSerializedObject, out Boolean pIsComplete, out String pError)
        {
            Object pObj = null;
            try
            {
                MemoryStream pMS = new MemoryStream(StringToUTF8ByteArray(pSerializedObject));
                // seek to position
                SoapFormatter sf = new SoapFormatter();
                sf.AssemblyFormat = FormatterAssemblyStyle.Simple;
                pObj = sf.Deserialize(pMS);
                pIsComplete = true;
                pError = "";
                return pObj;
            }
            catch (Exception ex)
            {
                pError = ex.ToString();
                pIsComplete = false;
                return null;
            }
            
        }



        /// <summary>
        /// To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
        /// </summary>
        /// <param name="characters">Unicode Byte Array to be converted to String</param>
        /// <returns>String converted from Unicode Byte Array</returns>
        private static String UTF8ByteArrayToString(Byte[] characters)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        /// <summary>
        /// To convert a string into a Byte Array of Unicode values (UTF-8 encoded).
        /// </summary>
        /// <param name="characters">String to be converted into Unicode Byte Array </param>
        /// <returns>Unicode Byte Array</returns>
        private static Byte[] StringToUTF8ByteArray(String pInput)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return encoding.GetBytes(pInput);
        }


        public static Boolean WriteFile(string pFileName, string fileInput)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(pFileName, FileMode.Create);
                fs.Close();
            }
            catch
            {
                if (fs != null) fs.Close();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Writes a generic object as XML with a specific filename
        /// </summary>
        /// <typeparam name="T">Generic object</typeparam>
        /// <param name="pObj">Generic object, which should be dumped as XML</param>
        /// <param name="pFilename">Filename of the XML file</param>
        /// <returns>Wether saving the object was successful or not</returns>
        public static bool SaveObjectToXML<T>(T pObj, string pFilename)
        {
            try
            {
                var x = new XmlSerializer(pObj.GetType());
                using (var Writer = new StreamWriter(pFilename, false))
                {
                    x.Serialize(Writer, pObj);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets and reads a specific XML file
        /// </summary>
        /// <typeparam name="T">Generic object</typeparam>
        /// <param name="pObj">Generic object, which holds relevant informations</param>
        /// <param name="pFileName">Destination XML file</param>
        /// <returns>Wether getting the object was successful or not</returns>
        public static bool GetObjectFromXML<T>(ref T pObj, string pFileName)
        {
            try
            {
                using (FileStream stream = new FileStream(pFileName, FileMode.Open))
                {
                    XmlTextReader reader = new XmlTextReader(stream);
                    var x = new XmlSerializer(pObj.GetType());
                    pObj = (T)x.Deserialize(reader);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets and reads a specific Json file
        /// </summary>
        /// <typeparam name="T">Generic object</typeparam>
        /// <param name="pObj">Generic object, which holds relevant informations</param>
        /// <param name="pFileName">Destination Json file</param>
        /// <returns>Wether getting the object was successful or not</returns>
        public static bool GetObjectFromJson<T>(ref T pObj, string pFileName)
        {
            try
            {
                string output = @System.IO.File.ReadAllText(@pFileName);
                pObj = JsonConvert.DeserializeObject<T>(output, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Writes a generic object as XML with a specific filename
        /// </summary>
        /// <typeparam name="T">Generic object</typeparam>
        /// <param name="pObj">Generic object, which should be dumped as XML</param>
        /// <param name="pFilename">Filename of the XML file</param>
        /// <returns>Wether saving the object was successful or not</returns>
        public static bool SaveObjectToJson<T>(T pObj, string pFileName)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.TypeNameHandling = TypeNameHandling.All;
            try
            {
                using (StreamWriter sw = new StreamWriter(@pFileName))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, pObj);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        public static string SaveObjectToJsonString<T>(T pObj)
        {

            JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                };
            return JsonConvert.SerializeObject(pObj, settings);
        }

        public static bool GetObjectFromJsonString<T>(ref T pObj, string pOutput)
        {
            try
            {
                pObj = JsonConvert.DeserializeObject<T>(pOutput, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
