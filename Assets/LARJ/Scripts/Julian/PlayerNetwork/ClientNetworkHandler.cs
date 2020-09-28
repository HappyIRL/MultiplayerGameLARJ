using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ClientNetworkHandler : MonoBehaviour, IOnEventCallback
{
	[SerializeField] private GameObject _player1;
	[SerializeField] private GameObject _player2;
	[SerializeField] private GameObject _player3;
	[SerializeField] private GameObject _player4;

	private delegate LARJNetworkID OnInstantiated(); //joined player event that if you are the master client tells you to return an id that you can use in this script

	private LARJNetworkID myID;

	public enum LARJNetworkID
	{
		Player1 = 10,
		Player2 = 11,
		Player3 = 12,
		Player4 = 13
	}

	public enum LARJNetworkEvents
	{
		PCUpdate = 128
	}

	private void Start()
	{
		if(PhotonNetwork.IsMasterClient)
		{
			myID = LARJNetworkID.Player1;
		}
		else
		{
			//myID = PlayerJoinEvent
		}
	}

	private void Update()
	{
		PlayerController1Update();
		PlayerController2Update();
		PlayerController3Update();
		PlayerController4Update();
	}

	private void PlayerController2Update()
	{

	}

	private void PlayerController3Update()
	{

	}

	private void PlayerController4Update()
	{
		
	}

	private void PlayerController1Update()
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

		ClientNetworkData clientNetworkData = new ClientNetworkData((byte)LARJNetworkID.Player1, gameObject.transform);

		PhotonNetwork.RaiseEvent((byte)LARJNetworkEvents.PCUpdate, clientNetworkData, raiseEventOptions, sendOptions);
	}

	private void ReceiveUpdatePC(LARJNetworkID id, Transform transform)
	{
		switch(id)
		{
			case LARJNetworkID.Player1:
				if(_player2 != null)
				{
					_player1.transform.position = transform.position;
					_player1.transform.rotation = transform.rotation;
				}
				break;
		}
	}

	public void OnEvent(EventData photonEvent)
	{
		LARJNetworkEvents eventCode = (LARJNetworkEvents)photonEvent.Code;
		ClientNetworkData clientNetworkData = (ClientNetworkData)photonEvent.CustomData;

		switch(eventCode)
		{
			case LARJNetworkEvents.PCUpdate:
				ReceiveUpdatePC((LARJNetworkID)clientNetworkData.ID, clientNetworkData.Transform);
				break;
		}
	}
}
