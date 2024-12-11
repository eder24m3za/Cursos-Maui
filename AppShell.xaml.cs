using Cursos.Views;

namespace Cursos
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            this.Navigated += OnNavigated;
        }

        private void OnNavigated(object sender, ShellNavigatedEventArgs e)
        {
            // Llamamos a la función para ajustar la visibilidad del botón de logout
            SetLogoutButtonVisibility();
        }

        private void OnNavigating(object sender, ShellNavigatingEventArgs e)
        {
            SetLogoutButtonVisibility();
        }

        private void SetLogoutButtonVisibility()
        {
            var currentPage = Shell.Current.CurrentPage;

            if (currentPage is MainPage || currentPage is Register)
            {
                LogoutButton.IsVisible = false;
            }
            else
            {
                LogoutButton.IsVisible = true;
            }
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            // Lógica de cerrar sesión
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Cerrar Sesión",
                "¿Estás seguro de que deseas cerrar sesión?",
                "Sí",
                "No");

            if (confirm)
            {
                // Redirigir al usuario a la página de inicio de sesión
                Preferences.Remove("IsLoggedIn");
                await Navigation.PushAsync(new MainPage());
            }
        }
    }
}
