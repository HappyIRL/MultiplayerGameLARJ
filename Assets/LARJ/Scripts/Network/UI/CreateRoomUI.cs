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
	[SerializeField] private PlayerFeedbackHandler _playerFeedbackHandler;
	[SerializeField] private UIHandler _uiHandler;

	private string _validationFailedReason;

	private string _createRoomFailed;
	private string _noNameForRoom;
	private string _roomNameToLong;
	private string _notConnected;

	private void Start()
	{
		_createRoomFailed = "Couldn't create a lobby: ";
		_noNameForRoom = "Your lobby needs a name!";
		_roomNameToLong = $"Lobby names can't be longer than {_maxRoomNameLength} characters!";
		_notConnected = "You are not connected to any server!";
	}

	public void OnClick_CreateRoom()
	{
		_uiHandler.PlayButtonClickSound();
		_uiHandler.EnableConnectingDialog(true);

		if (!PhotonNetwork.IsConnected)
		{
			_playerFeedbackHandler.SendLocalErrorPlayerFeedback(_notConnected);
			_uiHandler.EnableConnectingDialog(false);
			return;
		}

		if(!ValidateRoomName())
		{
			_uiHandler.EnableConnectingDialog(false);
			_playerFeedbackHandler.SendLocalErrorPlayerFeedback(_validationFailedReason);
			return;	
		}

		RoomOptions options = new RoomOptions
		{
			MaxPlayers = _maxPlayersInRoom
		};

		PhotonNetwork.CreateRoom(_roomName.text, options, TypedLobby.Default);
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