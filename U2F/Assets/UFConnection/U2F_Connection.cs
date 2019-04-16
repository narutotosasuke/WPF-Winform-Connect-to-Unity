using System;
using System.Text;
using UnityEngine;
using System.Net.Sockets;

/// <summary>
/// Unity3d to Winform Connection（unity到窗体的连接类）
/// </summary>
public partial class U2F_Connection : MonoBehaviour
{
    TcpClient client;
    int PortNo = 2019;
    byte[] RecBuffer;
    string ErrorInfo;

    public static U2F_Connection Instance = null;
    void Start()
    {
        Instance = this;
        try
        {
            //获取Winform传过来的端口号
            string[] Args = Environment.GetCommandLineArgs();
            if (Args.Length > 3)
                PortNo = int.Parse(Args[3]);

            //连接到服务器  
            client = new TcpClient();
            client.Connect("127.0.0.1", PortNo);
            RecBuffer = new byte[client.ReceiveBufferSize];
            SendWinMsg("Unity is ready");
            client.GetStream().BeginRead(RecBuffer, 0, client.ReceiveBufferSize, ReceiveWinMsg, null);

        }
        catch (Exception) { }
    }



    /// <summary>
    /// 发送消息给Winform
    /// </summary>
    /// <param name="msg"></param>
    public void SendWinMsg(string msg)
    {
        try
        {
            NetworkStream ns = client.GetStream();
            byte[] data = Encoding.UTF8.GetBytes(msg);
            ns.Write(data, 0, data.Length);
            ns.Flush();
        }
        catch (Exception ex)
        {
            ErrorInfo = ex.Message;
        }
    }

    /// <summary>
    /// 接收Winform的消息
    /// </summary>
    /// <param name="ar"></param>
    public void ReceiveWinMsg(IAsyncResult ar)
    {
        try
        {
            //清空ErrorInfo
            ErrorInfo = "";
            int bytesRead = client.GetStream().EndRead(ar);
            if (bytesRead < 1)
                return;
            else
            {
                _ReceivedMsg = Encoding.UTF8.GetString(RecBuffer, 0, bytesRead);    
            }
            client.GetStream().BeginRead(RecBuffer, 0, client.ReceiveBufferSize, ReceiveWinMsg, null);
        }
        catch (Exception ex)
        {
            ErrorInfo = ex.Message;
        }
    }

    private void OnDestroy()
    {
        client.Close();
    }



}
