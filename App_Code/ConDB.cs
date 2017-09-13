using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;
using Npgsql;
public class ConBD : IDisposable
{
    /// <summary>
    /// Имя провайдера
    /// </summary>
    public string ProviderName { get; set; }
    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string UserName { get; set; }
    /// <summary>
    /// Пароль пользователя
    /// </summary>
    public string Password { get; set; }
    /// <summary>
    /// Адрес БД
    /// </summary>
    public string Host { get; set; }
    /// <summary>
    /// Имя БазыДаных
    /// </summary>
    public string DataBase { get; set; }
    /// <summary>
    /// Порт БазыДаных
    /// </summary>
    public int Port { get; set; }
    /// <summary>
    /// наименование схемы БД
    /// </summary>
    public string Schema { get; set; }

    /// <summary>
    /// Подключения к БД
    /// </summary>
    List<NpgsqlConnection> _NpgsqlConnectionList = new List<NpgsqlConnection>();

    List<DbCommand> _DbCommandList = new List<DbCommand>();
    List<DbDataReader> _DbDataReaderList = new List<DbDataReader>();
    List<NpgsqlDataAdapter> _NpgsqlDataAdapterList = new List<NpgsqlDataAdapter>();
    List<DataSet> _DataSet = new List<DataSet>();


    private string PrepareSQL(string sql)
    {
        return System.Text.RegularExpressions.Regex.Replace(sql, "~(.*?)~", Schema + ".$1");
    }

    /// <summary>
    /// инициализация класа
    /// </summary>
    public ConBD(int module)
    {
        ProviderName = ConfigurationManager.AppSettings["ProviderName"];
        DataBase = ConfigurationManager.AppSettings["DataBase"];
        Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
        Host = ConfigurationManager.AppSettings["Host"];
        UserName = ConfigurationManager.AppSettings["UserName" + module.ToString()];
        Password = ConfigurationManager.AppSettings["Password" + module.ToString()];
        Schema = ConfigurationManager.AppSettings["Schema" + module.ToString()];
    }

    /// <summary>
    /// получает строку подключения
    /// </summary>
    /// <returns></returns>
    public string GetConnectionString()
    {
        return String.Format("Server={0};Port={1};Database={2};User Id={3};Password={4}", Host, Port, DataBase, UserName, Password);
    }

    /// <summary>
    /// проверка подключения к БД
    /// </summary>
    /// <returns> получилось ли подключиться к БД </returns>
    public bool Connected()
    {
        NpgsqlConnection _NpgsqlConnection = new NpgsqlConnection(GetConnectionString());
        try
        {
            _NpgsqlConnection.Open();
            _NpgsqlConnection.Close();
            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            _NpgsqlConnection.Dispose();
        }
    }


    public NpgsqlConnection CreateConnection()
    {
        NpgsqlConnection _NpgsqlConnection = new NpgsqlConnection(GetConnectionString());
        try
        {
            _NpgsqlConnection.Open();
            _NpgsqlConnectionList.Add(_NpgsqlConnection);
            return _NpgsqlConnection;
        }
        catch
        {
            _NpgsqlConnection.Dispose();
            return null;
        }
    }

    /// <summary>
    /// Создает комманду к или изменяет запрос БД
    /// </summary>
    /// <param name="Command"> команда</param>
    /// <param name="SQLString"> Sql запрос </param>
    /// <returns>получилось ли создать</returns>
    private bool RepairCommand(DbCommand Command, string SQLString)
    {
        SQLString = PrepareSQL(SQLString);
        if (Command == null)
        {
            Command = new NpgsqlCommand(SQLString);
            return true;
        }
        else
            Command.CommandText = SQLString;
        return true;
    }

    /// <summary>
    /// Создает команду
    /// </summary>
    /// <param name="SQLString"> Sql запрос </param>
    /// <returns>команда БД</returns>
    public DbCommand CreateCommand(string SQLString)
    {
        SQLString = PrepareSQL(SQLString);
        try
        {
            NpgsqlCommand Command = new NpgsqlCommand(SQLString, CreateConnection());
            _DbCommandList.Add(Command);
            return Command;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Добавляет параметр
    /// </summary>
    /// <param name="Command">ссылка на команду</param>
    /// <param name="Param_name"> наименование параметра </param>
    /// <param name="Param_value"> значение параметра </param>
    /// <param name="Type"> тип параметра </param>
    public void AddCommandParam(ref DbCommand Command, string Param_name, object Param_value, DbType Type)
    {
        DbParameter param = Command.CreateParameter();
        param.ParameterName = Param_name;
        param.Value = Param_value;
        param.DbType = Type;
        Command.Parameters.Add(param);
    }


    /// <summary>
    /// Добавляет параметр
    /// </summary>
    /// <param name="Command">ссылка на команду</param>
    /// <param name="Param_name"> наименование параметра </param>
    /// <param name="Param_value"> значение параметра </param>
    /// <param name="Type"> тип параметра </param>
    public void AddCommandParamIfNotNull(ref DbCommand Command, string Param_name, object Param_value, object Param_value_null, DbType Type)
    {
        if (Param_value.ToString() != Param_value_null.ToString())
        {
            AddCommandParam(ref Command, Param_name, Param_value, Type);
        }
        else
        {
            AddCommandParamNull(ref Command, Param_name);
        }

    }

    /// <summary>
    /// Добавляет параметр Null
    /// </summary>
    /// <param name="Command">ссылка на команду</param>
    /// <param name="Param_name"> наименование параметра </param>
    public void AddCommandParamNull(ref DbCommand Command, string Param_name)
    {
        DbParameter param = Command.CreateParameter();
        param.ParameterName = Param_name;
        Command.Parameters.Add(param);
    }

    /// <summary>
    /// Выполняет команду
    /// </summary>
    /// <param name="Command"> команда БД</param>
    /// <param name="SQLString"> Sql запрос </param>
    /// <returns></returns>
    public bool CommandExecute(string SQLString = "")
    {
        SQLString = PrepareSQL(SQLString);
        try
        {
            DbCommand Command = CreateCommand(SQLString);
            Command.ExecuteNonQuery();
            Command.Dispose();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool CommandExecute(DbCommand Command)
    {
        if (Command != null)
        {
            Command.ExecuteNonQuery();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Возвращает датаридер
    /// </summary>
    /// <param name="Command"> комманда БД</param>
    /// <returns> Датаридер </returns>
    public DbDataReader CreateReader(DbCommand Command)
    {
        var reader = Command.ExecuteReader();
        _DbDataReaderList.Add(reader);
        return reader;
    }

    /// <summary>
    /// Возвращает датаридер
    /// </summary>
    /// <param name="SQLString">Sql запрос</param>
    /// <returns>ДатаРидер</returns>
    public DbDataReader CreateReader(string SQLString)
    {
        var Command = CreateCommand(SQLString);
        return CreateReader(Command);
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="Command"></param>
    /// <param name="Param_name"></param>
    /// <param name="Type"></param>
    /// <param name="SourceColumn"></param>
    /// <param name="dataRowVersion"></param>
    public void AddCommandParam(ref DbCommand Command, string Param_name, DbType Type, string SourceColumn, DataRowVersion dataRowVersion)
    {
        DbParameter param = Command.CreateParameter();
        param.ParameterName = Param_name;
        param.DbType = Type;
        param.SourceColumn = SourceColumn;
        param.SourceVersion = dataRowVersion;
        Command.Parameters.Add(param);
    }


    public void AddCommandToAdapter(DbDataAdapter datadapter, DbCommand Command, string action)
    {
        if (action == "update")
        {
            ((NpgsqlDataAdapter)datadapter).UpdateCommand = (NpgsqlCommand)Command;
            return;
        }
        if (action == "insert")
        {
            ((NpgsqlDataAdapter)datadapter).InsertCommand = (NpgsqlCommand)Command;
            return;
        }
    }



    /// <summary>
    /// Возвращает датадаптер
    /// </summary>
    /// <param name="Dataset">датасет, тот что изменится</param>
    /// <param name="SQLString">Sql запрос</param>
    /// <returns>Получилось ли достать датасет</returns>
    public bool FillDataSet(ref DataSet dataSet, string SQLString, string datasetTableName)
    {
        try
        {
            if (dataSet == null) { dataSet = new DataSet(); };
            SQLString = PrepareSQL(SQLString);
            NpgsqlDataAdapter tmpadapter = new NpgsqlDataAdapter(SQLString, GetConnectionString());
            if (datasetTableName.Trim() != "")
            {
                tmpadapter.Fill(dataSet, datasetTableName);
            }
            else
            {
                tmpadapter.Fill(dataSet);
            }
            tmpadapter.Dispose();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool UpdateAdapter(ref DbDataAdapter dbDataAdapter, DataSet dataSet)
    {
        try
        {
            dbDataAdapter.Update(dataSet);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Возвращает датасет
    /// </summary>
    /// <param name="Dataset">датасет, тот что изменится</param>
    /// <param name="SQLString">Sql запрос</param>
    /// <returns>Получилось ли достать датасет</returns>
    public DbDataAdapter CreateDataAdapter(string SQLString)
    {
        try
        {
            SQLString = PrepareSQL(SQLString);
            NpgsqlDataAdapter tmpadapter = new NpgsqlDataAdapter(SQLString, GetConnectionString());
            return tmpadapter;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Возвращает датасет
    /// </summary>
    /// <param name="Dataset">датасет, тот что изменится</param>
    /// <param name="SQLString">Sql запрос</param>
    /// <returns>Получилось ли достать датасет</returns>
    public bool CreateDataSet(DataSet Dataset, string SQLString)
    {
        try
        {
            SQLString = PrepareSQL(SQLString);
            if (Dataset == null)
            {
                Dataset = new DataSet();
            };
            NpgsqlDataAdapter tmpadapter = new NpgsqlDataAdapter(SQLString, GetConnectionString());
            Dataset.Reset();
            tmpadapter.Fill(Dataset);
            tmpadapter.Dispose();
            return true;
        }
        catch
        {
            return false;
        }
    }


    /// <summary>
    /// Освобождаем память
    /// </summary>
    public void Dispose()
    {
        //освобождаем память от Ридеров
        _DbDataReaderList.ForEach(delegate(DbDataReader _DbDataReader)
        {
            if (_DbDataReader != null)
            {
                _DbDataReader.Dispose();
                _DbDataReader = null;
            }
        });

        //освобождаем память от Командов
        _DbCommandList.ForEach(delegate(DbCommand _DbCommand)
        {
            if (_DbCommand != null)
            {
                _DbCommand.Dispose();
                _DbCommand = null;
            }
        });


        //освобождаем память от Адаптеров
        _NpgsqlDataAdapterList.ForEach(delegate(NpgsqlDataAdapter _NpgsqlDataAdapter)
        {
            if (_NpgsqlDataAdapter != null)
            {
                _NpgsqlDataAdapter.Dispose();
                _NpgsqlDataAdapter = null;
            }
        });



        //освобождаем память от Конекшенов
        _NpgsqlConnectionList.ForEach(delegate(NpgsqlConnection _NpgsqlConnection)
        {
            if (_NpgsqlConnection != null)
            {
                _NpgsqlConnection.Close();
                _NpgsqlConnection.Dispose();
                _NpgsqlConnection = null;
            }
        });

    }

}