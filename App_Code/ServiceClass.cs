using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Collections;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Data;
using System.Threading.Tasks;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using System.Net;
using System.Linq;
using OfficeOpenXml;
using System.Configuration;
using System.Text;
using System.IO;
using Scada.Client;
using Scada.Data.Tables;
using Scada.Data.Models;
using Scada.Data;
using Scada.Comm;
using System.Threading;
using System.Collections.Specialized;
using System.Security.Cryptography.X509Certificates;
//using Scada.Web;
using WebSocketSharp;
using Calabonga.XmlRpc;
using System.Xml;
using xNet;
using DocumentFormat.OpenXml.Presentation;
using System.Text.RegularExpressions;
using AngleSharp.Parser.Html;
using System.Net.Http;
using AngleSharp.Dom.Html;
using AngleSharp;
using Newtonsoft.Json.Linq;
using CSRedis;
using Newtonsoft.Json;
using Utils;
using Npgsql;

[WebService(Namespace = "http://ukut.teplo96.ru")]
[System.ComponentModel.ToolboxItem(false)]
[ScriptService]
/// <summary>
/// Сводное описание для ServiceClass
/// </summary>
public class UkutAPI : System.Web.Services.WebService
{
    public string cs = "Server=10.235.63.252;Port=5432;User Id=postgres;Password=ghbdtn;Database=ukut_maps";
    public string _cs = "Server=10.235.63.252;Port=5432;User Id=postgres;Password=ghbdtn;Database=ukut_maps";
    MySqlConnection connection;
    NpgsqlConnection connection2;
    List<Ukut> objects_list = new List<Ukut>();
    List<Ukut> obj_list = new List<Ukut>(); // создание списка
    DataTable CurSrezTable = new DataTable();
    DataTable InCnl = new DataTable();
    DataTable UnitTable = new DataTable();
    private CookieCollection loginCookies;
    private string answer;
    private IWebProxy proxy;
    
 class NoCheckCertificatePolicy : ICertificatePolicy
 {
  public bool CheckValidationResult (ServicePoint srvPoint, X509Certificate certificate, WebRequest request, int certificateProblem)
  {
   return true;
  }
 }
 public class CookieAwareWebClient : WebClient
{
    public CookieAwareWebClient()
    {
        CookieContainer = new CookieContainer();
        this.ResponseCookies = new CookieCollection();
    }

    public CookieContainer CookieContainer { get; private set; }
    public CookieCollection ResponseCookies { get; set; }

    protected override WebRequest GetWebRequest(Uri address)
    {
        var request = (HttpWebRequest)base.GetWebRequest(address);
        request.CookieContainer = CookieContainer;
        return request;
    }

    protected override WebResponse GetWebResponse(WebRequest request)
    {
        var response = (HttpWebResponse)base.GetWebResponse(request);
        this.ResponseCookies = response.Cookies;
        return response;
    }
}

    /// <summary>
    /// Получить температуру наружного воздуха
    /// </summary>
    [WebMethod(Description = "Получение t наружного воздуха")]
[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
public void GetTnvWeb()
{ 
    TemperatureNV tnv = new TemperatureNV();
    Channel cnl = new Channel();
    cnl.channel = 470;
    cnl.channel_name = "Температура наружного воздуха";
    cnl.channel_value = tnv.tnv;
    this.Context.Response.ContentType = "application/json; charset=utf-8";
    this.Context.Response.Write("["+new JavaScriptSerializer().Serialize(cnl)+"]");
}
/// <summary>
/// Получить Архивы (номер КП, тип архива, дата от, дата до)
/// </summary>
//[WebMethod(Description = "Получение архива по idMD, типу архива, дата начала периода, дата конца периода")]
//[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
//public void GetArchive(int idMD, string arc, string dateFirst, string dateLast)
//{
   
//    int idDevName = 0;
//    //
//    string day_table = "daydata";
//    string hour_table = "hourdata";
//    string month_table = "monthdata";
//    MySqlConnection connection = new MySqlConnection(cs);
//    try
//    {
//        connection.Open();
//        MySqlCommand command = new MySqlCommand("SELECT * FROM ukut WHERE idMD=@idMD", connection);
//        command.Prepare();
//        command.Parameters.AddWithValue("@idMD", idMD);
//        MySqlDataReader reader = command.ExecuteReader();
//        while (reader.Read())
//        {
//            idDevName = Convert.ToInt32(reader["DevType"]);
//            if (idDevName == 2)
//            {
//                day_table = "daydata942";
//                hour_table = "hourdata942";

//            }
//           if (idDevName == 3)
//            {
//                day_table = "daydata941";
//                hour_table = "hourdata941";

//            }
//            if (idDevName == 4)
//            {
//                day_table = "daydata94310";
//                hour_table = "hourdata94310";

//            }
//           if (idDevName == 5)
//            {
//                day_table = "daydata944";
//                hour_table = "hourdata944";

//            }
//          if (idDevName == 6)
//            {
//                day_table = "daydata94120";
//                hour_table = "hourdata94120";

//            }
//        }
//        connection.Close();
//        string sQuery = "";
//        if (arc == "days")
//        {
//            sQuery = "Select * from " + day_table + " WHERE KPNum = " + idMD + " AND Date between " + dateFirst + " and " + dateLast;
//        }
//        if (arc == "hour")
//        {
//            sQuery = "Select * from " + hour_table + " WHERE KPNum = " + idMD + " AND Date between " + dateFirst + " and " + dateLast;
//        }
//        if (arc == "month")
//        {
//            sQuery = "Select * from " + month_table + " WHERE KPNum = " + idMD + " AND Date between " + dateFirst + " and " + dateLast;
//        }

//        MySqlDataAdapter myDA = new MySqlDataAdapter(sQuery, connection);
//        DataTable dt = new DataTable();
//        myDA.Fill(dt);
//        connection.Close();
//        connection.Dispose();
//        this.Context.Response.ContentType = "application/json; charset=utf-8";
//        this.Context.Response.Write(ConvertDataTabletoString(dt));
//        //string JSONresult;
//        // JSONresult = JsonConvert.SerializeObject(dt);
//        //this.Context.Response.Write(JSONresult);
//    }
//    catch(Exception ex)
//    {
//        this.Context.Response.ContentType = "application/json; charset=utf-8";
//        this.Context.Response.StatusCode = (int)System.Net.HttpStatusCode.ServiceUnavailable;
//        this.Context.Response.Write(new JavaScriptSerializer().Serialize(ex));
//    }
//}
[WebMethod(Description = "Получение архива по idMD, типу архива, дата начала периода, дата конца периода")]
[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
public void GetArchive(int idMD, string arc, string dateFirst, string dateLast)
{
   
    int idDevName = 0;
    //
    string day_table = "daydata";
    string hour_table = "hourdata";
    string month_table = "monthdata";
    NpgsqlConnection connection = new NpgsqlConnection(_cs);
    try
    {
        connection.Open();
        NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM ukut WHERE \"idMD\"=@idMD", connection);
        //command.Prepare();
        command.Parameters.AddWithValue("@idMD", idMD);
        NpgsqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            idDevName = Convert.ToInt32(reader["DevType"]);
            if (idDevName == 2)
            {
                day_table = "daydata942";
                hour_table = "hourdata942";

            }
           if (idDevName == 3)
            {
                day_table = "daydata941";
                hour_table = "hourdata941";

            }
            if (idDevName == 4)
            {
                day_table = "daydata94310";
                hour_table = "hourdata94310";

            }
           if (idDevName == 5)
            {
                day_table = "daydata944";
                hour_table = "hourdata944";

            }
          if (idDevName == 6)
            {
                day_table = "daydata94120";
                hour_table = "hourdata94120";

            }
        }
        connection.Close();
        string sQuery = "";
        if (arc == "days")
        {
            sQuery = "Select * from " + day_table + " WHERE \"KPNum\" = " + idMD + " AND \"Date\" between '" + dateFirst + "'::date and '" + dateLast+"'::date ORDER BY \"Date\" ASC";
        }
        if (arc == "hour")
        {
            sQuery = "Select * from " + hour_table + " WHERE \"KPNum\" = " + idMD + " AND \"Date\" between '" + dateFirst + "'::date and '" + dateLast+"'::date ORDER BY \"Date\" ASC";
        }
        if (arc == "month")
        {
            sQuery = "Select * from " + month_table + " WHERE \"KPNum\" = " + idMD + " AND \"Date\" between '" + dateFirst + "'::date and '" + dateLast+"'::date ORDER BY \"Date\" ASC";
        }
        
        NpgsqlDataAdapter myDA = new NpgsqlDataAdapter(sQuery, connection);
        DataTable dt = new DataTable();
        myDA.Fill(dt);
        connection.Close();
        connection.Dispose();
        this.Context.Response.ContentType = "application/json; charset=utf-8";
        this.Context.Response.Write(ConvertDataTabletoString(dt));
        //string JSONresult;
        // JSONresult = JsonConvert.SerializeObject(dt);
        //this.Context.Response.Write(JSONresult);
    }
    catch(Exception ex)
    {
        this.Context.Response.ContentType = "application/json; charset=utf-8";
        this.Context.Response.StatusCode = (int)System.Net.HttpStatusCode.ServiceUnavailable;
        this.Context.Response.Write(new JavaScriptSerializer().Serialize(ex.Message));
    }
}
//public void GetArchive(int idMD, string arc, string dateFirst, string dateLast, string json)
//{
//    int idDevName = 0;
//    //
//    string day_table = "daydata";
//    string hour_table = "hourdata";
//    string month_table = "monthdata";
//    MySqlConnection connection = new MySqlConnection(cs);
//    try
//    {
//        connection.Open();
//        MySqlCommand command = new MySqlCommand("SELECT * FROM ukut WHERE idMD=@idMD", connection);
//        command.Prepare();
//        command.Parameters.AddWithValue("@idMD", idMD);
//        MySqlDataReader reader = command.ExecuteReader();
//        while (reader.Read())
//        {
//            idDevName = Convert.ToInt32(reader["DevType"]);
//            if (idDevName == 2)
//            {
//                day_table = "daydata942";
//                hour_table = "hourdata942";

//            }
//           if (idDevName == 3)
//            {
//                day_table = "daydata941";
//                hour_table = "hourdata941";

//            }
//            if (idDevName == 4)
//            {
//                day_table = "daydata94310";
//                hour_table = "hourdata94310";

//            }
//           if (idDevName == 5)
//            {
//                day_table = "daydata944";
//                hour_table = "hourdata944";

//            }
//          if (idDevName == 6)
//            {
//                day_table = "daydata94120";
//                hour_table = "hourdata94120";

//            }
//        }
//        connection.Close();
//        string sQuery = "";
//        if (arc == "days")
//        {
//            sQuery = "Select * from " + day_table + " WHERE KPNum = " + idMD + " AND Date between " + dateFirst + " and " + dateLast;
//        }
//        if (arc == "hour")
//        {
//            sQuery = "Select * from " + hour_table + " WHERE KPNum = " + idMD + " AND Date between " + dateFirst + " and " + dateLast;
//        }
//        if (arc == "month")
//        {
//            sQuery = "Select * from " + month_table + " WHERE KPNum = " + idMD + " AND Date between " + dateFirst + " and " + dateLast;
//        }

//        MySqlDataAdapter myDA = new MySqlDataAdapter(sQuery, connection);
//        DataTable dt = new DataTable();
//        myDA.Fill(dt);
//        connection.Close();
//        connection.Dispose();

//    }
//    catch(Exception ex)
//    {
//        this.Context.Response.ContentType = "application/json; charset=utf-8";
//        this.Context.Response.StatusCode = (int)System.Net.HttpStatusCode.ServiceUnavailable;
//        this.Context.Response.Write(new JavaScriptSerializer().Serialize(ex));
//    }
//}
public string GetRedisDateTime(string KPNum)
{
    string result;
 using (var redis = new RedisClient("10.235.63.252"))
    {

    // blocking call

    result = redis.Get(KPNum);
            if (String.IsNullOrEmpty(result))
            {
                result = DateTime.MinValue.ToString();
            }

    
    }
    return result;
}
/// <summary>
/// Серилизация  DataTable в строку
/// </summary>
public string ConvertDataTabletoString(DataTable dt)
{
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);
}
/// <summary>
/// Получить список свойств каналов(имя, описание, etc) с RScada и записать их в dataTable (в память)
/// </summary>
public  void ReceiveInCnlTable()
{
    Log.WriteLineDelegate writeToLog;
    writeToLog = text => { }; // заглушка
    CommSettings cm = new CommSettings("10.235.63.252",10000,"ScadaApi","ScadaApi",10000);
    ServerComm sc = new ServerComm(cm, writeToLog);
    sc.ReceiveBaseTable("incnl.dat", InCnl);    
    sc.Close();
    InCnl.DefaultView.Sort = "[ParamID] ASC";
    InCnl = InCnl.DefaultView.ToTable();
    
  
}
 public void ReceiveInCnlTable(bool PG)
{
     string _cs = "Server=10.235.63.252;Port=5432;User Id=postgres;Password=ghbdtn;Database=ukut_maps";
     NpgsqlDataAdapter DA = new NpgsqlDataAdapter("SELECT * FROM incnls", _cs);
     DA.Fill(InCnl);
     NpgsqlDataAdapter DA1 = new NpgsqlDataAdapter("SELECT * FROM unit", _cs);
     DA1.Fill(UnitTable);
     
}
/// <summary>
/// Получить список форматированных каналов с RScada и записать их в dataTable (в память)
/// </summary>
public void ReceiveSrezTable()
{
    CurSrezTable.Columns.Add("last_update", typeof(DateTime));
    CurSrezTable.Columns.Add("idUkut", typeof(int));
    CurSrezTable.Columns.Add("channel", typeof(int));
    CurSrezTable.Columns.Add("channel_name", typeof(string));
    CurSrezTable.Columns.Add("channel_value", typeof(string));
    CurSrezTable.Columns.Add("channel_color", typeof(string));
        Log.WriteLineDelegate writeToLog;
        writeToLog = text1 => { }; // заглушка
        CommSettings cm = new CommSettings("10.235.63.252",10000,"ScadaApi","ScadaApi",10000);
    ServerComm sc = new ServerComm(cm, writeToLog);
    Utils.Log log = new Utils.Log(Utils.Log.Formats.Simple);
    DataCache dc = new DataCache(sc, log);
    DataAccess da = new DataAccess(dc, log);
    DataFormatter dataFormatter = new DataFormatter();

    SrezTableLight snapshotTable = new SrezTableLight();
    bool dataReceived = sc.ReceiveSrezTable("current.dat", snapshotTable);
    string text; // без размерности
    string textWithUnit; //включая размерность
    if (dataReceived)
    {
        DateTime lastChanges = snapshotTable.LastFillTime;
           foreach (SrezTableLight.Srez snapshot in snapshotTable.SrezList.Values)
           {
               int cnlCnt = snapshot.CnlNums.Length;
               string color;
               for (int i = 0; i < cnlCnt; i++)
               {
                  
                   DataRow rowChannel = CurSrezTable.NewRow();
                   rowChannel["last_update"] =  lastChanges;
                   rowChannel["channel"] = snapshot.CnlNums[i];
                   dataFormatter.FormatCnlVal(da.GetCurCnlData(snapshot.CnlNums[i]).Val, da.GetCurCnlData(snapshot.CnlNums[i]).Stat,da.GetCnlProps(snapshot.CnlNums[i]), out text, out textWithUnit);

                  rowChannel["channel_color"]  =  dataFormatter.GetCnlValColor(da.GetCurCnlData(snapshot.CnlNums[i]).Val,
                       da.GetCurCnlData(snapshot.CnlNums[i]).Stat,
                       da.GetCnlProps(snapshot.CnlNums[i]),
                       da.GetCnlStatProps(da.GetCurCnlData(snapshot.CnlNums[i]).Stat));

                  rowChannel["channel_value"] = textWithUnit;
                   rowChannel["channel_name"] = da.GetCnlProps(snapshot.CnlNums[i]).CnlName.ToString();
                   rowChannel["idUkut"] = i;
                   CurSrezTable.Rows.Add(rowChannel);
               }
           }
    }
    else
    {
       
    }
    sc.Close();
}
 public void ReceiveSrezTable(bool PG)
{
    CurSrezTable.Columns.Add("last_update", typeof(DateTime));
    CurSrezTable.Columns.Add("idUkut", typeof(int));
    CurSrezTable.Columns.Add("channel", typeof(int));
    CurSrezTable.Columns.Add("channel_name", typeof(string));
    CurSrezTable.Columns.Add("channel_value", typeof(string));
    CurSrezTable.Columns.Add("channel_color", typeof(string));

     foreach(DataRow row in InCnl.Rows)
     {
         try
         {
             DataRow[] resultUnit = UnitTable.Select("unitid=" + row["UnitID"]);
             DataRow newRow = CurSrezTable.NewRow();
             newRow[0] = (row["datetime"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["datetime"]);
             newRow[1] = (row["KPNum"] is DBNull) ? 0 : Convert.ToUInt32(row["KPNum"]);
             newRow[2] = (row["cnlnum"] is DBNull) ? 0 : Convert.ToUInt32(row["cnlnum"]);
             newRow[3] = row["name"].ToString();
             uint format = (row["FormatID"] is DBNull) ? 0 : Convert.ToUInt32(row["FormatID"]);
             if (!(row["value"] is DBNull))
             {
                 if (format == 0)
                 {
                     newRow[4] = ((row["value"] is DBNull) ? 0 : Math.Round(Convert.ToDouble(row["value"]), 0)) + " " + resultUnit[0]["Name"];
                 }
                 if (format == 2)
                 {
                     newRow[4] = ((row["value"] is DBNull) ? 0 : Math.Round(Convert.ToDouble(row["value"]), 2)) + " " + resultUnit[0]["Name"];
                 }
             }
             else
             {
                 if (String.IsNullOrEmpty(resultUnit[0]["Name"].ToString()))
                 {
                     newRow[4] = 0 +"  ";
                 }
                 else
                 {
                     newRow[4] = 0 + " " + resultUnit[0]["Name"];
                 }
             }
             newRow[5] = "black";
             CurSrezTable.Rows.Add(newRow);
         }
         catch(Exception ex)
         {

         }
     }

}
/// <summary>
/// Метод для отгрузки данных для таблицы неисправностей (используем таблицу InTable и Current Table) 
/// </summary>
[WebMethod(Description = "Получение таблицы неисправностей")]
[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
public void GetFaultTable()
{
    
    ReceiveSrezTable();
    ReceiveInCnlTable(true);
    DataTable FaultTable = CurSrezTable.Clone();
    

    foreach (DataRow CurSrezRow in CurSrezTable.Rows)
    {
        DataRow[] valueChannel = InCnl.Select("CnlNum=" + CurSrezRow.Field<int>("channel"));
        CurSrezRow["idUkut"] = valueChannel[0].Field<int>("KPNum");
    }
    DataRow[] Fault = CurSrezTable.Select("channel_name like '%Давление%'");
    foreach(DataRow dr in Fault)
    {
        FaultTable.ImportRow(dr);
    }
    List<object> allValues = FaultTable.AsEnumerable()
    .SelectMany(r => r.Field<string>(4).Split(' '))
    .Select(str => (object)str)
    .ToList();
    CurSrezTable.Dispose();
    this.Context.Response.ContentType = "application/json; charset=utf-8";
    this.Context.Response.Write(new JavaScriptSerializer().Serialize(ConvertDataTabletoString(FaultTable)));
    
}
    /// <summary>
/// Метод для отгрузки данных для таблицы неисправностей (используем таблицу InTable и Current Table) 
/// </summary>
[WebMethod(Description = "Получение таблицы конфигурации")]
[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
public void GetKPTable()
{
    DataTable KPTable = new DataTable();
    DataTable dt = new DataTable();
    KPTable.Columns.Add("id", typeof(Int32));
    KPTable.Columns.Add("value", typeof(String));
        Log.WriteLineDelegate writeToLog;
        writeToLog = text => { }; // заглушка
        CommSettings cm = new CommSettings("10.235.63.252",10000,"ScadaApi","ScadaApi",10000);
        ServerComm sc = new ServerComm(cm, writeToLog);

    sc.ReceiveBaseTable("kp.dat", dt);
    sc.Close();
    dt.DefaultView.Sort = "[Name] ASC";
    dt = dt.DefaultView.ToTable();

    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
    Dictionary<string, object> row;
        for (int i = 0; i < dt.Rows.Count; i++)
            {
            row = new Dictionary<string, object>();
            row.Add("id",dt.Rows[i].Field<Int32>("KPNum"));
            row.Add("value",dt.Rows[i].Field<String>("Name"));
            rows.Add(row);
            }
        
    this.Context.Response.ContentType = "application/json; charset=utf-8";
    this.Context.Response.Write(new JavaScriptSerializer().Serialize(rows));
    
}
/// <summary>
/// Метод для отгрузки данных для таблицы неисправностей (используем таблицу InTable и Current Table) 
/// </summary>
[WebMethod(Description = "Вставка записи в журнал")]
[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
public void InsertJournalEntry(string KPNum, string text, string author)
{
    Journal JJ = new Journal(cs);

    string res = JJ.CreateJournalEntry(KPNum, author, text);
    this.Context.Response.ContentType = "application/json; charset=utf-8";
    this.Context.Response.Write(res);
    
}
        /// <summary>
/// Метод для отгрузки данных для таблицы неисправностей (используем таблицу InTable и Current Table) 
/// </summary>
[WebMethod(Description = "Записи для объекта")]
[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
public void ShowJournalEntrys(string KPNum)
{
    Journal JJ = new Journal(cs);

       DataTable dt =  JJ.ShowJournal(KPNum);
    this.Context.Response.ContentType = "application/json; charset=utf-8";
    this.Context.Response.Write(ConvertDataTabletoString(dt));
    
}
        /// <summary>
/// Метод для отгрузки данных для таблицы неисправностей (используем таблицу InTable и Current Table) 
/// </summary>
[WebMethod(Description = "Таблица объектов")]
[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
public void GetListForJournals()
{
    Journal JJ = new Journal(cs);

    DataTable dt =  JJ.ShowUkutsForJournal();
    this.Context.Response.ContentType = "application/json; charset=utf-8";
    this.Context.Response.Write(ConvertDataTabletoString(dt));
    
}
/// <summary>
/// Метод для отгрузки данных для карты или таблицы (параметр res_mode)
/// </summary>
//[WebMethod(Description = "Основной метод получения данных для отрисовки геообъектов")]
//[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
//public void UkutsMaps(string type, int temp_mode, int peret_mode, string res_mode = "maps", bool PG)
//{
//    connection = new MySqlConnection(cs);
//    connection2 = new NpgsqlConnection(_cs);
//    try
//    {
//        connection.Open();
//        MySqlCommand command = new MySqlCommand("SELECT * FROM ukut WHERE idMD !=0", connection);
//        MySqlDataReader reader = command.ExecuteReader();
//        Ukut ukut_one = new Ukut();
//        while (reader.Read())
//        {
//            ukut_one.lat = Convert.ToString(reader["lon"]);
//            ukut_one.lon = Convert.ToString(reader["lat"]);
//           // ukut_one.date = GetRedisDateTime(Convert.ToString(reader["idMD"]));
//            ukut_one.devName = Convert.ToString(reader["devName"]) + " № " + Convert.ToString(reader["serialNum"]);
//            ukut_one.address = Convert.ToString(reader["address"]);
//            ukut_one.geu = (reader["geu"] != DBNull.Value) ? Convert.ToString(reader["geu"]): "0";
//            ukut_one.idUkut = Convert.ToInt32(reader["idUkut"]);
//            ukut_one.countTrub = Convert.ToString(reader["countTrub"]);
//            ukut_one.idMD = Convert.ToString(reader["idMD"]);
//            ukut_one.last_day_arc  = Convert.ToString(reader["last_day_arc"]);
//            ukut_one.last_hour_arc = Convert.ToString(reader["last_hour_arc"]);
//            if (Convert.ToInt32(reader["system"]) == 0)
//            {
//                ukut_one.system = "Открытая";
//            }
//            else
//            {
//                ukut_one.system = "Закрытая";
//            }
//            ukut_one.teplo_source = Convert.ToString(reader["teplo_source"]);
//            ukut_one.HID = (reader["HID"] != DBNull.Value) ? Convert.ToInt32(reader["HID"]): 0;
//            ukut_one.idDevType = Convert.ToInt32(reader["devType"]);
//            //Добавим в коллекцию   
//            obj_list.Add(ukut_one);
//        }
//        reader.Close();
//        //Получаем текущий срез от RS (Внимание!) Срез включает в себя, ВСЕ каналы
        
//        ReceiveInCnlTable(true);
//        ReceiveSrezTable(true);
//        Ukut ee = new Ukut();
//        TemperatureNV NV = new TemperatureNV();
//        //Температура наружного воздуха
//        int tnv_int = Convert.ToInt32(NV.tnv);
//        //подключаемся к АДС 
//        //SqlConnection ads = ConnectToAds();
//        //ads.Open();
//        //Проходим по коллекции
//        //foreach (Ukut ob in obj_list)
//        try
//        {
      
//           // Parallel.ForEach(obj_list, (ob) =>
//            // int i = 0;   
//            //obj_list.AsParallel().AsOrdered().ForAll(ob =>
//            foreach (Ukut ob in obj_list)
//            {
//                ee = ob;
          
//                ee.channels = GetChannelFromKP(ob.idUkut, ob.idMD, ref ee);
//               // ee.otkl = GetOtklsAds(ads, ob.HID,"maps");
//                //Итог в коллекцию
//                objects_list.Add(ee);
              
//            }
//            //);
//        }
//        catch(Exception ex)
//        {
//            string s = ex.StackTrace;


//        }

//        foreach (Ukut ob in objects_list.ToArray())
//        {
//            ee = ob;
//            int t_pod_int = 0;

//            if (!String.IsNullOrEmpty(ee.t_pod))
//            {
//                string[] t_pod = ee.t_pod.Split(' ');

//                Int32.TryParse(t_pod[0], out t_pod_int);
//            }
//            if (peret_mode == 1)
//            {
//                if(ob.idDevType != 3 || ob.teplo_source !="Водоканал")
//                {
//                    ee.temp_graph = GetTempGraphPodTrub(ob.teplo_source, 0, t_pod_int);
//                }
//            }
//            else
//            {
//                if (ob.idDevType != 3 || ob.teplo_source !="Водоканал")
//                {
//                    ee.temp_graph = GetTempGraphPodTrub(ob.teplo_source, tnv_int, 0);
//                }
//            }

//            objects_list.Remove(ob);
//            //ob = ee;
//            if (String.IsNullOrEmpty(ee.param_value))
//            {
//              ee.param_value = ee.type_obr.ToString();
//            }
//            objects_list.Add(ee);
//        }
        
//        //Закрывает подключение к АДС
//         //ads.Close();
//        //Итоговый результат
//        string RESULT_JSON = SerializeOfString(temp_mode, type);

//        if (res_mode == "maps")
//        {
//            RESULT_JSON = RESULT_JSON.Remove(RESULT_JSON.Length - 1);
//            RESULT_JSON = RESULT_JSON + ']';
//            this.Context.Response.ContentType = "application/json; charset=utf-8";
//            this.Context.Response.Write(RESULT_JSON);
//        }
//        else
//        {
//            this.Context.Response.ContentType = "application/json; charset=utf-8";
//            this.Context.Response.Write(new JavaScriptSerializer().Serialize(objects_list));
//        }
//        GC.Collect();
//        //Закрываем подключение к MySQL базе
//        connection.Close();
//        connection.Dispose();
//    }
//    catch ( Exception ex )
//    {
//          this.Context.Response.ContentType = "application/json; charset=utf-8";
//          this.Context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
//          this.Context.Response.Write("{\"success\": false, \"message\":\""+ex.StackTrace+ "\"}");
        
//    }
//}
    /// <summary>
/// Метод для отгрузки данных для карты или таблицы (параметр res_mode)
/// </summary>
[WebMethod(Description = "Основной метод получения данных для отрисовки геообъектов")]
[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
public void UkutsMaps(string type, int temp_mode, int peret_mode, string res_mode = "maps")
{
    connection2 = new NpgsqlConnection(_cs);
    try
    {
            string last_day_arc = null;
            string last_hour_arc = null;
        ReceiveInCnlTable(true);
        ReceiveSrezTable(true);
        connection2.Open();
        NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM ukut WHERE \"idMD\" !=0  ORDER by address", connection2);
        NpgsqlDataReader reader = command.ExecuteReader();
        Ukut ukut_one = new Ukut();
        while (reader.Read())
        {
            ukut_one.lat = Convert.ToString(reader["lon"]);
            ukut_one.lon = Convert.ToString(reader["lat"]);
               // ukut_one.date = GetRedisDateTime(reader["idMD"].ToString());

                ukut_one.date = reader["lastConnection"].ToString();
                ukut_one.devName = Convert.ToString(reader["devName"]) + " № " + Convert.ToString(reader["serialNum"]);
            ukut_one.address = Convert.ToString(reader["address"]);
            ukut_one.geu = (reader["geu"] != DBNull.Value) ? Convert.ToString(reader["geu"]): "0";
            ukut_one.idUkut = Convert.ToInt32(reader["idUkut"]);
            ukut_one.countTrub = Convert.ToString(reader["countTrub"]);
            ukut_one.idMD = Convert.ToString(reader["idMD"]);
                if(!String.IsNullOrEmpty(reader["last_day_arc"].ToString()))
                {
                    //last_day_arc = DateTime.Parse(Convert.ToString(reader["last_day_arc"])).ToString("yyyy-MM-dd HH:mm:ss");
                    last_day_arc = reader["last_day_arc"].ToString();
                }
                if (!String.IsNullOrEmpty(reader["last_hour_arc"].ToString()))
                {
                    //last_hour_arc = DateTime.Parse(Convert.ToString(reader["last_hour_arc"])).ToString("yyyy-MM-dd HH:mm:ss");
                    last_hour_arc = reader["last_hour_arc"].ToString();
                }

                ukut_one.last_day_arc = last_day_arc;
                ukut_one.last_hour_arc = last_hour_arc;
            if (Convert.ToInt32(reader["system"]) == 0)
            {
                ukut_one.system = "Открытая";
            }
            else
            {
                ukut_one.system = "Закрытая";
            }
            ukut_one.teplo_source = Convert.ToString(reader["teplo_source"]);
            ukut_one.HID = (reader["HID"] != DBNull.Value) ? Convert.ToInt32(reader["HID"]): 0;
            ukut_one.idDevType = Convert.ToInt32(reader["devType"]);
                if (!String.IsNullOrEmpty(reader["status"].ToString()))
                {
                    ukut_one.commlinestatus = Convert.ToString(reader["status"]);
                }
                //Добавим в коллекцию   
                obj_list.Add(ukut_one);
        }
        reader.Close();
        //Получаем текущий срез от RS (Внимание!) Срез включает в себя, ВСЕ каналы
        
        
        Ukut ee = new Ukut();
        TemperatureNV NV = new TemperatureNV();
        //Температура наружного воздуха
        int tnv_int = Convert.ToInt32(NV.tnv);
        //подключаемся к АДС 
        //SqlConnection ads = ConnectToAds();
        //ads.Open();
        //Проходим по коллекции
        //foreach (Ukut ob in obj_list)
        try
        {
      
            //Parallel.ForEach(obj_list, (ob) =>
            // int i = 0;   
           // obj_list.AsParallel().AsOrdered().ForAll(ob =>
            foreach (var ob in obj_list)
            {
                try
                {


                    ee = ob;

                    ee.channels = GetChannelFromKP(ob.idUkut, ob.idMD, ref ee);
                    // ee.otkl = GetOtklsAds(ads, ob.HID,"maps");
                    //Итог в коллекцию
                    objects_list.Add(ee);
                }
                catch(Exception ex)
                {
                    
                }
            }
           // );
        }
        catch(Exception ex)
        {
            string s = ex.StackTrace;


        }

        foreach (Ukut ob in objects_list.ToArray())
        {
            ee = ob;
            int t_pod_int = 0;

            if (!String.IsNullOrEmpty(ee.t_pod))
            {
                string[] t_pod = ee.t_pod.Split(' ');

                Int32.TryParse(t_pod[0], out t_pod_int);
            }
            if (peret_mode == 1)
            {
                if(ob.idDevType != 3 || ob.teplo_source !="Водоканал")
                {
                    ee.temp_graph = GetTempGraphPodTrub(ob.teplo_source, 0, t_pod_int);
                }
            }
            else
            {
                if (ob.idDevType != 3 || ob.teplo_source !="Водоканал")
                {
                    ee.temp_graph = GetTempGraphPodTrub(ob.teplo_source, tnv_int, 0);
                }
            }

            objects_list.Remove(ob);

            //if (String.IsNullOrEmpty(ee.param_value))
            //{
            //  ee.param_value = ee.t_obr;
            //  ee.type = ee.type_obr;
            //}
            objects_list.Add(ee);
        }
        
        //Закрывает подключение к АДС
         //ads.Close();
        //Итоговый результат
        string RESULT_JSON = SerializeOfString(temp_mode, type);

        if (res_mode == "maps")
        {
            RESULT_JSON = RESULT_JSON.Remove(RESULT_JSON.Length - 1);
            RESULT_JSON = RESULT_JSON + ']';
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(RESULT_JSON);
        }
        else
        {
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(new JavaScriptSerializer().Serialize(objects_list));
        }
        GC.Collect();
        //Закрываем подключение к MySQL базе
        connection2.Close();
        connection2.Dispose();
    }
    catch ( Exception ex )
    {
          this.Context.Response.ContentType = "application/json; charset=utf-8";
          this.Context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
          this.Context.Response.Write("{\"success\": false, \"message\":\""+ex.StackTrace+ "\"}");
        
    }
}
/// <summary>
/// Проход по коллекции УКУТов, выборка по типам
/// </summary>
public string SerializeOfString(int temp_mode, string type)
{
    string RESULT_JSON = "[";
    string[] types = type.Split(',');
    StringBuilder sb = new StringBuilder();
    sb.AppendFormat("[");
    foreach (Ukut ob in objects_list.ToArray())
    {
        
        ob.CellChannelsToTempGraph();
        ob.SetTypes();
        bool flag_type = false;
        foreach (string typ in types)
        {
            if (ob.type == Convert.ToInt32(typ))
            {
                flag_type = true;
            }
        }
        if (flag_type == false)
        {
            //убираем из коллекции, если не соответсвует запрошенным типам
            objects_list.Remove(ob);
            continue;
        }

        
        string channels = "";

        foreach (Channel ch in ob.channels_struct)
        {
            channels = channels + ch.idUkut + ":" + ch.channel_name + ":" + ch.channel + ":" + ch.channel_value + ":" + ch.channel_color + ";";
        }

        string param_for_PM = ob.param_value;
        int type_e = ob.type;

        if (String.IsNullOrEmpty(ob.param_value))
        {
            if(ob.idDevType == 1 || ob.idDevType == 2 || ob.idDevType == 4 || ob.idDevType == 5)
            {
                param_for_PM = "t обр: "+ob.t_obr;
                type_e = ob.type_obr;
                
                if(type_e == 0)
                {
               // param_for_PM = "Н/Д";
                }
            }
            if(ob.idDevType == 3)
            {
                param_for_PM = "ХВС: "+ob.hvs;
                type_e = 2;
            }
        }
        //Для режима по обратке
        if (temp_mode == 10)
        {
            param_for_PM = "t обр: "+ob.t_obr;
            type_e = ob.type_obr;
        }
        //
        
        RESULT_JSON = RESULT_JSON + "[" + '"' + ob.idObject + '"' + ',' + '"' + ob.lat + '"' + ','
            + '"' + ob.lon + '"' + ',' + '"' + ob.date + '"' + ',' + '"' + ob.devName + '"' + ','
            + '"' + param_for_PM + '"' + ',' + '"' + type_e + '"' + ',' + '"' + ob.temp_graph + '"' + ',' + '"' + ob.address + '"' + ','
            + '"' + ob.geu + '"' + ',' + '"' + ob.idUkut + '"' + ',' + '"' + ob.countTrub + '"' + ',' + '"' + ob.idMD + '"' + ','
            + '"' + ob.system + '"' + ',' + '"' + channels + '"' + ',' + '"' + ob.zabbix_status + '"' + ',' + '"' + ob.zabbix_time.ToString("yyyy-MM-dd HH:mm:ss") + '"' + ','
            + '"' + ob.otkl + '"' + ',' + '"' + ob.teplo_source + '"' + ',' + '"' + ob.regulat + '"' + ',' + '"' + ob.idDevType + '"' + ',' + '"' + ob.commlinestatus + '"'

            + "],";
         
      /*  sb.AppendFormat("[\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\",\"{15}\",\"{16}\",\"{17}\",\"{18}\",\"{19}\",\"{20}\"],",ob.idObject, ob.lat, ob.lon,ob.date.ToString("yyyy-MM-dd HH:mm:ss"),ob.devName,
            param_for_PM,type_e,ob.temp_graph,ob.address,ob.geu,ob.idUkut,ob.countTrub,ob.idMD,ob.system,channels,ob.zabbix_status,
            ob.zabbix_time.ToString("yyyy-MM-dd HH:mm:ss"),ob.otkl,ob.teplo_source,ob.regulat,ob.idDevType);
        //sb.AppendLine();
       * */
    }
    RESULT_JSON.Remove(RESULT_JSON.Length-1);
  //  sb.Remove(sb.Length-1, 1);
    // sb.AppendFormat("]");
    return RESULT_JSON;
}
  
/// <summary>
///Отключение по ЭЭ из базы АДС
/// </summary>
    public string GetOtklsAds(SqlConnection ads, int HID, string res_mode)
    {
    string result = "";
    try
    {
        string connectionString = "SELECT  TOP 1 * FROM tbl_Otkl2 WHERE ResourceID = 3 AND HID=@HID  AND  convert(varchar(10), Date_Otkl, 102) = convert(varchar(10), getdate(), 102) order by Date_Otkl desc";
        SqlCommand getAdsOtkl = new SqlCommand(connectionString, ads);
        getAdsOtkl.Prepare();
        getAdsOtkl.Parameters.AddWithValue("@HID", HID);
        SqlDataReader getAdsOtklReader = getAdsOtkl.ExecuteReader();
        while (getAdsOtklReader.Read())
        {
            if (res_mode == "maps")
            {
                result = "<font color='red'>Отключение ЭЭ: </font>" + Convert.ToDateTime(getAdsOtklReader["Date_Otkl"]).ToString("HH:mm") + "<br><font color='green'> Включение ЭЭ: </font>" + Convert.ToDateTime(getAdsOtklReader["Date_Vkl"]).ToString("HH:mm");
            }
            else
            {
                result = "Отключение ЭЭ: " + Convert.ToDateTime(getAdsOtklReader["Date_Otkl"]).ToString("HH:mm") + " Включение ЭЭ: " + Convert.ToDateTime(getAdsOtklReader["Date_Vkl"]).ToString("HH:mm");
            }
        }
        getAdsOtklReader.Close();
    }
    catch (Exception ex)
    {
        result = "Нет связи с базой АДС"+ex;
    }
    return result;
    }
/// <summary>
///Каналы по УКУТ из DataTable
/// </summary>
public string GetChannelFromKPBak173(int idUkut, string KPNum, ref Ukut ukut)
{
    string result = "";
    DataRow[] findKP = (from myRow in InCnl.AsEnumerable() where myRow.Field<int>("KPNum") == Convert.ToInt32(KPNum) &&  myRow.Field<bool>("Active") == true && myRow.Field<int>("ParamID") != 46  && myRow.Field<int>("ParamID") != 47 && myRow.Field<int>("ParamID") != 48 && myRow.Field<int>("CnlNum") != 709 select myRow).ToArray();
    Channel ch = new Channel();
    List<Channel> channels_struct1 = new List<Channel>();
    foreach (DataRow rowCnl in findKP)
    {
        DataRow[] findResult = (from myRow in CurSrezTable.AsEnumerable() where myRow.Field<int>("channel") == Convert.ToInt32(rowCnl["CnlNum"]) select myRow).ToArray();
        
        string channel_name = Convert.ToString(rowCnl["Name"]);
        int ParamID = Convert.ToInt32(rowCnl["ParamID"]);
        channel_name = channel_name.Trim();
        string channel_value = Convert.ToString(findResult[0].Field<string>("channel_value"));
        string channel_color = Convert.ToString(findResult[0].Field<string>("channel_color"));
        string value_str = channel_value;
        string[] values = channel_value.Split(' ');
        int value = 0;
         ch.idUkut = Convert.ToInt32(idUkut);
        ch.channel = Convert.ToInt16(findResult[0].Field<int>("channel"));
        ch.channel_name = channel_name;
        ch.channel_value = channel_value;
        ch.channel_color = channel_color;
        channels_struct1.Add(ch);
    }
    ukut.channels_struct = channels_struct1;
    return result;
}
/// <summary>
///Каналы по УКУТ из DataTable
/// </summary>
public string GetChannelFromKP(int idUkut, string KPNum, ref Ukut ukut, bool PG)
{
    //ukut_mapsEntities1 uk = new ukut_mapsEntities1();
    //var blogs = uk.incnls.Where(b => b.KPNum == KPNum);


    string result = "";
    //DataRow[] findKP = InCnl.Select("KPNum=" + KPNum+ " AND Active=true");
    DataRow[] findKP = (from myRow in InCnl.AsEnumerable() where myRow.Field<int>("KPNum") == Convert.ToInt32(KPNum) &&  myRow.Field<bool>("Active") == true select myRow).ToArray();
    Channel ch = new Channel();
    List<Channel> channels_struct1 = new List<Channel>();
    
    
    foreach (DataRow rowCnl in findKP)
    {
        DataRow[] findResult = CurSrezTable.Select("channel = " + Convert.ToString(rowCnl["CnlNum"]));
        //DataRow[] findResult = (from myRow in CurSrezTable.AsEnumerable() where myRow.Field<int>("channel") == Convert.ToInt32(rowCnl["CnlNum"]) select myRow).ToArray();
        
        string channel_name = Convert.ToString(rowCnl["Name"]);
        int ParamID = Convert.ToInt32(rowCnl["ParamID"]);
        channel_name = channel_name.Trim();
        string channel_value = Convert.ToString(findResult[0].Field<string>("channel_value"));
        string channel_color = Convert.ToString(findResult[0].Field<string>("channel_color"));
        //ukut.date = Convert.ToDateTime(findResult[0].Field<DateTime>("last_update"));
        string value_str = channel_value;
        string[] values = channel_value.Split(' ');
        int value = 0;
        try
        {
            if (Int32.TryParse(values[0], out value))
            {
                if (value < 0)
                {
                    value_str = "Обрыв датчика!";
                }
            }
            else
            {
                value_str = "Н/Д";
            }
 
            if (channel_name == "Температура подачи отопления")
            {
                ukut.t_pod = channel_value;
            }
            if (ParamID==42 || ParamID==47)
            {
                string LimHigh = (rowCnl["LimHigh"] != DBNull.Value) ? Convert.ToString(rowCnl["LimHigh"]): "0";
                if(LimHigh !="0")
                {
                channel_value = channel_value + " ("+Convert.ToString(rowCnl["LimHigh"])+")";
                }
            
            }
            if (channel_name == "Температура обратки отопления")
            {
                ukut.t_obr = channel_value;
            }
           
            if (channel_name == "Температура ГВС" || channel_name == "Температура подачи ГВС"  || channel_name == "Температура подачи ГВС под 1-3")
            {
                ukut.t_gvs = channel_value;
                string[] values_gvs = channel_value.Split(' ');
                int temp = 0;
                if (Int32.TryParse(values_gvs[0], out temp))
                {
                    ukut.param_value = Convert.ToString(temp);
                    if (temp == 60 || temp <= 75)
                    {
                        ukut.type = 2;
                        ukut.type_text = "Норма";
                    }
                    if (temp > 75)
                    {
                        ukut.type = 3;
                        ukut.type_text = "Завышение";
                    }
                    if (temp < 60)
                    {
                        ukut.type = 1;
                        ukut.type_text = "Занижение";
                    }
                    if (temp == 0)
                    {
                        ukut.type = 0;
                        ukut.param_value = "Н/Д";
                        ukut.type_text = "Нет связи";
                    }
                    if (temp < 0)
                    {
                        ukut.type = 1;
                        ukut.param_value = "Термометр отключен!";
                        ukut.type_text = "Термометр отключен!";
                    }

                }
                else
                {
                    ukut.t_gvs = "0";
                    ukut.type = 0;
                    ukut.param_value = "Н/Д";
                    ukut.type_text = "Нет связи";
                }
            }
            if (channel_name == "Расход ХВС")
            {
                ukut.hvs = channel_value;
                ukut.param_value = "ХВС: "+ channel_value;
                ukut.type = 1;
            }
    
        }
        catch (System.FormatException ex)
        {
            
        }
        ch.idUkut = Convert.ToInt32(idUkut);
        ch.channel = Convert.ToInt16(findResult[0].Field<int>("channel"));
        ch.channel_name = channel_name;
        ch.channel_value = channel_value;
        ch.channel_color = channel_color;

        if (ch.RepaitColorSet())
        {
            ch.channel_value = ch.channel_value + " (Внимание!)";
            ukut.repair_ukut = "Да"; 
        }

        if (ukut.repair_ukut != "Да")
        {
            ukut.repair_ukut = "Нет"; 
        }

        channels_struct1.Add(ch);
    }
    ukut.channels_struct = channels_struct1;
    return result;
}
/// <summary>
///Каналы по УКУТ из DataTable
/// </summary>
public string GetChannelFromKP(int idUkut, string KPNum, ref Ukut ukut)
{
    string result = "";
    DataRow[] findKP = InCnl.Select("KPNum=" + KPNum+ " AND Active=1");
    //DataRow[] findKP = (from myRow in InCnl.AsEnumerable() where myRow.Field<int>("KPNum") == Convert.ToInt32(KPNum) select myRow).ToArray();

    Channel ch = new Channel();
    List<Channel> channels_struct1 = new List<Channel>();
    foreach (DataRow rowCnl in findKP)
    {
       // DataRow[] findResult = (from myRow in CurSrezTable.AsEnumerable() where myRow.Field<int>("channel") == Convert.ToInt32(rowCnl["CnlNum"]) select myRow).ToArray();
       DataRow[] findResult = CurSrezTable.Select("channel="+Convert.ToInt32(rowCnl["CnlNum"]));
        
        
        string channel_name = Convert.ToString(rowCnl["Name"]);
        int ParamID = Convert.ToInt32(rowCnl["ParamID"]);
        channel_name = channel_name.Trim();
        string channel_value = Convert.ToString(findResult[0].Field<string>("channel_value"));
        string channel_color = Convert.ToString(findResult[0].Field<string>("channel_color"));
        //ukut.date = Convert.ToDateTime(findResult[0].Field<DateTime>("last_update"));
        string value_str = channel_value;
        string[] values = null;
        try
        {
            values = channel_value.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
        }
        catch
        {
            values[0] = null;
            values[1] = null;
        }
        int value = 0;
        try
        {
            if (Int32.TryParse(values[0], out value))
            {
                if (value < 0)
                {
                    value_str = "Обрыв датчика!";
                }
            }
            else
            {
                value_str = "Н/Д";
            }
 
            if (channel_name == "Температура подачи отопления")
            {
                ukut.t_pod = channel_value;
            }
            if (ParamID==42 || ParamID==47)
            {
                string LimHigh = (rowCnl["LimHigh"] != DBNull.Value) ? Convert.ToString(rowCnl["LimHigh"]): "0";
                if(LimHigh !="0")
                {
                channel_value = channel_value + " ("+Convert.ToString(rowCnl["LimHigh"])+")";
                }
            
            }
            if (channel_name == "Температура обратки отопления")
            {
                ukut.t_obr = channel_value;
            }
           
            if (channel_name == "Температура ГВС" || channel_name == "Температура подачи ГВС"  || channel_name == "Температура подачи ГВС под 1-3")
            {
                ukut.t_gvs = channel_value;
                string[] values_gvs = channel_value.Split(' ');
                int temp = 0;
                if (Int32.TryParse(values_gvs[0], out temp))
                {
                    ukut.param_value = Convert.ToString(temp);
                    if (temp == 60 || temp <= 75)
                    {
                        ukut.type = 2;
                        ukut.type_text = "Норма";
                    }
                    if (temp > 75)
                    {
                        ukut.type = 3;
                        ukut.type_text = "Завышение";
                    }
                    if (temp < 60)
                    {
                        ukut.type = 1;
                        ukut.type_text = "Занижение";
                    }
                    if (temp == 0)
                    {
                        ukut.type = 0;
                        ukut.param_value = "Н/Д";
                        ukut.type_text = "Нет связи";
                    }
                    if (temp < 0)
                    {
                        ukut.type = 1;
                        ukut.param_value = "Термометр отключен!";
                        ukut.type_text = "Термометр отключен!";
                    }

                }
                else
                {
                    ukut.t_gvs = "0";
                    ukut.type = 0;
                    ukut.param_value = "Н/Д";
                    ukut.type_text = "Нет связи";
                }
            }
            if (channel_name == "Расход ХВС")
            {
                ukut.hvs = channel_value;
                ukut.param_value = "ХВС: "+ channel_value;
                ukut.type = 1;
            }
    
        }
        catch (System.FormatException ex)
        {
            
        }
        ch.idUkut = Convert.ToInt32(idUkut);
        ch.channel = Convert.ToInt16(findResult[0].Field<int>("channel"));
        ch.channel_name = channel_name;
        ch.channel_value = channel_value;
        ch.channel_color = channel_color;

        if (ch.RepaitColorSet())
        {
            ch.channel_value = ch.channel_value + " (Внимание!)";
            ukut.repair_ukut = "Да"; 
        }

        if (ukut.repair_ukut != "Да")
        {
            ukut.repair_ukut = "Нет"; 
        }

        channels_struct1.Add(ch);
    }
    ukut.channels_struct = channels_struct1;
    return result;
}
/// <summary>
/// Метод для отгрузки текущих 
/// </summary>
[WebMethod(Description = "Получение текущих показаний для KPNum и idUkut(необязательно, можно нуль)")]
[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
public void GetCurrentValues(string idUkut, string KPNum)
{
    try
    {
        KPNum = KPNum.Replace("'", "''");
        ReceiveSrezTable();
        ReceiveInCnlTable(true);
        Ukut uk = new Ukut();
        int id = Convert.ToInt32(idUkut);
        string result = GetChannelFromKP(id, KPNum, ref uk);
        try
        {
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(new JavaScriptSerializer().Serialize(uk.channels_struct));
        }
        catch (Exception ex)
        {
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            this.Context.Response.Write("{\"success\": false, \"message\":\"" + ex.Message + "\"}");

        }
    }
    catch (Exception ex)
        {
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            this.Context.Response.Write("{\"success\": false, \"message\":\"" + ex.Message + "\"}");

        }

}
    /// <summary>
/// Метод для отгрузки текущих 
/// </summary>
[WebMethod(Description = "Получение текущих показаний для KPNum и idUkut(необязательно, можно нуль)")]
[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
public void GetCurrentValuesBak173(string KPNum)
{
    try
    {
        KPNum = KPNum.Replace("'", "''");
        ReceiveSrezTable();
        ReceiveInCnlTable();
        Ukut uk = new Ukut();
       string result = GetChannelFromKPBak173(0, KPNum, ref uk);
        //DataRow[] findKP = (from myRow in InCnl.AsEnumerable() where myRow.Field<int>("KPNum") == Convert.ToInt32(KPNum) &&  myRow.Field<bool>
        try
        {
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(new JavaScriptSerializer().Serialize(uk.channels_struct));
        }
        catch (Exception ex)
        {
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            this.Context.Response.Write("{\"success\": false, \"message\":\"" + ex.Message + "\"}");

        }
    }
    catch (Exception ex)
        {
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            this.Context.Response.Write("{\"success\": false, \"message\":\"" + ex.Message + "\"}");

        }

}
/// <summary>
///Получение температурного графика из базы SQL, в зависимости от источника, tnv , tпод
/// </summary>
public string GetTempGraphPodTrub(string teplo_source, int tnv, int t_pod_current)
{
    try
    {
        teplo_source = teplo_source.Trim();
        string templocal_graph = "0";
          NpgsqlCommand command = new NpgsqlCommand();
            if (t_pod_current != 0)
            {
             command = new NpgsqlCommand("SELECT * FROM tempgraphs WHERE \"Source\" = @Source AND \"tpod\" >=@tpod ORDER BY tpod LIMIT 1", connection2);
             //command.Prepare();
             command.Parameters.AddWithValue("@Source", teplo_source);
             command.Parameters.AddWithValue("@tpod", t_pod_current);
            }
            else
            {
             command = new NpgsqlCommand("SELECT * FROM tempgraphs WHERE \"Source\" = @Source AND \"tnv\" =@tnv", connection2);
             //command.Prepare();
             command.Parameters.AddWithValue("@Source", teplo_source);
             command.Parameters.AddWithValue("@tnv", tnv);
            }
            NpgsqlDataReader reader = command.ExecuteReader();
        bool result = false;
            while (reader.Read())
            {
                templocal_graph = reader["tpod"] + "/" + reader["tobr"];
                result = true;
            }
            reader.Close();

            if(!result && t_pod_current != 0)
            {
                //Если нет графика для tpod (высокая) уменьшаем на градус и повторяем функцию
                //templocal_graph = GetTempGraphPodTrub(teplo_source, 0, (t_pod_current-1));
            }
           if(!result && t_pod_current == 0)
            {
               templocal_graph = "<font color='red'>График не найден для tнв="+tnv+"</font>";
              // templocal_graph = GetTempGraphPodTrub(teplo_source, tnv+1, 0);
            }
            
            return templocal_graph;
    }
    catch
    {
        return "00/00";
    }
}
/// <summary>
///Подключение к АДС
/// </summary>
public SqlConnection ConnectToAds()
{
    SqlConnection myConnection = new SqlConnection("Server=vs4.remp.yek.ru;User Id=energoservice;" +
                                   "Password=energoservice;Server=vs4.remp.yek.ru;" +
                                   "Database=ads;" );
    return myConnection;
   
}

/// <summary>
/// Метод для получения Event по KPNum (для всех каналов в пределах KPNum)
/// </summary>
[WebMethod(Description = "Получить события начиная с даты, по номеру KP")]
[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
public void EventFromKpNum(int KPNum, string SelectDate)
{
        DateTime Monday = Convert.ToDateTime(SelectDate);
        NpgsqlConnection connection = new NpgsqlConnection(_cs);
    try
    {
        connection.Open();
        NpgsqlCommand command = new NpgsqlCommand(@"SELECT events.datetime, events.kpnum, param.ParamName, events.cnlnum,  events.oldcnlstat, events.descr,
        oldcnlstates.NameCnlState AS OldStates, ROUND(events.oldcnlval, 2) AS oldcnlval, events.newcnlstat, 
        newcnlstates.NameCnlState AS NewStates, ROUND(events.newcnlval, 2) AS newcnlval
        FROM events 
        INNER JOIN param ON param.paramID = events.paramid 
        INNER JOIN cnlstates as oldcnlstates ON oldcnlstates.cnlstatID = events.oldcnlstat 
        INNER JOIN cnlstates as newcnlstates ON newcnlstates.cnlstatID = events.newcnlstat 
        WHERE kpnum = @KPNum AND events.datetime >= @datetime ORDER by datetime DESC", connection);
        command.Prepare();
        command.Parameters.AddWithValue("@KPNum", KPNum);
        command.Parameters.AddWithValue("@datetime", Monday);
        NpgsqlDataAdapter myDA = new NpgsqlDataAdapter(command);
        DataTable dt = new DataTable();
        myDA.Fill(dt);
        connection.Close();
        connection.Dispose();
        this.Context.Response.ContentType = "application/json; charset=utf-8";
        this.Context.Response.Write(ConvertDataTabletoString(dt));
    }
    catch ( Exception ex )
    {
          this.Context.Response.ContentType = "application/json; charset=utf-8";
          this.Context.Response.Write("{\"success\": false, \"message\":\""+ex.Message+ "\"}");
        
    }
}
    /// <summary>
/// Метод для получения списка объектов, по которым не нулевое количество Event за указанный диапазон дат
/// </summary>
[WebMethod(Description = "Получить список объектов с количеством событий с даты")]
[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
public void GetObjectListEventCount(string SelectDate)
{
    //DateTime Monday = GetMonday(DateTime.Today);
    DateTime Monday = Convert.ToDateTime(SelectDate);
    NpgsqlConnection connection = new NpgsqlConnection(cs);
    try
    {
        connection.Open();
        /*
        MySqlCommand command = new MySqlCommand(@"SELECT 
                        idObject, lon, lat, address, geu, HID, 
                        teplo_source, idUkut, countTrub, idMD, 
                        system, regulat, devName, serialNum, 
                        DevType, highway, teplo_camera, dQotop, dGgvs, dQvent, 
                            (SELECT COUNT(*) FROM events WHERE KPNum = idMD AND datetime >= @monday) as CountTrabl
                            FROM ukut WHERE idMD !=0 HAVING CountTrabl > 0 ORDER BY idObject", connection);
         * */
        NpgsqlCommand command = new NpgsqlCommand(@"SELECT * FROM ukut where idMD != 0 ORDER BY idObject", connection);
        command.Parameters.AddWithValue("@monday", Monday);
        command.Prepare();
        NpgsqlDataAdapter myDA = new NpgsqlDataAdapter(command);
        DataTable dt = new DataTable();
        myDA.Fill(dt);
        connection.Close();
        connection.Dispose();
        this.Context.Response.ContentType = "application/json; charset=utf-8";
        this.Context.Response.Write(ConvertDataTabletoString(dt));
    }
    catch ( Exception ex )
    {
          this.Context.Response.ContentType = "application/json; charset=utf-8";
          this.Context.Response.Write("{\"success\": false, \"message\":\""+ex.Message+ "\"}");
        
    }
}
/// <summary>
/// Метод для получения архива Изменений БД
/// </summary>
[WebMethod(Description = "Получить архив изменений БД приборов СПТ941-СПТ943")]
[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
public void GetIzmArc(int KPNum)
{

    NpgsqlConnection connection = new NpgsqlConnection(_cs);
    try
    {
        connection.Open();
        NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM izmArc WHERE \"KPNum\" = @KPNum", connection);
        command.Parameters.AddWithValue("@KPNum", KPNum);
        
        NpgsqlDataAdapter myDA = new NpgsqlDataAdapter(command);
        DataTable dt = new DataTable();
        myDA.Fill(dt);
        connection.Close();
        connection.Dispose();
        this.Context.Response.ContentType = "application/json; charset=utf-8";
        this.Context.Response.Write(ConvertDataTabletoString(dt));
    }
    catch ( Exception ex )
    {
          this.Context.Response.ContentType = "application/json; charset=utf-8";
          this.Context.Response.Write("{\"success\": false, \"message\":\""+ex.Message+ "\"}");
        
    }
}
    static DateTime GetMonday(DateTime date)
    {
      while(date.DayOfWeek != System.DayOfWeek.Monday)
      {
        date = date.AddDays(-1);
      }
      return date;
    }
     /// <summary>
    /// Запустить опрос архива по KPNum
    /// </summary>
    [WebMethod(Description = "Запуск опроса архивов объекта KPNum")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void StartProccessArc(int KPNum)
    {   
        string error_message = null;
        if (KPNum == 0)
        {
            error_message = "KPNum не может быть равно 0";
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write("{\"success\": false, \"message\":\"" + error_message + "\"}");
        }
        else
        {
            Log.WriteLineDelegate writeToLog;
            writeToLog = text => { }; // заглушка
            CommSettings cm = new CommSettings("10.235.63.252", 10000, "ScadaApi", "ScadaApi", 10000);
            ServerComm sc = new ServerComm(cm, writeToLog);
            DataTable CtrlCnl = new DataTable();
            sc.ReceiveBaseTable("ctrlcnl.dat", CtrlCnl);
            error_message = sc.ErrMsg;
            try
            {
                DataRow[] findCtrlNum = CtrlCnl.Select("KPNum = " + KPNum);
                bool result = false;
                sc.SendBinaryCommand(0, Convert.ToInt32(findCtrlNum[0]["CtrlCnlNum"]), new byte[] { 0x00, 0x00 }, out result);
                sc.Close();
                this.Context.Response.ContentType = "application/json; charset=utf-8";
                this.Context.Response.Write("{\"success\": true, \"message\":\"Опрос KPNum:"+KPNum+ " успешно запущен\"}");
            }
            catch (Exception ex)
            {
                this.Context.Response.ContentType = "application/json; charset=utf-8";
                this.Context.Response.Write("{\"success\": false, \"message\":\"" + error_message + "\"}");
            }
        }
    }
     /// <summary>
    /// Запустить опрос архивов по всем объектам
    /// </summary>
    [WebMethod(Description = "Запустить внеплановый опрос архивов ВСЕХ объектов учета")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void StartAllArchives()
    {
        Log.WriteLineDelegate writeToLog;
        writeToLog = text => { }; // заглушка
        CommSettings cm = new CommSettings("10.235.63.252",10000,"ScadaApi","ScadaApi",10000);
        ServerComm sc = new ServerComm(cm, writeToLog);
        DataTable KPTable = new DataTable();
        DataTable CtrlCnl = new DataTable();
        sc.ReceiveBaseTable("kp.dat", KPTable);
        KPTable.Columns.Add("StatusCMD", typeof(bool));
        sc.ReceiveBaseTable("ctrlcnl.dat", CtrlCnl);
        DataRow[] findKP = KPTable.Select("KPTypeID = 216 OR KPTypeID = 217 OR KPTypeID = 218");


        List<Result_RS> Answers = new List<Result_RS>();
        foreach (DataRow rowKP in findKP)
        {
            DataRow[] findCtrl = CtrlCnl.Select("KPNum = "+rowKP["KPNum"]);
            if (findCtrl.Length > 0)
            {
            bool result = false;
            sc.SendBinaryCommand(0, Convert.ToInt32(findCtrl[0]["CtrlCnlNum"]), new byte[] { 0x00, 0x00 }, out result);

                Result_RS rs = new Result_RS();
                rs.KPNum = rowKP["KPNum"].ToString(); ; ;
                rs.Name = rowKP["Name"].ToString();
                rs.Result = result;
                Answers.Add(rs);
            }
            Thread.Sleep(100);
        }
        sc.Close();

        this.Context.Response.ContentType = "application/json; charset=utf-8";
        this.Context.Response.Write(new JavaScriptSerializer().Serialize(Answers));
    }
        /// <summary>
    /// Запустить опрос архивов по выбранным объектам
    /// </summary>
    [WebMethod(Description = "Запустить внеплановый опрос выборочных объектов")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void StartCustomArchives(string KPNums)
    {
        Log.WriteLineDelegate writeToLog;
        writeToLog = text => { }; // заглушка
        CommSettings cm = new CommSettings("10.235.63.252",10000,"ScadaApi","ScadaApi",10000);
        ServerComm sc = new ServerComm(cm, writeToLog);
        DataTable KPTable = new DataTable();
        DataTable CtrlCnl = new DataTable();
        sc.ReceiveBaseTable("kp.dat", KPTable);
        KPTable.Columns.Add("StatusCMD", typeof(bool));
        sc.ReceiveBaseTable("ctrlcnl.dat", CtrlCnl);
        
        
       List<Result_RS> Answers = new List<Result_RS>();
           string[] arrayKPNums = KPNums.Split(',');
           foreach (string obj in arrayKPNums)
           {
               DataRow[] findKP = KPTable.Select("KPNum="+obj);


               foreach (DataRow rowKP in findKP)
               {
                   DataRow[] findCtrl = CtrlCnl.Select("KPNum = " + rowKP["KPNum"]);
                   if (findCtrl.Length > 0)
                   {
                       bool result = false;
                       sc.SendBinaryCommand(0, Convert.ToInt32(findCtrl[0]["CtrlCnlNum"]), new byte[] { 0x00, 0x00 }, out result);

                       Result_RS rs = new Result_RS();
                       rs.KPNum = rowKP["KPNum"].ToString(); ; ;
                       rs.Name = rowKP["Name"].ToString();
                       rs.Result = result;
                       Answers.Add(rs);

                   }
               }
           }
        sc.Close();

        this.Context.Response.ContentType = "application/json; charset=utf-8";
        this.Context.Response.Write(new JavaScriptSerializer().Serialize(Answers));
    }
    /// <summary>
/// Метод для создания тикета
/// </summary>
[WebMethod(Description = "Создать заявку")]
[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
public void CreateTicket(string address, string kpnum, string author, string message)
{
    Tickets ticket = new Tickets(cs);
    
    try
    {
          this.Context.Response.ContentType = "application/json; charset=utf-8";
          this.Context.Response.Write("{\"success\": true, \"message\":\""+ticket.CreateTicket(address, kpnum, author, message)+ "\"}");
    }
    catch ( Exception ex )
    {
          this.Context.Response.ContentType = "application/json; charset=utf-8";
          this.Context.Response.Write("{\"success\": false, \"message\":\""+ex.Message+ "\"}");
        
    }
}
/// <summary>
/// Метод для получения заявок по объекту
/// </summary>
[WebMethod(Description = "Заявки по объекту")]
[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
public void ShowTickets(string kpnum)
{
    Tickets ticket = new Tickets(cs);
    
    try
    {
          this.Context.Response.ContentType = "application/json; charset=utf-8";
          this.Context.Response.Write(ConvertDataTabletoString(ticket.ShowTickets(kpnum)));
    }
    catch ( Exception ex )
    {
          this.Context.Response.ContentType = "application/json; charset=utf-8";
          this.Context.Response.Write("{\"success\": false, \"message\":\""+ex.Message+ "\"}");
        
    }
}
    /// <summary>
/// Метод для получения заявок по объекту
/// </summary>
[WebMethod(Description = "Отчет по ГВС для СТК")]
[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
public void GetReportToSTS(string arc, string dateFirst, string dateLast, string border, string countTrub, string objects, string dogovor)
{
    Reports rt = new Reports(Convert.ToInt16(countTrub), objects);
    DataSet ds = rt.GetCountGVS(arc, dateFirst, dateLast, border, Convert.ToInt16(countTrub), objects);
    try
    {
        EPPlus ep = new EPPlus(Server.MapPath("/template/ReportToSTS.xlsx"), ds,dateFirst.ToString()+" по "+dateLast.ToString(), dogovor);
        ds.Dispose();
        string filename = String.Format("Отчет по ГВС для СТК-{0}-{1}.xlsx", dateFirst,dateLast);
        string Header = String.Format("attachment;  filename={0}", filename);
        this.Context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        this.Context.Response.AddHeader("content-disposition", Header);
        this.Context.Response.BinaryWrite(ep.res);
    }
    catch ( Exception ex )
    {
          this.Context.Response.ContentType = "application/json; charset=utf-8";
          this.Context.Response.Write("{\"success\": false, \"message\":\""+ex.Message+ "\"}");
        
    }
}
/// <summary>
/// Метод для получения отчета по перетопам
/// </summary>
[WebMethod(Description = "Отчет по перетопам в формате xls")]
[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
public void GetReportPeretops(string arc, string dateFirst, string dateLast, string border, string objects)
{
    Reports rt = new Reports();
    DataSet ds = rt.GetPeretopGVS(arc, dateFirst, dateLast, border, objects);
    try
    {
        ReportPeretopGVS ep = new ReportPeretopGVS(Server.MapPath("/template/ReportPeretorGVS.xlsx"), ds);
        ds.Dispose();

    var template = new FileInfo(Server.MapPath("/template/ReportPeretorGVS.xlsx"));
    ExcelPackage pck = new ExcelPackage(template, false);
    ExcelWorksheet template_ws = pck.Workbook.Worksheets[1];
        foreach(DataTable dt in ds.Tables)
        {
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(dt.TableName, template_ws);
            ws.Cells["A7"].LoadFromDataTable(dt, false);
            ws.Cells["A2"].Value = "по адресу: "+dt.TableName;
        }
         string filename = String.Format("Отчет по перетопам ГВС-{0}-{1}.xlsx", dateFirst,dateLast);
        string Header = String.Format("attachment;  filename={0}",filename);
        this.Context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        this.Context.Response.AddHeader("content-disposition", Header);
        this.Context.Response.BinaryWrite(pck.GetAsByteArray());
    }
    catch ( Exception ex )
    {
          this.Context.Response.ContentType = "application/json; charset=utf-8";
          this.Context.Response.Write("{\"success\": false, \"message\":\""+ex.StackTrace+ "\"}");
        
    }
}
/// <summary>
/// Метод для получения списка объектов для отчета
/// </summary>
[WebMethod(Description = "Список объектов для отчета")]
[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
public void GetObjectListForReports()
{
    NpgsqlConnection connection = new NpgsqlConnection(cs);
    try
    {
        //   AND DevType IN(1,2)      AND column_pod != ' '
        connection.Open();
        NpgsqlCommand command = new NpgsqlCommand("SELECT \"address\", \"idMD\", \"teplo_source\", \"column_pod\", \"column_obr\", \"column_gvs\", \"Dogovor\", (1) AS \"markCheckbox\" FROM ukut WHERE \"idMD\" !=0 AND \"template\" IS NOT NULL ORDER BY \"address\" ASC", connection);
        NpgsqlDataAdapter myDA = new NpgsqlDataAdapter(command);
        DataTable dt = new DataTable();
        myDA.Fill(dt);
        connection.Close();
        connection.Dispose();
        this.Context.Response.ContentType = "application/json; charset=utf-8";
        this.Context.Response.Write(ConvertDataTabletoString(dt));

    }
    catch ( Exception ex )
    {
          this.Context.Response.ContentType = "application/json; charset=utf-8";
          this.Context.Response.Write("{\"success\": false, \"message\":\""+ex.Message+ "\"}");
        
    }
}
/// <summary>
/// Метод для получения списка договор для отчета
/// </summary>
[WebMethod(Description = "Список договоров для отчета")]
[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
public void GetDogovorList()
{
    NpgsqlConnection connection = new NpgsqlConnection(cs);
    try
    {
        connection.Open();
        NpgsqlCommand command = new NpgsqlCommand(@"SELECT DISTINCT dogovor FROM ukut", connection);
        NpgsqlDataAdapter myDA = new NpgsqlDataAdapter(command);
        DataTable dt = new DataTable();
        myDA.Fill(dt);
        connection.Close();
        connection.Dispose();
        this.Context.Response.ContentType = "application/json; charset=utf-8";
        this.Context.Response.Write(ConvertDataTabletoString(dt));

    }
    catch ( Exception ex )
    {
          this.Context.Response.ContentType = "application/json; charset=utf-8";
          this.Context.Response.Write("{\"success\": false, \"message\":\""+ex.Message+ "\"}");
        
    }
}
public void Prepare(ref DataTable dt, ref ExcelWorksheet ws, string nameRange)
{    EnumerableRowCollection selectedColumn1;
    if(nameRange == "Date")
    {
        selectedColumn1 = dt.AsEnumerable().Select(s=>s.Field<DateTime>(nameRange));
    }
   else
    {
        selectedColumn1 = dt.AsEnumerable().Select(s=>s.Field<Double?>(nameRange));
    }

        if (ws.Names.ContainsKey(nameRange))
        {
            using (ExcelNamedRange namedRange = ws.Names[nameRange])
            {
                var row = namedRange.Start.Row;
                var col = namedRange.Start.Column;
                foreach (var cl in selectedColumn1)
                {
                    ws.Cells[row, col].Value = cl;
                    row++;
                }
            }
        }
}
    public void Prepare2(ref DataTable dt, ref ExcelWorksheet ws, string nameRange)
{    EnumerableRowCollection selectedColumn1;
    if(nameRange == "Date" || nameRange == "Q_Tarif" || nameRange == "M_Tarif")
    {
        selectedColumn1 = dt.AsEnumerable().Select(s=>s.Field<String>(nameRange));
    }
   else
    {
        selectedColumn1 = dt.AsEnumerable().Select(s=>s.Field<Double?>(nameRange));
    }
        try
        {



            if (ws.Names.ContainsKey(nameRange))
            {
                using (ExcelNamedRange namedRange = ws.Names[nameRange])
                {
                    var row = namedRange.Start.Row;
                    var col = namedRange.Start.Column;
                    foreach (var cl in selectedColumn1)
                    {
                        ws.Cells[row, col].Value = cl;
                        row++;
                    }
                }
            }
        }
        catch
        {
            
        }
}
       public void InsertFormula(ref ExcelWorksheet ws, string nameRange,  int countRows)
       {
            if (ws.Names.ContainsKey(nameRange))
            {
                using (ExcelNamedRange namedRange = ws.Names[nameRange])
                {
                    var row = namedRange.Start.Row;
                    var col = namedRange.Start.Column;
                    //namedRange.Formula = formula;
                    //foreach (var cl in namedRange)
                    //{
                    //    ws.Cells[row, col].Formula = formula;
                    //    row++;
                    //}
                    StringBuilder sb = new StringBuilder();
                    for (int i = row; i <= countRows; i++)
                    {
                        sb.Clear();
                        sb.AppendFormat("=E{0}*M{0}+G{0}*R{0}", i);
                        ws.Cells[i, col].Formula = sb.ToString();
                    }
                }
            }

}
           public void InsertFormulaParametrs(ref ExcelWorksheet ws, string nameRange,  int countRows)
       {
            if (ws.Names.ContainsKey(nameRange))
            {
                using (ExcelNamedRange namedRange = ws.Names[nameRange])
                {
                    var row = namedRange.Start.Row;
                    var col = namedRange.Start.Column;

                    StringBuilder sb = new StringBuilder();
                    for (int i = row; i <= countRows; i++)
                    {
                        sb.Clear();
                        sb.AppendFormat("=IF(H{0}<60,IF(H{0}>40,\"Ниже нормы\",\"Ниже 40\"),\"Норма\")", i);
                        //sb.AppendFormat("=ЕСЛИ(H{0}<50;-1;1)", i);
                        ws.Cells[i, col].Formula = sb.ToString();
                    }
                }
            }

}
               public void InsertFormulaPlata(ref ExcelWorksheet ws, string nameRange,  int countRows)
       {
            if (ws.Names.ContainsKey(nameRange))
            {
                using (ExcelNamedRange namedRange = ws.Names[nameRange])
                {
                    var row = namedRange.Start.Row;
                    var col = namedRange.Start.Column;

                    StringBuilder sb = new StringBuilder();
                    for (int i = row; i <= countRows; i++)
                    {
                        sb.Clear();
                        sb.AppendFormat("=IF(X{0}=\"Ниже нормы\",ROUNDDOWN(((60-H{0}-D{0}))/3,0)*AC{0}*0.1%,)", i);
                        //sb.AppendFormat("=ЕСЛИ(H{0}<50;-1;1)", i);
                        ws.Cells[i, col].Formula = sb.ToString();
                    }
                }
            }

}
    public void ConditionFormat(ref ExcelWorksheet ws, string nameRange, string nameRange2)
{    
       
        if (ws.Names.ContainsKey(nameRange) && ws.Names.ContainsKey(nameRange2))
        {
            
            using (ExcelNamedRange namedRange = ws.Names[nameRange])
            {
                ExcelNamedRange namedRange2 = ws.Names[nameRange2];

                int row = namedRange.Start.Row;
                int col = namedRange.Start.Column;
                var _formatRangeAddress = new ExcelAddress(namedRange.Address);
                var _condp01 = ws.ConditionalFormatting.AddExpression(_formatRangeAddress);
               
                _condp01.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                _condp01.Style.Fill.BackgroundColor.Color = System.Drawing.Color.Red;
                _condp01.Formula = new ExcelFormulaAddress(namedRange.Start.Row, namedRange.Start.Column, namedRange.End.Row,namedRange.End.Column) + ">" +
                                   new ExcelFormulaAddress(namedRange2.Start.Row, namedRange2.Start.Column, namedRange2.End.Row,namedRange2.End.Column);
     
            }
        }
}
public void CheckAndEditNamedRange(ref ExcelWorksheet ws, string nameRange, string value)
{
        if (ws.Names.ContainsKey(nameRange))
        {
            using (ExcelNamedRange namedRange = ws.Names[nameRange])
            {
                 var row = namedRange.Start.Row;
                 var col = namedRange.Start.Column;
                ws.Cells[row, col].Value = value;
            }
        }
}
/// <summary>
/// Метод для формирования карточек
/// </summary>
[WebMethod(Description = "Метод для формирования карточек в СТК, формы карточек лежат в каталоге Template")]
[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
public void GetTeploReport(string dateFirst, string dateLast, string objects)
{
    MySqlConnection connection = new MySqlConnection(cs);
    connection.Open();
    var main_excel = new FileInfo(Server.MapPath("/Template/elmash.xlsx"));
    ExcelPackage pck = new ExcelPackage(main_excel, false);
    //ExcelWorksheet template_ws = pck.Workbook.Worksheets[1];
    try
    {
        
        string[] arrayObjects = objects.Split(',');
        foreach (string obj in arrayObjects)
        {
           
            ExcelWorksheet template_ws = pck.Workbook.Worksheets[1];
     
            TeploReport TR = new TeploReport(Convert.ToInt16(obj), connection, dateFirst, dateLast);
            //template start
            var temp_excel = new FileInfo(Server.MapPath("/Template/"+TR.template));
            ExcelPackage pck_template = new ExcelPackage(temp_excel, false);
            template_ws = pck_template.Workbook.Worksheets[1];
            //template end

            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(TR.Address, template_ws);
            
            DataTable dt = TR.archive.Copy();
            Prepare(ref dt, ref ws, "Date");
            Prepare(ref dt, ref ws, "TB1_P1");
            Prepare(ref dt, ref ws, "TB1_t1");
            Prepare(ref dt, ref ws, "TB1_M1");
            Prepare(ref dt, ref ws, "TB1_P2");
            Prepare(ref dt, ref ws, "TB1_t2");
            Prepare(ref dt, ref ws, "TB1_M2");
            Prepare(ref dt, ref ws, "TB1_Q");
            Prepare(ref dt, ref ws, "TB1_Ti");
           
            Prepare(ref dt, ref ws, "TB2_P1");
            Prepare(ref dt, ref ws, "TB2_t1");
            Prepare(ref dt, ref ws, "TB2_M1");
            Prepare(ref dt, ref ws, "TB2_P2");
            Prepare(ref dt, ref ws, "TB2_t2");
            Prepare(ref dt, ref ws, "TB2_M2");
            Prepare(ref dt, ref ws, "TB2_Q");
            Prepare(ref dt, ref ws, "TB2_Ti");
            //Расчетные
            Prepare(ref dt, ref ws, "G_GVS");
            //
            string[] location = TR.Address.Split(',');
            ws.Cells["A1:A100"].Style.Numberformat.Format = "dd.mm.yyyy";
            CheckAndEditNamedRange(ref ws,"abonent",TR.Abonent);
            CheckAndEditNamedRange(ref ws,"address",location[0]);
            CheckAndEditNamedRange(ref ws,"dom",location[1]);
            CheckAndEditNamedRange(ref ws,"dogovor",TR.Dogovor);
            CheckAndEditNamedRange(ref ws,"Qotop",TR.dQotop);
            CheckAndEditNamedRange(ref ws,"Ggvs",TR.dGgvs);
            CheckAndEditNamedRange(ref ws,"Device",TR.Device);
            CheckAndEditNamedRange(ref ws,"scheme",TR.Scheme);
             CheckAndEditNamedRange(ref ws,"uchet",TR.Uchet);
             CheckAndEditNamedRange(ref ws,"formula",TR.FormulaWinter);
             CheckAndEditNamedRange(ref ws,"Prim",TR.Prim);
            CheckAndEditNamedRange(ref ws,"txi",TR.tcw);



            ConditionFormat(ref ws, "TB1_P2","TB1_P1");
            ConditionFormat(ref ws, "TB2_P2","TB2_P1");

            ws.Calculate();
           // ws.Da
            //pck.Save();

        }

        pck.Workbook.Worksheets.Delete(1);
        this.Context.Response.Clear();
        this.Context.Response.ClearHeaders();
        this.Context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        this.Context.Response.AddHeader("content-disposition", "attachment;  filename=Карточки.xlsx");
        this.Context.Response.BinaryWrite(pck.GetAsByteArray());
        this.Context.Response.Flush();
    }
        
    
    catch ( System.InvalidOperationException ex )
    {
          this.Context.Response.ContentType = "application/json; charset=utf-8";
          this.Context.Response.Write("{\"success\": false, \"message\":\""+ex.Message+ "\"}");
        
    } 
   

    }

    /// <summary>
/// Метод для формирования карточек
/// </summary>
[WebMethod(Description = "Метод для формирования претензии по ГВС")]
[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
public void GetPretensionReport(string dateFirst, string dateLast, string objects)
{
    NpgsqlConnection connection = new NpgsqlConnection(cs);
    connection.Open();
    var main_excel = new FileInfo(Server.MapPath("/Template/Pretenzion/pret2.xlsx"));
    ExcelPackage pck = new ExcelPackage(main_excel);
    ExcelWorksheet template_ws = pck.Workbook.Worksheets[1];
    try
    {
        string[] arrayObjects = objects.Split(',');
        foreach (string obj in arrayObjects)
        {
                 if(connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

     
            PretenzionReport TR = new PretenzionReport(Convert.ToInt16(obj), connection, dateFirst, dateLast);
            
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(TR.Address.ToString(), template_ws);

            DataTable dt = TR.archiveDay.Copy();
            int rows = dt.Rows.Count;
            Prepare2(ref dt, ref ws, "Date");
            Prepare(ref dt, ref ws, "Qgvs");
            Prepare(ref dt, ref ws, "Mgvs");
            Prepare(ref dt, ref ws, "tgvs");
            Prepare(ref dt, ref ws, "Mgvsd");
            Prepare(ref dt, ref ws, "Qgvsd");
            Prepare2(ref dt, ref ws, "Q_Tarif");
            Prepare2(ref dt, ref ws, "M_Tarif");
            Prepare(ref dt, ref ws, "QgvsS");
            InsertFormula(ref ws, "nachisleno", rows);

            InsertFormulaParametrs(ref ws, "status", rows);
            InsertFormulaPlata(ref ws, "plata", rows);
             ws.Cells["A1:A1000"].Style.Numberformat.Format = "dd.mm.yyyy HH:MM";
             CheckAndEditNamedRange(ref ws,"startDate",TR.dateStart);
             CheckAndEditNamedRange(ref ws,"stopDate",TR.dateStop);
 

        }

        pck.Workbook.Worksheets.Delete(1);
       

        this.Context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        this.Context.Response.AddHeader("content-disposition", "attachment;  filename=Претензия.xlsx");
        this.Context.Response.BinaryWrite(pck.GetAsByteArray());
    }
        
    
    catch ( Exception ex )
    {
          this.Context.Response.ContentType = "application/json; charset=utf-8";
          this.Context.Response.Write("{\"success\": false, \"message\":\""+ex.Message+ "\"}");
        
    } 
   

    }
    /// <summary>
/// Метод для получения заявок по объекту
/// </summary>
[WebMethod(Description = "Список шаблонов")]
[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
public void GetTemplateList()
{
    try
    {
       
        string [] fileEntries =  new DirectoryInfo(Server.MapPath("/Template/")).GetFiles().Select(o => o.Name).ToArray();
        this.Context.Response.ContentType = "application/json; charset=utf-8";
        this.Context.Response.Write(new JavaScriptSerializer().Serialize(fileEntries));
    }
    catch ( Exception ex )
    {
          this.Context.Response.ContentType = "application/json; charset=utf-8";
          this.Context.Response.Write("{\"success\": false, \"message\":\""+ex.Message+ "\"}");
        
    } 

}
    /// <summary>
/// Метод для управления опросом
/// </summary>
[WebMethod(Description = "Управление опросом")]
[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
public void SendLinesCommand(string action)
{
    DataTable Commlines = new DataTable();
        Log.WriteLineDelegate writeToLog;
        writeToLog = text => { }; // заглушка
        CommSettings cm = new CommSettings("10.235.63.252",10000,"ScadaApi","ScadaApi",10000);
    ServerComm sc = new ServerComm(cm, writeToLog);

    sc.ReceiveBaseTable("commline.dat", Commlines);
    sc.Close();
     List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
      Dictionary<string, object> row;
        for (int i = 0; i < Commlines.Rows.Count; i++)
            {
                string message;
                string[] cmd = new string[2];
                //cmd[0] = "CmdType="+action;
                cmd[0] = "LifeTime=600";
                cmd[1] = "LineNum="+Commlines.Rows[i].Field<Int32>("CommLineNum");
                bool result = Scada.Comm.CommUtils.SaveCmd("C:\\SCADA\\ScadaComm\\Cmd\\","ScadaCommCtrl",action, cmd, out message);

                row = new Dictionary<string, object>();
                row.Add("KPName",Commlines.Rows[i].Field<String>("Name"));
                row.Add("Result",result);
                rows.Add(row);
            } 
    try
    {
           this.Context.Response.ContentType = "application/json; charset=utf-8";
           this.Context.Response.Write(new JavaScriptSerializer().Serialize(rows));
    }
    catch ( Exception ex )
    {
          this.Context.Response.ContentType = "application/json; charset=utf-8";
          this.Context.Response.Write("{\"success\": false, \"message\":\""+ex.Message+ "\"}");
        
    }
}

///// <summary>
///// Метод для получения заявок по объекту
///// </summary>
//[WebMethod(Description = "Получить данные по ХВС")]
//[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
//public void GetMDNodes()
//{
//     XmlRpcClient Client =  new XmlRpcClient { Url = "http://192.168.0.92:5555/"};
//    //Client.Po
//     var request = new XmlRpcRequest("login",new string[]{"admin","admin"});
//    var result = Client.Execute(request);
//    if (result.XmlRpcResponse.IsFault()) {
//        //Console.WriteLine(result.XmlRpcResponse.GetFaultString());
//    }
//     //this.Context.Response.ContentType = "application/json; charset=utf-8";
//      //     this.Context.Response.Write(result.XmlRpcResponse.GetString());
//    string token = result.XmlRpcResponse.GetString();

//    var request_nodes = new XmlRpcRequest("getNodes",token);
//    result = Client.Execute(request_nodes);
//     XmlDocument xm = new XmlDocument();
//     xm.LoadXml(result.XmlRpcResponse.GetString()); // here i got error

//      var productsenum = from p in xm.GetElementsByTagName("record").Cast<XmlElement>()
//                           select p;
//     List <MDObjects> md = new List<MDObjects>();
//       foreach (XmlElement p in productsenum) {
//            MDObjects pobj = new MDObjects();
//            pobj.name= p.GetAttribute("name");
//            pobj.number= p.GetAttribute("number");
//            pobj.id = p.GetAttribute("id");
//            md.Add(pobj);
//        }


//   this.Context.Response.ContentType = "application/json; charset=utf-8";
//   this.Context.Response.Write(new JavaScriptSerializer().Serialize(md));
//    //this.Context.Response.Write(result.XmlRpcResponse.GetString());
//}
            
/// <summary>
/// Метод для получения заявок по объекту
/// </summary>
[WebMethod(Description = "Получить данные по ХВС")]
[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
 public void HVS(string dateFrom, string dateTo)
{
    try
    {
        if (!String.IsNullOrEmpty(dateFrom) && !String.IsNullOrEmpty(dateTo))
        {
            var test = GetHVS(1212, dateFrom, dateTo);
            this.Context.Response.Write(new JavaScriptSerializer().Serialize(test));
        }
    }
      catch ( Exception ex )
    {
          this.Context.Response.ContentType = "application/json; charset=utf-8";
          this.Context.Response.Write("{\"success\": false, \"message\":\""+ex.Message+ "\"}");
        
    }
}
public HVS GetHVS(int id, string dateFrom, string dateTo)
{
    Uri uri = new Uri("https://amr.teleofis.ru/login");
    CookieAwareWebClient MainPageLoad = new CookieAwareWebClient();
    CookieContainer cont = new CookieContainer();

    string MainPageString = MainPageLoad.DownloadString("https://amr.teleofis.ru/login");

    CookieCollection Cookies = MainPageLoad.CookieContainer.GetCookies(uri);
    cont.Add(Cookies);
    var csrf_token =  MainPageString.Substrings("csrf\" value=\"", "\" ", 0);


    var loginClient= new CookieAwareWebClient();
    loginClient.CookieContainer.Add(Cookies);
             var values= new NameValueCollection
           {
             {"username","RempElmash"},
             {"password","RempElmash*#"}
           };

    values.Add("_csrf", csrf_token[0]);
    loginClient.Headers.Add("Accept","text/html, application/xhtml+xml, image/jxr, */*");
    loginClient.Headers.Add("Accept-Encoding","gzip, deflate");
    loginClient.Headers.Add("Accept-Language","ru-RU");
    loginClient.Headers.Add("User-Agent","Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");
     loginClient.UploadValues("https://amr.teleofis.ru/login","POST",values);

    Cookies = loginClient.CookieContainer.GetCookies(uri);
   
    var loadDataSet= new CookieAwareWebClient();
    loadDataSet.CookieContainer.Add(Cookies);
    loadDataSet.DownloadString(uri);
    
    
    //this.Context.Response.Write();

    var TeleofisResponse = JObject.Parse(loadDataSet.DownloadString("http://amr.teleofis.ru/json/datasource/"+id+"/report?fromDate="+dateFrom+"&toDate="+dateTo+"&interval=Day1"));

    var StartValue = from p in TeleofisResponse["archive"] select (double)p["startValue"];
    var EndValue = from p in TeleofisResponse["archive"] select (double)p["endValue"];
    var deltaValue = from p in TeleofisResponse["archive"] select (double)p["deltaValue"];
    
    double pokaz =  EndValue.Last() - StartValue.First();
    double result = 0;
    
    
    foreach(var item in deltaValue)
    {
        result = result + item;
    }
    HVS HVS = new HVS(result, EndValue.Last());
    return HVS;
    }
    //Обновление или вставка объекта из редактора
    private void UpdateUkutInDB(NameValueCollection nv, out int result)
    {
        result = 0;
        int KPNum = Convert.ToInt32(nv["idMD"]);
        using (NpgsqlConnection connection = new NpgsqlConnection(_cs))
        {
            if (!connection.State.HasFlag(ConnectionState.Open))
            {
                connection.Open();
            }
            try
            { 
                StringBuilder sb = new StringBuilder("INSERT into ukut (");
                StringBuilder UPDATE_SET = new StringBuilder(" ");
                foreach (string key in nv)
                {
                    if (key == "webix_operation" || key == "id")
                    {
                        continue;
                    }
                    sb.AppendFormat("\"{0}\",", key);

                }
                sb.Remove(sb.Length - 1, 1);
                sb.AppendFormat(") VALUES (");
                foreach (string key in nv)
                {
                    if(key == "webix_operation" || key == "id")
                    {
                        continue;
                    }
                    if (String.IsNullOrEmpty(nv[key]))
                    {
                        sb.AppendFormat("NULL,");
                        UPDATE_SET.AppendFormat("\"{0}\" = NULL,",key);
                    }
                    else
                    {
                        sb.AppendFormat(" '{0}',",nv[key].ToString());
                        UPDATE_SET.AppendFormat("\"{0}\" = '{1}',", key, nv[key].ToString());
                    }
                }
                sb.Remove(sb.Length-1,1);
                UPDATE_SET.Remove(UPDATE_SET.Length - 1, 1);
                sb.AppendFormat(") ON CONFLICT (\"idMD\") DO UPDATE SET {0}",UPDATE_SET.ToString());

                NpgsqlCommand command = new NpgsqlCommand(sb.ToString(), connection);
                result = command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                this.Context.Response.ContentType = "application/json; charset=utf-8";
                this.Context.Response.Write("{\"success\": false, \"message\":\"" + ex.Message + "\"}");
            }

            finally
            {
                connection.Close();
            }
        }
    }
    //private void InsertUkutInDB(NameValueCollection nv, out int result)
    //{
    //    result = 0;
    //    int KPNum = Convert.ToInt32(nv["idMD"]);
    //    using (NpgsqlConnection connection = new NpgsqlConnection(_cs))
    //    {
    //        if (!connection.State.HasFlag(ConnectionState.Open))
    //        {
    //            connection.Open();
    //        }
    //        try
    //        {
    //            StringBuilder sb = new StringBuilder("INSERT INTO ukut VALUES(");

    //            foreach (string key in nv)
    //            {
    //                if (key == "webix_operation" || key == "id")
    //                {
    //                    continue;
    //                }
    //                if (String.IsNullOrEmpty(nv[key]))
    //                {
    //                    sb.AppendFormat("NULL,", key);
    //                }
    //                else
    //                {
    //                    sb.AppendFormat("'{0}',",nv[key].ToString());
    //                }
    //            }
    //            sb.Remove(sb.Length - 1, 1);
    //            sb.AppendFormat(" )");

    //            NpgsqlCommand command = new NpgsqlCommand(sb.ToString(), connection);
    //            result = command.ExecuteNonQuery();
    //            connection.Close();
    //        }
    //        catch (Exception ex)
    //        {
    //            this.Context.Response.ContentType = "application/json; charset=utf-8";
    //            this.Context.Response.Write("{\"success\": false, \"message\":\"" + ex.Message + "\"}");
    //        }

    //        finally
    //        {
    //            connection.Close();
    //        }
    //    }
    //}
    /// <summary>
    /// Метод для получения заявок по объекту
    /// </summary>
    [WebMethod(Description = "Редактор карты")]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void GetUkuts()
    {
        string result = null;
        //Если idMD не равно нулю, то обновим или вставим запись
        if (!String.IsNullOrWhiteSpace(this.Context.Request.Form["idMD"]))
        {
            if(this.Context.Request.Form["webix_operation"] == "update" || this.Context.Request.Form["webix_operation"] == "insert")
            {
                NameValueCollection nv = this.Context.Request.Form;
                int res = 0;
                UpdateUkutInDB(nv, out res);
                try
                {
                    this.Context.Response.ContentType = "application/json; charset=utf-8";
                    this.Context.Response.Write("{\"success\": true, \"message\":\"success\"}");
                }
                catch(Exception ex)
                {

                    this.Context.Response.ContentType = "application/json; charset=utf-8";
                    this.Context.Response.Write("{\"success\": false, \"message\":\"" + ex.Message + "\"}");
                }
            }
        }
        if (!String.IsNullOrEmpty(this.Context.Request.Params["list"]))
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_cs))
            {
                try
                {
                    connection.Open();
                    NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM ukut WHERE \"idMD\" != 0 ORDER by \"address\"", connection);
                    NpgsqlDataAdapter myDA = new NpgsqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    myDA.Fill(dt);
                    result = JsonConvert.SerializeObject(dt);
                    connection.Close();
                }
                catch
                {
                    throw;
                }
            }
        }

            
        try
        {
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(result);
        }
        catch (Exception ex)
        {
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write("{\"success\": false, \"message\":\"" + ex.Message + "\"}");

        }
    }




}