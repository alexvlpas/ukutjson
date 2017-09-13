using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Npgsql;

/// <summary>
/// Класс работы с журналом (разработка)
/// </summary>
public class Journal
{
    public string _cs;
    NpgsqlConnection _connection;
	public Journal(string cs)
	{
        _cs = cs;
        ConnectToSQL();
         
	}
    private void ConnectToSQL()
    {
        _connection = new NpgsqlConnection(_cs);
    }
    private void Disconnect()
    {
            _connection.Close();
            _connection.Dispose();
    }
    /// <summary>
    /// Создать запись по параметрам
    /// </summary>
    public string CreateJournalEntry(string kpnum, string author, string message)
    {
        try
        {
            _connection.Open();
             NpgsqlCommand command = new NpgsqlCommand("INSERT INTO Journals (\"Kpnum\", \"author\", \"message\", \"dateStart\") VALUES (@kpnum, @author, @message, @dateStart)", _connection);
             command.Parameters.AddWithValue("@kpnum", Convert.ToInt32(kpnum));
             command.Parameters.AddWithValue("@author", author);
             command.Parameters.AddWithValue("@message", message);
             command.Parameters.AddWithValue("@dateStart", DateTime.Now);
            
             command.ExecuteNonQuery();
            return "success";
            
        }
        catch (Exception ex)
        {
            return ex.Message;
        }

        finally
        {
            Disconnect();
        }
    }
    /// <summary>
    /// Получение журнала по KPNum
    /// </summary>
    public DataTable ShowJournal(string KPNum)
    {
        try
        {
            _connection.Open();
        NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM Journals WHERE \"Kpnum\" = @KPNum", _connection);
        command.Parameters.AddWithValue("@KPNum", Convert.ToInt32(KPNum));
        NpgsqlDataAdapter myDA = new NpgsqlDataAdapter(command);
        DataTable dt = new DataTable();
        myDA.Fill(dt);
        return dt;
        }
        catch(Exception ex)
        {
            DataTable dt = new DataTable();
            return dt;
        }
    }
     /// <summary>
    /// Получение списка укут для таблицы
    /// </summary>
    public DataTable ShowUkutsForJournal()
    {
        try
        {
            _connection.Open();
        NpgsqlCommand command = new NpgsqlCommand("SELECT \"idUKUT\", \"address\", \"idMD\", \"geu\", concat(\"devName\", '№ ',\"SerialNum\") as \"devName\", \"lastConnection\", \"countTrub\", \"last_day_arc\", \"last_hour_arc\", (SELECT concat(\"dateStart\", ': ',\"message\") FROM journals WHERE \"Kpnum\" = ukut.\"idMD\" ORDER BY \"dateStart\" DESC LIMIT 1) AS \"lastEnryJournal\" FROM ukut WHERE \"idMD\" !=0 ORDER by \"address\"", _connection);
        NpgsqlDataAdapter myDA = new NpgsqlDataAdapter(command);
        DataTable dt = new DataTable();
        myDA.Fill(dt);
        return dt;
        }
        catch(Exception ex)
        {
            DataTable dt = new DataTable();
            return dt;
        }
    }
}