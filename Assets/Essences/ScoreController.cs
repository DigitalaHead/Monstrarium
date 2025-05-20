using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ScoreController : MonoBehaviour
{
    public static int score = 0;
    private static List<TextMeshProUGUI> scoreTexts = new List<TextMeshProUGUI>();

    public static void RegisterScoreText(TextMeshProUGUI text)
    {
        if (!scoreTexts.Contains(text))
        {
            scoreTexts.Add(text);
        }
    }

    public static void UpdateAllScoreTexts()
    {
        foreach (var text in scoreTexts)
        {
            if (text != null)
            {
                text.text = score.ToString();
            }
        }
    }

    public static void AddScore(int amount)
    {
        score += amount;
        UpdateAllScoreTexts();
    }
}