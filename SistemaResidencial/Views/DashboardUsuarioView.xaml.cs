using System.Windows.Controls;
using SistemaResidencial.ViewModels;

namespace SistemaResidencial.Views
{
    public partial class DashboardUsuarioView : UserControl
    {
        public DashboardUsuarioView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is DashboardUsuarioViewModel viewModel)
            {
                viewModel.CargarDatosCommand.Execute(null);
            }
        }
    }
}
