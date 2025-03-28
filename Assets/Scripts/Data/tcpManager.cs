using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tcpManager : MonoBehaviour
{
    private const string serverIP = "34.22.64.103";
    private const int serverPort = 8080;
    //private TcpClient client;
    //private NetworkStream stream;
    //public static tcpManager _instance;

    //private tcpManager() {}

    /*
    public static tcpManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new tcpManager();
            }
            return _instance;
        }
    }
    */

    public static Tuple<float[], string> CommunicateWithServer(string message)
    //public IEnumerator CommunicateWithServer(string message, Action<(float[], string)> callback)
    {
        /*
        client = new TcpClient("34.22.64.103", 8080);
        stream = client.GetStream();
        Debug.Log("서버 연결");

        // 서버에 데이터 요청
        byte[] requestData = Encoding.UTF8.GetBytes(message);
        stream.Write(requestData, 0, requestData.Length);
        Debug.Log($"서버에 데이터 요청 전송: {message}");

        // **서버 응답 대기**
        yield return Instance.StartCoroutine(WaitForServerResponse(callback));
        */
        try
        {
            TcpClient client = new TcpClient(serverIP, serverPort);
            NetworkStream stream = client.GetStream();
            BinaryWriter writer = new BinaryWriter(stream);
            BinaryReader reader = new BinaryReader(stream);

            // 전송할 문자열 정의
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            // 문자열 길이(4바이트) + 문자열 데이터 전송
            writer.Write(IPAddress.HostToNetworkOrder(messageBytes.Length));
            writer.Write(messageBytes);
            writer.Flush();
            Debug.Log("서버로 문자열 전송 완료: " + message);

            Thread.Sleep(10000);
            // yield retrun

            // 리스트 길이 읽기 (4바이트)
            int listLength = IPAddress.NetworkToHostOrder(reader.ReadInt32());

            // float 리스트 읽기
            float[] predList = new float[listLength];
            for (int i = 0; i < listLength; i++)
            {
                predList[i] = BitConverter.ToSingle(BitConverter.GetBytes(IPAddress.NetworkToHostOrder(reader.ReadInt32())), 0);
            }

            // 문자열 길이 읽기 (4바이트)
            int riskLength = IPAddress.NetworkToHostOrder(reader.ReadInt32());

            // 문자열 데이터 읽기
            string risk_level = Encoding.UTF8.GetString(reader.ReadBytes(riskLength));

            Debug.Log("서버로부터 받은 데이터: " + string.Join(", ", predList) + risk_level);

            writer.Close();
            reader.Close();
            stream.Close();
            client.Close();

            return new Tuple<float[], string>(predList, risk_level);
        }
        catch (Exception e)
        {
            Debug.LogError("에러 발생: " + e.Message);
            return null;
        }
    }
    /*
    IEnumerator WaitForServerResponse(Action<(float[], string)> callback)
    {
        byte[] buffer = new byte[1024];
        int bytesRead = 0;

        while (bytesRead == 0)  // **서버가 데이터를 보낼 때까지 대기**
        {
            if (stream.DataAvailable)
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
            }
            yield return null;  // **다음 프레임까지 대기**
        }

        // 받은 데이터 파싱
        int offset = 0;

        // 1. float 개수 읽기
        int floatCount = BitConverter.ToInt32(buffer, offset);
        offset += 4;

        // 2. float 리스트 읽기
        float[] pred_list = new float[floatCount];
        for (int i = 0; i < floatCount; i++)
        {
            pred_list[i] = BitConverter.ToSingle(buffer, offset);
            offset += 4;
        }

        // 3. 문자열 길이 읽기
        int stringLength = BitConverter.ToInt32(buffer, offset);
        offset += 4;

        // 4. 문자열 읽기
        string risk = Encoding.UTF8.GetString(buffer, offset, stringLength);

        // 결과 출력
        Debug.Log($"서버에서 받은 float 리스트: {string.Join(", ", pred_list)}");
        Debug.Log($"서버에서 받은 문자열: {risk}");

        callback((pred_list, risk));

        // 연결 종료
        stream.Close();
        client.Close();
        Debug.Log("서버 연결 종료");
    }
    */
}
