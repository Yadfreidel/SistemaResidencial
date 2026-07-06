using CommunityToolkit.Mvvm.ComponentModel;

namespace SistemaResidencial.ViewModels
{
    /// <summary>
    /// ViewModel base del que heredan todos los demás ViewModels.
    /// Usa ObservableObject de CommunityToolkit.Mvvm para notificaciones automáticas de cambio.
    /// </summary>
    public abstract partial class BaseViewModel : ObservableObject
    {
        // Propiedad que indica si el ViewModel está cargando datos (útil para spinners)
        [ObservableProperty]
        private bool _estaCargando;

        // Mensaje de error global que se puede mostrar en la UI
        [ObservableProperty]
        private string _mensajeError = string.Empty;

        // Título de la vista actual
        [ObservableProperty]
        private string _titulo = string.Empty;
    }
}
