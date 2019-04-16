using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Container
{
	public class F2U_Connection
	{
		private static F2U_Connection _instance;

		private Socket serverSocket;

		private Socket clientSocket;

		public static int conPort;

		private Thread listenThread;

		private Thread receiveThread;

		private bool isListening = false;

		private bool isReceiving = false;

		private static byte[] result;

		public msgDelegate StatusCallBack;

		public msgDelegate msgCallBack;

		private string _stateInfo;

		private string _receiveMsg;
        /// <summary>
        /// 单例
        /// </summary>
		public static F2U_Connection Instance
		{
			get
			{
				if (F2U_Connection._instance == null)
				{
					F2U_Connection._instance = new F2U_Connection();
				}
				return F2U_Connection._instance;
			}
		}

		public string receiveMsg
		{
			get
			{
				return this._receiveMsg;
			}
			set
			{
				this._receiveMsg = value;
				msgDelegate _msgDelegate = this.msgCallBack;
				if (_msgDelegate != null)
				{
					_msgDelegate(this._receiveMsg);
				}
				else
				{
				}
			}
		}

		public string statusInfo
		{
			get
			{
				return this._stateInfo;
			}
			set
			{
				this._stateInfo = value;
				msgDelegate statusCallBack = this.StatusCallBack;
				if (statusCallBack != null)
				{
					statusCallBack(this._stateInfo);
				}
				else
				{
				}
			}
		}

		static F2U_Connection()
		{
			F2U_Connection._instance = null;
			F2U_Connection.conPort = 2019;
			F2U_Connection.result = new byte[1024];
		}
        /// <summary>
        /// 等待客户端的连接 并且创建与之通信的Socket
        /// </summary>
		private void listenClientConnect()
		{
			while (this.isListening)
			{
				try
				{
					this.clientSocket = this.serverSocket.Accept();
					this.receiveThread = new Thread(new ParameterizedThreadStart(this.ReceiveU3DMsg));
					this.isReceiving = true;
					this.receiveThread.Start(this.clientSocket);
				}
				catch (Exception exception)
				{
					this.statusInfo = string.Concat("监听线程错误：", exception.Message);
				}
			}
		}

		public void QuitServer()
		{
			try
			{
				if (this.serverSocket != null)
				{
					this.serverSocket.Close();
				}
				if (this.clientSocket != null)
				{
					this.clientSocket.Close();
				}
				this.isListening = false;
				this.listenThread.Join();
				this.isReceiving = false;
				this.receiveThread.Join();
			}
			catch (Exception exception)
			{
                
			}
		}

		private void ReceiveU3DMsg(object cliSocket)
		{
			Socket socket = (Socket)cliSocket;
			while (this.isReceiving)
			{
				try
				{
                    //实际接收到的有效字符
                    int num = socket.Receive(F2U_Connection.result);
					if (num > 1)
					{
						this.receiveMsg = Encoding.UTF8.GetString(F2U_Connection.result, 0, num);
					}
				}
				catch (Exception exception2)
				{
					Exception exception = exception2;
					try
					{
						this.statusInfo = string.Concat("接收消息错误：", exception.Message);
						this.clientSocket.Shutdown(SocketShutdown.Both);
						this.clientSocket.Close();
					}
					catch (Exception exception1)
					{
					}
				}
			}
		}

		public void SendU3DMsg(string msg)
		{
			if ((this.clientSocket == null ? true : !this.clientSocket.Connected))
			{
				this.statusInfo = "Unity端离线。";
			}
			else
			{
				this.clientSocket.Send(Encoding.UTF8.GetBytes(msg));
			}
		}

		public void StartServer()
		{
            //在服务端创建一个负责监听IP和端口号的Socket
            IPAddress pAddress = IPAddress.Parse("127.0.0.1");
			this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //绑定端口号
            this.serverSocket.Bind(new IPEndPoint(pAddress, F2U_Connection.conPort));
            //设置监听
            this.serverSocket.Listen(10);
			this.statusInfo = string.Concat("启动监听:", this.serverSocket.LocalEndPoint.ToString(), "成功");
            //创建监听线程
            this.listenThread = new Thread(new ThreadStart(this.listenClientConnect));
			this.isListening = true;
			this.listenThread.Start();
		}
	}
}