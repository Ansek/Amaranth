
namespace Amaranth.Model.Data
{
    /// <summary>
    /// Хранит информацию о подключении к БД.
    /// </summary>
    public class ConnectionData : BindableBase
    {
        /// 
        public ConnectionData()
        {
            _server = "localhost";
            _port = 3306;
            _dbName = "amaranth";
            _userName = "root";
        }

        string _server;
        /// <summary>
        /// Адрес источника данных.
        /// </summary>
        public string Server
        {
            get => _server;
            set => SetValue(ref _server, value);
        }

        ushort _port;
        /// <summary>
        /// Порт источника данных.
        /// </summary>
        public ushort Port
        {
            get => _port;
            set => SetValue(ref _port, value);
        }

        string _dbName;
        /// <summary>
        /// Название базы данных.
        /// </summary>
        public string DBName
        {
            get => _dbName;
            set => SetValue(ref _dbName, value);
        }

        string _userName;
        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string UserName
        {
            get => _userName;
            set => SetValue(ref _userName, value);
        }

        string _password;
        /// <summary>
        /// Пароль.
        /// </summary>
        public string Password
        {
            get => _password;
            set => SetValue(ref _password, value);
        }

        /// 
        public override string ToString()
        {
            return $"datasource={Server};port={Port};database={DBName};username={UserName};password={Password}";
        }        
    }
}
