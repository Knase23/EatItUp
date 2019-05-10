using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
public class DiscordOverlayService : MonoBehaviour
{
    OverlayManager manager;
    // Start is called before the first frame update
    void Start()
    {
        manager = DiscordManager.INSTANCE.GetDiscord().GetOverlayManager();
    }

    void OpenVoiceSettings()
    {
        manager.OpenVoiceSettings((result) =>
        {
            if (result == Result.Ok)
            {
                Debug.Log("Open Voice Settings");
            }
        });
    }

    void UnlockOverlay()
    {
        manager.SetLocked(false, (result) =>
        {
            if (result == Result.Ok)
            {
                Debug.Log("Overlay Locke:" + manager.IsLocked());
            }
        });
    }
    void OpenActivityInvite()
    {
        manager.OpenActivityInvite(ActivityActionType.Join, (result) =>
        {
            if (result == Result.Ok)
            {
                Debug.Log("User is now inviting others to play!");
            }
            else
            {
                Debug.Log(result);
            }
        });
       
    }
    void OpenGuildInvite()
    {
        manager.OpenGuildInvite("code", (result) =>
         {
             if (result == Result.Ok)
             {
                 Debug.Log("User is now inviting others to Guild!");
             }
             else
             {
                 Debug.Log(result);
             }
         });
    }
}
