using Cursos.Models;
using Cursos.Service;
using System.Text;
using System.Text.Json;

namespace Cursos.Views;

public partial class Register : ContentPage
{
    private readonly HttpClient _httpClient;
    private readonly string _url;
    public Register()
	{
		InitializeComponent();
        _httpClient = new HttpClient();
        Url url = new Url();
        _url = url.url;
        _httpClient.BaseAddress = new Uri(_url);
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        var user = new User
        {
            nombre = NameEntry.Text,
            correo = EmailEntry.Text,
            password = PasswordEntry.Text
        };

        if(string.IsNullOrEmpty(user.nombre) || string.IsNullOrEmpty(user.correo) || string.IsNullOrEmpty(user.password))
        {
            ResultLabel.TextColor = Colors.Red;
            ResultLabel.Text = "Fill in all fields.";
            return;
        }

        try
        {
            var jsonRequest = JsonSerializer.Serialize(user);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("usuarios", content);

            if (response.IsSuccessStatusCode)
            {
                ResultLabel.TextColor = Colors.Green;
                ResultLabel.Text = $"User {user.nombre} registered!";

            }
            else
            {
                ResultLabel.TextColor = Colors.Red;
                ResultLabel.Text = "Registration failed. Check your data." + response.ToString();
            }
        }
        catch (Exception ex)
        {
            ResultLabel.TextColor = Colors.Red;
            ResultLabel.Text = $"Error: {ex.Message}";
        }
    }
}