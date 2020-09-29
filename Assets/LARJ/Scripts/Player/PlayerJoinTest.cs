using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoinTest : MonoBehaviour
{
	private ClientNetworkHandler _clientNetworkHandler;
	private int _playerCount;
	private void Start()
	{
		_clientNetworkHandler = GetComponent<ClientNetworkHandler>();
	}
	public void OnPlayerJoined(PlayerInput playerInput)
	{
		_playerCount++;
		switch(_playerCount)
		{
			case 1:
				_clientNetworkHandler.Player1 = playerInput.gameObject;
				break;
			case 2:
				_clientNetworkHandler.Player2 = playerInput.gameObject;
				break;
			default:
				break;
		}
	}
}
