using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

    /// <summary>
    /// Класс работы с тикетами (разработка)
    /// </summary>
public class Tickets
{
    public string _cs;
    MySqlConnection _connection;
	public Tickets(string cs)
	{
        _cs = cs;
        ConnectToSQL();
         
	}
    private void ConnectToSQL()
    {
        _connection = new MySqlConnection(_cs);
    }
    private void Disconnect()
    {
            _connection.Close();
            _connection.Dispose();
    }
    /// <summary>
    /// Создать тикет по параметрам
    /// </summary>
    public int CreateTicket(string address, string kpnum, string author, string message)
    {
        try
        {
            _connection.Open();
             MySqlCommand command = new MySqlCommand(@"INSERT INTO tickets (address, kpnum, author, message, status, dateStart) VALUES (@address, @kpnum, @author, @message, @status, @dateStart)", _connection);
             command.Prepare();
             command.Parameters.AddWithValue("@address", address);
             command.Parameters.AddWithValue("@kpnum", kpnum);
             command.Parameters.AddWithValue("@author", author);
             command.Parameters.AddWithValue("@message", message);
             command.Parameters.AddWithValue("@status", "Зарегистрирована");
             command.Parameters.AddWithValue("@dateStart", DateTime.Now);
            
            return command.ExecuteNonQuery();
            
        }
        catch (Exception ex)
        {
            return 0;
        }

        finally
        {
            Disconnect();
        }
    }
    /// <summary>
    /// Получение списка тикетов по idUkut
    /// </summary>
    public DataTable ShowTickets(string KPNum)
    {
        try
        {
            _connection.Open();
            MySqlCommand command = new MySqlCommand(@"SELECT * FROM tickets WHERE Kpnum = @KPNum", _connection);
        command.Parameters.AddWithValue("@KPNum", KPNum);
        command.Prepare();
        MySqlDataAdapter myDA = new MySqlDataAdapter(command);
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