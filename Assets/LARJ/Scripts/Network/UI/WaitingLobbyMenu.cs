using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaitingLobbyMenu : MonoBehaviourPunCallbacks
{
	[SerializeField] private GameObject _waitingForPlayersScreen;
	[SerializeField] private GameObject _networkSectionScreen;
	[SerializeField] private CreateRoomUI _createRoomUI;
	[SerializeField] private List<GameObject> _playerImageGOs = new List<GameObject>();
	[SerializeField] private TMP_Text _waitingForX;
	[SerializeField] private GameObject _startGamebutton;
	
	private List<Image> _playerImages = new List<Image>();

	private void Start()
	{
		foreach(GameObject go in _playerImageGOs)
		{
			_playerImages.Add(go.GetComponent<Image>());
		}
	}
	public override void OnEnable()
	{
		base.OnEnable();
		_createRoomUI.LARJOnRoomCreated += DoSmthing;
	}
	public override void OnDisable()
	{
		_createRoomUI.LARJOnRoomCreated -= DoSmthing;
		base.OnDisable();
	}

	private void DoSmthing()
	{
		_waitingForPlayersScreen.SetActive(true);
		_networkSectionScreen.SetActive(false);
		UpdatePlayerList();
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		UpdatePlayerList();
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		UpdatePlayerList();
	}

	private void UpdatePlayerList()
	{
		//needs rework
		foreach (Image img in _playerImages)
		{
			img.color = Color.red;
		}

		for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
		{
			_playerImages[i].color = Color.green;
		}

		if (PhotonNetwork.PlayerList.Length >= 1)
		{
			_waitingForX.text = "Waiting For Host";
			if(PhotonNetwork.IsMasterClient)
			{
				_startGamebutton.SetActive(true);
			}
		}
		else
		{
			_waitingForX.text = "Waiting For Players";
			_startGamebutton.SetActive(false);
		}
	}

	public override void OnJoinedRoom()
	{
		DoSmthing();
	}
}
