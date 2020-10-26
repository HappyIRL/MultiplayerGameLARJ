using System.Collections.Generic;
using UnityEngine;

public class AllowedInteractables : MonoBehaviour
{
	private static AllowedInteractables _instance;
	public static AllowedInteractables Instance
	{
		get
		{
			if (_instance == null)
				_instance = new AllowedInteractables();

			return _instance;
		}
		private set => _instance = value;
	}

	private List<Interactable> _interactables = new List<Interactable>();
	public List<Interactable> Interactables { get => _interactables; }

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		Interactable[] interactables = FindObjectsOfType<Interactable>();
		foreach (Interactable interactable in interactables)
		{
			if (interactable.AlwaysInteractable)
			{
				_interactables.Add(interactable);
			}
		}
	}
	public void AddInteractable(Interactable interactable)
	{
		if (!Interactables.Contains(interactable))
			_interactables.Add(interactable);
	}
}