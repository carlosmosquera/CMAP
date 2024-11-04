using UnityEngine;
using extOSC;


public class PlayerMovement : MonoBehaviour
{
    public OSCTransmitter OSCTransmitter;

    public float speed = 5f;
    public Rect boundary;


    void Start()
    {
        var initalYaw = new OSCMessage("/create/listener/yaw");
        initalYaw.AddValue(OSCValue.Float(180f));
        OSCTransmitter.Send(initalYaw);
    }


    void Update()
    {
        // Get input
        float moveX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float moveY = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        // Move the player
        Vector3 newPosition = transform.position + new Vector3(moveX, moveY, 0);

        // Clamp the player's position within the boundary
        newPosition.x = Mathf.Clamp(newPosition.x, boundary.xMin, boundary.xMax);
        newPosition.y = Mathf.Clamp(newPosition.y, boundary.yMin, boundary.yMax);

        // Apply the new position
        transform.position = newPosition;

        float scaledValueX = ScaleValueX(transform.position.x, -216f, 216f, -800f, 800f);

        float ScaleValueX(float value, float oldMin, float oldMax, float newMin, float newMax)
        {
            return newMin + ((value - oldMin) / (oldMax - oldMin)) * (newMax - newMin);
        }

        float scaledValueY = ScaleValueY(transform.position.y, -216f, 216f, -1000f, 1000f);

        float ScaleValueY(float value, float oldMin, float oldMax, float newMin, float newMax)
        {
            return newMin + ((value - oldMin) / (oldMax - oldMin)) * (newMax - newMin);
        }
        var messageListPosXYZ = new OSCMessage("/create/listener/xyz");
        messageListPosXYZ.AddValue(OSCValue.Float(scaledValueX * -1f));
        messageListPosXYZ.AddValue(OSCValue.Float(-1f * scaledValueY + 9f));
        messageListPosXYZ.AddValue(OSCValue.Float(1.7f));



        OSCTransmitter.Send(messageListPosXYZ);




    }


}
