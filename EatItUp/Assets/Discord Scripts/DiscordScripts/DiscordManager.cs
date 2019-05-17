using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
public class DiscordManager : MonoBehaviour
{
    public static DiscordManager INSTANCE;
    Discord.Discord discord;


    public long ApplicationId = 574643243728240642;
    private void Awake()
    {
        if (INSTANCE)
        {
            Destroy(this);
        }
        else
        {
            INSTANCE = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        System.Environment.SetEnvironmentVariable("DISCORD_INSTANCE_ID", "0");
        //var discord0 = new Discord.Discord(ApplicationId, (System.UInt64)Discord.CreateFlags.Default);

        //// This makes the SDK connect to PTB
#if UNITY_EDITOR
        System.Environment.SetEnvironmentVariable("DISCORD_INSTANCE_ID", "1");
        //var discord1 = new Discord.Discord(ApplicationId, (System.UInt64)Discord.CreateFlags.Default);
#endif

        discord = new Discord.Discord(ApplicationId, (System.UInt64)Discord.CreateFlags.Default);
    }

    // Update is called once per frame
    void Update()
    {
        discord.RunCallbacks();
    }

    public Discord.Discord GetDiscord()
    {
        return discord;
    }

    private void OnApplicationQuit()
    {
        discord.Dispose();
    }
}
