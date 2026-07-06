using System.Windows.Controls;
using SistemaResidencial.ViewModels;

namespace SistemaResidencial.Views
{
    public partial class DashboardRecepcionistaView : UserControl
    {
        public DashboardRecepcionistaView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is DashboardRecepcionistaViewModel viewModel)
            {
                viewModel.CargarDatosCommand.Execute(null);
            }
        }
    }
}
