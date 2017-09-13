using MySql.Data.MySqlClient;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
/// Сводное описание для TeploReport
/// </summary>
public class TeploReport
{
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
    public string Prim { get; set; }
        public int devType { get; set; }
        public int KPNum { get; set; }
        public string tcw { get; set; }
        public DataTable archive { get; set; }
     public string column_pod { get; set; }
     public string column_obr { get; set; }
     public string column_gvs { get; set; }
    public string template { get; set; }
    public DataSet ds = new DataSet();
    public string cs = @"server=10.235.63.252;userid=operator;password=ghbdtn;database=ukut_maps;Character Set=utf8;default command timeout=60000;";
    //
    public TeploReport(int idMD, MySqlConnection connection, string dateFirst, string dateLast)
    {
        MySqlCommand command = new MySqlCommand(@"SELECT * FROM ukut WHERE ukut.idMD = @idMD AND ukut.DevType IN(1,2,4) AND template !=' ' ORDER BY address ASC", connection);
             command.Prepare();
             command.Parameters.AddWithValue("@idMD", idMD);
        MySqlDataAdapter myDA = new MySqlDataAdapter(command);
        DataTable dt = new DataTable();
        myDA.Fill(dt);
        Abonent =dt.Rows[0]["Abonent"].ToString();
        Dogovor = dt.Rows[0]["Dogovor"].ToString();
        Device = dt.Rows[0]["devName"].ToString() +" №" +dt.Rows[0]["serialNum"].ToString();
        Address = dt.Rows[0]["address"].ToString();
        dQotop = dt.Rows[0]["dQotop"].ToString();
        dGgvs = dt.Rows[0]["dGgvs"].ToString();
       // gvs = dt.Rows[0]["gvs"].ToString();
        devType = Convert.ToInt16(dt.Rows[0]["devType"]);
        KPNum = Convert.ToInt16(dt.Rows[0]["idMD"]);
        FormulaSummer = dt.Rows[0]["formula_summer"].ToString();
        FormulaWinter = dt.Rows[0]["formula_winter"].ToString();
         countTrub =Convert.ToInt32(dt.Rows[0]["countTrub"]);
        column_pod =dt.Rows[0]["column_pod"].ToString();
        column_obr =dt.Rows[0]["column_obr"].ToString();
        column_gvs =dt.Rows[0]["column_gvs"].ToString();
        FormulaWinter =dt.Rows[0]["formula_winter"].ToString();
        FormulaSummer =dt.Rows[0]["formula_summer"].ToString();
        tcw =dt.Rows[0]["tcw"].ToString();
        Uchet =dt.Rows[0]["uchet"].ToString();
        Scheme =dt.Rows[0]["scheme"].ToString();
        //Шаблон 
        template =dt.Rows[0]["template"].ToString();
        archive = GetArhiveForReports(devType, KPNum, dateFirst, dateLast, column_pod, column_obr, column_gvs, connection);
    }
    //Возвращаем таблицу с архивами
    private DataTable GetArhiveForReports(int iDevType, int KPNum, string dateFirst, string dateLast, string column_pod, string column_obr, string column_gvs,MySqlConnection connection)
    {
        string[] pod = column_pod.Split('_');
        string[] obr = column_obr.Split('_');
        string[] gvs = column_gvs.Split('_');
        string daydata_table = "daydata";
       if(iDevType == 2)
       {
           daydata_table = "daydata942";
       }
       if(iDevType == 4)
       {
           daydata_table = "daydata94310";
       }
       if(iDevType == 5)
       {
           daydata_table = "daydata944";
       }
        DataTable dt = new DataTable();
        bool res = false;
        StringBuilder sb = new StringBuilder();
        /*
        if(countTrub == 2)
        {
               sb.AppendFormat("SELECT Date, {0}_P1, {0}_t1, {0}_M1, {1}_P2, {1}_t2, {1}_M2, {1}_Q, {2}_P1, {2}_t1, {2}_M1, {2}_P2, {2}_t2, {2}_M2, {2}_Q, {0}_Ti FROM {3} WHERE KPNum={4} AND Date between {5} AND {6}",pod[0], obr[0],gvs[0],daydata_table,KPNum,dateFirst,dateLast);
        }
        if(countTrub == 3)
        {
               sb.AppendFormat("SELECT Date, {0}_P1, {0}_t1, {0}_M1, {1}_P2, {1}_t2, {1}_M2, {1}_Q, {2}_P1, {2}_t1, {2}_M1, {2}_Q, {0}_Ti FROM {3} WHERE KPNum={4} AND Date between {5} AND {6}",pod[0], obr[0],gvs[0],daydata_table,KPNum,dateFirst,dateLast);
        }
         * */
     
            sb.AppendFormat("SELECT Date, {0}_P1, {0}_t1, {0}_M1, {1}_P2, {1}_t2, {1}_M2, {1}_Q, {2}_P1, {2}_t1, {2}_M1, {2}_P2, {2}_t2, {2}_M2, ({2}_M1-{2}_M2) AS G_GVS, {2}_Q, {0}_Ti FROM {3} WHERE KPNum={4} AND Date between {5} AND {6}",pod[0], obr[0],gvs[0],daydata_table,KPNum,dateFirst,dateLast);
        


        
       // MySqlCommand command = new MySqlCommand("SELECT Date, @pod_P1, @pod_t1, @pod_M1", connection);
        //command.Prepare();
        //command.Parameters.AddWithValue("@pod", idMD);
        
        
        
        //
        string day_table = "daydata";
        string sQuery = "";
            sQuery = "SELECT Date, TB1_P1, TB1_t1, TB1_M1, TB1_P2, TB1_t2, TB1_M2, TB1_Q, TB2_P1, TB2_t1, TB2_M1, TB2_P2, TB2_t2, TB2_M2, TB2_Q, TB2_Qg, TB2_Ti from " + day_table + " WHERE KPNum = " + KPNum + " AND Date between " + dateFirst + " AND " + dateLast;

        if (iDevType == 2)
        {
            day_table = "daydata942";
            sQuery = "";
            sQuery = "SELECT Date, TB1_P1, TB1_t1, TB1_M1, TB1_P2, TB1_t2, TB1_M2, TB1_Q, TB2_P1, TB2_t1, TB2_M1, TB2_P2, TB2_t2, TB2_M2, TB2_Q, TB2_Ti from " + day_table + " WHERE KPNum = " + KPNum + " AND Date between " + dateFirst + " AND " + dateLast;
        }

        MySqlDataAdapter myDA = new MySqlDataAdapter(sb.ToString(), connection);
        myDA.Fill(dt);
       /*
        if (dt.Rows.Count != 0)
        {
            res = true;
        }
        return res;
        * */
        return dt;
    }
    /*
    public byte[] DumpExcel(DataSet ds, FileInfo template, List<Kartochka> objects_list)
    {   
        //var file = new FileInfo("c:/test/1.xlsx");
        ExcelPackage pck = new ExcelPackage(template, false);
        //ExcelPackage pck2 = new ExcelPackage(template, false);
        //{
            //ExcelWorksheet template_ws = pck.Workbook.Worksheets[1];
            // ExcelPackage pck = new ExcelPackage(fileInfo, template);
            //foreach(DataTable tbl in ds.Tables) 
           foreach(Kartochka kr in objects_list)
            {
               // pck.Workbook.Worksheets.
            //Create the worksheet
            ExcelWorksheet template_ws = pck.Workbook.Worksheets[1];
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(kr.Address, template_ws);
            //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
            ws.Cells["A18"].LoadFromDataTable(kr.archive, false);
            ws.Cells["A1:A100"].Style.Numberformat.Format = "dd.mm.yyyy";
            ws.Cells["H3"].Value = kr.Address;
            ws.Cells["H5"].Value = kr.dQotop;
            ws.Cells["H6"].Value = kr.dGgvs;
           // ws.Cells["H6"].Value = kr.gvs;
            //ws.Calculate();
           // list_tables.Add(pck);
             }
             //pck.Workbook.Worksheets.Delete(1);
            //Write it back to the client
           // this.Context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
           // this.Context.Response.AddHeader("content-disposition", "attachment;  filename=ExcelDemo.xlsx");
            //this.Context.Response.BinaryWrite(pck.GetAsByteArray());
            //pck.Save();
            pck.Workbook.Worksheets.Delete(1);
           // pck.Stream.Flush();
            return pck.GetAsByteArray();
        //}
          
    }
     * */

}