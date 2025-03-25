using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Data;

public class StockDetail : MonoBehaviour
{
    private string stock_name;
    private string std_code;
    private string abbr;
    private string day;
    private int closing_price;
    private int contrast;
    private float fluctuation_rate;
    private int market_price;
    private int high_price;
    private int low_price;
    private double trading_vol;
    private long trs;
    private long cpt;
    private long num_of_sh;

    public StockDetail(string name, string std, string d, int cl, string abb = "", 
        int co = 0, float fl = 0, int ma = 0, int hi = 0, int lo = 0, double tra = 0, 
        long tr = 0, long cp = 0, long ns = 0)
    {
        stock_name = name;
        std_code = std;
        abbr = abb;
        day = d;
        closing_price = cl;
        contrast = co;
        fluctuation_rate = fl;
        market_price = ma;
        high_price = hi;
        low_price = lo;
        trading_vol = tra;
        trs = tr;
        cpt = cp;
        num_of_sh = ns;
    }
}
