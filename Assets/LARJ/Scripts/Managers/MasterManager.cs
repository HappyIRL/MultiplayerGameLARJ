using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Singletons/MasterManager")]
public class MasterManager : ScriptableObject
{
	[SerializeField]
	private GameSettings _gameSettings;

	public GameSettings GameSettings { get { return Instance._gameSettings; } }

	private static MasterManager _instance = null;

	public static MasterManager Instance
	{
		get
		{
			if (_instance == null)
			{
				MasterManager result = Resources.Load<MasterManager>("MasterManager");

				_instance = result;
			}
			return _instance;
		}
	} 
}
