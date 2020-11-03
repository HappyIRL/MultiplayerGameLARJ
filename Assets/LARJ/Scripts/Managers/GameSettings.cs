using UnityEngine;

[CreateAssetMenu(menuName = "Manager/GameSettings")]
public class GameSettings : ScriptableObject
{
	[SerializeField]
	private string _gameVersion = "0.0.1";
	[SerializeField]
	private string _nickName = "Player";

	public string GameVersion { get { return _gameVersion; } }

	public string NickName
	{
		get
		{
			int value = Random.Range(0, 9999);
			return _nickName + value.ToString();
			
		}
	}

}
