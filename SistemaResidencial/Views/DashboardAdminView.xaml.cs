using System.Windows.Controls;
using SistemaResidencial.ViewModels;

namespace SistemaResidencial.Views
{
    public partial class DashboardAdminView : UserControl
    {
        public DashboardAdminView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is DashboardAdminViewModel viewModel)
            {
                viewModel.CargarDatosCommand.Execute(null);
            }
        }
    }
}
