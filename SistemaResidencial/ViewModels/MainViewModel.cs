using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SistemaResidencial.Services;

namespace SistemaResidencial.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly NavigationService _navegacionService;
        private readonly SesionService _sesionService;

        [ObservableProperty]
        private BaseViewModel _vistaActual = null!;

        [ObservableProperty]
        private string _nombreUsuario = string.Empty;

        [ObservableProperty]
        private string _rolUsuario = string.Empty;

        [ObservableProperty]
        private bool _menuExpandido = true;

        [ObservableProperty]
        private bool _menuVisible = false;

        public MainViewModel(NavigationService navegacionService, SesionService sesionService)
        {
            _navegacionService = navegacionService;
            _sesionService = sesionService;

            Titulo = "Sistema Residencial";

            _navegacionService.VistaActualCambiada += OnVistaActualCambiada;
            _sesionService.SesionCambiada += OnSesionCambiada;

            CargarDatosUsuario();
        }

        private void OnVistaActualCambiada(BaseViewModel nuevaVista)
        {
            VistaActual = nuevaVista;
        }

        private void OnSesionCambiada()
        {
            CargarDatosUsuario();
        }

        private void CargarDatosUsuario()
        {
            if (_sesionService.UsuarioActual != null)
            {
                NombreUsuario = _sesionService.UsuarioActual.NombreUsuario;
                RolUsuario = _sesionService.RolActual.ToString();
                MenuVisible = true;
            }
            else
            {
                NombreUsuario = string.Empty;
                RolUsuario = string.Empty;
                MenuVisible = false;
            }
        }

        [RelayCommand]
        private void AlternarMenu()
        {
            MenuExpandido = !MenuExpandido;
        }

        [RelayCommand]
        private void Navegar(string nombreVista)
        {
            _navegacionService.Navegar(nombreVista);
        }

        [RelayCommand]
        private void CerrarSesion()
        {
            _sesionService.CerrarSesion();
            _navegacionService.Navegar("Login");
        }
    }
}
