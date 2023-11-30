using System.Windows;

namespace WpfApp4.View
{
    /// <summary>
    /// Логика взаимодействия для EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        public EditWindow()
        {
            InitializeComponent();
            DataContext = new FunctionsViewModel();
        }
    }
}
