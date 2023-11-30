using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace WpfApp4
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _password;
        private bool _isRegistrationSuccessful;

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

        public bool IsRegistrationSuccessful
        {
            get { return _isRegistrationSuccessful; }
            set
            {
                _isRegistrationSuccessful = value;
                OnPropertyChanged(nameof(IsRegistrationSuccessful));
            }
        }

        public ICommand RegisterCommand { get; private set; }

        public RegisterViewModel()
        {
            RegisterCommand = new RelayCommand(Register, CanRegister);
        }

        private void Register(object parameter)
        {
            string connectionString = "Data Source=dbs.mssql.app.biik.ru;Initial Catalog=s_accounts;Integrated Security=True";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string checkUserQuery = $"SELECT COUNT(*) FROM users WHERE login = @Username";
                    SqlCommand checkUserCommand = new SqlCommand(checkUserQuery, connection);
                    checkUserCommand.Parameters.AddWithValue("@Username", Username);
                    int existingUserCount = (int)checkUserCommand.ExecuteScalar();

                    if (existingUserCount > 0)
                    {
                        MessageBox.Show("Пользователь с таким именем уже существует!", "Ошибка", MessageBoxButton.OK);
                        return;
                    }

                    string insertQuery = "INSERT INTO Users (login, password) VALUES (@Username, @Password)";
                    SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                    insertCommand.Parameters.AddWithValue("@Username", Username);
                    insertCommand.Parameters.AddWithValue("@Password", Password);

                    insertCommand.ExecuteNonQuery();

                    MessageBox.Show("Готово! Пользователь зарегистрирован.");
                    LoginWindow LoginWindow = new LoginWindow();
                    LoginWindow.Show();
                    Application.Current.Windows[0].Close();

                    // Устанавливаем флаг успешной регистрации
                    IsRegistrationSuccessful = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK);
            }
        }

        private bool CanRegister(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OpenRegisterWindow()
        {
            try
            {
                RegisterWindow registerWindow = new RegisterWindow();
                registerWindow.Show();
            }
            catch (Exception ex)
            {
               MessageBox.Show($"Ошибка при открытии окна регистрации: {ex.Message}");
                // Или используйте другой механизм логирования
            }
        }
    }
}
