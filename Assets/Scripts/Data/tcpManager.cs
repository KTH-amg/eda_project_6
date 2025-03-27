using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class tcpManager : MonoBehaviour
{
    private const string serverIP = "34.22.64.103";
    private const int serverPort = 8080;

    static tcpManager() {}
    public static List<int> CommunicateWithServer(string message)
    {
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

            Thread.Sleep(3000);

            // 정수 리스트 수신
            int length = reader.ReadInt32();
            List<int> data = new List<int>(length);

            for (int i = 0; i < length; i++)
            {
                data.Add(reader.ReadInt32());
            }

            Debug.Log("서버로부터 받은 데이터: " + string.Join(", ", data));

            writer.Close();
            reader.Close();
            stream.Close();
            client.Close();

            return data;
        }
        catch (Exception e)
        {
            Debug.LogError("에러 발생: " + e.Message);
            return new List<int>();
        }
    }
}
