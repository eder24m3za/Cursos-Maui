using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Nodes;
using Cursos.Models;
using Cursos.Service;

namespace Cursos.Views;

public partial class Cursos : ContentPage
{
    private readonly HttpClient _httpClient;
    private readonly string _url;
    public ObservableCollection<Curso> cursos { get; set; }
    //public Curso SelectedCurso { get; set; }

    public Cursos()
    {
        InitializeComponent();
        _httpClient = new HttpClient();
        Url url = new Url();
        _url = url.url;
        _httpClient.BaseAddress = new Uri(_url);
        getCursos();
    }

    private async void getCursos()
    {
        var response = _httpClient.GetAsync("cursos").Result;
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            // Deserializamos el JSON completo, y luego tomamos solo la propiedad 'data'
            var responseData = JsonSerializer.Deserialize<JsonObject>(content);

            if (responseData != null && responseData.TryGetPropertyValue("data", out var data))
            {
                // Deserializamos la propiedad 'data' en una colección de Cursos
                cursos = JsonSerializer.Deserialize<ObservableCollection<Curso>>(data.ToString());

                // Asignamos la colección al CollectionView
                CursosCollectionView.ItemsSource = cursos;
            }
        }
        else 
        {
            //ResultLabel.TextColor = Colors.Red;
            //ResultLabel.Text = "Error loading courses.";
        }

        
    }

    private async void OnCursoSelected(object sender, EventArgs e)
    {
        // Obtener el botón que disparó el evento
        var button = sender as Button;

        // Validar que no sea nulo
        if (button == null)
        {
            await DisplayAlert("Error", "No se pudo identificar el botón", "OK");
            return;
        }

        // Obtener el CommandParameter del botón
        var cursoId = button.CommandParameter?.ToString();

        // Validar que CommandParameter no sea nulo
        if (string.IsNullOrEmpty(cursoId))
        {
            await DisplayAlert("Error", "ID del curso no encontrado", "OK");
            return;
        }

        // Intentar convertir el cursoId a int
        if (int.TryParse(cursoId, out int id))
        {
            // Navegar a la página con el ID del curso
            await Navigation.PushAsync(new RegisterCurso(id));
        }
        else
        {
            await DisplayAlert("Error", "ID del curso no válido", "OK");
        }
    }



    protected override void OnAppearing()
    {
        base.OnAppearing();
        getCursos();
    }

    private async void OnAddCursoClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterCurso());
    }

    private async void OnClientesClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Clientes());
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