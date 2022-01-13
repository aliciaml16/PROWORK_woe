using extOSC;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // This is our game

    [Header("ZIG SIM settings")]
    private int oscPortNumber; // port number
    private string oscDeviceUUID; // UUID
    private string operatingSystem; // type of operating system (iOs or Windows)

    [Header("Menu")]
    public GameObject helpMenu;
    public GameObject[] buttonsMenu;
    private int selectedOption = 0;
    private int maxButton = 3;
    private bool helpOpen = false;

    // audio

    public AudioSource backgroundMusic;
    public AudioClip buttonScroll;
    public AudioClip buttonSelected;
    public AudioClip buttonBack;

    private AudioSource audioSource;


    [Header("Exit Alert")]
    public GameObject exitAlertScreen;
    public Button exitAlertButtonYes;
    public Button exitAlertButtonNo;

    private void Start()
    {
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
            receiver.Bind("/ZIGSIM/" + oscDeviceUUID + "/touch0", CloseUI);
        }
        else {
            receiver.Bind("/" + oscDeviceUUID + "/gyro", OnMove);
            receiver.Bind("/" + oscDeviceUUID + "/touch0", CloseUI);
        }

        // get audio

        audioSource = GetComponent<AudioSource>();


        exitAlertButtonYes.onClick.AddListener(QuitGame);
        exitAlertButtonNo.onClick.AddListener(closeExitAlert);
        exitAlertScreen.SetActive(false);
    }

    void Update()
    {
        // Menu
        for (int i = 0; i < buttonsMenu.Length; i++)
        {
            buttonsMenu[i].GetComponent<Image>().color = new Color(0.9686275f, 0.9372549f, 0.8941177f, 0.2f);
        }
        // last button has a different color
        buttonsMenu[buttonsMenu.Length - 1].GetComponent<Image>().color = new Color(0.8117647058f, 0.6156862745f, 0.36862745098f, 0.2f);

        // selected button color
        if (selectedOption == maxButton)
        {
            // last button (exit) has different select color
            buttonsMenu[selectedOption].GetComponent<Image>().color = new Color(0.8117647058f, 0.6156862745f, 0.36862745098f, 0.4f);
        }
        else
        {
            buttonsMenu[selectedOption].GetComponent<Image>().color = new Color(0.6078432f, 0.654902f, 0.3333333f, 0.4f);
        }
    }

    public void OnMove(OSCMessage message)
    {
        if(!helpOpen)
        {
            if (message.Values[1].FloatValue < -0.3)
            {
                selectedOption += 1;
                if (selectedOption > maxButton)
                {
                    selectedOption = maxButton;
                }
                else
                {
                    audioSource.PlayOneShot(buttonScroll);
                }


            }
            else if (message.Values[1].FloatValue > 0.3)
            {
                selectedOption -= 1;
                if (selectedOption < 0)
                {
                    selectedOption = 0;
                }

                else
                {
                    audioSource.PlayOneShot(buttonScroll);
                }

            }
        }
    }

    public void CloseUI(OSCMessage message)
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

        //Back from help
        if (x > 0 && x < 1 && y < 0 && y > -1)
            {
                if (helpOpen == true)
                {
                    helpOpen = false;
                    helpMenu.SetActive(false);

                    audioSource.PlayOneShot(buttonBack);
                }
            }
        // Confirm
        if (x < 0 && x > -1 && y < 0 && y > -1)
        {
            audioSource.PlayOneShot(buttonSelected);

            switch (selectedOption)
            {
                case 0:
                    // RESUME
                    PlayGame();
                    break;
                case 1:
                    // HELP
                    helpOpen = true;
                    helpMenu.SetActive(true);
                    break;
                case 2:
                    // SETTINGS
                    GoSettings();
                    break;
                case 3:
                    // MAIN MENU
                    exitAlertScreen.SetActive(true);
                    break;
            }
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }

    private void closeExitAlert()
    {
        exitAlertScreen.SetActive(false);
        Time.timeScale = 1;
    }

    public void GoSettings()
    {
        SceneManager.LoadScene("Settings-ingame");
    }
}
