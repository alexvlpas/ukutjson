using OfficeOpenXml;
using OfficeOpenXml.Style;
//using OpenExcel.OfficeOpenXml;
//using OpenExcel.OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

/// <summary>
/// Сводное описание для Objects
/// </summary>

    public struct ObjectsAnaliz
    {
        public int idUkut { get; set; }
        public string address { get; set; }
        public int KPNum { get; set; }
        public int idDevName { get; set; }
        public string column_gvs { get; set; }
        public string column_Q { get; set; }
        public string column_M { get; set; }
        public int HID { get; set; }
        public string geu { get; set; }
        public int countTrub { get; set; }
        //Удалить пробелы
        public void RemoveSpaces()
        {
            address = address.Replace("  ", " ");

            if (address.Contains("  "))
                RemoveSpaces();
        }
        //Получить колонки для ГВС
        public void GetColumn()
        {
            column_Q = String.Format("TB{0}_Q", column_gvs[2]);
            column_M = String.Format("TB{0}_M{1}",column_gvs[2], column_gvs[5]);

        }

    }
    public  struct SummReport
    {
        public string address;
        public string sum;
        public int HID;
        public int geu;
       
    }
    class EPPlus
    {
         
         FileInfo newFile;
        FileInfo templateFile;
        DataSet _ds;

        public ExcelPackage xlPackage;
        public string _ErrorMessage;
        public byte[] res;
        public EPPlus(string templateFilePath, DataSet ds, string period, string dogovor)
        {
            //newFile = new FileInfo(@filePath);
            templateFile = new FileInfo(@templateFilePath);

            _ds = ds;
            _ErrorMessage = string.Empty;

            CreateFileWithTemplate(period,dogovor);

        }

        private bool CreateFileWithTemplate(string period, string dogovor)
        {
            try
            {
                _ErrorMessage = string.Empty;

                using (xlPackage = new ExcelPackage(templateFile, true))
                {
                    List<SummReport> ListMS = new List<SummReport>();
                    int i = 0;
                    foreach (DataTable dt in _ds.Tables)
                    {
                        SummReport SM = new SummReport();
                        AddSheet(xlPackage, dt, i, dt.TableName, ref SM);
                        i++;
                        ListMS.Add(SM);
                    }
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.First();
                    //xlPackage.Workbook.Calculate();
                    xlPackage.Workbook.Worksheets.Delete(worksheet);
                    ExcelWorksheet SVOD =  xlPackage.Workbook.Worksheets.Add("Сводная");
                    int g = 4;

                    SVOD.Cells["A1:D1"].Merge = true;
                    SVOD.Column(1).Width = 6.5;
                    SVOD.Column(2).Width = 29;
                    SVOD.Column(3).Width = 27;
                    SVOD.Column(4).Width = 17;
                    SVOD.Row(1).Height = 80;
                    SVOD.Row(3).Height = 60;
                    SVOD.Row(3).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    SVOD.Cells["A1:D3"].Style.WrapText = true;
                    SVOD.Cells["A1:D3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    SVOD.Cells["A1:D3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    SVOD.Cells["A1:D1"].Value = @"перечень  многоквартирных  жилых  домов, в которых  установлены   общедомовые  приборы учета  тепловой энергии с суммами  перерасчета  платы  за   теплоэнергию по договору энергоснабжения № "+dogovor+" за  период "+period ; 
                   
                    SVOD.Cells["A3"].Value = "№ п.п.";
                    SVOD.Cells["B3"].Value = "адрес   многоквартирного жилого дома";
                    SVOD.Cells["C3"].Value = "сумма  перерасчета  платы за  теплоэнергию";
                    //SVOD.Cells["D3"].Value = "Код дома";
                    foreach(SummReport SL in ListMS)
                    {

                        string cell_number = string.Format("A{0}", g);
                        string cellAddress = string.Format("B{0}", g);
                        string cellRange = string.Format("C{0}", g);
                        string cellTemp = string.Format("D{0}", g);
                        SVOD.Cells[cell_number].Value = g-3;
                        SVOD.Cells[cellAddress].Value = SL.address;
                        SVOD.Cells[cellTemp].Value = SL.HID;
                        using (ExcelRange rng = SVOD.Cells[cellRange])
                        {
                            string previons_cell = string.Format("'{0}'!{1}", SL.address, SL.sum);
                            rng.Formula = previons_cell;
                        }
                        g++;
                    }
                   
                    ///* Set title, Author.. */
                    //xlPackage.Workbook.Properties.Title = "Title: Office Open XML Sample";
                    //xlPackage.Workbook.Properties.Author = "Author: Muhammad Mubashir.";
                    ////xlPackage.Workbook.Properties.SetCustomPropertyValue("EmployeeID", "1147");
                    //xlPackage.Workbook.Properties.Comments = "Sample Record Details";
                    //xlPackage.Workbook.Properties.Company = "TRG Tech.";



                    ///* Save */
                    //xlPackage.Save();
                    res = xlPackage.GetAsByteArray();
                }
                return true;
            }
            catch (Exception ex)
            {
                _ErrorMessage = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// This AddSheet method generates a .xlsx Sheet with your provided Template file, //DataTable and SheetIndex.
        /// </summary>
        public static void AddSheetWithTemplate(ExcelPackage xlApp, DataTable dt, int SheetIndex)
        {
            string _SheetName = dt.TableName;
            ExcelWorksheet worksheet = xlApp.Workbook.Worksheets[SheetIndex];
            /* WorkSheet */
            if (SheetIndex == 0)
            {
                worksheet = xlApp.Workbook.Worksheets[SheetIndex + 1]; // add a new worksheet to the empty workbook
            }
            else
            {
                worksheet = xlApp.Workbook.Worksheets[SheetIndex]; // add a new worksheet to the empty workbook
            }
            

            if (worksheet == null)
            {
                worksheet = xlApp.Workbook.Worksheets.Add(_SheetName); // add a new worksheet to the empty workbook    
            }
            else
            {

            }

            /* Load the datatable into the sheet, starting from cell A1. Print the column names on row 1 */
           // worksheet.Cells["A7"].LoadFromDataTable(dt, true);


        }


        private static void AddSheet(ExcelPackage xlApp, DataTable dt, int Index, string sheetName, ref SummReport SM)
        {
            string _SheetName = string.Empty;

            if (string.IsNullOrEmpty(sheetName) == true)
            {
                _SheetName = string.Format("Sheet{0}", Index.ToString());
            }
            else
            {
                _SheetName = sheetName;
            }

            /* WorkSheet */
            ExcelWorksheet worksheet = xlApp.Workbook.Worksheets[_SheetName]; // add a new worksheet to the empty workbook
            if (worksheet == null)
            {
                worksheet = xlApp.Workbook.Worksheets.Add(_SheetName,xlApp.Workbook.Worksheets.First()); // add a new worksheet to the empty workbook    
            }
            else
            {

            }



            /* Load the datatable into the sheet, starting from cell A1. Print the column names on row 1 */
            worksheet.Cells["A7"].LoadFromDataTable(dt, false);
            worksheet.Cells["A2"].Value = "по адресу: "+sheetName;

           
            SM.address = sheetName;
            SM.HID = Convert.ToInt32(dt.Namespace);

            int rowCount = dt.Rows.Count;
            int colCount = dt.Columns.Count;





            #region Set Column Type to Date using LINQ.
            
            IEnumerable<int> dateColumns = from DataColumn d in dt.Columns
                                           where d.ColumnName == "Day" || d.ColumnName == "Hour"
                                           select d.Ordinal + 1;

            foreach (int dc in dateColumns)
            {
                worksheet.Cells[2, dc, rowCount + 1, dc].Style.Numberformat.Format = "0";
            }
            

            #endregion
            #region Set Column Type to Date using LOOP.

            /* Set Column Type to Date. */
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if ((dt.Columns[i].DataType).FullName == "System.DateTime" && (dt.Columns[i].DataType).Name == "DateTime")
                {
                    //worksheet.Cells[2,4] .Style.Numberformat.Format = "yyyy-mm-dd h:mm"; //OR "yyyy-mm-dd h:mm" if you want to include the time!
                    worksheet.Column(i + 1).Style.Numberformat.Format = "dd/MM/yyyy h:mm"; //OR "yyyy-mm-dd h:mm" if you want to include the time!
                    //worksheet.Column(i + 1).Width = 25;
                }
            }

            #endregion

            //(from DataColumn d in dt.Columns select d.Ordinal + 1).ToList().ForEach(dc =>
            //{
            //    //background color
            //    worksheet.Cells[1, 1, 1, dc].Style.Fill.PatternType = ExcelFillStyle.Solid;
            //    worksheet.Cells[1, 1, 1, dc].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

            //    //border
            //    worksheet.Cells[1, dc, rowCount + 1, dc].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            //    worksheet.Cells[1, dc, rowCount + 1, dc].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            //    worksheet.Cells[1, dc, rowCount + 1, dc].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            //    worksheet.Cells[1, dc, rowCount + 1, dc].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            //    worksheet.Cells[1, dc, rowCount + 1, dc].Style.Border.Top.Color.SetColor(System.Drawing.Color.LightGray);
            //    worksheet.Cells[1, dc, rowCount + 1, dc].Style.Border.Right.Color.SetColor(System.Drawing.Color.LightGray);
            //    worksheet.Cells[1, dc, rowCount + 1, dc].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.LightGray);
            //    worksheet.Cells[1, dc, rowCount + 1, dc].Style.Border.Left.Color.SetColor(System.Drawing.Color.LightGray);
            //});

            /* Format the header: Prepare the range for the column headers 
            string cellRange = "A7:" + Convert.ToChar('A' + colCount - 1) + 1;
            using (ExcelRange rng = worksheet.Cells[cellRange])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
                rng.Style.Font.Color.SetColor(Color.White);
            }
            */
            
            for (int i = 7; i < rowCount + 7; i++)
            {
                //Тн
                string cellRange = string.Format("G{0}", i);
                using (ExcelRange rng = worksheet.Cells[cellRange])
                {
                    string previons_cell = string.Format("E{0}*G6", i);

                    rng.Formula = previons_cell;
                    rng.Style.Numberformat.Format = "###,###,##0.00 руб";
                }
                //Тэ
                string cellRange_te = string.Format("H{0}", i);
                using (ExcelRange rng = worksheet.Cells[cellRange_te])
                {
                    string previons_cell = string.Format("F{0}*H6", i);

                    rng.Formula = previons_cell;
                    rng.Style.Numberformat.Format = "###,###,##0.00 руб";
                }
                //Сумма T
                string cellRange_summT = string.Format("I{0}", i);
                using (ExcelRange rng = worksheet.Cells[cellRange_summT])
                {
                    string previons_cell = string.Format("G{0}+H{0}", i);

                    rng.Formula = previons_cell;
                    rng.Style.Numberformat.Format = "###,###,##0.00 руб";
                }
                //Минимальное снижение
                string cellRange_min = string.Format("J{0}", i);
                using (ExcelRange rng = worksheet.Cells[cellRange_min])
                {
                    string previons_cell = "60";

                    rng.Formula = previons_cell;
                }
                //Максимальное снижение
                string cellRange_max = string.Format("K{0}", i);
                using (ExcelRange rng = worksheet.Cells[cellRange_max])
                {
                    string previons_cell = "75";

                    rng.Formula = previons_cell;
                }
                //Допустимое отклонение
                string cellRange_delta = string.Format("L{0}", i);
                using (ExcelRange rng = worksheet.Cells[cellRange_delta])
                {
                    string previons_cell = string.Format("IF(B{0}<5,5,3)", i);

                    rng.Formula = previons_cell;
                }
                //Отклонение ниже 40
                string cellRange_40 = string.Format("M{0}", i);
                using (ExcelRange rng = worksheet.Cells[cellRange_40])
                {
                    string previons_cell = string.Format("IF(D{0}<40,1,0)", i);

                    rng.Formula = previons_cell;
                }
                //От минимально допустимого
                string cellRange_mind = string.Format("N{0}", i);
                using (ExcelRange rng = worksheet.Cells[cellRange_mind])
                {
                    string previons_cell = string.Format("IF(D{0}<J{0}-L{0},1-M{0},0)", i);

                    rng.Formula = previons_cell;
                }
                //От максимально допустимого
                string cellRange_maxd = string.Format("O{0}", i);
                using (ExcelRange rng = worksheet.Cells[cellRange_maxd])
                {
                    string previons_cell = string.Format("IF(D{0}>J{0}+K{0},1,0)", i);

                    rng.Formula = previons_cell;
                }
                //От более 40 градусов
                string cellRange_max40 = string.Format("P{0}", i);
                using (ExcelRange rng = worksheet.Cells[cellRange_max40])
                {
                    string previons_cell = string.Format("O{0}+N{0}", i);

                    rng.Formula = previons_cell;
                }
                ///отклонение температуры ГВС от нормативного значения, но более 40 Град.
                ///
                //от минимального
                string cellRange_Q = string.Format("Q{0}", i);
                using (ExcelRange rng = worksheet.Cells[cellRange_Q])
                {
                    string previons_cell = string.Format("IF(N{0},J{0}-L{0}-D{0},0)", i);

                    rng.Formula = previons_cell;
                }
                //от максимального
                string cellRange_R = string.Format("R{0}", i);
                using (ExcelRange rng = worksheet.Cells[cellRange_R])
                {
                    string previons_cell = string.Format("IF(O{0}=1,D{0}-K{0},0)", i);

                    rng.Formula = previons_cell;
                }
                //всего
                string cellRange_S = string.Format("S{0}", i);
                using (ExcelRange rng = worksheet.Cells[cellRange_S])
                {
                    string previons_cell = string.Format("R{0}+Q{0}", i);

                    rng.Formula = previons_cell;
                }
                //расчетное
                string cellRange_T = string.Format("T{0}", i);
                using (ExcelRange rng = worksheet.Cells[cellRange_T])
                {
                    string previons_cell = string.Format("S{0}/3", i);

                    rng.Formula = previons_cell;
                }
                //целое
                string cellRange_U = string.Format("U{0}", i);
                using (ExcelRange rng = worksheet.Cells[cellRange_U])
                {
                    string previons_cell = string.Format("INT(T{0})", i);

                    rng.Formula = previons_cell;
                }
                //в связи со снижением температуры ГВ ниже 40 Град.
                string cellRange_V = string.Format("V{0}", i);
                using (ExcelRange rng = worksheet.Cells[cellRange_V])
                {
                    string previons_cell = string.Format("IF(M{0}=1,H7,0)", i);

                    rng.Formula = previons_cell;
                    rng.Style.Numberformat.Format = "###,###,##0.00 руб";
                }
                //в связи со снижением температуры ГВ ниже 40 Град.
                string cellRange_W = string.Format("W{0}", i);
                using (ExcelRange rng = worksheet.Cells[cellRange_W])
                {
                    string previons_cell = string.Format("$I${1}*0.1%*U{0}", i, rowCount+8);

                    rng.Formula = previons_cell;
                    rng.Style.Numberformat.Format = "###,###,##0.00 руб";
                }
                //ВСего в рублях
                string cellRange_X = string.Format("X{0}", i);
                using (ExcelRange rng = worksheet.Cells[cellRange_X])
                {
                    string previons_cell = string.Format("V{0}+W{0}", i);

                    rng.Formula = previons_cell;
                    rng.Style.Numberformat.Format = "###,###,##0.00 руб";
                }
            }
            //FOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOTEEEEEEEEEEEEEEEEEEEEEEEEEEERRRRRRRRRRRRRRRRRRR
            //ВСего в рублях
            string cellRange_CELL = string.Format("A{0}", rowCount+8);
            using (ExcelRange rng = worksheet.Cells[cellRange_CELL])
            {
                //string previons_cell = string.Format("V{0}+W{0}", i);

               // rng.Formula = previons_cell;
                rng.Value = "Итого";
            }
            //ВСего в рублях
            string cellRange_CELA = string.Format("A{0}", rowCount + 9);
            using (ExcelRange rng = worksheet.Cells[cellRange_CELA])
            {
                //string previons_cell = string.Format("V{0}+W{0}", i);

                // rng.Formula = previons_cell;
                rng.Value = "Всего с НДС";
            }
            //ВСего в рублях
            string cellRange_CELAA = string.Format("A{0}", rowCount + 10);
            using (ExcelRange rng = worksheet.Cells[cellRange_CELAA])
            {
                //string previons_cell = string.Format("V{0}+W{0}", i);

                // rng.Formula = previons_cell;
                rng.Value = "Снижение платы в соответствии с ПП РФ № 354, руб. с НДС";
            }
            //ВСего в рублях
            string cellRange_CELAAA = string.Format("A{0}", rowCount + 11);
            using (ExcelRange rng = worksheet.Cells[cellRange_CELAAA])
            {
                //string previons_cell = string.Format("V{0}+W{0}", i);

                // rng.Formula = previons_cell;
                rng.Value = "Стоимость ГВС с учетом снижения";
            }

            //ВСего в рублях
            string cellRange_CELLE = string.Format("E{0}", rowCount+8);
            using (ExcelRange rng = worksheet.Cells[cellRange_CELLE])
            {
                string previons_cell = string.Format("SUM(E7:E{0})", rowCount+6);

                rng.Formula = previons_cell;
                rng.Style.Numberformat.Format = "###,###,##0.00 руб";
                
            }
            //ВСего в рублях
            string cellRange_CELLF = string.Format("F{0}", rowCount + 8);
            using (ExcelRange rng = worksheet.Cells[cellRange_CELLF])
            {
                string previons_cell = string.Format("SUM(F7:F{0})", rowCount + 6);

                rng.Formula = previons_cell;
                rng.Style.Numberformat.Format = "###,###,##0.00 руб";

            }
            //ВСего в рублях
            string cellRange_CELLG = string.Format("G{0}", rowCount + 8);
            using (ExcelRange rng = worksheet.Cells[cellRange_CELLG])
            {
                string previons_cell = string.Format("SUM(G7:G{0})", rowCount + 6);

                rng.Formula = previons_cell;
                rng.Style.Numberformat.Format = "###,###,##0.00 руб";

            }
            //ВСего в рублях
            string cellRange_CELLH = string.Format("H{0}", rowCount + 8);
            using (ExcelRange rng = worksheet.Cells[cellRange_CELLH])
            {
                string previons_cell = string.Format("SUM(H7:H{0})", rowCount + 6);

                rng.Formula = previons_cell;
                rng.Style.Numberformat.Format = "###,###,##0.00 руб";

            }
            //ВСего в рублях
            string cellRange_CELLI = string.Format("I{0}", rowCount + 8);
            using (ExcelRange rng = worksheet.Cells[cellRange_CELLI])
            {
                string previons_cell = string.Format("SUM(I7:I{0})", rowCount + 6);

                rng.Formula = previons_cell;
                rng.Style.Numberformat.Format = "###,###,##0.00 руб";

            }
            //ВСего в рублях
            string cellRange_CELLV = string.Format("V{0}", rowCount + 8);
            using (ExcelRange rng = worksheet.Cells[cellRange_CELLV])
            {
                string previons_cell = string.Format("SUM(V7:V{0})", rowCount + 6);

                rng.Formula = previons_cell;
                rng.Style.Numberformat.Format = "###,###,##0.00 руб";

            }
            //ВСего в рублях с НДС
            string cellRange_CELLVNDS = string.Format("V{0}", rowCount + 9);
            using (ExcelRange rng = worksheet.Cells[cellRange_CELLVNDS])
            {
                string previons_cell = string.Format("V{0}*1.18", rowCount + 8);

                rng.Formula = previons_cell;
                rng.Style.Numberformat.Format = "###,###,##0.00 руб";

            }
            //ВСего в рублях
            string cellRange_CELLW = string.Format("W{0}", rowCount + 8);
            using (ExcelRange rng = worksheet.Cells[cellRange_CELLW])
            {
                string previons_cell = string.Format("SUM(W7:W{0})", rowCount + 6);

                rng.Formula = previons_cell;
                rng.Style.Numberformat.Format = "###,###,##0.00 руб";

            }
            //ВСего в рублях с НДС
            string cellRange_CELLWNDS = string.Format("W{0}", rowCount + 9);
            using (ExcelRange rng = worksheet.Cells[cellRange_CELLWNDS])
            {
                string previons_cell = string.Format("W{0}*1.18", rowCount + 8);

                rng.Formula = previons_cell;
                rng.Style.Numberformat.Format = "###,###,##0.00 руб";

            }
            //ВСего в рублях
            string cellRange_CELLX = string.Format("X{0}", rowCount + 8);
            using (ExcelRange rng = worksheet.Cells[cellRange_CELLX])
            {
                string previons_cell = string.Format("SUM(X7:X{0})", rowCount + 6);

                rng.Formula = previons_cell;
                rng.Style.Numberformat.Format = "###,###,##0.00 руб";
            }

            //ВСего в рублях
            string cellRange_CELLXNDS = string.Format("X{0}", rowCount + 9);
            using (ExcelRange rng = worksheet.Cells[cellRange_CELLXNDS])
            {
                string previons_cell = string.Format("SUM(V{0}:W{0})", rowCount + 9);

                rng.Formula = previons_cell;
                rng.Style.Numberformat.Format = "###,###,##0.00 руб";
                
            }




            string cellRange_CELLGPLUS = string.Format("G{0}", rowCount + 9);
            using (ExcelRange rng = worksheet.Cells[cellRange_CELLGPLUS])
            {
                string previons_cell = string.Format("G{0}*1.18", rowCount + 8);

                rng.Formula = previons_cell;
                rng.Style.Numberformat.Format = "###,###,##0.00 руб";

            }
            string cellRange_CELLHPLUS = string.Format("H{0}", rowCount + 9);
            using (ExcelRange rng = worksheet.Cells[cellRange_CELLHPLUS])
            {
                string previons_cell = string.Format("H{0}*1.18", rowCount + 8);

                rng.Formula = previons_cell;
                rng.Style.Numberformat.Format = "###,###,##0.00 руб";

            }
            string cellRange_CELLIPLUS = string.Format("I{0}", rowCount + 9);
            using (ExcelRange rng = worksheet.Cells[cellRange_CELLIPLUS])
            {
                string previons_cell = string.Format("G{0}+H{0}", rowCount + 9);

                rng.Formula = previons_cell;
                rng.Style.Numberformat.Format = "###,###,##0.00 руб";
       
            }
           //снижение платы на гвс
            string cellRange_CELLgvs = string.Format("I{0}", rowCount + 10);
            using (ExcelRange rng = worksheet.Cells[cellRange_CELLgvs])
            {
                string previons_cell = string.Format("X{0}", rowCount + 9);

                rng.Formula = previons_cell;
                rng.Style.Numberformat.Format = "###,###,##0.00 руб";
                SM.sum = cellRange_CELLgvs;

            }
            //снижение платы на гвс
            string cellRange_CELLgvsminus = string.Format("I{0}", rowCount + 11);
            using (ExcelRange rng = worksheet.Cells[cellRange_CELLgvsminus])
            {
                string previons_cell = string.Format("I{0}-I{1}", rowCount + 9, rowCount+10);

                rng.Formula = previons_cell;
                rng.Style.Numberformat.Format = "###,###,##0.00 руб";
                

            }
            /* Header Footer */
            //worksheet.HeaderFooter.OddHeader.CenteredText = "Header: Tinned Goods Sales";
            //worksheet.HeaderFooter.OddFooter.RightAlignedText = string.Format("Footer: Page {0} of {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages); // add the page number to the footer plus the total number of pages
        }


    }// class 

    class ReportPeretopGVS
    {
         
         FileInfo newFile;
        FileInfo templateFile;
        DataSet _ds;

        public ExcelPackage xlPackage;
        public string _ErrorMessage;
        public byte[] res;
        public ReportPeretopGVS(string templateFilePath, DataSet ds)
        {
            //newFile = new FileInfo(@filePath);
            templateFile = new FileInfo(@templateFilePath);

            _ds = ds;
            _ErrorMessage = string.Empty;

            CreateFileWithTemplate();

        }

        private bool CreateFileWithTemplate()
        {
            try
            {
                _ErrorMessage = string.Empty;

                using (xlPackage = new ExcelPackage(templateFile, true))
                {
                    List<SummReport> ListMS = new List<SummReport>();
                    int i = 0;
                    foreach (DataTable dt in _ds.Tables)
                    {
                        SummReport SM = new SummReport();
                        AddSheet(xlPackage, dt, i, dt.TableName, ref SM);
                        i++;
                        ListMS.Add(SM);
                    }
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.First();
                    //xlPackage.Workbook.Calculate();
                    xlPackage.Workbook.Worksheets.Delete(worksheet);
                    ExcelWorksheet SVOD =  xlPackage.Workbook.Worksheets.Add("Сводная");
                    int g = 4;
                    SVOD.Cells["A3"].Value = "#";
                    SVOD.Cells["B3"].Value = "Адрес";
                    SVOD.Cells["C3"].Value = "ЖЭУ";
                    SVOD.Cells["D3"].Value = "Перетопов";
                    foreach(SummReport SL in ListMS)
                    {

                        string cell_number = string.Format("A{0}", g);
                        string cellAddress = string.Format("B{0}", g);
                        string cellRange = string.Format("C{0}", g);
                        string cellTemp = string.Format("D{0}", g);
                        SVOD.Cells[cell_number].Value = g-3;
                        SVOD.Cells[cellAddress].Value = SL.address;
                        SVOD.Cells[cellTemp].Value = SL.sum;
                        SVOD.Cells[cellRange].Value = SL.HID;
                        g++;
                    }
                   
                    ///* Set title, Author.. */
                    //xlPackage.Workbook.Properties.Title = "Title: Office Open XML Sample";
                    //xlPackage.Workbook.Properties.Author = "Author: Muhammad Mubashir.";
                    ////xlPackage.Workbook.Properties.SetCustomPropertyValue("EmployeeID", "1147");
                    //xlPackage.Workbook.Properties.Comments = "Sample Record Details";
                    //xlPackage.Workbook.Properties.Company = "TRG Tech.";



                    ///* Save */
                    //xlPackage.Save();
                    res = xlPackage.GetAsByteArray();
                }
                return true;
            }
            catch (Exception ex)
            {
                _ErrorMessage = ex.Message.ToString();
                return false;
            }
        }

        /// <summary>
        /// This AddSheet method generates a .xlsx Sheet with your provided Template file, //DataTable and SheetIndex.
        /// </summary>
        public static void AddSheetWithTemplate(ExcelPackage xlApp, DataTable dt, int SheetIndex)
        {
            string _SheetName = dt.TableName;
            ExcelWorksheet worksheet = xlApp.Workbook.Worksheets[SheetIndex];
            /* WorkSheet */
            if (SheetIndex == 0)
            {
                worksheet = xlApp.Workbook.Worksheets[SheetIndex + 1]; // add a new worksheet to the empty workbook
            }
            else
            {
                worksheet = xlApp.Workbook.Worksheets[SheetIndex]; // add a new worksheet to the empty workbook
            }
            

            if (worksheet == null)
            {
                worksheet = xlApp.Workbook.Worksheets.Add(_SheetName); // add a new worksheet to the empty workbook    
            }
            else
            {

            }

            /* Load the datatable into the sheet, starting from cell A1. Print the column names on row 1 */
           // worksheet.Cells["A7"].LoadFromDataTable(dt, true);


        }


        private static void AddSheet(ExcelPackage xlApp, DataTable dt, int Index, string sheetName, ref SummReport SM)
        {
            string _SheetName = string.Empty;

            if (string.IsNullOrEmpty(sheetName) == true)
            {
                _SheetName = string.Format("Sheet{0}", Index.ToString());
            }
            else
            {
                _SheetName = sheetName;
            }

            /* WorkSheet */
            ExcelWorksheet worksheet = xlApp.Workbook.Worksheets[_SheetName]; // add a new worksheet to the empty workbook
            if (worksheet == null)
            {
                worksheet = xlApp.Workbook.Worksheets.Add(_SheetName,xlApp.Workbook.Worksheets.First()); // add a new worksheet to the empty workbook    
            }
            else
            {

            }



            /* Load the datatable into the sheet, starting from cell A1. Print the column names on row 1 */
            worksheet.Cells["A7"].LoadFromDataTable(dt, false);
            worksheet.Cells["A2"].Value = "по адресу: "+sheetName;

           
            SM.address = sheetName;
            SM.HID = Convert.ToInt32(dt.Namespace);
            SM.sum = dt.Rows.Count.ToString();
            //SM.geu = dt.Prefix;
            int rowCount = dt.Rows.Count;
            int colCount = dt.Columns.Count;





            #region Set Column Type to Date using LINQ.
            
            IEnumerable<int> dateColumns = from DataColumn d in dt.Columns
                                           where d.ColumnName == "Day" || d.ColumnName == "Hour"
                                           select d.Ordinal + 1;

            foreach (int dc in dateColumns)
            {
                worksheet.Cells[2, dc, rowCount + 1, dc].Style.Numberformat.Format = "0";
            }
            

            #endregion
            #region Set Column Type to Date using LOOP.

            /* Set Column Type to Date. */
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if ((dt.Columns[i].DataType).FullName == "System.DateTime" && (dt.Columns[i].DataType).Name == "DateTime")
                {
                    //worksheet.Cells[2,4] .Style.Numberformat.Format = "yyyy-mm-dd h:mm"; //OR "yyyy-mm-dd h:mm" if you want to include the time!
                    worksheet.Column(i + 1).Style.Numberformat.Format = "dd/MM/yyyy h:mm"; //OR "yyyy-mm-dd h:mm" if you want to include the time!
                    //worksheet.Column(i + 1).Width = 25;
                }
            }

            #endregion

            //(from DataColumn d in dt.Columns select d.Ordinal + 1).ToList().ForEach(dc =>
            //{
            //    //background color
            //    worksheet.Cells[1, 1, 1, dc].Style.Fill.PatternType = ExcelFillStyle.Solid;
            //    worksheet.Cells[1, 1, 1, dc].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

            //    //border
            //    worksheet.Cells[1, dc, rowCount + 1, dc].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            //    worksheet.Cells[1, dc, rowCount + 1, dc].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            //    worksheet.Cells[1, dc, rowCount + 1, dc].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            //    worksheet.Cells[1, dc, rowCount + 1, dc].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            //    worksheet.Cells[1, dc, rowCount + 1, dc].Style.Border.Top.Color.SetColor(System.Drawing.Color.LightGray);
            //    worksheet.Cells[1, dc, rowCount + 1, dc].Style.Border.Right.Color.SetColor(System.Drawing.Color.LightGray);
            //    worksheet.Cells[1, dc, rowCount + 1, dc].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.LightGray);
            //    worksheet.Cells[1, dc, rowCount + 1, dc].Style.Border.Left.Color.SetColor(System.Drawing.Color.LightGray);
            //});

            /* Format the header: Prepare the range for the column headers 
            string cellRange = "A7:" + Convert.ToChar('A' + colCount - 1) + 1;
            using (ExcelRange rng = worksheet.Cells[cellRange])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
                rng.Style.Font.Color.SetColor(Color.White);
            }
            */
            
            /* Header Footer */
            //worksheet.HeaderFooter.OddHeader.CenteredText = "Header: Tinned Goods Sales";
            //worksheet.HeaderFooter.OddFooter.RightAlignedText = string.Format("Footer: Page {0} of {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages); // add the page number to the footer plus the total number of pages
        }


    }// class 