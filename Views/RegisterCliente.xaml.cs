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
    private int? _cursoId;

    public RegisterCliente(int? id = null)
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
            titulo.Text = "Actualizar Cliente";
            var titleLabel = (Label)Shell.Current.FindByName("titleShell");
            titleLabel.Text = "Actualizar Cliente";
        }
        else
        {
            var titleLabel = (Label)Shell.Current.FindByName("titleShell");
            titleLabel.Text = "Registrar Cliente";
        }
        getCursos();
    }

    private async void getCliente(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"clientes/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                // Deserializamos el JSON y obtenemos el cliente
                var responseData = JsonSerializer.Deserialize<JsonObject>(content);

                if (responseData != null && responseData.TryGetPropertyValue("data", out var data))
                {
                    var cliente = JsonSerializer.Deserialize<Cliente>(data.ToString());

                    if (cliente != null)
                    {
                        // Rellenamos los Entry con los valores del cliente
                        NameEntry.Text = cliente?.nombre ?? string.Empty;
                        PhoneEntry.Text = cliente?.telefono.ToString() ?? "0";
                        EmailEntry.Text = cliente?.correo ?? string.Empty;

                        // Verificación de curso, asignar solo si tiene valor
                        if (cliente?.curso != null)
                        {
                            // Asegúrate de que curso tenga un valor antes de asignar
                            string Text = cliente.curso;
                        }
                        else
                        {
                            // Si no hay curso, asignamos un valor vacío
                            string Text = string.Empty;
                        }

                        // Asegúrate de que _cursoId solo se asigna si no es null
                        _cursoId = cliente?.curso_id;

                        // Agregar verificación para asegurar que curso_id no es null cuando se usa
                        if (!_cursoId.HasValue)
                        {
                            // Maneja el caso en que _cursoId es null, tal vez asignar un valor por defecto
                            _cursoId = 0; // O lo que sea adecuado para tu lógica
                        }
                    }
                    else
                    {
                        ResultLabel.TextColor = Colors.Red;
                        ResultLabel.Text = "Cliente no encontrado o datos inválidos.";
                    }
                }
            }
            else
            {
                // Respuesta no exitosa
                ResultLabel.TextColor = Colors.Red;
                ResultLabel.Text = "Error al obtener el cliente.";
            }
        }
        catch (Exception ex)
        {
            // Manejo de errores: Esto te ayudará a identificar exactamente dónde ocurre el problema
            ResultLabel.TextColor = Colors.Red;
            ResultLabel.Text = $"Error al obtener el cliente: {ex.Message}";
            Console.WriteLine($"Error: {ex.Message}");  // Para ver el stack trace en el output
        }
    }

    private async void getCursos()
    {
        try
        {
            var response = await _httpClient.GetAsync("cursos");

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
                    CursosPicker.ItemsSource = cursos;

                    if (_id.HasValue)
                    {
                        // Buscamos el curso correspondiente al _cursoId
                        var selectedCurso = cursos.FirstOrDefault(c => c.id == _cursoId);

                        if (selectedCurso != null)
                        {
                            // Establecemos el curso seleccionado en el Picker
                            CursosPicker.SelectedItem = selectedCurso;
                        }
                        else
                        {
                            // Si no se encuentra el curso, podemos hacer algo, por ejemplo, seleccionar el primero
                            CursosPicker.SelectedItem = cursos.FirstOrDefault();
                        }
                    }
                }
            }
            else
            {
                // Error en la obtención de los cursos
                ResultLabel.TextColor = Colors.Red;
                ResultLabel.Text = "Error al obtener los cursos.";
            }
        }
        catch (Exception ex)
        {
            // Capturamos el error aquí
            ResultLabel.TextColor = Colors.Red;
            ResultLabel.Text = $"Error al obtener cursos: {ex.Message}";
            Console.WriteLine($"Error: {ex.Message}");  // Esto imprimirá detalles en el output de la consola
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
                ResultLabel.Text = _id.HasValue ? "Cliente actualizado!" : "Cliente registrado!";
                await Navigation.PopAsync();
            }
            else
            {
                ResultLabel.TextColor = Colors.Red;
                ResultLabel.Text = "Error al registrar el cliente.";
            }
        }
        catch (Exception ex)
        {
            ResultLabel.TextColor = Colors.Red;
            ResultLabel.Text = $"Error: {ex.Message}";
        }
    }
}