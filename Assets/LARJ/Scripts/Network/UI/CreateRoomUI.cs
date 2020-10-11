using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreateRoomUI : MonoBehaviourPunCallbacks
{
	[SerializeField] private TMP_InputField _roomName;
	[SerializeField] private byte _maxPlayersInRoom = 4;
	[SerializeField] private TextMeshProUGUI _placeHolder;
	[SerializeField] private int _maxRoomNameLength;

	public delegate void LARJOnRoomCreatedEvent();
	public event LARJOnRoomCreatedEvent LARJOnRoomCreated;

	private string _validationFailedReason;

	private string _createRoomFailed;
	private string _noNameForRoom;
	private string _roomNameToLong;

	private void Start()
	{
		_createRoomFailed = "Couldn't create a lobby: ";
		_noNameForRoom = "Your lobby needs a name!";
		_roomNameToLong = $"Lobby names can't be longer than {_maxRoomNameLength} characters!";
	}

	public void OnClick_CreateRoom()
	{
		if (!PhotonNetwork.IsConnected || !(PhotonNetwork.NetworkClientState == ClientState.JoinedLobby))
			return;

		if(!ValidateRoomName())
			return;

		RoomOptions options = new RoomOptions
		{
			MaxPlayers = _maxPlayersInRoom
		};

		PhotonNetwork.CreateRoom(_roomName.text, options, TypedLobby.Default);
	}

	public override void OnCreatedRoom()
	{
		LARJOnRoomCreated?.Invoke();
		Debug.Log("Created room successfully. " + this);
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		Debug.Log("Failed creation of room. " + message + this);
		PhotonNetwork.JoinLobby();
		Debug.Log("Joined Lobby");
	}

	private bool ValidateRoomName()
	{
		if (_roomName.text == string.Empty)
		{
			_validationFailedReason = _createRoomFailed + _noNameForRoom;
			return false;
		}
		if(_roomName.text.Length > _maxRoomNameLength)
		{
			_validationFailedReason = _createRoomFailed + _roomNameToLong;
			return false;
		}
		return true;
	}
}