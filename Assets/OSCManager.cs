using UnityEngine;
using extOSC;
using System.Collections;
using UnityEngine.UI;

public class OSCManager : MonoBehaviour
{
    public InputField ipInputField;
    public OSCTransmitter[] oscTransmitters;
    public OSCTransmitter heartbeatTransmitter;
    public OSCReceiver heartbeatReceiver;
    public SpriteRenderer connectionIndicator;
    public float heartbeatInterval = 1f;
    public float connectionTimeout = 2f;

    private bool isConnected = false;
    private float lastHeartbeatTime;

    void Start()
    {
        if (ipInputField != null)
        {
            ipInputField.onEndEdit.AddListener(UpdateTransmittersIP);
            ipInputField.Select();
            ipInputField.ActivateInputField();
        }

        if (heartbeatReceiver != null)
        {
            heartbeatReceiver.Bind("/heartbeat_ack", OnHeartbeatAckReceived);
        }

        StartCoroutine(HeartbeatCoroutine());
    }

    public void UpdateTransmittersIP(string newIP)
    {
        if (IsValidIPAddress(newIP))
        {
            foreach (var transmitter in oscTransmitters)
            {
                if (transmitter != null) // Check if transmitter is not destroyed
                {
                    transmitter.RemoteHost = newIP;
                    Debug.Log($"Updated OSC Transmitter on {transmitter.gameObject.name} to IP: {newIP}");
                }
            }
            if (heartbeatTransmitter != null) // Check if heartbeat transmitter is not destroyed
            {
                heartbeatTransmitter.RemoteHost = newIP;
                Debug.Log($"Heartbeat transmitter set to IP: {newIP}");
            }
        }
        else
        {
            Debug.LogWarning("Invalid IP Address entered.");
        }
    }

    private IEnumerator HeartbeatCoroutine()
    {
        while (true)
        {
            if (heartbeatTransmitter != null && IsValidIPAddress(heartbeatTransmitter.RemoteHost))
            {
                SendHeartbeat();
                yield return new WaitForSeconds(heartbeatInterval);

                if (Time.time - lastHeartbeatTime > connectionTimeout && isConnected)
                {
                    isConnected = false;
                    UpdateConnectionIndicator(false);
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    private void SendHeartbeat()
    {
        if (heartbeatTransmitter != null)
        {
            var message = new OSCMessage("/heartbeat");
            message.AddValue(OSCValue.Int(1));
            heartbeatTransmitter.Send(message);

            if (!isConnected)
            {
                UpdateConnectionIndicator(false);
            }
        }
    }

    private void OnHeartbeatAckReceived(OSCMessage message)
    {
        isConnected = true;
        lastHeartbeatTime = Time.time;
        UpdateConnectionIndicator(true);
    }

    private void UpdateConnectionIndicator(bool connected)
    {
        if (connectionIndicator != null)
        {
            connectionIndicator.color = connected ? Color.green : Color.red;
        }
    }

    private bool IsValidIPAddress(string ipAddress)
    {
        System.Net.IPAddress parsedIP;
        return System.Net.IPAddress.TryParse(ipAddress, out parsedIP);
    }
}
