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
    public string stock_name;
    public string std_code;
    public string abbr;
    public string day;
    public int closing_price;
    public int contrast = 0;
    public float fluctuation_rate = 0;
    public int market_price = 0;
    public int high_price = 0;
    public int low_price = 0;
    public double trading_vol = 0;
    public long trs = 0;
    public long cpt = 0;
    public long num_of_sh = 0;

    public StockDetail(DataRow row)
    {
        stock_name = row["종목명"].ToString();
        std_code = row["표준코드"].ToString();
        abbr = row["단축코드"].ToString();
        day = row["일자"].ToString();
        closing_price = Convert.ToInt32(row["종가"]);
        contrast = Convert.ToInt32(row["대비"]);
        fluctuation_rate = Convert.ToSingle(row["등락률"]);
        market_price = Convert.ToInt32(row["시가"]);
        high_price = Convert.ToInt32(row["고가"]);
        low_price = Convert.ToInt32(row["저가"]);
        trading_vol = Convert.ToDouble(row["거래량"]);
        trs = Convert.ToInt64(row["거래대금"]);
        cpt = Convert.ToInt64(row["시가총액"]);
        num_of_sh = Convert.ToInt64(row["상장주식수"]);
    }
}
