// <copyright file=EditFailState.xaml.cs
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

using System.Windows;
using motionEAPAdmin.Model.Process;
using System.Collections.ObjectModel;


namespace motionEAPAdmin.GUI
{
    /// <summary>
    /// Interaction logic for EditFailState.xaml
    /// </summary>
    public partial class EditFailStateDialog : Window
    {
        private WorkflowFailState m_FailState = null;
        ObservableCollection<WorkflowCondition> fsConditons = new ObservableCollection<WorkflowCondition>();
        private bool wasOk = false;

        public EditFailStateDialog(WorkflowFailState f)
        {
            InitializeComponent();
            m_FailState = f;
            textBoxBoxName.Text = m_FailState.Name;
            foreach (var condition in m_FailState.Conditions)
            {
                fsConditons.Add(condition);
            }
            m_listBoxConditions.ItemsSource = fsConditons;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            m_FailState.Name = textBoxBoxName.Text;
            m_FailState.Conditions = fsConditons;
            wasOk = true;

            this.Close();
        }

        public bool wasOkay()
        {
            return wasOk;
        }


        public WorkflowFailState EditedFailState
        {
            get
            {
                return m_FailState;
            }
            set
            {
                m_FailState = value;
            }
        }

        private void MenuItemDeleteTrigger_Cick(object sender, RoutedEventArgs e)
        {
            int idxCondition = m_listBoxConditions.SelectedIndex;
            if (idxCondition >= 0) 
            {
                fsConditons.RemoveAt(idxCondition);
            }
            
        }
    }
}
