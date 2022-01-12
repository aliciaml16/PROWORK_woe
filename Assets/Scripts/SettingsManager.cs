using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using extOSC;

public class SettingsManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button step1Next;
    public Button step2Back;
    public Button step2Next;
    public Button step3Back;
    public Button step3Next;
    public Button step4Back;
    public Button step4Next;
    public Button iosButton;
    public Button androidButton;
    private string operatingSystemInput = "ios";

    [Header("Screens")]
    public GameObject step1Screen;
    public GameObject step2Screen;
    public GameObject step3Screen;
    public GameObject step4Screen;

    [Header("Input Fields - Text")]
    public Text inputPort;
    public Text inputUIID;

    [Header("Alert - Step 2")]
    public Button step2ok;
    public GameObject alertBackground;
    public GameObject alertScreen;

    [Header("Sprites - Step 3")]
    public Image topLeft;
    public Image topMiddle;
    public Image topRight;
    public Image bottomLeft;
    public Image bottomMiddle;
    public Image bottomRight;
    public Sprite clickWhite;
    public Sprite clickGreen;
    public Sprite moveWhite;
    public Sprite moveGreen;

    [Header("ZIG SIM settings")]
    private int oscPortNumber; // port number
    private string oscDeviceUUID; // UUID
    private string operatingSystem; // type of operating system (iOs or Windows)

    void Start()
    {
        // We add listeners to each button
        step1Next.onClick.AddListener(step1NextF);
        step2Back.onClick.AddListener(step2BackF);
        step2Next.onClick.AddListener(step2NextF);
        step3Back.onClick.AddListener(step3BackF);
        step3Next.onClick.AddListener(step3NextF);
        step4Back.onClick.AddListener(step4BackF);
        step4Next.onClick.AddListener(step4NextF);
        iosButton.onClick.AddListener(changeToIOS);
        androidButton.onClick.AddListener(changeToAndroid);
        step2ok.onClick.AddListener(closeAlert);

        // We hide all the screens but the step1 screen
        step1Screen.SetActive(true);
        step2Screen.SetActive(false);
        step3Screen.SetActive(false);
        step4Screen.SetActive(false);
        alertBackground.SetActive(false);
        alertScreen.SetActive(false);

        // We delete all the player settings
        PlayerPrefs.DeleteAll();
    }

    // Functions that will change between screens
    private void step1NextF()
    {
        step1Screen.SetActive(false);
        step2Screen.SetActive(true);
    }

    private void step2BackF()
    {
        step1Screen.SetActive(true);
        step2Screen.SetActive(false);
    }

    private void step2NextF()
    {
        if (inputPort.text == "" || inputUIID.text == "")
        {
            alertBackground.SetActive(true);
            alertScreen.SetActive(true);
        } else
        {
            PlayerPrefs.SetInt("portNumber", int.Parse(inputPort.text));
            PlayerPrefs.SetString("UIID", inputUIID.text);
            PlayerPrefs.SetString("OS", operatingSystemInput);
            PlayerPrefs.Save();
            step2Screen.SetActive(false);
            step3Screen.SetActive(true);

            // We get the values of ZIG SIM of the user
            oscPortNumber = PlayerPrefs.GetInt("portNumber");
            oscDeviceUUID = PlayerPrefs.GetString("UIID");
            operatingSystem = PlayerPrefs.GetString("OS");

            // We connect the phone with the computer by creating a OSCReceiver
            // This will have the values we determinated in the editor (port, uuid and operating system)
            OSCReceiver receiver = gameObject.AddComponent<OSCReceiver>();
            receiver.LocalPort = oscPortNumber;
            if (operatingSystem == "ios")
            {
                receiver.Bind("/ZIGSIM/" + oscDeviceUUID + "/gyro", OnMove);
                receiver.Bind("/ZIGSIM/" + oscDeviceUUID + "/touch0", OnClick);
            }
            else
            {
                receiver.Bind("/" + oscDeviceUUID + "/gyro", OnMove);
                receiver.Bind("/" + oscDeviceUUID + "/touch0", OnClick);
            }
        }
    }

    private void closeAlert()
    {
        alertBackground.SetActive(false);
        alertScreen.SetActive(false);
    }

    private void step3BackF()
    {
        step2Screen.SetActive(true);
        step3Screen.SetActive(false);
    }

    private void step3NextF()
    {
        step3Screen.SetActive(false);
        step4Screen.SetActive(true);
    }

    private void step4BackF()
    {
        step3Screen.SetActive(true);
        step4Screen.SetActive(false);
    }

    private void step4NextF()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void changeToIOS()
    {
        iosButton.GetComponent<Image>().color = new Color32(155, 167, 85, 255);
        androidButton.GetComponent<Image>().color = new Color32(247, 239, 228, 51);
        operatingSystemInput = "ios";
    }

    private void changeToAndroid()
    {
        androidButton.GetComponent<Image>().color = new Color32(155, 167, 85, 255);
        iosButton.GetComponent<Image>().color = new Color32(247, 239, 228, 51);
        operatingSystemInput = "other";
    }

    // ZIG SIM Working
    public void OnMove(OSCMessage message)
    {
        // DOWN
        if (message.Values[1].FloatValue < -0.4)
        {
            topMiddle.sprite = moveGreen;
            bottomMiddle.sprite = moveWhite;
        }
        // UP
        else if (message.Values[1].FloatValue > 0.4)
        {
            bottomMiddle.sprite = moveGreen;
            topMiddle.sprite = moveWhite;
        }
        else {
            bottomMiddle.sprite = moveWhite;
            topMiddle.sprite = moveWhite;
        }
    }

    public void OnClick(OSCMessage message)
    {
        double x = 0;
        double y = 0;
        if (operatingSystem == "ios")
        {
            x = message.Values[0].FloatValue;
            y = message.Values[1].FloatValue;
        }
        else
        {
            x = message.Values[0].DoubleValue;
            y = message.Values[1].DoubleValue;
        }

        // Close fact (right bottom)
        if (x < 0 && x > -1 && y > 0 && y < 1)
        {
            bottomRight.sprite = clickGreen;
        }
        else
        {
            bottomRight.sprite = clickWhite;
        }

        // Menu (right top)
        if (x > 0 && x < 1 && y > 0 && y < 1)
        {
            topRight.sprite = clickGreen;
        }
        else
        {
            topRight.sprite = clickWhite;
        }

        // Back (left top)
        if (x > 0 && x < 1 && y < 0 && y > -1)
        {
            topLeft.sprite = clickGreen;
        }
        else
        {
            topLeft.sprite = clickWhite;
        }

        // Confirm (left bottom)
        if (x < 0 && x > -1 && y < 0 && y > -1)
        {
            bottomLeft.sprite = clickGreen;
        }
        else
        {
            bottomLeft.sprite = clickWhite;
        }

    }
}
