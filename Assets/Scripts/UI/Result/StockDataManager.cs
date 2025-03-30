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

        // 종목명
        GameObject.Find("stock_name_txt").GetComponent<TextMeshProUGUI>().text = stock_name;

        // 현재가
        GameObject.Find("cur_price_data").GetComponent<TextMeshProUGUI>().text = 
            $"{stock_data_arr[stock_data_arr.Count - 1].closing_price:N0}원";

        // 전일대비
        TextMeshProUGUI contrastText = GameObject.Find("cur_price_cont").GetComponent<TextMeshProUGUI>();
        float contrast = stock_data_arr[stock_data_arr.Count - 1].contrast;
        float currentPrice = stock_data_arr[stock_data_arr.Count - 1].closing_price;
        float previousPrice = currentPrice - contrast;
        float percentageChange = (contrast / previousPrice) * 100;
        contrastText.text = $"{contrast:N0}원 ({percentageChange:F2}%)";
        contrastText.color = contrast >= 0 ? Color.red : new Color(0, 0.7f, 1f);

        // 평균가
        GameObject.Find("avg_price_data").GetComponent<TextMeshProUGUI>().text = 
            $"{(sum / stock_data_arr.Count):N0}원";

        // 최고가
        GameObject.Find("max_price_data").GetComponent<TextMeshProUGUI>().text = 
            $"{maxPrice:N0}원";
        GameObject.Find("max_price_date").GetComponent<TextMeshProUGUI>().text = 
            stock_data_arr[maxIndex].day.Replace("/", "-");

        // 최저가
        GameObject.Find("min_price_data").GetComponent<TextMeshProUGUI>().text = 
            $"{minPrice:N0}원";
        GameObject.Find("min_price_date").GetComponent<TextMeshProUGUI>().text = 
            stock_data_arr[minIndex].day.Replace("/", "-");

        // 등락률
        GameObject.Find("fluc_rate_data").GetComponent<TextMeshProUGUI>().text = 
            $"{stock_data_arr[stock_data_arr.Count - 1].fluctuation_rate:F2}%";

        // 거래량
        long totalShares = stock_data_arr[stock_data_arr.Count - 1].num_of_sh;
        string shareText;
        if (totalShares >= 1000000)
        {
            float millionShares = totalShares / 1000000f;
            shareText = $"{millionShares:F2}M";
        }
        else
        {
            shareText = totalShares.ToString("N0");
        }
        GameObject.Find("total_share_data").GetComponent<TextMeshProUGUI>().text = shareText;

        // 위험도
        GameObject.Find("risk_data").GetComponent<TextMeshProUGUI>().text = predict_risk.Item2;

        // 예측가
        GameObject.Find("pred_data").GetComponent<TextMeshProUGUI>().text = 
            $"{cur_pred:N0}원";

        // 예측가 대비
        TextMeshProUGUI predContText = GameObject.Find("pred_cont").GetComponent<TextMeshProUGUI>();
        float priceDiff = cur_pred - cur_price;
        predContText.text = $"{priceDiff:N0}원";
        predContText.color = priceDiff >= 0 ? Color.red : new Color(0, 0.7f, 1f);
    }
}
