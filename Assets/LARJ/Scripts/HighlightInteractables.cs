using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HighlightInteractables : MonoBehaviour
{
	private static HighlightInteractables _instance;
	public static HighlightInteractables Instance
	{ 
		get
		{
			if (_instance == null)
				_instance = new HighlightInteractables();

			return _instance;
		}
		private set => _instance = value;
	}



	[SerializeField] private List<Interactable> _interactables;
	public bool OutlineEnabled { get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		OnHighlightTasks();
	}

	public void OnHighlightTasks()
	{		
		OutlineInteractables(OutlineEnabled);
		OutlineEnabled = !OutlineEnabled;
	}

	public void OutlineInteractables(bool enabled)
	{
		foreach(Interactable interactable in _interactables)
		{
			interactable.OutlineRef.enabled = enabled;
		}
	}

	public void OutlineInteractables(bool enabled, int startIndex, int length)
	{
		for(int i = startIndex; i < startIndex + length; i++)
		{
			_interactables[i].OutlineRef.enabled = enabled;
		}
	}

	public void AddInteractable(Interactable interactable)
	{
		if (_interactables.Contains(interactable))
			return;

		_interactables.Add(interactable);

		interactable.OutlineRef.enabled = false;
	}
}
