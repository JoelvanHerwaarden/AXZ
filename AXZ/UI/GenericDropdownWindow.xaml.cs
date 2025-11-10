using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using AXZ.Utilities;

namespace AXZ.UI
{
    public partial class GenericDropdownWindow :  Window
    {
        public bool Cancelled { get; set; }
        public Dictionary<string, dynamic> Items { get; set; }
        public List<dynamic> CurrentSelection { get; set; }
        public List<dynamic> SelectedItems { get; set; }
        public string AdditionalOptionsFirstText { get; set; }
        public bool AdditionalOptionsFirstStatus = true;
        public string AdditionalOptionsSecondText { get; set; }
        public bool AdditionalOptionsSecondStatus = true;
        public string AdditionalOptionsThirdText { get; set; }
        public bool AdditionalOptionsThirdStatus = true;

        public GenericDropdownWindow(string windowTitle, 
            string instruction, 
            Dictionary<string, dynamic> items, 
            Window owner, 
            bool selectMultiple, 
            string additionalOptionsFirstText = null,
            string additionalOptionsSecondText = null,
            string additionalOptionsThirdText = null)
        {
            InitializeComponent();
            this.Items = items.SortDictionary();
            this.InstructionLabel.Content = instruction;
            this.Title = windowTitle;
            this.Owner = owner;
            SearchList("");
            if (!selectMultiple)
            {
                this.SelectAllButton.Visibility = System.Windows.Visibility.Hidden;
                this.SelectNoneButton.Visibility = System.Windows.Visibility.Hidden;
                this.ItemNamesListBox.SelectionMode = System.Windows.Controls.SelectionMode.Single;
            }
            this.Cancelled = true;
            this.SelectedItems = new List<dynamic>();
            this.CurrentSelection = new List<dynamic>();
            AdditionalOptionsFirstText = additionalOptionsFirstText;
            AdditionalOptionsSecondText = additionalOptionsSecondText;
            AdditionalOptionsThirdText = additionalOptionsThirdText;
            ShowAdditionalOptions();

            AXZThemeManager.ApplyTheme(this);
        }

        private void ShowAdditionalOptions()
        {
            if (AdditionalOptionsFirstText != null)
            {
                this.AdditionalOptionsFirstRow.Height = new GridLength(60);
                this.AdditionalOptionsFirstCheckbox.Visibility = System.Windows.Visibility.Visible;
                this.AdditionalOptionsFirstCheckbox.Content = AdditionalOptionsFirstText;
            }
            if (AdditionalOptionsSecondText != null)
            {
                this.AdditionalOptionsSecondRow.Height = new GridLength(60);
                this.AdditionalOptionsSecondCheckbox.Visibility = System.Windows.Visibility.Visible;
                this.AdditionalOptionsSecondCheckbox.Content = AdditionalOptionsSecondText;
            }
            if (AdditionalOptionsThirdText != null)
            {
                this.AdditionalOptionsThirdRow.Height = new GridLength(60);
                this.AdditionalOptionsThirdCheckbox.Visibility = System.Windows.Visibility.Visible;
                this.AdditionalOptionsThirdCheckbox.Content = AdditionalOptionsThirdText;
            }
        }
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Cancelled = false;
            foreach (string item in this.CurrentSelection)
            {
                SelectedItems.Add(Items[item]);
            }
            this.AdditionalOptionsFirstStatus = this.AdditionalOptionsFirstCheckbox.IsChecked.Value;
            this.AdditionalOptionsSecondStatus = this.AdditionalOptionsSecondCheckbox.IsChecked.Value;
            this.AdditionalOptionsThirdStatus = this.AdditionalOptionsThirdCheckbox.IsChecked.Value;
            this.Close();
        }
        private void Listbox_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            foreach(var item in args.AddedItems)
            {
                this.CurrentSelection.Add(item);
            }
            foreach(var item in args.RemovedItems)
            {
                this.CurrentSelection.Remove(item);
            }
            this.SelectionIndicator.Content = string.Format("Selected {0} Items", this.CurrentSelection.Count);
        }
        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            this.ItemNamesListBox.SelectAll();
        }
        private void SelectNone_Click(object sender, RoutedEventArgs e)
        {

            this.ItemNamesListBox.SelectedItem = null;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.ItemNamesListBox.SelectionChanged -= Listbox_SelectionChanged;
            System.Windows.Controls.TextBox box = (System.Windows.Controls.TextBox)sender;
            string searchString = box.Text;
            if (searchString == "")
            {
                try
                {
                    SearchList("");
                }
                catch { }
                SearchLabel.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                SearchList(searchString);
                SearchLabel.Visibility = System.Windows.Visibility.Hidden;
            }
            foreach (var selectedItem in CurrentSelection)
            {
                this.ItemNamesListBox.SelectedItems.Add(selectedItem);
            }

            this.ItemNamesListBox.SelectionChanged += Listbox_SelectionChanged;
        }

        private void SearchList(string searchTerm)
        {
            this.ItemNamesListBox.Items.Clear();
            string[] searchTerms = searchTerm.Split(' ');
            foreach (string key in this.Items.Keys)
            {
                bool match = true;
                foreach (string searchString in searchTerms)
                {
                    if (!key.ToLower().Contains(searchString.ToLower()))
                    {
                        if (Regex.Match(key, searchTerm).Success)
                        {
                            match = true;
                            break;
                        }
                        match = false;
                    }
                }
                if (match)
                {
                    this.ItemNamesListBox.Items.Add(key);
                    
                }
            }
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.TextBox box = (System.Windows.Controls.TextBox)sender;
            SearchLabel.Visibility = System.Windows.Visibility.Hidden;

        }
        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.TextBox box = (System.Windows.Controls.TextBox)sender;
            if (box.Text.Length == 0)
            {
                SearchLabel.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                SearchLabel.Visibility = System.Windows.Visibility.Hidden;
            }

        }
    }
}