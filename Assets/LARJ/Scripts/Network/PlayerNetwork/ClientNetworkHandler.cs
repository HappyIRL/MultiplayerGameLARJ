using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UIElements;

public class ClientNetworkHandler : MonoBehaviour, IOnEventCallback
{
	[SerializeField] private GameObject _player1;
	[SerializeField] private GameObject _player2;

	public GameObject Player1 { set { _player1 = value; } }
	public GameObject Player2 { set { _player2 = value; } }

	//[SerializeField] private GameObject _player3;
	//[SerializeField] private GameObject _player4;

	private LARJNetworkID myID = 0;

	public enum LARJNetworkID
	{
		Player1 = 0,
		Player2 = 1,
		Player3 = 2,
		Player4 = 3
	}

	public enum LARJNetworkEvents
	{
		PCUpdate = 128
	}


	private void Start()
	{
		PhotonPeer.RegisterType(typeof(ClientNetworkData), (byte)LARJNetworkEvents.PCUpdate, ClientNetworkData.SerializeMethod, ClientNetworkData.DeserializeMethod);
	}

	private void Update()
	{
		myID = (LARJNetworkID)PhotonNetwork.LocalPlayer.ActorNumber;
		UpdateLocalPlayerController();
	}

	private void UpdateLocalPlayerController()
	{
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions
		{
			Receivers = ReceiverGroup.Others,
			CachingOption = EventCaching.DoNotCache
		};

		SendOptions sendOptions = new SendOptions
		{
			Reliability = true
		};

		GameObject player = GetPlayerFromID(myID);

		if(player != null)
		{
			ClientNetworkData clientNetworkData = new ClientNetworkData()
			{
				ID = (byte)myID,
				Position = player.transform.position,
				Rotation = player.transform.eulerAngles
			};
			PhotonNetwork.RaiseEvent((byte)LARJNetworkEvents.PCUpdate, clientNetworkData, raiseEventOptions, sendOptions);
		}
		else
		{
			Debug.LogError("Player is null ID: " + myID);
		}
	}

	private GameObject GetPlayerFromID(LARJNetworkID id)
	{
		switch(id)
		{
			case LARJNetworkID.Player1:
				return _player1;
			case LARJNetworkID.Player2:
				return _player2;
			//case LARJNetworkID.Player3:
			//	return _player3;
			//case LARJNetworkID.Player4:
			//	return _player4;
		}
		return null;
	}
	private void ReceiveUpdatePC(ClientNetworkData data)
	{
		GameObject player = GetPlayerFromID((LARJNetworkID)data.ID);
		if(player != null)
		{
			player.transform.position = data.Position;
			player.transform.eulerAngles = data.Rotation;
		}
	}

	public void OnEvent(EventData photonEvent)
	{
		LARJNetworkEvents eventCode = (LARJNetworkEvents)photonEvent.Code;

		switch(eventCode)
		{
			case LARJNetworkEvents.PCUpdate:
				ReceiveUpdatePC((ClientNetworkData)photonEvent.CustomData);
				break;
		}
	}
}
