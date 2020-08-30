using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyListingsMenu : MonoBehaviourPunCallbacks
{
	[SerializeField] private LobbyListing _lobbyView;
	[SerializeField] private Transform _content;

	private List<LobbyListing> _listings = new List<LobbyListing>();

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		foreach (RoomInfo info in roomList)
		{
			if(info.RemovedFromList)
			{
				int index = _listings.FindIndex(x => x.RoomInfo.Name == info.Name);
				if(index != -1)
				{
					Destroy(_listings[index].gameObject);
					_listings.RemoveAt(index);
				}
			}
			else
			{
				LobbyListing listing = Instantiate(_lobbyView, _content);

				if (listing != null)
				{
					listing.SetRoomInfo(info);
					_listings.Add(listing);
				}
			}

		}
	}
}
