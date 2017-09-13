using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Сводное описание для HVS
/// </summary>
public class HVS
{
	public HVS(double _M, double _P)
	{
		M = _M;
        P = _P;
	}
    public double M { get; set; }
    public double P { get; set; }
}