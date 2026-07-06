using System.Windows;
using SistemaResidencial.ViewModels;

namespace SistemaResidencial
{
    /// <summary>
    /// Ventana principal: contiene el menú lateral y el ContentControl
    /// donde se muestra la vista activa (VistaActual del MainViewModel).
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}