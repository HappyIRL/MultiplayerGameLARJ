using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreCountText;
    [SerializeField] private TextMeshProUGUI _scoreAddText;
    [SerializeField] private TextMeshProUGUI _scoreText;

    private int _score = 0;
    public void UpdateScore(int reward, bool positive)
    {
        _scoreAddText.text = positive ? "+" + reward.ToString() : "-" + reward.ToString();
        _scoreAddText.color = positive ? Color.green : Color.red;
        _scoreAddText.CrossFadeAlpha(0, 3, false);
        _scoreAddText.CrossFadeAlpha(1, 0, false);
        _scoreAddText.CrossFadeAlpha(0, 1, false);
        _score += positive ? reward : -reward;
        _scoreText.color = _score < 0 ? Color.black : Color.yellow;
        _scoreCountText.color = _score < 0 ? Color.black : Color.yellow;
        _scoreText.text = _score < 0 ? "Debt:" : "Money:";
        _scoreCountText.text = _score.ToString();
    }
}
