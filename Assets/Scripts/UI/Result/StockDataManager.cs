using UnityEngine;
using System.Collections.Generic;
using stockinfo;
using stockdetail;

public class StockDataManager : MonoBehaviour
{
    [SerializeField] private DrawGraph drawGraph;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        drawGraph = FindFirstObjectByType<DrawGraph>();
        // 오늘 날짜 구하기
        System.DateTime today = System.DateTime.Today;
        // 29일 전 날짜 구하기
        System.DateTime startDate = today.AddDays(-13);
        string today_str = today.ToString("yyyy-MM-dd");
        string startDate_str = startDate.ToString("yyyy-MM-dd");
        StockInfo stockInfo = new StockInfo(startDate_str, today_str);
        List<StockDetail> stock_data_arr = await stockInfo.get_stock_info("삼성전자");

        List<float> dataValues = new List<float>();
        List<string> dataLabels = new List<string>();

        foreach (StockDetail stock in stock_data_arr)
        {
            dataValues.Add(stock.closing_price);
            dataLabels.Add(stock.day);
        }

        drawGraph.SetStockData(dataValues, dataLabels);

    }
}
