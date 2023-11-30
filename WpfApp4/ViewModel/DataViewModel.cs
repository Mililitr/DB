using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp4.View
{
    public class DataViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<UserModel> _users;
        private DataGrid _dataGrid; // Добавляем свойство DataGrid

        public ObservableCollection<UserModel> Users
        {
            get { return _users; }
            set
            {
                _users = value;
                OnPropertyChanged(nameof(Users));
            }
        }

        public ICommand LoadDataCommand { get; private set; }
        public ICommand DeleteUserCommand { get; private set; }
        public ICommand EditUserCommand { get; private set; }

        public DataViewModel(DataGrid dataGrid) // Принимаем DataGrid в конструкторе
        {
            _dataGrid = dataGrid; // Присваиваем DataGrid

            Users = new ObservableCollection<UserModel>();
            LoadDataCommand = new RelayCommand(LoadData);
            _dataGrid = dataGrid;
            Users = new ObservableCollection<UserModel>();
            LoadDataCommand = new RelayCommand(LoadData);
            DeleteUserCommand = new RelayCommand(DeleteUser);
            EditUserCommand = new RelayCommand(EditUser);

            LoadData(null);
        }

        private void LoadData(object parameter)
        {
            try
            {
                string connectionString = "Data Source=dbs.mssql.app.biik.ru;Initial Catalog=s_accounts;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT id, login, password FROM users"; // Измените запрос на выборку нужных полей
                    SqlCommand command = new SqlCommand(query, connection);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    Users.Clear();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        Users.Add(new UserModel
                        {
                            UserID = Convert.ToInt32(row["id"]),
                            Username = row["login"].ToString(),
                            Password = row["password"].ToString()
                            // Другие свойства UserModel, если они есть
                        });
                    }

                    // Привязываем данные к DataGrid
                    _dataGrid.ItemsSource = Users;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных из БД: {ex.Message}");
            }
        }

        private void DeleteUser(object parameter)
        {
            DeleteWindow DeleteWindow = new DeleteWindow();
            DeleteWindow.Show();
            Application.Current.Windows[0].Close();
        }

        private void EditUser(object parameter)
        {
            EditWindow EditWindow = new EditWindow();
            EditWindow.Show();
            Application.Current.Windows[0].Close();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
