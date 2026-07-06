using System.Windows;

namespace SistemaResidencial.Services
{
    public class DialogService
    {
        public bool MostrarConfirmacion(string mensaje)
        {
            var result = MessageBox.Show(mensaje, "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        public void MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void MostrarInfo(string mensaje)
        {
            MessageBox.Show(mensaje, "Información", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
