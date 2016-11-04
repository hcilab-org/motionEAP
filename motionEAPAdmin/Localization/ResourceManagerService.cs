// <copyright file=ResourceManagerService.cs
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
using System.Resources;
using System.Globalization;
using System.Threading;

namespace motionEAPAdmin.Localization
{
    public static class ResourceManagerService
    {

        private static Dictionary<string, ResourceManager> _managers;

        public static event LocaleChangedEventHander LocaleChanged;
        private static void RaiseLocaleChanged(Locale newLocale)
        {
            var evt = LocaleChanged;

            if (evt != null)
                evt.Invoke(null, new LocaleChangedEventArgs(newLocale));
        }

        /// <summary>
        /// Current application locale
        /// </summary>
        public static Locale CurrentLocale { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ResourceManager class.
        /// </summary>
        static ResourceManagerService()
        {
            _managers = new Dictionary<string, ResourceManager>();

            // Set to default culture
            ChangeLocale(CultureInfo.CurrentCulture.IetfLanguageTag);
        }

        /// <summary>
        /// Retrieves a string resource with the given key from the given
        /// resource manager. Will load the string relevant to the current culture.
        /// </summary>
        /// <param name="managerName">Name of the ResourceManager</param>
        /// <param name="resourceKey">Resource to lookup</param>
        /// <returns></returns>
        public static string GetResourceString(string managerName, string resourceKey)
        {
            ResourceManager manager = null;
            string resource = String.Empty;

            if (_managers.TryGetValue(managerName, out manager))
                resource = manager.GetString(resourceKey);

            return resource;
        }

        /// <summary>
        /// Changes the current locale
        /// </summary>
        /// <param name="newLocaleName">locale name</param>
        public static void ChangeLocale(string newLocaleName)
        {
            CultureInfo newCultureInfo = new CultureInfo(newLocaleName);
            Thread.CurrentThread.CurrentCulture = newCultureInfo;
            Thread.CurrentThread.CurrentUICulture = newCultureInfo;

            Locale newLocale = new Locale() { Name = newLocaleName, RTL = newCultureInfo.TextInfo.IsRightToLeft };
            CurrentLocale = newLocale;

            RaiseLocaleChanged(newLocale);
        }

        /// <summary>
        /// Fires the LocaleChange event to reload bindings
        /// </summary>
        public static void Refresh()
        {
            ChangeLocale(CultureInfo.CurrentCulture.IetfLanguageTag);
        }

        /// <summary>
        /// Register a ResourceManager, does not fire a refresh
        /// </summary>
        /// <param name="managerName">Name to store the manager under, used with GetResourceString/UnregisterManager</param>
        /// <param name="manager">ResourceManager to store</param>
        public static void RegisterManager(string managerName, ResourceManager manager)
        {
            RegisterManager(managerName, manager, false);
        }

        /// <summary>
        /// Register a ResourceManager
        /// </summary>
        /// <param name="managerName">Name to store the manager under, used with GetResourceString/UnregisterManager</param>
        /// <param name="manager">ResourceManager to store</param>
        /// <param name="refresh">Whether to fire the LocaleChanged event to refresh bindings</param>
        public static void RegisterManager(string managerName, ResourceManager manager, bool refresh)
        {
            ResourceManager _manager = null;

            _managers.TryGetValue(managerName, out _manager);

            if (_manager == null)
                _managers.Add(managerName, manager);

            if (refresh)
                Refresh();
        }

        /// <summary>
        /// Remove a ResourceManager
        /// </summary>
        /// <param name="name">Name of the manager to remove</param>
        public static void UnregisterManager(string name)
        {
            ResourceManager _manager = null;

            _managers.TryGetValue(name, out _manager);

            if (_manager != null)
                _managers.Remove(name);
        }

    }
}
