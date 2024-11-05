using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class DisplayLocalIPAddress : MonoBehaviour
{
    public Text ipDisplayText; // Assign a TextMeshPro UI component in the Inspector

    void Start()
    {
        string localIP = GetLocalIPAddress();
        if (ipDisplayText != null)
        {
            ipDisplayText.text = $"Local IP Address: {localIP}";
        }
        else
        {
            Debug.Log($"Local IP Address: {localIP}");
        }
    }

    private string GetLocalIPAddress()
    {
        string localIP = "Not Available";
        foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
    }
}
