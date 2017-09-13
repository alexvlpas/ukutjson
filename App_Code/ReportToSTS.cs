using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MySql.Data.MySqlClient;
using System.Web.Script.Services;
using System.Data;
using System.Collections;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using System.Web.Script.Serialization;
using Npgsql;
using System.Text;
/// <summary>
/// Сводное описание для ReportToSTS
/// </summary>
public class Reports
{   int _countTrub = 2;
    string _objects = null;
    public string cs = "Server=10.235.63.252;Port=5432;User Id=postgres;Password=ghbdtn;Database=ukut_maps;Pooling=true";
    public Reports()
	{
		//
		// TODO: добавьте логику конструктора
		//
	}
    public Reports(int countTrub, string objects)
	{
		//
		// TODO: добавьте логику конструктора
		//
        _countTrub = countTrub;
        _objects = objects;
	}
    private void ChangeColumnType(System.Data.DataTable dt, string p, Type type)
    {
        dt.Columns.Add(p + "_new", type);
        foreach (System.Data.DataRow dr in dt.Rows)
        {   // Will need switch Case for others if Date is not the only one.
            dr[p + "_new"] = DateTime.FromOADate(double.Parse(dr[p].ToString())); // dr[p].ToString();
        }
        dt.Columns.Remove(p);
        dt.Columns[p + "_new"].ColumnName = p;
    }
    /// <summary>
    /// Получить Архивы (номер КП, тип архива, дата от, дата до)
    /// </summary>
    public bool GetArchivesForAnaliz(ObjectsAnaliz OB, string arc, string dateFirst, string dateLast, NpgsqlConnection connection, out DataTable dt, int border_gvs)
    {
        dt = new DataTable();
        bool res = false;
        //
        string day_table = "daydata";
        string hour_table = "hourdata";

        string TB1_t3 = "ROUND(TB1_t3,2) AS TB1_t3";
        if (OB.idDevName == 2)
        {
            day_table = "daydata942";
            hour_table = "hourdata942";
            TB1_t3 = "SPACE(1)";
        }
        //string sQuery = "";
        StringBuilder sQuery = new StringBuilder(" ");
        string border_expression = " AND " + OB.column_gvs + " <=" + border_gvs;
       
        if(border_gvs == 0)
        {
            border_expression = " ";
        }
        if(OB.countTrub == 4)
        {
            OB.column_M = "(TB2_M1-TB2_M2)";
        }
        if (arc == "days")
        {
            sQuery.AppendFormat("SELECT EXTRACT(DAY FROM Date) AS Day, EXTRACT(HOUR FROM Date) AS Hour, Date, ROUND({0}) AS t1, {1} AS M1, {2} AS Q, from {3} WHERE KPNum = {4} {5} AND {0} > 0 AND Date between {6} AND {7}", OB.column_gvs, OB.column_M, OB.column_Q, day_table, OB.KPNum, border_expression, dateFirst, dateLast);
            //sQuery = "SELECT Date, 'Низкая температура ГВС' AS KPNum, ROUND(TB1_t1,2) AS TB1_t1, ROUND(TB1_t2,2) AS TB1_t2, " + TB1_t3 + ", ROUND(TB1_M1,2) AS TB1_M1, ROUND(TB1_M2,2) AS TB1_M2, ROUND(TB2_M1,2) AS TB2_M1, ROUND(TB2_M2,2) AS TB2_M2, ROUND(TB2_t1,2) AS TB2_t1, ROUND(TB2_t2,2) AS TB2_t2 from " + day_table + " WHERE KPNum = " + OB.KPNum + " AND " + OB.column_gvs + " <=" + border_gvs + " AND " + OB.column_gvs + " > 0" + " AND Date between " + dateFirst + " AND " + dateLast;
            //sQuery = @"SELECT EXTRACT(DAY FROM ""Date"") AS ""Day"", EXTRACT(HOUR FROM ""Date"") AS ""Hour"", ""Date"",  ROUND(" + OB.column_gvs + ") AS ""t1"",  " + OB.column_M + " AS ""M1"", "" + OB.column_Q + "" AS ""Q"" from "" + day_table + "" WHERE ""KPNum"" = "" + OB.KPNum + border_expression + "" AND "" + OB.column_gvs + "" > 0" + " AND Date between " + dateFirst + " AND " + dateLast;
        }
        else
        {
            sQuery.AppendFormat("SELECT EXTRACT(DAY FROM {8}Date{8}) AS {8}Day{8}, EXTRACT(HOUR FROM {8}Date{8}) AS {8}Hour{8}, {8}Date{8}, ROUND({8}{0}{8}) AS {8}t1{8}, {8}{1}{8} AS {8}M1{8}, {8}{2}{8} AS {8}Q{8} from {3} WHERE {8}KPNum{8} = {4} {5} AND {8}{0}{8} > 0 AND {8}Date{8} between '{6}'::date AND '{7}'::date", OB.column_gvs, OB.column_M, OB.column_Q, hour_table, OB.KPNum, border_expression, dateFirst, dateLast,'"');

            //sQuery = "SELECT Date, 'Низкая температура ГВС' AS KPNum, ROUND(TB1_t1,2) AS TB1_t1, ROUND(TB1_t2,2) AS TB1_t2, "+TB1_t3+", ROUND(TB1_M1,2) AS TB1_M1, ROUND(TB1_M2,2) AS TB1_M2, ROUND(TB2_M1,2) AS TB2_M1, ROUND(TB2_M2,2) AS TB2_M2, ROUND(TB2_t1,2) AS TB2_t1, ROUND(TB2_t2,2) AS TB2_t2 from " + hour_table + " WHERE KPNum = " + OB.KPNum + " AND " + OB.column_gvs + " <=" + border_gvs + " AND " + OB.column_gvs + " > 0" + " AND Date between " + dateFirst + " AND " + dateLast;
            //sQuery = @"SELECT EXTRACT(DAY FROM ""Date"") AS ""Day"", EXTRACT(HOUR FROM ""Date"") AS ""Hour"", ""Date"", ROUND(" + OB.column_gvs + ") AS ""t1"",  " + OB.column_M + " AS ""M1"", ""TB2_Q"" AS " + OB.column_Q + "  from " + hour_table + " WHERE KPNum = " + OB.KPNum + border_expression + " AND " + OB.column_gvs + " > 0" + " AND Date between " + dateFirst + " AND " + dateLast;
        }

        using (NpgsqlDataAdapter myDA = new NpgsqlDataAdapter(sQuery.ToString(), connection))
        {
            try
            {
                myDA.Fill(dt);
                if (dt.Rows.Count != 0)
                {
                    res = true;
                }
            }
            catch (Exception ex)
            {
                return res;
            }
        }
        return res;
    }
    /// <summary>
    /// Получить Архивы (номер КП, тип архива, дата от, дата до)
    /// </summary>
    public bool GetArchivesForAnalizPeretop(ObjectsAnaliz OB, string arc, string dateFirst, string dateLast, NpgsqlConnection connection, out DataTable dt, int border_gvs)
    {
        dt = new DataTable();
        bool res = false;
        //
        string day_table = "daydata";
        string hour_table = "hourdata";

        string TB1_t3 = "ROUND(TB1_t3,2) AS TB1_t3";
        if (OB.idDevName == 2)
        {
            day_table = "daydata942";
            hour_table = "hourdata942";
            TB1_t3 = "SPACE(1)";
        }
        string sQuery = "";
        string border_expression = " AND " + OB.column_gvs + " <=" + border_gvs;
        if(border_gvs == 0)
        {
            border_expression = " ";
        }

        if (arc == "days")
        {
            //sQuery = "SELECT Date, 'Низкая температура ГВС' AS KPNum, ROUND(TB1_t1,2) AS TB1_t1, ROUND(TB1_t2,2) AS TB1_t2, " + TB1_t3 + ", ROUND(TB1_M1,2) AS TB1_M1, ROUND(TB1_M2,2) AS TB1_M2, ROUND(TB2_M1,2) AS TB2_M1, ROUND(TB2_M2,2) AS TB2_M2, ROUND(TB2_t1,2) AS TB2_t1, ROUND(TB2_t2,2) AS TB2_t2 from " + day_table + " WHERE KPNum = " + OB.KPNum + " AND " + OB.column_gvs + " <=" + border_gvs + " AND " + OB.column_gvs + " > 0" + " AND Date between " + dateFirst + " AND " + dateLast;
            sQuery = "SELECT CAST(EXTRACT(DAY FROM Date) AS UNSIGNED) AS Day, HOUR(Date) AS Hour, Date,  ROUND(" + OB.column_gvs + ",2) AS t1,  " + OB.column_M + " AS M1, " + OB.column_Q + " AS Q from " + day_table + " WHERE KPNum = " + OB.KPNum + border_expression + " AND " + OB.column_gvs + " > 73.5" + " AND "+ OB.column_M+ " > 0 AND Date between " + dateFirst + " AND " + dateLast;
        }
        else
        {
            //sQuery = "SELECT Date, 'Низкая температура ГВС' AS KPNum, ROUND(TB1_t1,2) AS TB1_t1, ROUND(TB1_t2,2) AS TB1_t2, "+TB1_t3+", ROUND(TB1_M1,2) AS TB1_M1, ROUND(TB1_M2,2) AS TB1_M2, ROUND(TB2_M1,2) AS TB2_M1, ROUND(TB2_M2,2) AS TB2_M2, ROUND(TB2_t1,2) AS TB2_t1, ROUND(TB2_t2,2) AS TB2_t2 from " + hour_table + " WHERE KPNum = " + OB.KPNum + " AND " + OB.column_gvs + " <=" + border_gvs + " AND " + OB.column_gvs + " > 0" + " AND Date between " + dateFirst + " AND " + dateLast;
            sQuery = "SELECT CAST(EXTRACT(DAY FROM Date) AS UNSIGNED) AS Day, HOUR(Date) AS Hour, Date, ROUND(" + OB.column_gvs + ",2) AS t1,  " + OB.column_M + " AS M1, TB2_Q AS " + OB.column_Q + "  from " + hour_table + " WHERE KPNum = " + OB.KPNum + border_expression + " AND " + OB.column_gvs + " > 73.5" + " AND "+OB.column_M+" > 0 AND Date between " + dateFirst + " AND " + dateLast;
        }

        NpgsqlDataAdapter myDA = new NpgsqlDataAdapter(sQuery, connection);
        myDA.Fill(dt);
        connection.Close();
        connection.Dispose();
        if (dt.Rows.Count != 0)
        {
            res = true;
        }
        return res;
    }
    /// <summary>
    /// Получения архива для метода низкой температуры ГВС
    /// </summary>
    public DataSet GetCountGVS(string arc, string dateFirst, string dateLast, string border, int _countTrub, string _objects)
    {
        List<SummReport> SUM_REPORT = new List<SummReport>();
        string filename = String.Format("Отчет по ГВС для СТК-{0}-{1}.xlsx", dateFirst,dateLast);
        string Header = String.Format("attachment;  filename={0}",filename);
        int border_gvs = Convert.ToInt16(border);
        //Перечень всех объектов
        List<ObjectsAnaliz> arc_report = new List<ObjectsAnaliz>();
        NpgsqlConnection connection = new NpgsqlConnection(cs);
        connection.Open();
        NpgsqlCommand command = new NpgsqlCommand(@"SELECT * FROM Ukut WHERE Ukut.""idMD"" != 0 AND (Ukut.""column_gvs"" != ' ' OR Ukut.""column_gvs"" is NOT NULL) AND Ukut.""devType"" IN(1,2) AND Ukut.""countTrub"" >=@countTrub AND ""idMD"" IN ("+_objects+")", connection);
        command.Parameters.AddWithValue("@countTrub",_countTrub);
      //  command.Prepare();
        NpgsqlDataReader reader = command.ExecuteReader();
        ObjectsAnaliz arc_rep = new ObjectsAnaliz();
        while (reader.Read())
        {
            arc_rep.address = Convert.ToString(reader["address"]).Replace('.',' ').Replace('/','-');
            arc_rep.KPNum = Convert.ToInt32(reader["idMD"]);
            arc_rep.idUkut = Convert.ToInt32(reader["idUkut"]);
            arc_rep.idDevName = Convert.ToInt32(reader["DevType"]);
            arc_rep.column_gvs = reader["column_gvs"].ToString();
            arc_rep.HID = Convert.ToInt32(reader["HID"]);
            arc_rep.geu = (reader["geu"] != DBNull.Value) ? Convert.ToString(reader["geu"]): "0";
            arc_rep.countTrub = (reader["countTrub"] != DBNull.Value) ? Convert.ToInt32(reader["countTrub"]): 0;
            arc_rep.RemoveSpaces();
            arc_rep.GetColumn();
            arc_report.Add(arc_rep);
        }
        reader.Close();
        ArrayList dt_list = new ArrayList();
        //connection.Close();
        //connection.Dispose();
        DataTable dateArc = new DataTable();
        DataSet ds = new DataSet();
        //Переберем каждый объект
        foreach (ObjectsAnaliz archive_record in arc_report)
        {      
            int i = 0;
            bool res = GetArchivesForAnaliz(archive_record, arc, dateFirst, dateLast, connection, out dateArc, border_gvs);
            if (res)
            {
                dateArc.TableName = archive_record.address;
                dateArc.Namespace = archive_record.HID.ToString();
                ds.Tables.Add(dateArc);
            }
        }
        connection.Close();
        connection.Dispose();
        return ds;
    }
    /// <summary>
    /// Получения архива для метода низкой температуры ГВС
    /// </summary>
    public DataSet GetPeretopGVS(string arc, string dateFirst, string dateLast, string border, string _objects)
    {
        List<SummReport> SUM_REPORT = new List<SummReport>();
        string filename = String.Format("Отчет по перетопам ГВС-{0}-{1}.xlsx", dateFirst,dateLast);
        string Header = String.Format("attachment;  filename={0}",filename);
        int border_gvs = Convert.ToInt16(border);
        //Перечень всех объектов
        List<ObjectsAnaliz> arc_report = new List<ObjectsAnaliz>();
        NpgsqlConnection connection = new NpgsqlConnection(cs);
        connection.Open();
        NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM Ukut WHERE Ukut.idMD != 0 AND (Ukut.column_gvs != ' ' OR Ukut.column_gvs is NOT NULL) AND Ukut.DevType IN(1,2) AND Ukut.countTrub >=2 AND idMD IN ("+_objects+")", connection);
        NpgsqlDataReader reader = command.ExecuteReader();
        ObjectsAnaliz arc_rep = new ObjectsAnaliz();
        while (reader.Read())
        {
            arc_rep.address = Convert.ToString(reader["address"]).Replace('.',' ').Replace('/','-');
            arc_rep.KPNum = Convert.ToInt32(reader["idMD"]);
            arc_rep.idUkut = Convert.ToInt32(reader["idUkut"]);
            arc_rep.idDevName = Convert.ToInt32(reader["devType"]);
            arc_rep.column_gvs = reader["column_gvs"].ToString();
            arc_rep.geu = (reader["HID"] != DBNull.Value) ? Convert.ToString(reader["HID"]): "0";
            arc_rep.countTrub = (reader["countTrub"] != DBNull.Value) ? Convert.ToInt32(reader["countTrub"]): 0;
            arc_rep.RemoveSpaces();
            arc_rep.GetColumn();
            arc_report.Add(arc_rep);
        }
        ArrayList dt_list = new ArrayList();
        connection.Close();
        connection.Dispose();
        DataTable dateArc = new DataTable();
        DataSet ds = new DataSet();
        //Переберем каждый объект
        foreach (ObjectsAnaliz archive_record in arc_report)
        {      
            int i = 0;
            bool res = GetArchivesForAnalizPeretop(archive_record, arc, dateFirst, dateLast, connection, out dateArc, border_gvs);
            if (res)
            {
                dateArc.TableName = archive_record.address;
                dateArc.Namespace = archive_record.HID.ToString();
                ds.Tables.Add(dateArc);
            }
        }
        return ds;
    }
}