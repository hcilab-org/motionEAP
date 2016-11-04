// <copyright file=NotificationManager.cs
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

using System.Collections.Generic;
using System.Windows.Media;

namespace motionEAPAdmin.Backend
{
    class NotificationManager
    {
        private static List<string> m_Notifications;
        private static int m_Counter;
        private const int m_ShowTime = 60;
        private static Scene.SceneTextViewer m_NotificationItem;
        private static bool m_Added = false;
        private static int m_Id = -2;

        public static void Init()
        {
            m_Notifications = new List<string>();
            m_NotificationItem = new Scene.SceneTextViewer(0.8f, 0.05f, 0.2f, 0.2f, "", new FontFamily("Arial"), 10.0, System.Windows.Media.Color.FromRgb(255, 255, 255));
        }

        /// <summary>
        /// Schedule a notification which will be displayed for 'NotificationManager.showTime' ticks, when all previous notifications have been shown.
        /// </summary>
        /// <param name="notification">Text of the notification</param>
        /// 
        /// 
        public static void Show(string notification)
        {
            m_Notifications.Add(notification);
        }

        public static void Update()
        {
            --m_Counter;
            if (m_Counter <= 0)
            {
                if (m_Notifications.Count == 0)
                {
                    if (m_Added)
                    {
                        SceneManager.Instance.removeTemporarylyDisplayedSceneItem(m_Id);
                        m_Added = false;
                    }
                    m_Counter = 0;
                }
                else
                {
                    var notification = m_Notifications[0];
                    m_Notifications.Remove(notification);
                    m_NotificationItem.Text = notification;
                    if (!m_Added)
                    {
                        SceneManager.Instance.temporarylyDisplaySceneItem(m_NotificationItem);
                        m_Added = true;
                    }
                    m_Counter = m_ShowTime;
                }
            }
        }
    }
}
