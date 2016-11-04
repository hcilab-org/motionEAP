// <copyright file=EditWorkingStepDialog.xaml.cs
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

using motionEAPAdmin.Model.Process;
using System.Windows;

namespace motionEAPAdmin.GUI.Dialog
{
    /// <summary>
    /// Interaction logic for EditWorkingStepDialog.xaml
    /// </summary>
    public partial class EditWorkingStepDialog : Window
    {
        private int timeOut;
        private string name;
        private string endCondition;
        private int expectedDuration;
        private bool isManualStep;
        private bool isQSStep;
        private bool wasOk = false;

        public EditWorkingStepDialog(WorkingStep step)
        {
            InitializeComponent();
            name = step.Name;
            endCondition = step.EndConditionObjectName;
            timeOut = step.TimeOut;
            expectedDuration = step.ExpectedDuration;
            isManualStep = step.IsManualStep;
            isQSStep = step.IsQSStep;
            textBoxName.Text = name;
            textBoxEndCondition.Text = endCondition;
            inputTimeOut.Value = timeOut;
            inputExpectedDuration.Value = expectedDuration;
            inputIsManualStep.IsChecked = isManualStep;
            inputIsQSStep.IsChecked = isQSStep;
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            name = textBoxName.Text;
            endCondition = textBoxEndCondition.Text;
            timeOut = (int)inputTimeOut.Value;
            expectedDuration = (int)inputExpectedDuration.Value;
            isManualStep = (bool)inputIsManualStep.IsChecked;
            isQSStep = (bool)inputIsQSStep.IsChecked;
            wasOk = true;

            this.Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public bool wasOkay()
        {
            return wasOk;
        }

        public string EditedName
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public string EditedEndCondition
        {
            get
            {
                return endCondition;
            }
            set
            {
                endCondition = value;
            }
        }

        public int EditedTimeOut
        {
            get
            {
                return timeOut;
            }
            set
            {
                timeOut = value;
            }
        }

        public int EditedExpectedDuration
        {
            get
            {
                return expectedDuration;
            }
            set
            {
                expectedDuration = value;
            }
        }
        public bool IsManualStep
        {
            get
            {
                return isManualStep;
            }
            set
            {
                isManualStep = value;
            }
        }

        public bool IsQSStep
        {
            get
            {
                return isQSStep;
            }
            set
            {
                isQSStep = value;
            }
        }
    }
}
