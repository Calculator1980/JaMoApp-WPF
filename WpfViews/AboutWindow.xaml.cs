using System;
using System.Diagnostics;
using System.Windows;

namespace JaMoApp.WpfViews
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        // Optional property for version if you want to set it dynamically from the command
        public string AppVersion
        {
            set => txtVersion.Text = $"Version: {value}";
        }

        // Optional property for copyright if you want to set it dynamically
        public string CopyrightInfo
        {
            set => txtCopyright.Text = value;
        }

        private void BtnGitHub_Click(object sender, RoutedEventArgs e)
        {
            // Open your GitHub page in the default browser
            OpenUrl("https://github.com/Calculator1980");
        }

        private void BtnLinkedIn_Click(object sender, RoutedEventArgs e)
        {
            // Open your LinkedIn page in the default browser
            OpenUrl("https://www.linkedin.com/public-profile/settings?trk=d_flagship3_profile_self_view_public_profile");
        }

        private void BtnLicense_Click(object sender, RoutedEventArgs e)
        {
            // Open a license page or MIT License link
            OpenUrl("https://opensource.org/licenses/MIT");
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Helper method to open URL in default browser
        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open URL: {ex.Message}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }
    }
}
