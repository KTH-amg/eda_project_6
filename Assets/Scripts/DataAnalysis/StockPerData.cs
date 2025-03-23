using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Data;

public class StockPerData : MonoBehaviour
{
    public string stock_name;
    public string std_code;
    public string abbr;
    public string day;
    public int closing_price;
    public int contrast;
    public float fluctuation_rate;
    public int market_price;
    public int high_price;
    public int low_price;
    public double trading_vol;
    public long trs;
    public long cpt;
    public long num_of_sh;

    public StockPerData(DataRow row)
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
