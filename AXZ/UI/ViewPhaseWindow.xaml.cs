using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static AXZ.UI.GenericInputWindow;

namespace AXZ.UI
{
    public partial class ViewPhaseWindow : Window
    {
        public bool Cancelled { get; set; }
        public string Phase { get; private set; }
        public string SelectedPhaseLevel { get; private set; }
        public PhaseFilterOverride SelectedPhaseFilter { get; private set; }
        public bool AddToView { get; private set; }
        private Dictionary<string, PhaseFilterOverride> PhaseFilters = new Dictionary<string, PhaseFilterOverride>();
        public ViewPhaseWindow(string windowTitle,
            Document document,
            Window owner,
            List<PhaseFilterOverride> filters)
        {
            InitializeComponent();
            this.Phase = document.ActiveView.LookupParameter("SP_ViewPhase").AsString();
            this.AddToView = false;
            this.Title = windowTitle;
            this.Owner = owner;
            this.PhaseTextBox.Text = this.Phase;
            Cancelled = true;
            PhaseLevelListbox.Items.Add("Phase 1");
            PhaseLevelListbox.SelectedItem = "Phase 1";
            PhaseLevelListbox.Items.Add("Phase 2");
            PhaseLevelListbox.Items.Add("Phase 3");
            PhaseLevelListbox.Items.Add("Phase Combined");
            foreach (PhaseFilterOverride filter in filters)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem()
                {
                    Content = filter.Name,
                    ToolTip = filter.Description,
                    Tag = filter
                };
                this.PhaseFilterList.Items.Add(comboBoxItem);
            }
            PhaseFilterList.SelectedIndex = 0;

        }
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (PhaseLevelListbox.SelectedItem == null)
            {
                Utils.Show("Please select a Phase Filter Level.");
                return;
            }
            this.SelectedPhaseLevel = PhaseLevelListbox.SelectedItem.ToString();
            this.Cancelled = false;
            this.AddToView = this.AddToViewCheckbox.IsChecked.Value;
            if(this.PhaseFilterList.SelectedItem != null)
            {
                ComboBoxItem selectedItem = (ComboBoxItem)this.PhaseFilterList.SelectedItem;

                this.SelectedPhaseFilter = selectedItem.Tag as PhaseFilterOverride;
            }
            else
            {
                this.SelectedPhaseFilter = null;
            };

            this.Close();
        }
    }
}
