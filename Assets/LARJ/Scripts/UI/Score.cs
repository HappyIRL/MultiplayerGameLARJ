using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreCountText;
    [SerializeField] private TextMeshProUGUI _scoreAddText;

    private int _score = 0;
    public int ScoreCount { get => _score; }

    public void UpdateScore(int reward, bool positive)
    {
        _scoreAddText.text = positive ? "+" + reward.ToString() : "-" + reward.ToString();
        _scoreAddText.color = positive ? Color.green : Color.red;
        _scoreAddText.CrossFadeAlpha(0, 3, false);
        _scoreAddText.CrossFadeAlpha(1, 0, false);
        _scoreAddText.CrossFadeAlpha(0, 1, false);
        _score += positive ? reward : -reward;
        _scoreCountText.color = _score < 0 ? Color.black : Color.yellow;
        _scoreCountText.text = _score.ToString();
    }

    public void SaveMoney()
    {
        int totalMoney = PlayerPrefs.GetInt("TotalMoneyScore", 0) + _score;
        PlayerPrefs.SetInt("TotalMoneyScore", totalMoney);
    }
}
