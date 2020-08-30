using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinInitialLobby : MonoBehaviour
{
	public void JoinLobby()
	{
		PhotonNetwork.JoinLobby();
	}
}
