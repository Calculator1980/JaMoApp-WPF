using System.Windows;
using Autodesk.Revit.UI;
using JaMoApp.ViewModels;

namespace JaMoApp.WpfViews
{
    public partial class MassSurfaceWindow : Window
    {
        public MassSurfaceWindow()
        {
            InitializeComponent();
            DataContext = new MassSurfaceViewModel();
        }

        public UIDocument Uidoc
        {
            get => ((MassSurfaceViewModel)DataContext).Uidoc;
            set => ((MassSurfaceViewModel)DataContext).Uidoc = value;
        }
    }
}