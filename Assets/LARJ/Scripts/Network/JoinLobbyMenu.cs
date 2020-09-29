using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class JoinLobbyMenu : MonoBehaviour
{
	[SerializeField] private GameObject _joinOrCreateRoomMenu;
	[SerializeField] private GameObject _backButton;

	public void OnClick_JoinLobby()
	{
		if (!PhotonNetwork.IsConnected || !(PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer))
			return;

		JoinLobby();
		ChangeUI();
	}

	private void ChangeUI()
	{
		_backButton.SetActive(true);
		_joinOrCreateRoomMenu.SetActive(true);
		transform.parent.gameObject.SetActive(false);
	}

	private void JoinLobby()
	{
		PhotonNetwork.JoinLobby();
		Debug.Log("Joined Lobby");
	}
}
