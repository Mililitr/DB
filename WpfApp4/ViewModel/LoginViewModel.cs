using System.ComponentModel;
using System.Windows.Input;
using System.Windows;
using WpfApp4;
using System.Data.SqlClient;
using System;
using WpfApp4.View;
using System.Collections.ObjectModel;
using System.Runtime.Remoting.Proxies;

namespace WpfApp4
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _password;
        private RegisterViewModel _registerViewModel;

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }


        public ICommand RegisterCommand { get; private set; }
        public ICommand LoginCommand { get; private set; }
        public ICommand ClickCommand { get; private set; }
        public ICommand DarkThemeCommand { get; private set; }
        public ICommand LightThemeCommand { get; private set; }
        public ObservableCollection<string> Styles { get; private set; }

        public LoginViewModel()
        {
            _registerViewModel = new RegisterViewModel();

            RegisterCommand = new RelayCommand(Register, CanRegister);
            LoginCommand = new RelayCommand(Login, CanLogin);
            ClickCommand = new RelayCommand(Switch);
            DarkThemeCommand = new RelayCommand(DarkTheme);
            LightThemeCommand = new RelayCommand(LightTheme);
        }

        private void Register(object parameter)
        {
            // Используем _registerViewModel для выполнения регистрации
            _registerViewModel.RegisterCommand.Execute(null);

            // После регистрации, проверяем, была ли регистрация успешной
            if (_registerViewModel.IsRegistrationSuccessful)
            {
                // Регистрация была успешной, создаем новое окно логина
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.DataContext = new LoginViewModel(); // или установите существующий экземпляр LoginViewModel
                loginWindow.Show();
            }
        }

        private void Login(object parameter)
        {
            try
            {
                string connectionString = "Data Source=dbs.mssql.app.biik.ru;Initial Catalog=s_accounts;Integrated Security=True";
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string loginQuary = "SELECT COUNT(*) FROM users WHERE login = @Username AND password = @Password";
                        SqlCommand loginCommand = new SqlCommand(loginQuary, connection);
                        loginCommand.Parameters.AddWithValue("@Username", Username);
                        loginCommand.Parameters.AddWithValue("@Password", Password);

                        int userCount = (int)loginCommand.ExecuteScalar();

                        if (userCount > 0)
                        {
                            DataWindow DataWindow = new DataWindow();
                            DataWindow.Show();
                            Application.Current.Windows[0].Close();
                        }
                        else
                        {
                            MessageBox.Show("Ошибка входа. Проверьте логин и пароль.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении входа: {ex.Message}");
            }

        }

        private void Switch(object parameter)
        {
            // Закрываем текущее окно логина


            // Открываем окно регистрации
            _registerViewModel.OpenRegisterWindow();
            CloseLoginWindow();
        }

        private bool CanRegister(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }

        private bool CanLogin(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CloseLoginWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();  // Используйте window.Close() вместо window.Hide()
                    break;
                }
            }
        }

        private void DarkTheme(object parameter)
        {
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/Themes/Dark.xaml", UriKind.Relative) });
        }

        private void LightTheme(object parameter)
        {
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/Themes/Light.xaml", UriKind.Relative) });
        }

        private void OpenLoginWindow()
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.DataContext = this; // Установим DataContext для окна логина
            loginWindow.ShowDialog();
        }

    }
}