using PubNubAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleChat : MonoBehaviour
{
    PubNub pubnub;
    string cipherKey = "";
    string channel = "Word4Word";

    public UnityEngine.UI.InputField input;

    private void Start()
    {
        Init();
    }
    void Init()
    {
        Debug.Log("Starting");
        PNConfiguration pnConfiguration = new PNConfiguration();
        pnConfiguration.SubscribeKey = "sub-c-29000a87-5fb6-48b1-b999-4aa999af9bc2";
        pnConfiguration.PublishKey = "pub-c-27b92812-ce75-4fb5-b940-558078cbbbf0";
        pnConfiguration.SecretKey = "sec-c-NmExOGNlMmEtNGFmYy00Y2ZjLWIzZDgtMDJkYTE2ZTU0YzBh";

        pnConfiguration.CipherKey = cipherKey;
        pnConfiguration.LogVerbosity = PNLogVerbosity.BODY;
        pnConfiguration.PresenceTimeout = 120;
        pnConfiguration.PresenceInterval = 60;
        pnConfiguration.HeartbeatNotificationOption = PNHeartbeatNotificationOption.All;

        //TODO: remove
        pnConfiguration.UserId = "PubNubUnityExample";
        Debug.Log("PNConfiguration");
        pubnub = new PubNub(pnConfiguration);

        pubnub.SubscribeCallback += SubscribeCallbackHandler;
        pubnub.Subscribe().Channels(new List<string>() { channel }).Execute();
    }

    private void SubscribeCallbackHandler(object sender, EventArgs e)
    {
        Debug.Log("SubscribeCallbackHandler Event handler");
        SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;

        if (mea.Status != null)
        {
            switch (mea.Status.Category)
            {
                case PNStatusCategory.PNConnectedCategory:
                    //PrintStatus(mea.Status);
                    break;
                case PNStatusCategory.PNUnexpectedDisconnectCategory:
                case PNStatusCategory.PNTimeoutCategory:
                    pubnub.Reconnect();
                    pubnub.CleanUp();
                    break;
            }
        }
        else
        {
            Debug.Log("mea.Status null" + e.GetType().ToString() + mea.GetType().ToString());
        }
        if (mea.MessageResult != null)
        {
            Debug.Log("In Example, SubscribeCallback in message" + mea.MessageResult.Channel + " : " + mea.MessageResult.Payload);
            SaveWords(mea.MessageResult.Payload.ToString());
            //Display(string.Format("SubscribeCallback Result: {0}", pubnub.JsonLibrary.SerializeToJsonString(mea.MessageResult.Payload)));
        }
        if (mea.PresenceEventResult != null)
        {
            Debug.Log("In Example, SubscribeCallback in presence" + mea.PresenceEventResult.Channel + mea.PresenceEventResult.Occupancy + mea.PresenceEventResult.Event + mea.PresenceEventResult.State);
        }
        if (mea.SignalEventResult != null)
        {
            Debug.Log("In Example, SubscribeCallback in SignalEventResult" + mea.SignalEventResult.Channel + mea.SignalEventResult.Payload);
            //Display(string.Format("SubscribeCallback SignalEventResult: {0}", pubnub.JsonLibrary.SerializeToJsonString(mea.SignalEventResult.Payload)));
        }
    }

    public void PublishMsg()
    {
        pubnub.Publish()
    .Channel(channel)
    .Message(input.text)
    .Async((result, status) => {
        if (!status.Error)
        {
            Debug.Log(string.Format("DateTime {0}, In Publish Example, Timetoken: {1}", DateTime.UtcNow, result.Timetoken));
        }
        else
        {
            Debug.Log(status.Error);
            Debug.Log(status.ErrorData.Info);
        }
    });
    }

    private void SaveWords(string text)
    {
        // text :: would be comma separated words (KITE,NONE,GOOD etc)
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, "New_Words.txt");
        Debug.Log("--SaveWords--" + filePath);

        System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath, true);
        System.IO.StringReader sr = new System.IO.StringReader(text);
        while(sr.Peek() != -1)
        {
            string strLine = sr.ReadLine();
            strLine = strLine.Trim();
            string[] arr = strLine.Split(',');
            foreach(string word in arr)
            {
                sw.WriteLine(word.Trim());
            }
        }
        sw.Close();
        sr.Close();
    }
}
