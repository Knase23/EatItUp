using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreUpdater : MonoBehaviour
{
    public static ScoreUpdater INSTANCE;
    public List<Score> allScores = new List<Score>();
    public Dictionary<long, Score> scores = new Dictionary<long, Score>();
    public static Score lowestScore;
    public static Score highestScore;
    // Start is called before the first frame update
    private void Awake()
    {
        if(INSTANCE)
        {
            Destroy(this);
        }
        else
        {
            INSTANCE = this;
        }
    }

    void Start()
    {
        allScores.AddRange(GetComponentsInChildren<Score>());
        foreach (var handler in allScores)
        {
            scores.Add(handler.id, handler);
        }
    }

    public void UpdateScore(ScoreData data)
    {
        Score score;
        if(scores.TryGetValue(data.scoreId,out score))
        {
            score.SetValue(data.value);
            CheckHigestOrLowestScore(score);
        }
    }

    public static void CheckHigestOrLowestScore(Score score)
    {
        if (lowestScore == null || score.GetValue() < lowestScore.GetValue())
        {
            lowestScore = score;
        }

        if (highestScore == null || score.GetValue() > highestScore.GetValue())
        {
            highestScore = score;
        }
    }
}
