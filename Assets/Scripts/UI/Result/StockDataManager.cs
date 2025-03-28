using UnityEngine;
using System.Collections.Generic;
using stockinfo;
using stockdetail;
using System.Linq;
using TMPro;
using System;


public class StockDataManager : MonoBehaviour
{private DrawGraph drawGraph;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        drawGraph = FindFirstObjectByType<DrawGraph>();
        GameObject loadResult = GameObject.Find("LoadResult");
        string stock_name = loadResult.GetComponent<LoadResult>().GetStockName();
        loadResult.GetComponent<LoadResult>().DestroySelf();

        // 오늘 날짜 구하기
        System.DateTime today = System.DateTime.Today;
        // 29일 전 날짜 구하기
        System.DateTime startDate = today.AddDays(-29);
        string today_str = today.ToString("yyyyMMdd");
        string startDate_str = startDate.ToString("yyyyMMdd");
        StockInfo stockInfo = new StockInfo(startDate_str, today_str);
        List<StockDetail> stock_data_arr = await stockInfo.get_stock_info(stock_name);
        string std = stock_data_arr[0].std_code;
        float cur_price = Convert.ToSingle(stock_data_arr[stock_data_arr.Count - 1].closing_price);

        List<float> dataValues = new List<float>();
        List<string> dataLabels = new List<string>();

        foreach (StockDetail stock in stock_data_arr)
        {
            dataValues.Add(stock.closing_price);
            string dateStr = stock.day;
            DateTime date = DateTime.ParseExact(dateStr, "yyyy/MM/dd", null);
            string formattedDate = date.ToString("MM-dd");
            dataLabels.Add(formattedDate);
        }

        Tuple<float[], string> predict_risk = stockInfo.predict_stock_info(std);
        float[] pred_list = predict_risk.Item1;
        for (int i = 0; i < pred_list.Length; i++)
        {
            dataValues.Add(pred_list[i]);
            dataLabels.Add($"+ {i+1}일");
        }
        float cur_pred = pred_list[pred_list.Length - 1];

        drawGraph.SetStockData(dataValues, dataLabels);

        int sum = 0;
        foreach (StockDetail stock in stock_data_arr)
        {
            sum += stock.closing_price;
        }

        // 최대 종가 찾기
        int maxIndex = 0;
        int minIndex = 0;
        int maxPrice = stock_data_arr[0].closing_price;
        int minPrice = stock_data_arr[0].closing_price;

        for (int i = 1; i < stock_data_arr.Count; i++)
        {
            if (stock_data_arr[i].closing_price > maxPrice)
            {
                maxPrice = stock_data_arr[i].closing_price;
                maxIndex = i;
            }

            if (stock_data_arr[i].closing_price < minPrice)
            {
                minPrice = stock_data_arr[i].closing_price;
                minIndex = i;
            }
        }

        GameObject.Find("stock_name_txt").GetComponent<TextMeshProUGUI>().text = stock_name;

        GameObject.Find("cur_price_data").GetComponent<TextMeshProUGUI>().text = stock_data_arr[stock_data_arr.Count - 1].closing_price.ToString();
        GameObject.Find("cur_price_cont").GetComponent<TextMeshProUGUI>().text = stock_data_arr[stock_data_arr.Count - 1].contrast.ToString();

        GameObject.Find("avg_price_data").GetComponent<TextMeshProUGUI>().text = (sum / stock_data_arr.Count).ToString();

        GameObject.Find("max_price_data").GetComponent<TextMeshProUGUI>().text = maxPrice.ToString();
        GameObject.Find("max_price_date").GetComponent<TextMeshProUGUI>().text = stock_data_arr[maxIndex].day;

        GameObject.Find("min_price_data").GetComponent<TextMeshProUGUI>().text = minPrice.ToString();
        GameObject.Find("min_price_date").GetComponent<TextMeshProUGUI>().text = stock_data_arr[minIndex].day;

        GameObject.Find("fluc_rate_data").GetComponent<TextMeshProUGUI>().text = stock_data_arr[stock_data_arr.Count - 1].fluctuation_rate.ToString();

        GameObject.Find("total_share_data").GetComponent<TextMeshProUGUI>().text = stock_data_arr[stock_data_arr.Count - 1].num_of_sh.ToString();
        GameObject.Find("risk_data").GetComponent<TextMeshProUGUI>().text = predict_risk.Item2;
        GameObject.Find("pred_data").GetComponent<TextMeshProUGUI>().text = cur_pred.ToString();
        GameObject.Find("pred_cont").GetComponent<TextMeshProUGUI>().text = (cur_pred - cur_price).ToString();
    }
}
