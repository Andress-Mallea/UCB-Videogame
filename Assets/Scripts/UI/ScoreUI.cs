using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        ScoreManager.instance.onScoreChanged += UpdateScoreUI;
        
    }

    private void OnDestroy()
    {
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.onScoreChanged -= UpdateScoreUI;
        }
    }

    private void UpdateScoreUI(int score)
    {
        scoreText.text = $"Score: {score}";
    }
}
