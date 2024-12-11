using Cursos.Models;
using Cursos.Service;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Cursos.Views;

public partial class Usuarios : ContentPage
{
    private readonly HttpClient _httpClient;
    private readonly string _url;
    public ObservableCollection<User> users { get; set; }

    public Usuarios()
	{
		InitializeComponent();
        _httpClient = new HttpClient();
        Url url = new Url();
        _url = url.url;
        _httpClient.BaseAddress = new Uri(_url);
        getUsuarios();
    }

    private async void getUsuarios()
    {
        var response = _httpClient.GetAsync("usuarios").Result;
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            // Deserializamos el JSON completo, y luego tomamos solo la propiedad 'data'
            var responseData = JsonSerializer.Deserialize<JsonObject>(content);

            if (responseData != null && responseData.TryGetPropertyValue("data", out var data))
            {
                // Deserializamos la propiedad 'data' en una colección de Cursos
                users = JsonSerializer.Deserialize<ObservableCollection<User>>(data.ToString());

                // Asignamos la colección al CollectionView
                UsuariosCollectionView.ItemsSource = users;
            }
        }
        else
        {
            await DisplayAlert("Error", "Error cargando usuarios", "OK");
        }
    }

    private async void OnUsuarioSelected(object sender, EventArgs e)
    {
        var button = sender as Button;

        // Validar que no sea nulo
        if (button == null)
        {
            await DisplayAlert("Error", "No se pudo identificar el botón", "OK");
            return;
        }

        // Obtener el CommandParameter del botón
        var userId = button.CommandParameter?.ToString();

        // Validar que CommandParameter no sea nulo
        if (string.IsNullOrEmpty(userId))
        {
            await DisplayAlert("Error", "ID del curso no encontrado", "OK");
            return;
        }

        // Intentar convertir el cursoId a int
        if (int.TryParse(userId, out int id))
        {
            // Navegar a la página con el ID del curso
            await Navigation.PushAsync(new RegisterUsuario(id));
        }
        else
        {
            await DisplayAlert("Error", "ID del curso no válido", "OK");
        }
    }

    private async void OnAddUsuarioClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterUsuario());
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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        getUsuarios();
    }


}