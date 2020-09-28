using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestTrigger : MonoBehaviour
{
    [SerializeField] private QuestManager _questManager = null; 
    [SerializeField] private Quest _quest = null;

    [SerializeField] private TextMeshProUGUI _newQuestText = null; 


    
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {


            _questManager.StartQuest(_quest);

            _newQuestText.text = "New Quest!";
            _newQuestText.CrossFadeAlpha(1, 0, false);
            _newQuestText.CrossFadeAlpha(0, 4, false);

            gameObject.SetActive(false);
        }
    }
}
