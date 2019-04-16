using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using Container;

namespace W2U
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        F2U_Connection F2U_Con = F2U_Connection.Instance;
        public MainWindow()
        {
            InitializeComponent();
        }

        void showStatus(string msg)
        {
            Thread t = new Thread(new ThreadStart(delegate
            {
                this.Dispatcher.Invoke(new Action(delegate
                {
                    Conn_Status.Content = msg;
                }), null);
            }));
            t.Start();
        }
        void showReceiveMsg(string msg)
        { 
            {
                Thread t = new Thread(new ThreadStart(delegate
                {
                    this.Dispatcher.Invoke(new Action(delegate
                    {
                        MSG_U2F.Content = msg;
                    }), null);
                }));
                t.Start();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            F2U_Con.QuitServer();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            F2U_Con.StatusCallBack += showStatus;
            F2U_Con.msgCallBack += showReceiveMsg;
            F2U_Con.StartServer();
        }

        private void MSG_Send2U_Click(object sender, RoutedEventArgs e)
        {
            F2U_Con.SendU3DMsg(DateTime.Now.Ticks.ToString());
        }
    }


}
