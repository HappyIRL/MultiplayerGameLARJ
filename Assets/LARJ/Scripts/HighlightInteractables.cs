using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HighlightInteractables : MonoBehaviour
{
	private List<Interactables> _interactables = new List<Interactables>();
	private bool _outlineEnabled = false;
	public bool OutlineEnabled { get { return _outlineEnabled; } set { _outlineEnabled = value; } }

	private void Start()
	{
		_interactables = FindObjectsOfType<Interactables>().ToList();
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
		foreach(Interactables interactable in _interactables)
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

	public void UpdateInteractables(List<Interactables> interactables)
	{
		_interactables = new List<Interactables>(interactables);
	}

	public void AddInteractables(Interactables interactables)
	{
		if (_interactables.Contains(interactables)) return;

		_interactables.Add(interactables);

		OutlineInteractables(!_outlineEnabled, _interactables.Count - 1, 1);
	}
	public void AddInteractables(List<Interactables> interactables)
	{
        foreach (Interactables interactable in interactables)
        {
			_interactables.Add(interactable);
        }
	}
}
