using System.Collections;
using System.Collections.Generic;
using extOSC;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    // This is our game

    [Header("ZIG SIM settings")]
    public int oscPortNumber = 3300; // port number
    public string oscDeviceUUID = "aliciaphone"; // UUID
    public string operatingSystem = "ios"; // type of operating system (iOs or Windows)

    [Header("Movement variables")]
    public float speed = 10.0f; // Speed of the ball
    private float movementX; // Horizontal movement of the ball
    private float movementY; // Vertical movement of the ball
    private Rigidbody rb; // Rigidbody of the ball

    private void Start()
    {
        // We connect the phone with the computer by creating a OSCReceiver
        // This will have the values we determinated in the editor (port, uuid and operating system)
        OSCReceiver receiver = gameObject.AddComponent<OSCReceiver>();
        receiver.LocalPort = oscPortNumber;
        if (operatingSystem == "ios")
        {
            receiver.Bind("/ZIGSIM/" + oscDeviceUUID + "/gyro", OnMove);
        }
        else {
            receiver.Bind("/" + oscDeviceUUID + "/gyro", OnMove);
        }

        // We assign the rigidbody to the variable
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // We update the movement of the ball
        Vector3 movement = new Vector3(movementX, movementY, 0.0f);
        rb.AddForce(movement * speed);

        // Rotation of the earth
        transform.Rotate(new Vector3(0, 30, 0) * Time.deltaTime);
    }

    public void OnMove(OSCMessage message)
    {
        // We get the values we receive from the phone and assign them to the movement
        movementX = message.Values[0].FloatValue;
        movementY = message.Values[1].FloatValue;
    }
}
