using Cursos.Models;
using Cursos.Service;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Cursos.Views;

public partial class Clientes : ContentPage
{
    private readonly HttpClient _httpClient;
    private readonly string _url;
    public ObservableCollection<Cliente> clientes { get; set; }
    public Clientes()
	{
		InitializeComponent();
        _httpClient = new HttpClient();
        Url url = new Url();
        _url = url.url;
        _httpClient.BaseAddress = new Uri(_url);
        getClientes();
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
                CursosCollectionView.ItemsSource = clientes;
            }
        }
        else
        {
            //ResultLabel.TextColor = Colors.Red;
            //ResultLabel.Text = "Error loading courses.";
        }
    }
    private async void OnClienteSelected(object sender, TappedEventArgs e)
    {

        //var cursoId = e.Parameter.ToString();  // Obtén el ID del curso desde el CommandParameter
        // Realiza alguna acción con el ID del curso, como mostrar detalles

        var clienteId = e.Parameter.ToString();

        await DisplayAlert("Curso Seleccionado", $"ID del curso: {clienteId}", "OK");

        // Intentar convertir a int
        if (int.TryParse(clienteId, out int result))
        {
            // Si la conversión es exitosa, pasa el valor a RegisterCurso
            //await Navigation.PushAsync(new RegisterCurso(result));
        }
        else
        {
            // Si la conversión falla, manejar el caso de error (por ejemplo, mostrar un mensaje)
            await DisplayAlert("Error", "ID del curso no válido", "OK");
        }

    }

    private async void OnAddClienteClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterCurso());
    }
}