using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using SistemaResidencial.ViewModels;

namespace SistemaResidencial.Services
{
    /// <summary>
    /// Servicio de navegación entre ViewModels.
    /// Soporta navegación por nombre de vista (string) para compatibilidad con los ViewModels.
    /// También expone el evento VistaActualCambiada que escucha MainViewModel.
    /// </summary>
    public class NavigationService
    {
        private readonly IServiceProvider _serviceProvider;

        // Evento que MainViewModel escucha para actualizar VistaActual
        public event Action<BaseViewModel>? VistaActualCambiada;

        // Evento legacy para compatibilidad (alias del anterior)
        public event Action<ObservableObject>? VistaCambiada;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Navega a una vista por su nombre de cadena.
        /// Este es el método que usan todos los ViewModels de Yadfreidel.
        /// </summary>
        public void Navegar(string nombreVista)
        {
            BaseViewModel? viewModel = nombreVista switch
            {
                "Login"                  => _serviceProvider.GetRequiredService<LoginViewModel>(),
                "DashboardAdmin"         => _serviceProvider.GetRequiredService<DashboardAdminViewModel>(),
                "DashboardRecepcionista" => _serviceProvider.GetRequiredService<DashboardRecepcionistaViewModel>(),
                "DashboardUsuario"       => _serviceProvider.GetRequiredService<DashboardUsuarioViewModel>(),
                "Apartamento"            => _serviceProvider.GetRequiredService<ApartamentoViewModel>(),
                "Inquilino"              => _serviceProvider.GetRequiredService<InquilinoViewModel>(),
                "Contrato"               => _serviceProvider.GetRequiredService<ContratoViewModel>(),
                "Pago"                   => _serviceProvider.GetRequiredService<PagoViewModel>(),
                "CambiarPassword"        => _serviceProvider.GetRequiredService<CambiarPasswordViewModel>(),
                _                        => throw new ArgumentException($"Vista no registrada: {nombreVista}")
            };

            VistaActualCambiada?.Invoke(viewModel);
            VistaCambiada?.Invoke(viewModel);
        }

        /// <summary>
        /// Navega por tipo genérico (método original de Raylin, mantenido por compatibilidad).
        /// </summary>
        public void NavegarA<TViewModel>() where TViewModel : BaseViewModel
        {
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
            VistaActualCambiada?.Invoke(viewModel);
            VistaCambiada?.Invoke(viewModel);
        }
    }
}
