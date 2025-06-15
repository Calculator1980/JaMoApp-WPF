using System;
using System.Windows;
using Autodesk.Revit.UI;
using JaMoApp.ViewModels;

namespace JaMoApp.WpfViews
{
    /// <summary>
    /// Interaction logic for PaRadarWindow.xaml
    /// </summary>
    public partial class PaRadarWindow : Window
    {
        private UIDocument _uidoc;
        private PaRadarViewModel _viewModel;

        public UIDocument Uidoc
        {
            get => _uidoc;
            set
            {
                _uidoc = value;
                if (_viewModel != null)
                {
                    _viewModel.Uidoc = value;
                }
            }
        }

        public PaRadarWindow()
        {
            InitializeComponent();

            try
            {
                // Initialize the ViewModel
                _viewModel = new PaRadarViewModel();
                DataContext = _viewModel;

                // Set window properties for better Revit integration
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
                ShowInTaskbar = false;
                Topmost = false;

                // Handle window events
                Loaded += PaRadarWindow_Loaded;
                Closing += PaRadarWindow_Closing;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing PaRadar window: {ex.Message}",
                    "PaRadar Initialization Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void PaRadarWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Ensure the ViewModel has the UIDocument
                if (_viewModel != null && _uidoc != null)
                {
                    _viewModel.Uidoc = _uidoc;

                    // Update status to show we're ready
                    _viewModel.StatusText = $"Ready - Document: {_uidoc.Document.Title}";

                    // Set the shared parameter file path if available
                    string sharedParamFile = _uidoc.Document.Application.SharedParametersFilename;
                    if (!string.IsNullOrEmpty(sharedParamFile))
                    {
                        _viewModel.SharedParameterFilePath = sharedParamFile;
                    }
                }
                else
                {
                    MessageBox.Show("Warning: No active document detected. Some features may not work correctly.",
                        "PaRadar Warning",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting up PaRadar: {ex.Message}",
                    "PaRadar Setup Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void PaRadarWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                // Clean up any resources if needed
                if (_viewModel != null)
                {
                    _viewModel.StatusText = "Closing PaRadar...";
                }
            }
            catch (Exception ex)
            {
                // Log error but don't prevent closing
                System.Diagnostics.Debug.WriteLine($"Error during PaRadar cleanup: {ex.Message}");
            }
        }
    }
}