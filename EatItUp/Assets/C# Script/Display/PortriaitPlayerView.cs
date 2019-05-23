using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PortriaitPlayerView : MonoBehaviour
{
    public long id;
    public long memberId;
    public TextMeshProUGUI userName;
    public TextMeshProUGUI score;
    public Image border;
    public RawImage Avatar;

    private string defaultUserName;
    private string defaultScore;
    private Color defaultBorder;

    private int displayedScore;
    private int actualScore;
    bool coroutine = true;

    private void Start()
    {
        defaultUserName = userName.text;
        defaultScore = score.text;
        defaultBorder = border.color;
    }
    public void SetUser(string userName, string score, Texture2D avatarImage = null)
    {
        this.userName.text = userName;
        this.score.text = score;
        Avatar.texture = avatarImage;
    }
    public void SetToDefault()
    {
        userName.text = defaultUserName;
        score.text = defaultScore;
        border.color = defaultBorder;
        Avatar.texture = null;
    }
    public void SetScoreText(int value)
    {
        actualScore = value;
        if (coroutine)
            StartCoroutine(ScoreGoingUp());
    }
    IEnumerator ScoreGoingUp()
    {
        coroutine = false;
        while (displayedScore < actualScore)
        {
            displayedScore++;
            score.text = "Score: " + displayedScore;

            float t = 1f / ((actualScore - displayedScore) + 1);
            yield return new WaitForSeconds(t);
        }
        coroutine = true;
        yield break;
    }

}
