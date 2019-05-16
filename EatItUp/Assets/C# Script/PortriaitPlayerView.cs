using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PortriaitPlayerView : MonoBehaviour
{

    public TextMeshProUGUI userName;
    public TextMeshProUGUI score;
    public Image border;
    public RawImage Avatar;

    private string defaultUserName;
    private string defaultScore;
    private Color defaultBorder;

    private void Start()
    {
        defaultUserName = userName.text;
        defaultScore = score.text;
        defaultBorder = border.color;
    }
    public void SetUser(string userName, string score,Texture2D avatarImage = null)
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
        score.text = "Score: " + value;
    }
}
