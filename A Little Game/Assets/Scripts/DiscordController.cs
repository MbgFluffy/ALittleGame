using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;

public class DiscordController : MonoBehaviour
{
    public Discord.Discord discord;
    public Discord.Activity activity;
    public Discord.ActivityManager activityManager;

    void Start()
    {
        discord = new Discord.Discord(847139669845082162, (System.UInt64)Discord.CreateFlags.Default);
        activityManager = discord.GetActivityManager();
        activity = new Discord.Activity {
            Details = "Testing Discord Rich presence",
            State = "Playing on ExampleScene",
            Timestamps =
            {
                Start = System.DateTimeOffset.Now.ToUnixTimeSeconds()
            }
        };
        activityManager.UpdateActivity(activity, (res) => {
            if (res == Discord.Result.Ok)
                Debug.Log("Discord status set!");
            else
                Debug.LogError("Discord status failed!");
        });
    }

    void Update()
    {
        discord.RunCallbacks();
    }

    void OnApplicationQuit()
    {
        activityManager.ClearActivity((res) => { });
        discord.Dispose();
        Debug.Log("Game has stopped!");
    }
}