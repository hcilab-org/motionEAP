// <copyright file=SceneManager.cs
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

using HciLab.motionEAP.InterfacesAndDataModel;
using HciLab.Utilities;
using motionEAPAdmin.Backend;
using motionEAPAdmin.Backend.AssembleyZones;
using motionEAPAdmin.Backend.Boxes;
using motionEAPAdmin.ContentProviders;
using motionEAPAdmin.Scene;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media;

namespace motionEAPAdmin
{
    public class SceneManager : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// the Scene that belongs to the current working step
        /// </summary>
        private Scene.Scene m_CurrentScene;

        /// <summary>
        /// the Scene that is responsible for the temprorary items
        /// </summary>
        private Scene.Scene m_TemporaryScene;

        /// <summary>
        /// the Scene that is responsible for the temprorary items and box feedback
        /// </summary>
        private Scene.Scene m_TemporaryBoxScene;

        private Scene.Scene m_TemporaryAssemblyZoneScene;

        private Scene.Scene m_TemporaryObjectsScene;

        private Scene.Scene m_TemporaryStatsScene;

        private Scene.Scene m_TemporaryDebugScene;

        private Scene.Scene m_TemporaryObjectsTextScene;

        private Scene.Scene m_TemporaryFeedbackScene;

        private Scene.Scene m_TemporaryWaitingScene;

        private bool m_DisableObjectScenes = false;

        private bool m_DisplayTempFeedback = false;

        private SceneItem m_DemoRect;

        private static SceneManager m_Instance;



        public static SceneManager Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new SceneManager();
                return m_Instance;
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        private SceneManager()
        {
            m_CurrentScene = new Scene.Scene();
            m_TemporaryScene = new Scene.Scene();
            m_TemporaryBoxScene = new Scene.Scene();
            m_TemporaryAssemblyZoneScene = new Scene.Scene();
            m_TemporaryObjectsScene = new Scene.Scene();
            m_TemporaryStatsScene = new Scene.Scene();
            m_TemporaryDebugScene = new Scene.Scene();
            m_TemporaryObjectsTextScene = new Scene.Scene();
            m_TemporaryFeedbackScene = new Scene.Scene();
            m_TemporaryWaitingScene = new Scene.Scene();
            NotificationManager.Init();
        }

        /// <summary>
        /// this method initializes the content that is to be drawn by the scene.
        /// This content is static FOR NOW! This has to be dynamically at some point
        /// </summary>
        /// 
        /// 
        public void initContent()
        {
            if (SettingsManager.Instance.Settings.SettingsTable.ShowDemoAnimation)
            {
                //m_DemoRect = new DemoRectScene(m_CurrentScene);
                m_DemoRect = new SceneRect(150, 150, 150, 150, Color.FromRgb(255, 0, 0));
                m_CurrentScene.Add(m_DemoRect);
            }
        }

        private void NotifyPropertyChanged(string Obj)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(Obj));
            }
        }
        
        /// <summary>
        /// This method displays a given SceneItem temporarily on top of the
        /// current Scene. This can be used for information that does not
        /// belog to the fixed Scene belonging to each step of a workflow.
        /// 
        /// USE THIS TO DISPLAY SCENEITEMS TEMPORARILY e.g. for selection
        /// </summary>
        /// 
        /// 
        public void temporarylyDisplaySceneItem(SceneItem pItem)
        {
            m_TemporaryScene.Add(pItem);
        }

        /// <summary>
        /// This method removes the SceneItem with the id pId from the
        /// temporaryly displayed Scene.
        /// 
        /// USE THIS TO DELETE TEMP-SCENEITEMS
        /// </summary>
        /// <param name="pId">id of the to be deleted sceneItem</param>
        /// 
        /// 
        public void removeTemporarylyDisplayedSceneItem(int pId)
        {
            SceneItem toRemoveItem = m_TemporaryScene.Items.GetBySceneObjId(pId);
            m_TemporaryScene.Remove(toRemoveItem);
        }

        public void Update()
        {
            NotificationManager.Update();
        }

        public void SaveSceneToFile()
        {
            // check if scnes dir exists
            if (!Directory.Exists(ProjectConstants.SCENES_DIR))
            {
                // if not create it
                Directory.CreateDirectory(ProjectConstants.SCENES_DIR);
            }

            System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
            dlg.InitialDirectory = Directory.GetCurrentDirectory() + "\\" + ProjectConstants.SCENES_DIR;
            dlg.Filter = "scene files (*.scene)|*.scene";

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            string filename = dlg.FileName;
            if (!filename.EndsWith(ProjectConstants.SCENE_FILE_ENDING))
            {
                filename = filename + ProjectConstants.SCENE_FILE_ENDING;
            }

            UtilitiesIO.SaveObjectToJson(CurrentScene, filename);
        }

        public void LoadScene()
        {
            CurrentScene = LoadSceneFromFile();
        }

        public Scene.Scene LoadSceneFromFile()
        {
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.InitialDirectory = Directory.GetCurrentDirectory() + "\\" + ProjectConstants.SCENES_DIR;
            dlg.Filter = "scene files (*.scene)|*.scene";
            dlg.FilterIndex = 2;
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() != DialogResult.OK)
                return null;

            Scene.Scene ret = null;

            bool isOkay = UtilitiesIO.GetObjectFromJson(ref ret, dlg.FileName);

            if (!isOkay)
                return null;

            return ret;
        }

        /// <summary>
        /// All scenes that have to be displayed at the moment
        /// </summary>
        /// <returns></returns>
        public List<Scene.Scene> getAllScenes()
        {
            List<Scene.Scene> ret = new List<Scene.Scene>();

            if (m_CurrentScene != null)
            {
                ret.Add(m_CurrentScene);
            }

            if (SettingsManager.Instance.Settings.AssemblyZoneVisualFeedbackProject && StateManager.Instance.State != AllEnums.State.WORKFLOW_PLAYING && StateManager.Instance.State != AllEnums.State.EDIT)
                ret.Add(AssemblyZoneManager.Instance.drawProjectorUI());

            if (SettingsManager.Instance.Settings.BoxesVisualFeedbackProject && StateManager.Instance.State != AllEnums.State.WORKFLOW_PLAYING && StateManager.Instance.State != AllEnums.State.EDIT)
                ret.Add(BoxManager.Instance.drawProjectorUI());

            if (StateManager.Instance.State == AllEnums.State.WORKFLOW_PLAYING && StateManager.Instance.State != AllEnums.State.EDIT)
            {
                ret.Add(BoxManager.Instance.drawErrorFeedback());
            }

            if (!m_DisableObjectScenes)
            {
                ret.Add(m_TemporaryObjectsScene);
                ret.Add(m_TemporaryObjectsTextScene);
            }

            if (m_DisplayTempFeedback)
            {
                ret.Add(m_TemporaryFeedbackScene);
            }

            ret.Add(m_TemporaryWaitingScene);

            ret.Add(m_TemporaryStatsScene);

            ret.Add(m_TemporaryDebugScene);

            return ret;
        }

        #region Getter / Setter

        public bool DisableObjectScenes
        {
            get
            {
                return m_DisableObjectScenes;
            }
            set
            {
                m_DisableObjectScenes = value;
                NotifyPropertyChanged("DisableObjectScenes");
            }
        }

        public bool DisplayTempFeedback
        {
            get
            {
                return m_DisplayTempFeedback;
            }
            set
            {
                m_DisplayTempFeedback = value;
                NotifyPropertyChanged("DisplayTempFeedback");
            }
        }


        public Scene.Scene CurrentScene
        {
            get
            {
                return m_CurrentScene;
            }
            set
            {
                // do stuff with old scene
                m_CurrentScene.setShown(false);

                // do stuff with new scene
                m_CurrentScene = value;
                m_CurrentScene.setShown(true);
                NotifyPropertyChanged("CurrentScene");
            }
        }

        public Scene.Scene TemporaryScene
        {
            get
            {
                return m_TemporaryScene;
            }
            set
            {
                m_TemporaryScene = value;
                NotifyPropertyChanged("TemporaryScene");
            }
        }

        public Scene.Scene TemporaryBoxScene
        {
            get
            {
                return m_TemporaryBoxScene;
            }
            set
            {
                m_TemporaryBoxScene = value;
                NotifyPropertyChanged("TemporaryBoxScene");
            }
        }

        public Scene.Scene TemporaryObjectsScene
        {
            get
            {
                return m_TemporaryObjectsScene;
            }
            set
            {
                m_TemporaryObjectsScene = value;
                NotifyPropertyChanged("TemporaryObjectsScene");
            }
        }


        public Scene.Scene TemporaryObjectsTextScene
        {
            get
            {
                return m_TemporaryObjectsTextScene;
            }
            set
            {
                m_TemporaryObjectsTextScene = value;
                NotifyPropertyChanged("TemporaryObjectsTextScene");
            }
        }

        public Scene.Scene TemporaryAssemblyZoneScene
        {
            get
            {
                return m_TemporaryAssemblyZoneScene;
            }
            set
            {
                m_TemporaryAssemblyZoneScene = value;
                NotifyPropertyChanged("TemporaryAssemblyZoneScene");
            }
        }

        public Scene.Scene TemporaryFeedbackScene
        {
            get
            {
                return m_TemporaryFeedbackScene;
            }
            set
            {
                m_TemporaryFeedbackScene = value;
                NotifyPropertyChanged("TemporaryFeedbackScene");
            }
        }

        public Scene.Scene TemporaryStatsScene
        {
            get
            {
                return m_TemporaryStatsScene;
            }
            set
            {
                m_TemporaryStatsScene = value;
                NotifyPropertyChanged("TemporaryStatsScene");
            }
        }

        public Scene.Scene TemporaryDebugScene
        {
            get
            {
                return m_TemporaryDebugScene;
            }
            set
            {
                m_TemporaryDebugScene = value;
                NotifyPropertyChanged("TemporaryDebugScene");
            }
        }

        public Scene.Scene TemporaryWaitingScene
        {
            get { return m_TemporaryWaitingScene; }
            set
            {
                m_TemporaryWaitingScene = value;
                NotifyPropertyChanged("TemporaryWaitingScene");
            }
        }

        #endregion

    }
}
