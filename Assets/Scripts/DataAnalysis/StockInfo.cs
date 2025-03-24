using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using CsvHelper;
using CsvHelper.Configuration;
using System.Linq;
using System.Globalization;
using MySqlConnector;

public class StockInfo : MonoBehaviour
{
    //private static TextMeshProUGUI price_text;
    static DataTable indv_table;
    List<StockDetail> stock_data_arr;
    string strtDd;
    string endDd;
    float risk;
    //List<Int> predicted_price;

    /*
    private static async Task OnSceneLoaded()
    {
        if (SceneManager.GetActiveScene().name == "ResultScene")
        {
            if (price_text != null)
            {
                await get_stock_info("삼성전자");
                //predict_stock_info("2024-08-20");
            }
        }
    }
    */
    
    static DataTable ConvertCsvToTable(string csvData)
    {
        DataTable dataTable = new DataTable();
        using (var reader = new StringReader(csvData))
        using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,   // 첫 줄을 헤더로 인식
            IgnoreBlankLines = true,  // 빈 줄 무시
            BadDataFound = null       // 잘못된 데이터 무시
        }))
        {
            using (var dr = new CsvDataReader(csv))
            {
                dataTable.Load(dr);
            }
            return dataTable;
        }
    }

    static async Task get_stock_info(string stock_name)
    {
        string std_code = "", abbr = "";
        using (var stock_reader = dbManager.select("stock", "*"))
        {
            while (stock_reader.Read())
            {
                if (stock_reader["stock_name"].ToString() == stock_name)
                {
                    std_code = (string)stock_reader["std_code"];
                    abbr = (string)stock_reader["abbr"];
                    Debug.Log($"std: {std_code}, abbr: {abbr}");
                    break;  // 조건에 맞는 첫 번째 행만 찾으면 종료
                }
            }

            // 리소스 해제
            stock_reader.Close();
        }

        // generate 헤더 요청 URL
        string url_price = "http://data.krx.co.kr/comm/fileDn/GenerateOTP/generate.cmd";
        string day = DateTime.Now.ToString("yyyy/MM/dd");

        // generate 페이로드의 양식 데이터
        var data_price = new Dictionary<string, string>
        {
            { "locale", "ko_KR" },
            { "tboxisuCd_finder_stkisu0_1", $"{abbr}/{stock_name}" },
            { "isuCd", std_code },
            { "isuCd2", "KR7005930003" },
            { "codeNmisuCd_finder_stkisu0_1", stock_name },
            { "param1isuCd_finder_stkisu0_1", "ALL"},
            { "strtDd", day },
            { "endDd", day },
            { "adjStkPrc_check", "Y"},
            { "adjStkPrc", "2" },
            { "share", "1" },
            { "money", "1" },
            { "csvxls_isNo", "false" },
            { "name", "fileDown" },
            { "url", "dbms/MDC/STAT/standard/MDCSTAT01701" }
        };

        // 브라우저에서 서버로 보내는 헤더값
        var headers = new Dictionary<string, string>
        {
            { "Referer", "http://data.krx.co.kr/contents/MDC/MDI/mdiLoader/index.cmd?menuId=MDC0201020203" },
            { "User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/134.0.0.0 Safari/537.36" }
        };

        string indv_data;
        using (var client = new HttpClient())
        {
            foreach (var header in headers)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            // generate 부분의 헤더에서 Referer과 User-Agent를 따올 수 있음 User-Agent는 모두 동일
            // KRX의 다른 정보를 따올 때는 Referer만 바꿔주기
            // download payload(요청데이터)와 동일해야함
            var content_indv = new FormUrlEncodedContent(data_price);
            var response_indv = await client.PostAsync(url_price, content_indv);
            var otp_indv = await response_indv.Content.ReadAsStringAsync();

            // download의 헤더 요청 URL
            string downUrl_indv = "http://data.krx.co.kr/comm/fileDn/download_csv/download.cmd";

            // 서버로부터 데이터 요청 후 읽어들이기
            var downloadData_indv = new Dictionary<string, string>
            {
                { "code", otp_indv }
            };
            var downloadContent_indv = new FormUrlEncodedContent(downloadData_indv);
            var downSectorResponse_indv = await client.PostAsync(downUrl_indv, downloadContent_indv);
            var stream_indv = await downSectorResponse_indv.Content.ReadAsStreamAsync();

            using (var reader = new StreamReader(stream_indv, Encoding.GetEncoding("EUC-KR")))
            {
                indv_data = reader.ReadToEnd();
            }
        }

        DataTable indv_table = ConvertCsvToTable(indv_data);
        string cur_price; //현재가
        using(var indv_reader = indv_table.CreateDataReader())
        {
            while (indv_reader.Read())
            {
                cur_price = indv_reader["종가"].ToString();
            }
        };
    }

    static void predict_stock_info(int inquiry_period)
    {
        stock_data_arr = new StockDetail[inquiry_period];
        using (var reader = dbManager.select("stock_price_per_date", "*", $"std_code"))
    }
}
