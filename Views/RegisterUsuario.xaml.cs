using Cursos.Models;
using Cursos.Service;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Cursos.Views;

public partial class RegisterUsuario : ContentPage
{
    public ObservableCollection<User> cursos { get; set; }
    private readonly HttpClient _httpClient;
    private readonly string _url;
    private int? _id;

    public RegisterUsuario(int? id = null)
	{
		InitializeComponent();
        _httpClient = new HttpClient();
        Url url = new Url();
        _url = url.url;
        _httpClient.BaseAddress = new Uri(_url);

        if (id.HasValue)
        {
            _id = id.Value;
            getCliente(id.Value);
            registerButton.Text = "Actualizar";
            titulo.Text = "Actualizar Usuario";
            PasswordStackLayout.IsVisible = false;
        }
    }

    private async void getCliente(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"usuarios/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                // Deserializamos el JSON y obtenemos el cliente
                var responseData = JsonSerializer.Deserialize<JsonObject>(content);

                if (responseData != null && responseData.TryGetPropertyValue("data", out var data))
                {
                    var user = JsonSerializer.Deserialize<User>(data.ToString());

                    if (user != null)
                    {
                        // Rellenamos los Entry con los valores del cliente
                        NameEntry.Text = user?.nombre ?? string.Empty;
                        EmailEntry.Text = user?.correo ?? string.Empty;
                    }
                    else
                    {
                        ResultLabel.TextColor = Colors.Red;
                        ResultLabel.Text = "Usuario no encontrado o datos inválidos.";
                    }
                }
            }
            else
            {
                // Respuesta no exitosa
                ResultLabel.TextColor = Colors.Red;
                ResultLabel.Text = "Error al obtener el usuario.";
            }
        }
        catch (Exception ex)
        {
            // Manejo de errores: Esto te ayudará a identificar exactamente dónde ocurre el problema
            ResultLabel.TextColor = Colors.Red;
            ResultLabel.Text = $"Error al obtener el usuario: {ex.Message}";
            Console.WriteLine($"Error: {ex.Message}");  // Para ver el stack trace en el output
        }
    }



    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Usuarios());
    }

    private async void OnCreateUserClicked(object sender, EventArgs e)
    {
        var curso = new User
        {
            nombre = NameEntry.Text,
            correo = EmailEntry.Text
        };

        // Si el _id tiene un valor, no incluir el password
        if (_id.HasValue)
        {
            // Eliminar el password si _id tiene valor
            curso.password = null; // O podrías no asignarlo en este caso
        }
        else
        {
            curso.password = PasswordEntry.Text;
        }

        try
        {
            var jsonRequest = JsonSerializer.Serialize(curso);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            HttpResponseMessage response;

            if (_id.HasValue)
            {
                response = await _httpClient.PutAsync($"usuarios/{_id.Value}", content);
            }
            else
            {
                // Si no tenemos id, hacemos una petición POST (para crear)
                response = await _httpClient.PostAsync("usuarios", content);
            }

            if (response.IsSuccessStatusCode)
            {
                ResultLabel.TextColor = Colors.Green;
                ResultLabel.Text = _id.HasValue ? "Usuario actualizado!" : "Usuario registrado!";
                await Navigation.PopAsync();
            }
            else
            {
                ResultLabel.TextColor = Colors.Red;
                ResultLabel.Text = "Error al registrar el usuario.";
            }
        }
        catch (Exception ex)
        {
            ResultLabel.TextColor = Colors.Red;
            ResultLabel.Text = $"Error: {ex.Message}";
        }

    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }

    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Cursos());
    }

    private async void OnClientsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Clientes());
    }

    private async void OnUsersClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Usuarios());
    }
}