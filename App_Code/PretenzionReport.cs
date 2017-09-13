using MySql.Data.MySqlClient;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;
using System.Web;
using Npgsql;
/// <summary>
/// Структура описания объекта для обработки архивов суточных
/// </summary>
public struct DayStructElement
{
    public DateTime Date { get; set; }
    public double Qgvs { get; set; }
    public double tgvs { get; set; }
    public double Mgvs { get; set; }   
}
/// <summary>
/// Сводное описание для TeploReport
/// </summary>
public class PretenzionReport
{
       public List<DayStructElement> dayStructs = new List<DayStructElement>();
        public string Abonent { get; set; }
        public string Address { get; set; }
        public string Dogovor { get; set; }
        public string dQotop { get; set; }
        public string dGgvs { get; set; }
        public string Device { get; set; }
        public string gvs { get; set; }
        public string Scheme { get; set; }
        public string Uchet { get; set; }
        public string FormulaSummer { get; set; }
        public int countTrub { get; set; }
        public string FormulaWinter { get; set; }
        public int devType { get; set; }
        public int KPNum { get; set; }
        public string tcw { get; set; }
        public DataTable archiveDay { get; set; }
        public DataTable archiveHour { get; set; }
    public string dateStart { get; set; }
        public string dateStop { get; set; }
     public string column_pod { get; set; }
     public string column_obr { get; set; }
     public string column_gvs { get; set; }
    public string template { get; set; }
    public DataSet ds = new DataSet();
    public string cs = "Server=10.235.63.252;Port=5432;User Id=postgres;Password=ghbdtn;Database=ukut_maps";
    //
    public PretenzionReport(int idMD, NpgsqlConnection connection, string dateFirst, string dateLast)
    {
        NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM ukut WHERE ukut.\"idMD\" = @idMD AND ukut.\"DevType\" IN(1,2,3,4) AND \"column_gvs\" != ' ' ORDER BY \"address\" ASC", connection);
             command.Parameters.AddWithValue("@idMD", idMD);
        NpgsqlDataAdapter myDA = new NpgsqlDataAdapter(command);
        DataTable dt = new DataTable();
        myDA.Fill(dt);
        Abonent =dt.Rows[0]["Abonent"].ToString();
        Dogovor = dt.Rows[0]["Dogovor"].ToString();
        Device = dt.Rows[0]["devName"].ToString() +" №" +dt.Rows[0]["serialNum"].ToString();
        Address = dt.Rows[0]["address"].ToString();
        column_gvs =dt.Rows[0]["column_gvs"].ToString();
        KPNum = Convert.ToInt32(dt.Rows[0]["idMD"]);
        countTrub = Convert.ToInt32(dt.Rows[0]["countTrub"]);
        archiveHour = new DataTable();
        dateStart = dateFirst;
        dateStop = dateLast;
        //Архивы
       archiveDay = GetDayArchivePeriod(countTrub, devType, KPNum, dateFirst, dateLast, column_pod, column_obr, column_gvs, connection);
      
    }
    //Возвращаем таблицу с архивами
    private DataTable GetArhiveForReports(int iDevType, int KPNum, string dateFirst, string dateLast, string column_pod, string column_obr, string column_gvs, NpgsqlConnection connection)
    {
        string[] gvs = column_gvs.Split('_');
        string daydata_table = "daydata";
       if(iDevType == 2)
       {
           daydata_table = "daydata942";
       }
        DataTable dt = new DataTable();
        bool res = false;
        StringBuilder sb = new StringBuilder();

            sb.AppendFormat("SELECT Date, \"{0}_P1\", \"{0}_t1\", \"{0}_M1\", \"{0}_P2\", \"{0}_t2\", \"{0}_M2\", (\"{0}_M1\"-\"{0}_M2\") AS \"G_GVS\", \"{0}_Q\", \"{0}_Ti\" FROM {1} WHERE \"KPNum\"={2} AND \"Date\" between {3}::date AND {4}::date",gvs[0],daydata_table,KPNum,dateFirst,dateLast);

        NpgsqlDataAdapter myDA = new NpgsqlDataAdapter(sb.ToString(), connection);
        myDA.Fill(dt);
        return dt;
    }
        //Возвращаем таблицу с архивами
    private DataTable GetDayArchivePeriod(int countTrub, int iDevType, int KPNum, string dateFirst, string dateLast, string column_pod, string column_obr, string column_gvs, NpgsqlConnection connection)
    {
        string[] gvs = column_gvs.Split('_');
        string daydata_table = "daydata";
        string hourdata_table = "hourdata";
        //942
       if(iDevType == 2)
       {
           daydata_table = "daydata942";
           hourdata_table = "hourdata942";
       }
        //943.10
       if(iDevType == 3)
       {
           daydata_table = "daydata9310";
           hourdata_table = "hourdata94310";
       }
        //944
       if(iDevType == 4)
       {
           daydata_table = "daydata944";
           hourdata_table = "hourdata944";
       }
        DataTable dt = new DataTable();
        StringBuilder sb = new StringBuilder();
        if(connection.State != ConnectionState.Open)
        {
            connection.Open();
        }
        sb.AppendFormat(
            countTrub == 4
                ? "SELECT Date, Mgvs, tgvs, Qgvs, Mgvsd, Qgvsd, '1589,18' AS Q_Tarif, '23,54' AS M_Tarif, (SELECT SUM({1}.{0}_Q) FROM {1} WHERE KPNum={2} AND Date between {3} AND {4}) AS QgvsS  FROM (" +
                  "(SELECT Date_Format({1}.Date, \"%Y-%m-%d\") AS Date, {1}.{0}_t1 AS tgvs, ({1}.{0}_M1-{1}.{0}_M2) AS Mgvs, {1}.{0}_Q as Qgvs, ({1}.{0}_M1-{1}.{0}_M2) AS Mgvsd, {1}.{0}_Q as Qgvsd FROM {1} WHERE KPNum={2} AND Date between {3} AND {4}) UNION ALL (SELECT {5}.Date, {5}.{0}_t1 AS tgvs, ({5}.{0}_M1-{5}.{0}_M2) AS Mgvs, {5}.{0}_Q as Qgvs, (SELECT ({1}.{0}_M1-{1}.{0}_M2)  FROM {1} WHERE KPNUM = {2} AND Date = Date_Format({5}.Date, '%Y-%m-%d 00:00:00')) AS Mgvsd, (SELECT {1}.{0}_Q FROM {1} WHERE KPNUM = {2} AND Date = Date_Format({5}.Date, '%Y-%m-%d 00:00:00')) AS Qgvsd FROM {5} WHERE KPNum={2} AND Date between {3} AND {4})) AS UnionTable ORDER BY Date ASC"

                : "SELECT Date, Mgvs, tgvs, Qgvs, Mgvsd, Qgvsd, '1589,18' AS Q_Tarif, '23,54' AS M_Tarif, (SELECT SUM({1}.{0}_Q) FROM {1} WHERE KPNum={2} AND Date between {3} AND {4}) AS QgvsS FROM (" +
                "(SELECT Date_Format({1}.Date, \"%Y-%m-%d\") AS Date, {1}.{0}_t1 AS tgvs, {1}.{0}_M2 AS Mgvs, {1}.{0}_Q as Qgvs, {1}.{0}_M2 AS Mgvsd, {1}.{0}_Q as Qgvsd FROM {1} WHERE KPNum={2} AND Date between {3} AND {4}) UNION ALL (SELECT {5}.Date, {5}.{0}_t1 AS tgvs, {5}.{0}_M2 AS Mgvs, {5}.{0}_Q as Qgvs, (SELECT {1}.{0}_M2  FROM {1} WHERE KPNUM = {2} AND Date = Date_Format({5}.Date, '%Y-%m-%d 00:00:00')) AS Mgvsd, (SELECT {1}.{0}_Q FROM {1} WHERE KPNUM = {2} AND Date = Date_Format({5}.Date, '%Y-%m-%d 00:00:00')) AS Qgvsd FROM {5} WHERE KPNum={2} AND Date between {3} AND {4}) ) AS UnionTable ORDER BY Date ASC",
            gvs[0], daydata_table, KPNum, dateFirst, dateLast, hourdata_table);
       
       
         NpgsqlDataAdapter myDA = new NpgsqlDataAdapter(sb.ToString(), connection);
         myDA.Fill(dt);
        connection.Close();
        return dt;
    }

}