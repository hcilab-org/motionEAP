// <copyright file=EditWorkflowManager.cs
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

using System.IO;
using System.Linq;
using motionEAPAdmin.Model.Process;
using HciLab.motionEAP.InterfacesAndDataModel;
using HciLab.Utilities;
using HciLab.Kinect;
using motionEAPAdmin.GUI.Dialog;
using System.Collections.Generic;

namespace motionEAPAdmin.Backend
{
    /// <summary>
    /// This class handles currently edited workflow.
    /// Have this class synchronized when editing a workflow.
    /// 
    /// </summary>
    /// 
    /// 
    public class EditWorkflowManager
    {
        private static EditWorkflowManager m_Instance;  // the instance
        private Workflow m_CurrentWorkflow = null;  // the currently loaded workflow
        private int m_CrrentWorkingStepNumber;
        private int m_HighestWorkingStepNumber;

        public SceneEditorDialog EditorGUIHandle = null;

        private EditWorkflowManager() { }

        public static EditWorkflowManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new EditWorkflowManager();
                }
                return m_Instance;
            }
        }

        public Scene.Scene getBoxAutoScene(Boxes.Box b)
        {
            Scene.SceneItem drawable = b.getDrawable(true);
            Scene.Scene autoScene;
            if (drawable is Scene.Scene)
            {
                // we already have a scene
                autoScene = (Scene.Scene)drawable;
            }
            else
            {
                autoScene = new Scene.Scene();
                autoScene.Add(drawable);
            }
            return autoScene;
        }

        public Scene.Scene getAssemblyZoneAutoScene(AssembleyZones.AssemblyZone z)
        {
            Scene.SceneItem drawable = z.getDrawable(true);
            Scene.Scene autoScene;
            if (drawable is Scene.Scene)
            {
                // we already have a scene
                autoScene = (Scene.Scene)drawable;
            }
            else
            {
                autoScene = new Scene.Scene();
                autoScene.Add(drawable);
            }
            return autoScene;
        }
        

        public void AddWorkingStep(string pName, string endCondition)
        {
            // add the working step
            WorkingStep newStep = new WorkingStep(pName,"", endCondition,0);
            newStep.StepNumber = calculateHighestStepnumber() + 1;
            m_CurrentWorkflow.AddWorkingStep(newStep);
        }

        public void createStep(AllEnums.PBD_Mode mode, AdaptiveScene adaptiveScene, string name, string endCondition)
        {
            createStep(mode, new List<AdaptiveScene> { adaptiveScene }, name, endCondition);
        }

        public void createStep(AllEnums.PBD_Mode mode, List<AdaptiveScene> adaptiveScenes, string name, string endCondition)
        {
            string stepname = "";
            string withdrawel = "";

            //stepname = "Schritt " + EditWorkflowManager.Instance.CurrentWorkingStepNumber + ": ";
            if (mode == AllEnums.PBD_Mode.BOX_WITHDRAWEL)
            {
                withdrawel = name;
                stepname = "Entnahme " + withdrawel;
            }
            else if (mode == AllEnums.PBD_Mode.ASSEMBLY_DONE)
            {
                stepname = "Montage";
            }
            else if (mode == AllEnums.PBD_Mode.NETWORK_TABLE_DONE)
            {
                stepname = "Warte auf Tisch";
            }
            else if (mode == AllEnums.PBD_Mode.OBJECT_RECOGNIZED)
            {
                stepname = "Objekt " + name;
            }
            else if (mode == AllEnums.PBD_Mode.END_CONDITION)
            {
                stepname = "ASSEMBLY COMPLETE";
            }

            // add this to the workflow that is currently created
            EditWorkflowManager.Instance.AddWorkingStepByDemonstration(adaptiveScenes, stepname, withdrawel, EditWorkflowManager.Instance.CurrentWorkingStepNumber, endCondition, mode);
            EditWorkflowManager.Instance.CurrentWorkingStepNumber += 1;

            // refresh the whole thing
            //refreshLoadedWorkflow();
            //}
        }

        public void AddWorkingStepByDemonstration(AdaptiveScene adaptiveScene, string pName, string withdrawel, int sNumber, string endCondition, AllEnums.PBD_Mode mode)
        {
            AddWorkingStepByDemonstration(new List<AdaptiveScene> {adaptiveScene}, pName, withdrawel, sNumber, endCondition, mode);
        }

        public void AddWorkingStepByDemonstration(List<AdaptiveScene> adaptiveScenes, string pName, string withdrawel, int sNumber, string endCondition, AllEnums.PBD_Mode mode)
        {
            // add the working step
            WorkingStep newStep = new WorkingStep(pName, withdrawel, endCondition, sNumber);
            newStep.StepNumber = m_HighestWorkingStepNumber+1;
            newStep.Withdrawel = withdrawel;
            newStep.Mode = mode;

            if (mode == AllEnums.PBD_Mode.END_CONDITION)
            {
                newStep.createNewName(mode);
            }
            newStep.AdaptiveScenes.Clear(); // delete adaptivitylevels that are created from the usual way automatically
            adaptiveScenes.ForEach(aScene => newStep.AdaptiveScenes.Add(aScene));
            m_HighestWorkingStepNumber += 1;
            
            m_CurrentWorkflow.AddWorkingStep(newStep);
            if (mode == AllEnums.PBD_Mode.BOX_WITHDRAWEL)
            {
                string stepCsv = "StepENT" + sNumber;
            }
            if (mode == AllEnums.PBD_Mode.ASSEMBLY_DONE)
            {
                string stepCsv = "StepMON" + sNumber;
            }
        }

        public void createEndScene(string pName, string pEndcondition)
        {
            // create a scene for displaying the withdrawel from the box
            Scene.Scene autoScene = new Scene.Scene();
            Scene.SceneText text = new Scene.SceneText(0.5 * KinectManager.Instance.ImageSize.Width, 0.5 * KinectManager.Instance.ImageSize.Height, "Fertig", System.Windows.Media.Color.FromRgb(255, 255, 255), 10.0, new System.Windows.Media.FontFamily("Arial"));
            autoScene.Add(text);

            AdaptiveScene adaptiveScene = new AdaptiveScene(autoScene, AdaptivityLevel.AdaptivityLevels.FirstOrDefault());

            createStep(AllEnums.PBD_Mode.END_CONDITION, adaptiveScene, pName, pEndcondition);
        }

        /// <summary>
        /// This calculates the highest stepnumber that is used in the current workflow
        /// </summary>
        /// <returns>the highest number</returns>
        /// 
        /// 
        private int calculateHighestStepnumber()
        {
            int highestStepNumber = -1;
            foreach (WorkingStep step in m_CurrentWorkflow.WorkingSteps)
            {
                if (step.StepNumber > highestStepNumber)
                {
                    highestStepNumber = step.StepNumber;
                }
            }

            return highestStepNumber;
        }

        public void SaveCurrentWorkflow(bool showFileSaveDialog)
        {
            // check if workflow dir exists
            if(!Directory.Exists(ProjectConstants.WORKFLOW_DIR))
            {
                // if not create it
                Directory.CreateDirectory(ProjectConstants.WORKFLOW_DIR);
            }

            string filename;
            if (showFileSaveDialog)
            {
                System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();

                dlg.InitialDirectory = Directory.GetCurrentDirectory() + "\\" + ProjectConstants.WORKFLOW_DIR;
                dlg.ShowDialog();

                filename = dlg.FileName;
                if (!filename.EndsWith(ProjectConstants.WORKFLOW_FILE_ENDING))
                {
                    filename = filename + ProjectConstants.WORKFLOW_FILE_ENDING;
                }
            }
            else
            {
                filename = ProjectConstants.WORKFLOW_DIR + "\\" + CurrentWorkflow.Name + ProjectConstants.WORKFLOW_FILE_ENDING;
            }

            UtilitiesIO.SaveObjectToJson(m_CurrentWorkflow, filename);
        }

        public int CalculateHighestWorkingStepNumber()
        {
            int tempWorkNumber = 0;
            foreach (WorkingStep step in EditWorkflowManager.Instance.CurrentWorkflow.WorkingSteps)
            {
                if (tempWorkNumber < step.StepNumber)
                {
                    tempWorkNumber = step.StepNumber;
                }
            }
            m_HighestWorkingStepNumber = tempWorkNumber;
            return m_HighestWorkingStepNumber;
        }


        #region getter/setter
        public Workflow CurrentWorkflow
        {
            get
            {
                return m_CurrentWorkflow;
            }
            set
            {
                m_CurrentWorkflow = value;
            }
        }

        public int CurrentWorkingStepNumber
        {
            get
            {
                return m_CrrentWorkingStepNumber;
            }
            set
            {
                m_CrrentWorkingStepNumber = value;
            }
        }

        public int HighestWorkingStepNumber
        {
            get
            {
                return calculateHighestStepnumber();
            }
            set
            {
                m_HighestWorkingStepNumber = value;
            }
        }
        #endregion
    }
}
