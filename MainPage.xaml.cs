using Cursos.Models;
using Cursos.Service;
using Cursos.Views;
using System.Text;
using System.Text.Json;


namespace Cursos
{
    public partial class MainPage : ContentPage
    {

        private readonly HttpClient _httpClient;
        private readonly string _url;

        public MainPage()
        {
            InitializeComponent();
            var titleLabel = (Label)Shell.Current.FindByName("titleShell");
            titleLabel.Text = "Iniciar Sesión";
            //Shell.Current.Title = "Iniciar Sesión";
            _httpClient = new HttpClient();
            Url url = new Url();
            _url = url.url;
            _httpClient.BaseAddress = new Uri(_url);
            bool isLoggedIn = Preferences.Get("IsLoggedIn", false);

            if (isLoggedIn)
            {

                //Cursos.Views.Cursos Cursos = new Cursos.Views.Cursos();
                Navigation.PushAsync(new Cursos.Views.Cursos());
            }
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var user = new User
            {
                correo = EmailEntry.Text,
                password = PasswordEntry.Text
            };

            try
            {
                var jsonRequest = JsonSerializer.Serialize(user);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("login", content);

                if (response.IsSuccessStatusCode)
                {

                    ResultLabel.TextColor = Colors.Green;
                    ResultLabel.Text = $"Login successful!";

                    bool isLoggedIn = true;
                    Preferences.Set("IsLoggedIn", isLoggedIn);

                    await Navigation.PushAsync(new Cursos.Views.Cursos());
                }
                else
                {
                    ResultLabel.TextColor = Colors.Red;
                    ResultLabel.Text = "Login failed. Check your credentials.";
                }
            }
            catch (Exception ex)
            {
                ResultLabel.TextColor = Colors.Red;
                ResultLabel.Text = $"Error: {ex.Message}";
            }
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Register());

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            bool isLoggedIn = Preferences.Get("IsLoggedIn", false);

            if (isLoggedIn)
            {

                Navigation.PushAsync(new Cursos.Views.Cursos());
            }
        }
    }

}
