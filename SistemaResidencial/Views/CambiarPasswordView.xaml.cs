using System.Windows.Controls;
using System.Windows;
using SistemaResidencial.ViewModels;

namespace SistemaResidencial.Views
{
    public partial class CambiarPasswordView : UserControl
    {
        public CambiarPasswordView()
        {
            InitializeComponent();
        }

        private void PasswordActualBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is CambiarPasswordViewModel vm)
                vm.PasswordActual = PasswordActualBox.Password;
        }

        private void NuevoPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is CambiarPasswordViewModel vm)
                vm.NuevoPassword = NuevoPasswordBox.Password;
        }

        private void ConfirmarPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is CambiarPasswordViewModel vm)
                vm.ConfirmarPassword = ConfirmarPasswordBox.Password;
        }
    }
}
