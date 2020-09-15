using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomListingsMenu : MonoBehaviourPunCallbacks
{
	[SerializeField] private RoomListing _roomListing;
	[SerializeField] private Transform _content;

	private List<RoomInfo> _roomList;

	private List<RoomListing> _listings = new List<RoomListing>();

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		_roomList = new List<RoomInfo>(roomList);
		UpdateRoomList();
	}

	public void UpdateRoomList()
	{
		if(_roomList != null)
		{
			foreach (RoomInfo info in _roomList)
			{
				if (info.RemovedFromList)
				{
					int index = _listings.FindIndex(x => x.RoomInfo.Name == info.Name);
					if (index != -1)
					{
						Destroy(_listings[index].gameObject);
						_listings.RemoveAt(index);
					}
				}
				else
				{
					int index = _listings.FindIndex(x => x.RoomInfo.Name == info.Name);
					if (index == -1)
					{
						RoomListing listing = Instantiate(_roomListing, _content);
						if (listing != null)
						{
							listing.SetRoomInfo(info);
							_listings.Add(listing);
							listing.GetComponent<JoinRoomButton>().RoomName = listing.RoomInfo.Name;
						}
					}
				}

			}
			Debug.Log("Updated List");
		}
	}
}
