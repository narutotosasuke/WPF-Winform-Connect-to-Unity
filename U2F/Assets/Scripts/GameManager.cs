using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private GameObject obj;
    private Text Receive_text;
    void Start()
    {
        obj = GameObject.Find("Receive_MSG");
        Receive_text = obj.GetComponent<Text>();
    }


    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            U2F_Connection.Instance.SendWinMsg("按下空格键");
        }
        if (Input.GetMouseButton(0))
        {
            U2F_Connection.Instance.SendWinMsg("按下鼠标左键");
        }
        if (Input.GetMouseButton(1))
        {
            U2F_Connection.Instance.SendWinMsg("按下鼠标右键");
        }

        Receive_text.text =  U2F_Connection.Instance._ReceivedMsg;
    }
    
}
