using System.Windows.Controls;
using SistemaResidencial.ViewModels;

namespace SistemaResidencial.Views
{
    public partial class PagoView : UserControl
    {
        public PagoView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is PagoViewModel viewModel)
            {
                viewModel.CargarDatosCommand.Execute(null);
            }
        }
    }
}
