using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Chatbox : MonoBehaviourPunCallbacks
{
    public int maxMessages = 100;

    private Vector2 chatScroll = Vector2.zero;
    private List<string> chatMessages = new List<string>();

    private string message = "";

    private void OnGUI()
    {
        if(GUILayout.Button("Leave Room"))
        {
            PhotonNetwork.LeaveRoom();
        }

        chatScroll = GUILayout.BeginScrollView(chatScroll, GUILayout.Width(Screen.width), GUILayout.ExpandHeight(true));

        foreach(string msg in chatMessages)
        {
            GUILayout.Label(msg);
        }

        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();

        message = GUILayout.TextField(message, GUILayout.ExpandWidth(true));

        if (GUILayout.Button("Send", GUILayout.Width(100f)))
        {
            string nickname = PhotonNetwork.NickName.ToString();
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("AddChat", Photon.Pun.RpcTarget.All);
            message = "";
        }

        GUILayout.EndHorizontal();
    }

    [PunRPC]
    void AddChat()
    {
        string message = "msg";
        string nick = "Nick";
        chatMessages.Add(nick + ": " + message);

        if(chatMessages.Count > maxMessages)
        {
            chatMessages.RemoveAt(0);
        }

        chatScroll.y = 9999;
    }
  
}
