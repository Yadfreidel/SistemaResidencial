using System.Windows.Controls;
using Microsoft.Win32;
using SistemaResidencial.ViewModels;

namespace SistemaResidencial.Views
{
    public partial class InquilinoView : UserControl
    {
        public InquilinoView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is InquilinoViewModel viewModel)
            {
                viewModel.CargarDatosCommand.Execute(null);
            }
        }

        private void SeleccionarFoto_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is not InquilinoViewModel viewModel) return;

            var dialogo = new OpenFileDialog
            {
                Filter = "Imágenes (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"
            };

            if (dialogo.ShowDialog() == true)
            {
                viewModel.FotoRuta = dialogo.FileName;
            }
        }
    }
}
