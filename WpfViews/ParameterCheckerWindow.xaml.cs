using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Media;
using JaMoApp.Utilities;
using JaMoApp.Handlers;
using JaMoApp.ViewModels;
using Visibility = System.Windows.Visibility;

namespace JaMoApp
{
    public partial class ParameterCheckerWindow : Window
    {
        public ObservableCollection<ParameterGroupViewModel> ParameterGroups { get; set; } = new ObservableCollection<ParameterGroupViewModel>();
        private UIDocument _uidoc;
        public UIDocument Uidoc => _uidoc; // Expose for handler
        private ParameterCheckerExternalEventHandler _handler;
        private ExternalEvent _externalEvent;

        private readonly ClearOverridesHandler _clearHandler = new ClearOverridesHandler();
        private readonly ExternalEvent _clearEvent;
        private HashSet<int> _overriddenElementIds = new HashSet<int>();

        // Data for handler
        internal List<ParameterGroup> SelectedGroupsForRun { get; private set; }
        internal string SharedParameterFilePath { get; private set; }
        internal Action<List<ElementResultViewModel>> OnResultsReady;
        internal Action<List<MissingParameterInfo>> OnMissingParamsReady;

        public ParameterCheckerWindow(UIDocument uidoc)
        {
            InitializeComponent();
            _uidoc = uidoc;
            lstParameterGroups.ItemsSource = ParameterGroups;

            // Wire up event handlers
            btnBrowse.Click += BtnBrowse_Click;
            btnRun.Click += BtnRun_Click;
            btnCancel.Click += BtnClose_Click;
            dgResults.MouseDoubleClick += DgResults_MouseDoubleClick;
            txtFilter.TextChanged += TxtFilter_TextChanged;

            // Setup handler and event
            _handler = new ParameterCheckerExternalEventHandler(this);
            _externalEvent = ExternalEvent.Create(_handler);
            _clearEvent = ExternalEvent.Create(_clearHandler);

            // Wire up result callbacks
            OnResultsReady = results =>
            {
                dgResults.ItemsSource = results;
                // Track overridden element IDs for later clearing
                _overriddenElementIds.Clear();
                foreach (var r in results)
                    _overriddenElementIds.Add(r.ElementId);
            };
            OnMissingParamsReady = missingParams =>
            {
                dgMissingParams.ItemsSource = missingParams;
                dgMissingParams.Visibility = missingParams.Any() ? Visibility.Visible : Visibility.Collapsed;
            };

            this.Closed += (s, e) =>
            {
                // On window close, clear overrides
                _clearHandler.Setup(_uidoc, _overriddenElementIds);
                _clearEvent.Raise();
            };
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*" };
            if (dlg.ShowDialog() == true)
            {
                txtFilePath.Text = dlg.FileName;
                LoadSharedParameterFile(dlg.FileName);
            }
        }

        private void BtnRun_Click(object sender, RoutedEventArgs e)
        {
            if (!SelectedParameterGroups.Any())
            {
                MessageBox.Show("Please select at least one parameter group with required parameters.",
                    "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Queue up data for handler
            SelectedGroupsForRun = SelectedParameterGroups;
            SharedParameterFilePath = txtFilePath.Text;
            _externalEvent.Raise();
        }

        private void DgResults_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgResults.SelectedItem is ElementResultViewModel selected)
            {
                _handler.QueueShowMissingParams(selected.ElementId);
                _externalEvent.Raise();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();

        private void TxtFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var filter = txtFilter.Text.ToLower();
            lstParameterGroups.ItemsSource = string.IsNullOrEmpty(filter)
                ? ParameterGroups
                : new ObservableCollection<ParameterGroupViewModel>(
                    ParameterGroups.Where(pg => pg.GroupName.ToLower().Contains(filter)));
        }

        private void LoadSharedParameterFile(string filePath)
        {
            var parameters = SharedParameterParser.ParseGeometricParameters(filePath);
            ParameterGroups.Clear();

            foreach (var group in parameters.GroupBy(p => p.Group))
            {
                var groupVM = new ParameterGroupViewModel
                {
                    GroupName = group.Key,
                    IsSelected = false,
                    Parameters = new ObservableCollection<ParameterItemViewModel>(
                        group.Select(p => new ParameterItemViewModel
                        {
                            Name = p.Name,
                            IsChecked = false
                        }))
                };
                ParameterGroups.Add(groupVM);
            }
        }

        public List<ParameterGroup> SelectedParameterGroups =>
            ParameterGroups.Where(g => g.IsSelected)
                .Select(g => new ParameterGroup(
                    g.GroupName,
                    g.Parameters.Where(p => p.IsChecked).Select(p => p.Name).ToList()))
                .Where(g => g.ParameterNames.Any())
                .ToList();
    }
}
