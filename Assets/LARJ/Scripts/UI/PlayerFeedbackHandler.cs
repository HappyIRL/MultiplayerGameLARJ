using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerFeedbackHandler : MonoBehaviour
{
	[SerializeField] private TMP_Text _playerErrorFeedbackText;

	public void SendLocalErrorPlayerFeedback(string feedback)
	{
		_playerErrorFeedbackText.text = feedback;
		_playerErrorFeedbackText.gameObject.SetActive(true);
		StartCoroutine(WaitForSeconds(3));
	}

	private IEnumerator WaitForSeconds(int seconds)
	{
		yield return new WaitForSeconds(seconds);
		_playerErrorFeedbackText.gameObject.SetActive(false);
	}
}
