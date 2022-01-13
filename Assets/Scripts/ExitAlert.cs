using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitAlert : MonoBehaviour
{
    [Header("Exit Alert")]
    public GameObject exitAlertScreen;
    public Button exitAlertButtonYes;
    public Button exitAlertButtonNo;

    void Start()
    {
        exitAlertButtonYes.onClick.AddListener(QuitGame);
        exitAlertButtonNo.onClick.AddListener(closeExitAlert);
        exitAlertScreen.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            exitAlertScreen.SetActive(true);
        }
    }

    // Exit Alert
    private void closeExitAlert()
    {
        exitAlertScreen.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
