using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public struct Result_RS
{
    public string KPNum { get; set; }
    public string Name { get; set; }
    public bool Result { get; set; }
}
public struct CountArchives
{
    public string KPNum { get; set; }
    public string Name { get; set; }
    public int CountDayArc { get; set; }
    public int CountHourArc { get; set; }
}
