using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake screen;
    public float amountOfTime = 0.3f;
    private void Awake()
    {
        screen = this;
    }
    public void Shake()
    {
        iTween.PunchPosition(gameObject, Vector3.down * 0.5f, amountOfTime);
    }
}
