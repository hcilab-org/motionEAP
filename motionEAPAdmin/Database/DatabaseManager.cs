// <copyright file=DatabaseManager.cs
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

using HciLab.motionEAP.InterfacesAndDataModel.Data;
using motionEAPAdmin.Backend;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using Remotion.Linq.Collections;
using System;
using System.Collections.Generic;
using System.IO;

namespace motionEAPAdmin.Database
{
    /// <summary>
    /// Databasemanager
    /// </summary>
    /// 
    /// 
    public class DatabaseManager
    {
        
        private static DatabaseManager m_Instance; // singleton instance

        
        private ISession sess = null;
        
        private ObservableCollection<TrackableObject> objectList = new ObservableCollection<TrackableObject>();

        private DatabaseManager()
        {
            initDatabase(); // init
            listTrackableObject(); // create the list for databinding
        }


        public ObservableCollection<TrackableObject> Objects
        {
            get { return objectList; }
            set { objectList = value; }
        }

        public static DatabaseManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new DatabaseManager();
                }
                return m_Instance;
            }
        }

        /// <summary>
        /// initialize the database and open the session
        /// </summary>
        /// 
        /// 
        private void initDatabase()
        {
            var cfg = new Configuration();
            //cfg.Configure("hibernate.config.xml");
            cfg.DataBaseIntegration(x =>
            {
                x.ConnectionString = "Data Source=objects.db;Version=3;";
                x.Driver<SQLite20Driver>();
                x.Dialect<SQLiteDialect>();
            });
            cfg.AddAssembly(typeof(TrackableObject).Assembly);

            //new SchemaExport(cfg).Execute(true, true, false, _session.Connection, Console.Out); 
            //new SchemaExport(cfg).Execute(true, true, false, _session.Connection, null);
            
            // DEBUG: run this code to recreate the database file (ALL CONTENT WILL BE DELETED)

            // TODO: Connecting to database doesn't work and causes an exception
            if (!File.Exists("objects.db"))
                new NHibernate.Tool.hbm2ddl.SchemaExport(cfg).Execute(false, true, false);


            //// Get ourselves an NHibernate Session
            sess = cfg.BuildSessionFactory().OpenSession();
        }

        /// <summary>
        /// This method inserts the given object into the database
        /// </summary>
        /// <param name="obj">object to insert</param>
        /// 
        /// 
        public void insertTrackableObject(TrackableObject obj)
        {
            sess.Save(obj);
            sess.Flush();
        }

        /// <summary>
        /// selects all trackable objects from the database and updates them in the 
        /// databasemanager's list
        /// </summary>
        /// <returns>a list containing all the objects from the database</returns>
        /// 
        /// 
        public List<TrackableObject> listTrackableObject()
        {
            try
            {
                IQuery q = sess.CreateQuery("FROM TrackableObject");
                List<TrackableObject> list = (List<TrackableObject>)q.List<TrackableObject>();


                // put the new values into a list that is managed by the database manager
                objectList.Clear();
                foreach (TrackableObject o in list)
                {
                    // create transient data and add it to the object
                    o.ImageFullPath = System.IO.Path.GetFullPath(o.Image);
                    o.EmguImage = new Emgu.CV.Image<Emgu.CV.Structure.Gray, byte>(o.ImageFullPath);
                    objectList.Add(o);
                }

                return list;
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.Message, Logger.LoggerState.ERROR);
                return new List<TrackableObject>();
            }
        }

        /// <summary>
        /// updates the given TrackableObject in the Database
        /// </summary>
        /// <param name="obj">object to update</param>
        /// 
        /// 
        public bool updateTrackableObject(TrackableObject obj)
        {
            bool ret = true;
            try
            {
                using (ITransaction transaction = sess.BeginTransaction())
                {
                    sess.Update(obj);
                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                ret = false;
                Logger.Instance.Log(e.Message, Logger.LoggerState.ERROR);
            }

            return ret;
        }

        /// <summary>
        /// deletes the given object from the database
        /// </summary>
        /// <param name="obj">object to delete</param>
        /// 
        /// 
        public void deleteTrackableObject(TrackableObject obj)
        {
            try
            {
                using (ITransaction transaction = sess.BeginTransaction())
                {
                    sess.Delete(obj);
                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Log(e.Message, Logger.LoggerState.ERROR);
            }
        }

    }
}
