using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// Структура описание канала
/// </summary>
public struct Channel
{
    public int idUkut { get; set; }
    public int channel { get; set; }
    public string channel_name { get; set; }
    public string channel_value { get; set; }
    public string channel_color { get; set; }
    public string repair_color { get; set; }
    public bool RepaitColorSet()
    {
        bool result = false;
        repair_color = "Black";
        if (channel_value=="---")
        {
            channel_value = "Нет связи с ТВЧ";
            result = true;
        }
        if (System.Text.RegularExpressions.Regex.IsMatch(channel_name, "Давление", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
        {
            string[] val = channel_value.Split(' ');
            try
            {
                if (Convert.ToDouble(val[0]) < 0 || Convert.ToDouble(val[0]) > 18)
                {
                    repair_color = "Red";
                    result = true;
                }
            }
            catch
            {
                repair_color = "Black";
            }
        }
        else if (System.Text.RegularExpressions.Regex.IsMatch(channel_name, "Температура", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
        {
            string[] val = channel_value.Split(' ');
            try
            {
                if (Convert.ToDouble(val[0]) < 0 || Convert.ToDouble(val[0]) > 120)
                {
                    repair_color = "Red";
                    result = true;
                }
            }
            catch
            {
                repair_color = "Black";
            }
        }
        else if (System.Text.RegularExpressions.Regex.IsMatch(channel_name, "Баланс", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
        {
            string[] val = channel_value.Split(' ');
            try
            {
                if (Convert.ToDouble(val[0]) < -2 || Convert.ToDouble(val[0]) > 3.7)
                {
                    repair_color = "Red";
                    result = true;
                }
            }
            catch
            {
                repair_color = "Black";
            }
        }
        return result;
    }
}
/// <summary>
/// Структура описания объекта для обработки архивов
/// </summary>
public struct ObjectFromAnalizArchives
{
    public int idUkut { get; set; }
    public string address { get; set; }
    public int KPNum { get; set; }
    public int idDevName { get; set; }
    public string column_gvs { get; set; }

   
}
public struct MDObjects
{
    public string id { get; set; }
    public string name { get; set; }
    public string number { get; set; }

   
}
/// <summary>
/// Структура описания УКУТ для отображения на карте
/// </summary>
public struct Ukut
{
    public int idObject;
    public string lat;
    public string lon;
    public string date;
    public string devName;
    public string param_value;
    public int type;
    public string temp_graph;
    public string address;
    public string geu;
    public int idUkut;
    public string countTrub;
    public string idMD;
    public string system;
    public string channels;
    public List<Channel> channels_struct { get; set; }
    public string zabbix_status;
    public DateTime zabbix_time;
    public string otkl;
    public string teplo_source;
    public string regulat;
    public int HID;
    public int idDevType;
    public string t_pod;
    public string t_obr;
    public string t_gvs { get; set; }
    public string hvs;
    public int type_obr;
    public string type_text { get; set; }
    public string count_arc;
    public string repair_ukut { get; set; }
    public string last_day_arc { get; set; }
    public string last_hour_arc { get; set; }
    public string commlinestatus { get; set; }
    /// <summary>
    ///Раскрашиваем t под и t обратки в соотвествии с температурным графиком
    /// </summary>
    public void CellChannelsToTempGraph()
    {
     if(idDevType != 3)
        {
        string[] graph = temp_graph.Split('/');
        try
        {
            int t_pod_graph = Convert.ToInt32(graph[0]);
            int t_obr_graph = Convert.ToInt32(graph[1]);
            List<Channel> channels_struct1 = new List<Channel>();
            foreach (Channel ch in channels_struct)
            {
                Channel ch_l = ch;
                string[] values = ch.channel_value.Split(' ');
                double value = 0;
                if (Double.TryParse(values[0], out value))
                {
                    if (ch.channel_name == "Температура подачи отопления")
                    {
                        if (value > t_pod_graph)
                        {
                            ch_l.channel_color = "Red";
                        }
                        if (value < t_pod_graph)
                        {
                            ch_l.channel_color = "Blue";
                        }
                        if (value == t_pod_graph)
                        {
                            ch_l.channel_color = "Green";
                        }
                        t_pod = Convert.ToString(value);
                    }
                    if (ch.channel_name == "Температура обратки отопления")
                    {
                        if (value > t_obr_graph)
                        {
                            ch_l.channel_color = "Red";
                            type_obr = 3;
                        }
                        if (value < t_obr_graph)
                        {
                            ch_l.channel_color = "Blue";
                            type_obr = 1;
                        }
                        if (value == t_obr_graph)
                        {
                            ch_l.channel_color = "Green";
                            type_obr = 2;
                        }
                        t_obr = Convert.ToString(value);

                    }
                    if(value < 0 && ch.channel != 470)
                    {
                        ch_l.channel_value = "Обрыв датчика!";
                    }
                }
                else
                {
                    t_obr = "Н/Д";
                    type_obr = 0;
                }
                //Этот онанизм нужен для того, чтобы модифиировать элементы коллекции
                channels_struct1.Add(ch_l);
                channels_struct = channels_struct1;
            }
        }
        catch 
        {
        
        }
    }
        
    }
    public void SetTypes()
    {
        if(t_gvs == "0")
        {
            type = type_obr;
            type_text = "Обратка";
            
        }
        else
        {
         // type_text = "Нет связи";
        }
    }
}
public class DateTimeJavaScriptConverter : JavaScriptConverter
{
    public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
    {
        return new JavaScriptSerializer().ConvertToType(dictionary, type);
    }

    public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
    {
        if (!(obj is DateTime)) return null;
        return new CustomString(((DateTime)obj).ToUniversalTime().ToString("O"));
    }

    public override IEnumerable<Type> SupportedTypes
    {
        get { return new[] { typeof(DateTime) }; }
    }

    private class CustomString : Uri, IDictionary<string, object>
    {
        public CustomString(string str)
            : base(str, UriKind.Relative)
        {
        }

        void IDictionary<string, object>.Add(string key, object value)
        {
            throw new NotImplementedException();
        }

        bool IDictionary<string, object>.ContainsKey(string key)
        {
            throw new NotImplementedException();
        }

        ICollection<string> IDictionary<string, object>.Keys
        {
            get { throw new NotImplementedException(); }
        }

        bool IDictionary<string, object>.Remove(string key)
        {
            throw new NotImplementedException();
        }

        bool IDictionary<string, object>.TryGetValue(string key, out object value)
        {
            throw new NotImplementedException();
        }

        ICollection<object> IDictionary<string, object>.Values
        {
            get { throw new NotImplementedException(); }
        }

        object IDictionary<string, object>.this[string key]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        void ICollection<KeyValuePair<string, object>>.Clear()
        {
            throw new NotImplementedException();
        }

        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        int ICollection<KeyValuePair<string, object>>.Count
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<KeyValuePair<string, object>>.IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}

     