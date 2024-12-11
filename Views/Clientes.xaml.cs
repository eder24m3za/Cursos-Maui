using Cursos.Models;
using Cursos.Service;
using System.Collections.ObjectModel;
using System.Text.Json.Nodes;
using System.Text.Json;
namespace Cursos.Views;

public partial class Clientes : ContentPage
{
    private readonly HttpClient _httpClient;
    private readonly string _url;
    private int _idCurso = 0;
    public ObservableCollection<Cliente> clientes { get; set; }

    public Clientes(int? idCurso = null)
	{
		InitializeComponent();
        _httpClient = new HttpClient();
        Url url = new Url();
        _url = url.url;
        _httpClient.BaseAddress = new Uri(_url);
        if (idCurso.HasValue)
        {
            _idCurso = idCurso.Value;
            getClientesByIdCurso(_idCurso);
            var titleLabel = (Label)Shell.Current.FindByName("titleShell");
            titleLabel.Text = $"Clientes del curso {_idCurso}";
        }
        else
        {
            var titleLabel = (Label)Shell.Current.FindByName("titleShell");
            titleLabel.Text = "Clientes";
            getClientes();
        }
    }

    private async void getClientes()
    {
        var response = _httpClient.GetAsync("clientes").Result;
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            // Deserializamos el JSON completo, y luego tomamos solo la propiedad 'data'
            var responseData = JsonSerializer.Deserialize<JsonObject>(content);

            if (responseData != null && responseData.TryGetPropertyValue("data", out var data))
            {
                // Deserializamos la propiedad 'data' en una colección de Cursos
                clientes = JsonSerializer.Deserialize<ObservableCollection<Cliente>>(data.ToString());

                // Asignamos la colección al CollectionView
                ClientesCollectionView.ItemsSource = clientes;
            }
        }
        else
        {
            //ResultLabel.TextColor = Colors.Red;
            //ResultLabel.Text = "Error loading courses.";
        }
    }

    private async void getClientesByIdCurso(int idCurso)
    {
        var response = _httpClient.GetAsync($"cursos/clientes/{idCurso}").Result;
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            // Deserializamos el JSON completo, y luego tomamos solo la propiedad 'data'
            var responseData = JsonSerializer.Deserialize<JsonObject>(content);

            if (responseData != null && responseData.TryGetPropertyValue("data", out var data))
            {
                // Deserializamos la propiedad 'data' en una colección de Cursos
                clientes = JsonSerializer.Deserialize<ObservableCollection<Cliente>>(data.ToString());

                // Asignamos la colección al CollectionView
                ClientesCollectionView.ItemsSource = clientes;
            }
        }
        else
        {
            //ResultLabel.TextColor = Colors.Red;
            //ResultLabel.Text = "Error loading courses.";
        }
    }

    private async void OnClienteSelected(object sender, EventArgs e)
    {

        var button = sender as Button;

        // Validar que no sea nulo
        if (button == null)
        {
            await DisplayAlert("Error", "No se pudo identificar el botón", "OK");
            return;
        }

        // Obtener el CommandParameter del botón
        var clienteId = button.CommandParameter?.ToString();

        // Validar que CommandParameter no sea nulo
        if (string.IsNullOrEmpty(clienteId))
        {
            await DisplayAlert("Error", "ID del curso no encontrado", "OK");
            return;
        }

        // Intentar convertir el cursoId a int
        if (int.TryParse(clienteId, out int id))
        {
            // Navegar a la página con el ID del curso
            await Navigation.PushAsync(new RegisterCliente(id));
        }
        else
        {
            await DisplayAlert("Error", "ID del curso no válido", "OK");
        }

    }

    private async void OnEditClienteTapped(object sender, EventArgs e)
    {
        var mi = ((MenuItem)sender);
        var cliente = (Cliente)mi.CommandParameter;
        //await Navigation.PushAsync(new RegisterCliente(cliente));
    }

    private async void OnAddClienteClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterCliente());
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
        if (_idCurso != 0)
        {
            getClientesByIdCurso(_idCurso);
        }
        else
        {
            getClientes();
        }
    }

}