using System.Windows.Controls;
using SistemaResidencial.ViewModels;

namespace SistemaResidencial.Views
{
    public partial class ContratoView : UserControl
    {
        public ContratoView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is ContratoViewModel viewModel)
            {
                viewModel.CargarDatosCommand.Execute(null);
            }
        }
    }
}
