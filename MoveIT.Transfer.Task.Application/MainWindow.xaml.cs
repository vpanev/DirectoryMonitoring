using MoveIT.Transfer.Task.Application.Helpers;
using MoveIT.Transfer.Task.Application.Models;
using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http;
using System.Web;
using System.Windows;
using System.Windows.Input;
using Cursors = System.Windows.Input.Cursors;
using MessageBox = System.Windows.MessageBox;

namespace MoveIT.Transfer.Task.Application
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private async void button1_Click(object sender, RoutedEventArgs e)
		{
			if (textBoxUsername.Text.Length == 0)
			{
				errormessage.Text = "Enter username.";
				textBoxUsername.Focus();
			}

			if (passwordBox1.Password.Length == 0)
			{
				errormessage.Text = "Enter password.";
				passwordBox1.Focus();
			}

			Mouse.OverrideCursor = Cursors.Wait;
			button1.IsEnabled = false;
			using var client = new HttpClient();
			try
			{
				var url = new UriBuilder(ConfigurationManager.AppSettings["API_URL"] + "Authentication");

				var key = ConfigurationManager.AppSettings["ENCODE_SALT_KEY"];
				var encryptedPassword = await EncryptorHelper.EncryptStringAsync(key!, passwordBox1.Password);

				//var query = HttpUtility.ParseQueryString(url.Query);
				//query["username"] = textBoxUsername.Text;
				//query["password"] = encryptedPassword;
				//url.Query = query.ToString();

				var request = new GetTokenRequest()
				{
					Username = textBoxUsername.Text,
					Password = encryptedPassword
				};

				var content = new StringContent(JsonConvert.SerializeObject(request), System.Text.Encoding.UTF8, "application/json");

				var response = await client.PostAsync(url.ToString(), content);
				response.EnsureSuccessStatusCode();

				var responseData = JsonConvert.DeserializeObject<GetTokenResponse>(await response.Content.ReadAsStringAsync());

				var homeWindow = new Home();
				homeWindow.Show();
				Close();

				Mouse.OverrideCursor = null;
				button1.IsEnabled = true;
			}
			catch (Exception)
			{
				MessageBox.Show("Login failed");
				errormessage.Text = "Invalid credentials";
				Mouse.OverrideCursor = null;
				button1.IsEnabled = true;
			}
		}
	}
}