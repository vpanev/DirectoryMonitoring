using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Windows;
using MessageBox = System.Windows.Forms.MessageBox;

namespace MoveIT.Transfer.Task.Application
{
	/// <summary>
	/// Interaction logic for Home.xaml
	/// </summary>
	public partial class Home
	{
		private string _chosenDirectory = null!;
		private bool _hasChoseDirectory;

		public Home()
		{
			InitializeComponent();
			StopMonitorButton.IsEnabled = false;
		}

		private void SelectDirectory(object sender, RoutedEventArgs e)
		{
			using var dialog = new FolderBrowserDialog();
			var result = dialog.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
			{
				_chosenDirectory = dialog.SelectedPath;
				_hasChoseDirectory = true;
				SelectedDirectoryTextBox.Text = _chosenDirectory;
			}
		}

		private async void StartMonitor(object sender, RoutedEventArgs e)
		{
			if (_hasChoseDirectory)
			{
				using var client = new HttpClient();
				var directoryBytes = Encoding.UTF8.GetBytes(_chosenDirectory);
				var encodedDirectory = Convert.ToBase64String(directoryBytes);
				var url = new UriBuilder(ConfigurationManager.AppSettings["API_URL"] + $"Monitor/folder/{encodedDirectory}/start");
				var response = await client.GetAsync(url.ToString());

				if (response.IsSuccessStatusCode)
				{
					MessageBox.Show(
						"Successfully settled directory for monitoring",
						"Successful operation!",
						MessageBoxButtons.OK);

					StartMonitorButton.IsEnabled = false;
					StopMonitorButton.IsEnabled = true;
				}
				else
				{
					MessageBox.Show(
						"Something went wrong while trying to set directory for monitoring",
						"Error",
						MessageBoxButtons.OK);
				}
			}
			else
			{
				errorMessage.Text = "Please choose directory";
				SelectedDirectoryTextBox.Focus();
			}
		}

		private async void StopMonitor(object sender, RoutedEventArgs e)
		{
			using var client = new HttpClient();
			var directoryBytes = Encoding.UTF8.GetBytes(_chosenDirectory);
			var encodedDirectory = Convert.ToBase64String(directoryBytes);
			var url = new UriBuilder(ConfigurationManager.AppSettings["API_URL"] + $"Monitor/folder/{encodedDirectory}/stop");
			var response = await client.GetAsync(url.ToString());

			if (response.IsSuccessStatusCode)
			{
				MessageBox.Show(
					"Monitoring is stopped!",
					"Successful!",
					MessageBoxButtons.OK);

				StartMonitorButton.IsEnabled = true;
				StopMonitorButton.IsEnabled = false;
			}
			else
			{
				MessageBox.Show(
					"Something went wrong while trying to stop monitoring",
					"Error",
					MessageBoxButtons.OK);
			}
		}
	}
}
