using Cursos.Models;
using Cursos.Service;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Cursos.Views;

public partial class RegisterCurso : ContentPage
{
    private readonly HttpClient _httpClient;
    private readonly string _url;
    private int? _id;

    public RegisterCurso(int? id = null)
	{
		InitializeComponent();
        _httpClient = new HttpClient();
        Url url = new Url();
        _url = url.url;
        _httpClient.BaseAddress = new Uri(_url);
        if(id.HasValue) 
        {
            _id = id.Value;
            getCurso(id.Value);
            registerButton.Text = "Actualizar";
        }
    }


    private async void getCurso(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"cursos/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                // Deserializamos el JSON y obtenemos el curso
                var responseData = JsonSerializer.Deserialize<JsonObject>(content);

                if (responseData != null && responseData.TryGetPropertyValue("data", out var data))
                {
                    var curso = JsonSerializer.Deserialize<Curso>(data.ToString());

                    // Rellenamos los Entry con los valores del curso
                    NameEntry.Text = curso?.nombre ?? string.Empty;
                    DescriptionEntry.Text = curso?.descripcion ?? string.Empty;
                }
            }
        }
        catch (Exception ex)
        {
            // Manejo de errores
            ResultLabel.TextColor = Colors.Red;
            ResultLabel.Text = $"Error al obtener el curso: {ex.Message}";
        }
    }

    private async void OnRegisterCursoClicked(object sender, EventArgs e)
    {
        var curso = new Curso
        {
            nombre = NameEntry.Text,
            descripcion = DescriptionEntry.Text
        };

        try
        {
            var jsonRequest = JsonSerializer.Serialize(curso);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            HttpResponseMessage response;

            if (_id.HasValue)
            {
                response = await _httpClient.PutAsync($"cursos/{_id.Value}", content);
            }
            else
            {
                // Si no tenemos id, hacemos una petición POST (para crear)
                response = await _httpClient.PostAsync("cursos", content);
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