using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreUpdater : MonoBehaviour
{
    public static ScoreUpdater INSTANCE;
    public Dictionary<long, Score> scores = new Dictionary<long, Score>();
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
        var childScores = GetComponentsInChildren<Score>();
        foreach (var handler in childScores)
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
        }
    }
}
