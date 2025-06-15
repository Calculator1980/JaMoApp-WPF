using System;
using System.Windows;
using System.Windows.Interop;
using Autodesk.Revit.UI;
using JaMoApp.ViewModels;
using Microsoft.Win32;

namespace JaMoApp.WpfViews
{
    public partial class ParametersWindow : Window
    {
        public ParametersWindow(ParametersViewModel vm, ExternalCommandData commandData)
        {
            InitializeComponent();
            DataContext = vm;

            // Parent this WPF window to Revit’s main window
            new WindowInteropHelper(this)
            {
                Owner = commandData.Application.MainWindowHandle
            };

            vm.BrowseFileRequested += OnBrowseFileRequested;
        }

        private void OnBrowseFileRequested(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Shared Parameter Files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (dlg.ShowDialog(this) == true && sender is ParametersViewModel vm)
            {
                vm.FilePath = dlg.FileName;
                vm.LoadParameters(vm.FilePath);
            }
        }
    }
}
