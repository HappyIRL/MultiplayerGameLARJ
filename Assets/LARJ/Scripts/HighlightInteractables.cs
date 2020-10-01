using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HighlightInteractables : MonoBehaviour
{
	private List<Interactable> _interactables = new List<Interactable>();
	private bool _outlineEnabled = false;
	public bool OutlineEnabled { get { return _outlineEnabled; } set { _outlineEnabled = value; } }

	private void Start()
	{
		_interactables = FindObjectsOfType<Interactable>().ToList();
		OnHighlightTasks();
	}

	public void OnHighlightTasks()
	{		
		OutlineInteractables(_outlineEnabled);
		_outlineEnabled = !_outlineEnabled;
		Debug.Log(_outlineEnabled);
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

	public void UpdateInteractables(List<Interactable> interactables)
	{
		_interactables = new List<Interactable>(interactables);
	}

	public void AddInteractables(Interactable interactables)
	{
		if (_interactables.Contains(interactables)) return;

		_interactables.Add(interactables);

		OutlineInteractables(!_outlineEnabled, _interactables.Count - 1, 1);
	}
	public void AddInteractables(List<Interactable> interactables)
	{
        foreach (Interactable interactable in interactables)
        {
			_interactables.Add(interactable);
        }
	}
}
