using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HighlightInteractables : MonoBehaviour
{
	private List<Interactable> _interactables = new List<Interactable>();
	public bool OutlineEnabled { get; private set; }

	private void Start()
	{
		_interactables = FindObjectsOfType<Interactable>().ToList();
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

	public void AddInteractables(Interactable interactables)
	{
		if (_interactables.Contains(interactables)) return;

		_interactables.Add(interactables);

		OutlineInteractables(!OutlineEnabled, _interactables.Count - 1, 1);
	}
	public void AddInteractables(List<Interactable> interactables)
	{
        foreach (Interactable interactable in interactables)
        {
			_interactables.Add(interactable);
        }
	}
}
