using Scada.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using Utils;

/// <summary>
/// Сводное описание для TemperatureNV
/// </summary>
public class TemperatureNV
{
    public string tnv;
    //если канал tnv от wheater API
	public TemperatureNV()
	{
       tnv =  GetTempOnline();        
	}
    //Если канал tnv от скады
    public TemperatureNV(int channel)
	{
       tnv =  GetChannelValue(channel);        
	}
    /// <summary>
    ///Получить от RS канал, по номеру
    /// </summary>
    public string GetChannelValue(int channel)
    {
        Log.WriteLineDelegate writeToLog;
        writeToLog = text => { }; // заглушка
        Utils.Log log = new Utils.Log(Utils.Log.Formats.Simple);

        
        CommSettings cm = new CommSettings("10.235.63.252",10000,"ScadaApi","ScadaApi",10000);
        ServerComm sc = new ServerComm(cm, writeToLog);
    
       DataCache dc = new DataCache(sc, log);
        DataAccess da = new DataAccess(dc, log);
    
      DataFormatter dataFormatter = new DataFormatter();
    //значение канала от RS
    string value;
    //Флаг на получение единиц измерения
    //bool unit = true;
    //Цвет по мнению RS
    //string color;
    //Для не нулевых каналов
    if (channel != 0)
    {
           string text;
           string textWithUnit;
       // MD.RefreshData();
        dataFormatter.FormatCnlVal(da.GetCurCnlData(channel).Val, da.GetCurCnlData(channel).Stat,da.GetCnlProps(channel), out text, out textWithUnit);
        value = textWithUnit;
    }
    else
    {
       return "Н/Д";
    }
    //Вернем экземпляр класса cnl
    return value;
}
    private string GetTempOnline()
{     
    DateTime configData = Convert.ToDateTime(GetConfigValue("date"));
    TimeSpan ts = DateTime.Now - configData;
if (ts.Minutes > 20)
    {
        var url = "http://api.openweathermap.org/data/2.5/weather?id=1510203&appid=7578c666176626d3af6f29e2ee074f0d";
        try
        {
               // WebProxy proxy = new WebProxy();
               // proxy.Address = new Uri("http://192.168.1.5:3128");
               // proxy.Credentials = new NetworkCredential("user", "user","REMP");  //These can be replaced by user input, if wanted.
               // proxy.UseDefaultCredentials = true;
               // proxy.BypassProxyOnLocal = false;
       
            using (var webClient = new System.Net.WebClient())
            {
                //webClient.Proxy = proxy;
                //webClient.UseDefaultCredentials = true;
                var json = webClient.DownloadString(url);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                dynamic item = serializer.Deserialize<object>(json);
                decimal name = decimal.Round(item["main"]["temp"] - 273, 0);

                ChangeConfigValue("temperatura", name.ToString());
                ChangeConfigValue("date", DateTime.Now.ToString());
                return name.ToString();
            }
        }
        catch(Exception ex)
        {
                //Handle, log, rethrow exception
             return Convert.ToString(GetConfigValue("temperatura"));
            //return ex.Message;

        }
    }

  else
    {         return Convert.ToString(GetConfigValue("temperatura"));
         //return "11";
    }
   
   
    
 
}
    private bool ChangeConfigValue(string name, string value)
{
    try
    {
        ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
        fileMap.ExeConfigFilename = AppDomain.CurrentDomain.BaseDirectory + @"App.config";
        Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
        config.AppSettings.Settings[name].Value = value;
        config.Save();
        return true;
    }
    catch
    {
        return false;
    }
}
private string GetConfigValue(string name)
{
    try
    {
        ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
        fileMap.ExeConfigFilename = AppDomain.CurrentDomain.BaseDirectory + @"App.config";
        Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
        return config.AppSettings.Settings[name].Value;
       
    }
    catch
    {
        return "0";
    }
}
}
