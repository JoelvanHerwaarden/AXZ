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
        public bool AddToView { get; private set; }
        private Dictionary<string, PhaseFilter> PhaseFilters { get; set; }
        public ViewPhaseWindow(string windowTitle,
            Document document,
            Window owner)
        {
            InitializeComponent();
            this.Phase = document.ActiveView.LookupParameter("SP_ViewPhase").AsString();
            this.AddToView = false;
            this.Title = windowTitle;
            this.Owner = owner;
            this.PhaseTextBox.Text = this.Phase;
            Cancelled = true;
            PhaseLevelListbox.Items.Add("Phase 1");
            PhaseLevelListbox.Items.Add("Phase 2");
            PhaseLevelListbox.Items.Add("Phase 3");
            PhaseLevelListbox.Items.Add("Phase Combined");

            PhaseFilters = new Dictionary<string, PhaseFilter>();
            foreach(PhaseFilter pf in new FilteredElementCollector(document)
                .OfClass(typeof(PhaseFilter))
                .Cast<PhaseFilter>())
            {
                this.PhaseFilterList.Items.Add(pf.Name);  
                PhaseFilters.Add(pf.Name, pf);
            }

        }
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Cancelled = false;
            this.SelectedPhaseLevel = PhaseLevelListbox.SelectedItem.ToString();
            this.AddToView = this.AddToViewCheckbox.IsChecked.Value;
            this.Close();
        }
    }
}
