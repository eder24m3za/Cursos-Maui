using Cursos.Models;
using Cursos.Service;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Cursos.Views;

public partial class RegisterCliente : ContentPage
{
    public ObservableCollection<Curso> cursos { get; set; }
    private readonly HttpClient _httpClient;
    private readonly string _url;
    private int? _id;
    public RegisterCliente()
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
                cursos = JsonSerializer.Deserialize<ObservableCollection<Models.Curso>>(data.ToString());

                // Asignamos la colección al CollectionView
                CursosPicker.ItemsSource = cursos; ;
            }
        }
        else
        {
            //ResultLabel.TextColor = Colors.Red;
            //ResultLabel.Text = "Error loading courses.";
        }


    }
    private async void OnRegisterClienteClicked(object sender, EventArgs e)
    {
        var curso = new Cliente
        {
            nombre = NameEntry.Text,
            telefono = Convert.ToInt64(PhoneEntry.Text),
            correo = EmailEntry.Text,
            curso_id = CursosPicker.SelectedItem != null ? ((Curso)CursosPicker.SelectedItem).id : 0
        };

        try
        {
            var jsonRequest = JsonSerializer.Serialize(curso);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            HttpResponseMessage response;

            if (_id.HasValue)
            {
                response = await _httpClient.PutAsync($"clientes/{_id.Value}", content);
            }
            else
            {
                // Si no tenemos id, hacemos una petición POST (para crear)
                response = await _httpClient.PostAsync("clientes", content);
            }

            if (response.IsSuccessStatusCode)
            {
                ResultLabel.TextColor = Colors.Green;
                ResultLabel.Text = _id.HasValue ? "Curso actualizado!" : "Curso registrado!";
                await Navigation.PopAsync();
            }
            else
            {
                ResultLabel.TextColor = Colors.Red;
                ResultLabel.Text = "Error al registrar el curso.";
            }
        }
        catch (Exception ex)
        {
            ResultLabel.TextColor = Colors.Red;
            ResultLabel.Text = $"Error: {ex.Message}";
        }
    }
}