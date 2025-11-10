using AXZ.Utilities;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AXZ.UI
{ 
    public sealed partial class GenericInputWindow : Window
    {
        public enum WindowSize
        {
            SmallWindow = 0,
            LargeWindow = 1,
            Oneline = 2
        }
        public enum InputType
        {
            NumbersOnly = 0,
            NumbersCommasPeriods = 1,
            Everything = 2
        }
        public bool Cancelled { get; set; }
        /// <summary>
        /// Returns the input given by the user as a string. If you expect a number, parse to Double after.
        /// </summary>
        public string Input { get; set; }
        private bool NumbersOnly { get; set; }
        public GenericInputWindow(string windowTitle,
            string instruction,
            bool NumbersOnly, 
            Window owner,
            WindowSize windowSize, InputType inputType)
        {
            this.InitializeComponent();
            this.Title = Title;
            this.InstructionLabel.Content= instruction;
            this.Owner = owner;
            this.NumbersOnly = NumbersOnly;
            ConfigureWindow(windowSize, inputType);
        }

        private void ConfigureWindow(WindowSize windowSize, InputType inputType)
        {
            //Row Heights
            //Small Text = 120
            //Large Text = 300 / 300
            //Number = 60 (One Liner)
            switch(windowSize)
            {
                case WindowSize.SmallWindow:
                    this.VariableRow.Height = new GridLength(120);
                    SetWindowHeight(205 + 60);
                    break;
                case WindowSize.LargeWindow:
                    this.VariableRow.Height = new GridLength(300);
                    SetWindowHeight(205 + 240);
                    break;
                case WindowSize.Oneline:
                    this.VariableRow.Height = new GridLength(60);
                    this.InputTextbox.MaxLines = 1;
                    SetWindowHeight(205);
                    break;
            }
            switch (inputType)
            {
                case InputType.NumbersOnly:
                    this.InputTextbox.PreviewTextInput += Textbox_NumbersOnly;
                    break;
                case InputType.NumbersCommasPeriods:
                    this.InputTextbox.PreviewTextInput += Textbox_NumbersCommasAndPeriods;
                    break;
                default:
                    break;
            }
            if (NumbersOnly)
            {
                this.InputTextbox.PreviewTextInput += Textbox_NumbersCommasAndPeriods;
            }
        }
        private void SetWindowHeight(double height, bool ResizedAllow = false)
        {
            this.Height = height;
            if (!ResizedAllow)
            {
                this.MinHeight = height;
                this.MaxHeight = height;
            }
        }
        private void Textbox_NumbersCommasAndPeriods(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            //Allow digits, period, and comma
            if (!char.IsDigit(e.Text, 0) && e.Text != "." && e.Text != ","&&e.Text != "-")
            {
                e.Handled = true; //Block the input
            }
        }
        private void Textbox_NumbersOnly(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            //Allow digits, period, and comma
            if (!char.IsDigit(e.Text, 0))
            {
               
                e.Handled = true; //Block the input
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Cancelled = false;
            this.Input = this.InputTextbox.Text;
            this.Close();
        }
    }
}
