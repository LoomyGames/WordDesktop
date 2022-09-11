using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PubNubAPI;
using System;

public class PubNubManager
{
    PubNub pubnub;
    string cipherKey = "";
    string channel = "Word4Word";

    public void Init()
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
        }
        if (mea.PresenceEventResult != null)
        {
            Debug.Log("In Example, SubscribeCallback in presence" + mea.PresenceEventResult.Channel + mea.PresenceEventResult.Occupancy + mea.PresenceEventResult.Event + mea.PresenceEventResult.State);
        }
        if (mea.SignalEventResult != null)
        {
            Debug.Log("In Example, SubscribeCallback in SignalEventResult" + mea.SignalEventResult.Channel + mea.SignalEventResult.Payload);
        }
    }

    public void PublishMsg(object msg)
    {
        pubnub.Publish()
    .Channel(channel)
    .Message(msg)
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
}
