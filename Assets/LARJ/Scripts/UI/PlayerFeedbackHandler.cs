using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerFeedbackHandler : MonoBehaviour
{
	[SerializeField] private TMP_Text _playerErrorFeedbackText;

	private List<string> _playerErrorFeedbacks = new List<string>();

	private string _playerErrorFeedback;

	public void SendLocalErrorPlayerFeedback(string error)
	{
		_playerErrorFeedbacks.Add(error);

		UpdatePlayerErrors();

		float input = 2 + error.Length * 0.05f;

		StartCoroutine(WaitAndSend(input, error));
	}

	private IEnumerator WaitAndSend(float seconds, string error)
	{
		yield return new WaitForSeconds(seconds);
		_playerErrorFeedbacks.Remove(error);
		UpdatePlayerErrors();
	}

	private void UpdatePlayerErrors()
	{
		_playerErrorFeedback = "";

		foreach (string s in _playerErrorFeedbacks)
		{
			_playerErrorFeedback = $"{s}{Environment.NewLine}{_playerErrorFeedback}";
		}
		_playerErrorFeedbackText.text = _playerErrorFeedback;
	}
}
