using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class U2F_Connection
{
    public string _ReceivedMsg = "";

    /// <summary>
    /// 处理接收到的Winform消息
    /// </summary>
    /// <param name="msg"></param>
    void processMsg()
    {
        if (_ReceivedMsg.Length < 1)
            return;

        #region ********处理消息的代码段********       


        #endregion ********处理消息的代码段********

        _ReceivedMsg = "";
    }
    /// <summary>
    /// 测试消息显示的Debug，正式开发时注释掉。。。
    /// </summary>
    //private void OnGUI()
    //{
    //    GUILayout.Label(_ReceivedMsg);
    //}
}
