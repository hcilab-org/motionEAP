// <copyright file=Localization.cs
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
//  Markus Funk, Thomas Kosch, Michael Matheis, Sven Mayer
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
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Reflection;
using System.IO;
using System.Windows;

namespace motionEAPAdmin.Localization
{
    public static class Localization
    {

        //These fields are used for caching inforamtion about resources, data providers and available cultures.
        private static Dictionary<string, object> m_LocalizationResources = new Dictionary<string, object>();
        // ObjectDataProvider manages XAML binding resources
        private static Dictionary<string, ObjectDataProvider> m_ResourceDataProviders = new Dictionary<string, ObjectDataProvider>();
        private static List<CultureInfo> m_AvailableCultures = new List<CultureInfo>();

        /// <summary>
        /// Get or set desired UI culture
        /// </summary>
        /// <author>Thomas Kosch</author>
        public static string CurrentCulture
        {
            get;
            set;
        }

        /// <summary>
        /// Get resource and manifest it into the assembly
        /// </summary>
        /// <author>Thomas Kosch</author>
        public static Dictionary<string, object> LocalizationResources
        {
            get
            {
                if (Localization.m_LocalizationResources.Keys.Count == 0)
                {
                    string[] resourcesInThisAssembly = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();
                    foreach (string resource in resourcesInThisAssembly)
                    {
                        string res = resource.Substring(0, resource.LastIndexOf("."));
                        string resKey = res.Substring(res.LastIndexOf(".") + 1);
                        if (!Localization.m_LocalizationResources.ContainsKey(res))
                        {
                            Type t = Type.GetType(res);
                            object resourceInstance = t.GetConstructor(
                                    BindingFlags.NonPublic | BindingFlags.Instance,
                                    null,
                                    Type.EmptyTypes, null)
                                        .Invoke(new object[] { });
                            Localization.m_LocalizationResources.Add(resKey, resourceInstance);
                        }
                    }
                }
                return m_LocalizationResources;
            }
        }

        /// <summary>
        /// Get the actual object which is binded
        /// </summary>
        /// <author>Thomas Kosch</author>
        public static Dictionary<string, ObjectDataProvider> ResourceDataProviders
        {
            get
            {
                Localization.PopulateDataProviders();
                return m_ResourceDataProviders;
            }
        }

        /// <summary>
        /// Get the list with all actual available cultures
        /// </summary>
        /// <author>Thomas Kosch</author>
        public static List<CultureInfo> AvailableCultures
        {
            get
            {
                if (m_AvailableCultures.Count == 0)
                {
                    if (m_LocalizationResources.Count > 0)
                    {
                        m_AvailableCultures.Add(new CultureInfo("en-US"));
                    }
                    string resourceFileName = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location) + ".resources.dll";
                    DirectoryInfo rootDir = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                    m_AvailableCultures.AddRange((from culture in CultureInfo.GetCultures(CultureTypes.AllCultures)
                                                join folder in rootDir.GetDirectories() on culture.IetfLanguageTag equals folder.Name
                                                where folder.GetFiles(resourceFileName).Any()
                                                select culture));
                }
                return m_AvailableCultures;
            }
        }


        /// <summary>
        /// Get a single resource
        /// </summary>
        /// <param name="resname">Name of the resource</param>
        /// <returns></returns>
        /// <author>Thomas Kosch</author>
        public static object GetResource(string resname)
        {
            if (LocalizationResources.ContainsKey(resname))
            {
                return LocalizationResources[resname];
            }
            return null;
        }

        /// <summary>
        /// Changes UI Culture
        /// </summary>
        /// <param name="culture">New culture</param>
        public static void ChangeCulture(CultureInfo culture)
        {
            CurrentCulture = culture.Name;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
            for (int i = 0; i < ResourceDataProviders.Keys.Count; i++)
            {
                ObjectDataProvider prov = InstantiateDataProvider(ResourceDataProviders.ElementAt(i).Key);
                if (prov != null)
                    prov.Refresh();
            }
        }

        /// <summary>
        /// Init for localizing everything. This method is called from App.xaml.cs
        /// </summary>
        /// <author>Thomas Kosch</author>
        public static void PopulateDataProviders()
        {
            if (Localization.m_ResourceDataProviders.Keys.Count == 0)
            {
                string[] resourcesInThisAssembly = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();
                foreach (string resource in resourcesInThisAssembly)
                {
                    string res = resource.Substring(0, resource.LastIndexOf("."));
                    string resKey = res.Substring(res.LastIndexOf(".") + 1);
                    if (!Localization.m_ResourceDataProviders.ContainsKey(res))
                    {
                        ObjectDataProvider prov = null;
                        try
                        {
                            if (Application.Current.Resources.Contains(resKey))
                                prov = (ObjectDataProvider)Application.Current.FindResource(resKey);
                            else
                            {
                                prov = new ObjectDataProvider() { ObjectInstance = Localization.GetResource(resKey) };
                                Application.Current.Resources.Add(resKey, prov);
                            }
                        }
                        catch
                        {
                            prov = null;
                        }
                        Localization.m_ResourceDataProviders.Add(resKey, prov);
                    }
                }
            }
        }

        /// <summary>
        /// Get one single value from a given key
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <author>Thomas Kosch</author>
        public static string GetString(string resourceName, string key)
        {
            string str = null;
            object resource = GetResource(resourceName);
            if (resource != null)
            {
                PropertyInfo resStr = resource.GetType().GetProperty(key);
                if (resStr != null)
                {
                    str = System.Convert.ToString(resStr.GetValue(null, null));
                }
            }
            return str;
        }

        /// <summary>
        /// Returns the actual binded resource from the ObjectDataProvider
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        /// <author>Thomas Kosch</author>
        private static ObjectDataProvider InstantiateDataProvider(string resource)
        {
            try
            {
                if (ResourceDataProviders.ContainsKey(resource))
                {
                    if (ResourceDataProviders[resource] == null)
                        ResourceDataProviders[resource] = (ObjectDataProvider)Application.Current.FindResource(resource);
                }
            }
            catch
            {
                return null;
            }
            return ResourceDataProviders[resource];
        }

    }
}
