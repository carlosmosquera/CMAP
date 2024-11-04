using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using extOSC;

public class OutputMeters : MonoBehaviour
{
    public OSCReceiver OSCReceiver;


    private string _channel1LevelOut = "/channelOut/1";
    private string _channel2LevelOut = "/channelOut/2";
    private string _channel3LevelOut = "/channelOut/3";
    private string _channel4LevelOut = "/channelOut/4";
    private string _channel5LevelOut = "/channelOut/5";
    private string _channel6LevelOut = "/channelOut/6";
    private string _channel7LevelOut = "/channelOut/7";
    private string _channel8LevelOut = "/channelOut/8";
    private string _channel9LevelOut = "/channelOut/9";
    private string _channel10LevelOut = "/channelOut/10";


    public Slider channel1Out;
    public Slider channel2Out;
    public Slider channel3Out;
    public Slider channel4Out;
    public Slider channel5Out;
    public Slider channel6Out;
    public Slider channel7Out;
    public Slider channel8Out;
    public Slider channel9Out;
    public Slider channel10Out;


    // Start is called before the first frame update
    void Start()
    {


        OSCReceiver.Bind(_channel1LevelOut, ReceivedOutput1);
        OSCReceiver.Bind(_channel2LevelOut, ReceivedOutput2);
        OSCReceiver.Bind(_channel3LevelOut, ReceivedOutput3);
        OSCReceiver.Bind(_channel4LevelOut, ReceivedOutput4);
        OSCReceiver.Bind(_channel5LevelOut, ReceivedOutput5);
        OSCReceiver.Bind(_channel6LevelOut, ReceivedOutput6);
        OSCReceiver.Bind(_channel7LevelOut, ReceivedOutput7);

        OSCReceiver.Bind(_channel8LevelOut, ReceivedOutput8);
        OSCReceiver.Bind(_channel9LevelOut, ReceivedOutput9);
        OSCReceiver.Bind(_channel10LevelOut, ReceivedOutput10);

    }


    // -------- OUTPUT ------------

    public void ReceivedOutput1(OSCMessage message)
    {
        //Debug.Log("channel1Out" + message);

        if (message.ToFloat(out var value))
        {
            channel1Out.value = value;
        }

    }

    public void ReceivedOutput2(OSCMessage message)
    {
        //Debug.Log("channel2Out" + message);

        if (message.ToFloat(out var value))
        {
            channel2Out.value = value;
        }

    }

    public void ReceivedOutput3(OSCMessage message)
    {
        //Debug.Log("channel3Out" + message);

        if (message.ToFloat(out var value))
        {
            channel3Out.value = value;
        }

    }

    public void ReceivedOutput4(OSCMessage message)
    {
        //Debug.Log("channel4Out" + message);

        if (message.ToFloat(out var value))
        {
            channel4Out.value = value;
        }

    }

    public void ReceivedOutput5(OSCMessage message)
    {
        //Debug.Log("channel5Out" + message);

        if (message.ToFloat(out var value))
        {
            channel5Out.value = value;
        }

    }

    public void ReceivedOutput6(OSCMessage message)
    {
        //Debug.Log("channel6Out" + message);

        if (message.ToFloat(out var value))
        {
            channel6Out.value = value;
        }

    }

    public void ReceivedOutput7(OSCMessage message)
    {
        //Debug.Log("channel7Out" + message);

        if (message.ToFloat(out var value))
        {
            channel7Out.value = value;
        }

    }

    public void ReceivedOutput8(OSCMessage message)
    {
        //Debug.Log("channel8Out" + message);

        if (message.ToFloat(out var value))
        {
            channel8Out.value = value;
        }

    }

    public void ReceivedOutput9(OSCMessage message)
    {
        //Debug.Log("channel9Out" + message);

        if (message.ToFloat(out var value))
        {
            channel9Out.value = value;
        }

    }

    public void ReceivedOutput10(OSCMessage message)
    {
        //Debug.Log("channel10Out" + message);

        if (message.ToFloat(out var value))
        {
            channel10Out.value = value;
        }

    }


}
