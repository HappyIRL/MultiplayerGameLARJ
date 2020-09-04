using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomListingsMenu : MonoBehaviourPunCallbacks
{
	[SerializeField] private RoomListing _roomListing;
	[SerializeField] private Transform _content;

	private List<RoomListing> _listings = new List<RoomListing>();

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
				//Julian: Mir wäre lieber hier einen vergleich zu machen ob der Name oder das GameObject schon vorhanden ist, als die Methode ClearLisings() zu verwenden.
				RoomListing listing = Instantiate(_roomListing, _content);
				if(listing != null)
				{
					listing.SetRoomInfo(info);
					_listings.Add(listing);
				}
			}

		}
	}

	public void ClearListings()
	{
		foreach (RoomListing roomListing in _listings)
		{
			Destroy(roomListing.gameObject);
		}
		_listings.Clear();
	}
}
