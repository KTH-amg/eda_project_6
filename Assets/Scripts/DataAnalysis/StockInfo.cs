using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Data;
//using System.Data.OleDb;
//using MySql.Data;
//using MySql.Data.MySqlClient;

public class StockInfo : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /*
    static async Task get_stock_info(string stock_name)
    {
        // generate 헤더 요청 URL
        string url = "http://data.krx.co.kr/comm/fileDn/GenerateOTP/generate.cmd";

        // generate 페이로드의 양식 데이터
        var data = new Dictionary<string, string>
        {
            { "locale", "ko_KR" },
            { "mktId", "ALL" },
            { "share", "1" },
            { "csvxls_isNo", "false" },
            { "name", "fileDown" },
            { "url", "dbms/MDC/STAT/standard/MDCSTAT01901" }
        };

        // 브라우저에서 서버로 보내는 헤더값
        var headers = new Dictionary<string, string>
        {
            { "Referer", "http://data.krx.co.kr/contents/MDC/MDI/mdiLoader/index.cmd?menuId=MDC0201020203" },
            { "User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/134.0.0.0 Safari/537.36" }
        };

        using (var client = new HttpClient())
        {
            foreach (var header in headers)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            // generate 부분의 헤더에서 Referer과 User-Agent를 따올 수 있음 User-Agent는 모두 동일
            // KRX의 다른 정보를 따올 때는 Referer만 바꿔주기
            // download payload(요청데이터)와 동일해야함
            var content = new FormUrlEncodedContent(data);
            var response = await client.PostAsync(url, content);
            var otp = await response.Content.ReadAsStringAsync();

            // download의 헤더 요청 URL
            string downUrl = "http://data.krx.co.kr/comm/fileDn/download_csv/download.cmd";

            // 서버로부터 데이터 요청 후 읽어들이기
            var downloadData = new Dictionary<string, string>
            {
                { "code", otp }
            };
            var downloadContent = new FormUrlEncodedContent(downloadData);
            var downSectorResponse = await client.PostAsync(downUrl, downloadContent);
            var stream = await downSectorResponse.Content.ReadAsStreamAsync();

            using (var reader = new StreamReader(stream, Encoding.GetEncoding("EUC-KR")))
            {
                var csvData = reader.ReadToEnd();
                // Process csvData as needed
            }
        }
    }
    */
}
