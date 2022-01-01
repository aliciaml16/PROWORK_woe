using System.Collections;
using System.Collections.Generic;
using extOSC;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;




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

    [Header("QM - Lifetime bar")]
    public Image lifetimeBar; // Lifetime bar (green)
    private float lifetime = 100;
    private float initialBarWidth;

    [Header("QM - Gradients")]
    public Gradient greenGradient;
    public Gradient blueGradient;
    public Gradient whiteGradient;
    private Renderer worldRenderer;
    public Renderer cloudsRenderer;

    [Header("QM - Boxes")]
    public GameObject box1;
    public GameObject box2;
    private float timer = 0.0f; // timer that will activate a new question
    private Vector3 positionBadBox;
    private Vector3 positionGoodBox;
    public GameObject[] spawnPoint;

    [Header("QM - Questions")]
    public float timeBetweenQuestions = 10;
    public int numberOfQuestionsToGet = 10;
    public GameObject questionText;
    private int numberOfQuestions = 0;
    private bool isPossibleQuestion = false;

    [Header("QM - Facts")]
    public GameObject factTitle;
    public GameObject factText;
    public GameObject quickTip;
    public GameObject factUI;

    [Header("QM - UI")]
    public GameObject gameOverScreen;
    public GameObject winScreen;
    public GameObject mediumScreen;
    private int correctpoint, wrongpoint;
    int index;

    [Header("QM - Arrays")]
    public GameObject[] goodObjects;
    public GameObject[] badObjects;
    private List<string> questions = new List<string>() {
        "Choose between eating a steak or vegetables",
        "Choose between eating a hamburger or a salad",
        "Choose between taking the car or your bike",
        "Choose between taking the car or going for a walk",
        "Choose between buying a plastic bottle or using a reusable one",
        "Choose between buying a plastic bag or using a reusable one",
        "Choose between sorting trash or using only one bin",
        "Choose between flying or taking the train",
        "Choose between flying or taking the bus",
        "Choose between flying or using a sailing boat",
        "Choose between turning off the lights or leaving them on",
        "Choose between buying new things or second-hand ones",
        "Choose between buying local or at a big supermarket",
        "Choose between using a LED bulb or a normal one",
        "Choose between using coal energy or wind energy",
        "Choose between using oil-generated energy or using solar power"
    };

    private List<string> badTip = new List<string>() {
        "Certain meats are worse than others for the environment. Maybe try out a Meatless-Monday for instance?",
        "Oops, did you know that by eating a hamburger with meat, you are using the same amount of water as taking a shower for 2 months? ",
        "Did you know that sometimes we take the car for short distances and that contributes more to pollution and CO\u2082?",
        "Is it necessary? Not only could the distance be a short one, but the fuel costs can get expensive.",
        "Think again! The production of single-use plastic bottles is made out of oil and gas, and only 14% are recyclable.",
        "Did you know that plastic can easily be spread everywhere and end up in the sea? Sea animals might think that the plastic looks like jellyfish and try to eat it, and that could potentially kill them.",
        "Did you know, if you are not sorting your trash correctly, it gets difficult to recycle and decompose everything?",
        "Air travel dominates a frequent traveler’s individual contribution to climate change. Yet aviation overall accounts for only 2.5% of global carbon dioxide (CO\u2082) emissions.",
        "Flights produce greenhouse gases - mainly carbon dioxide (CO\u2082) - from burning fuel. These contribute to global warming when released into the atmosphere.",
        "Emissions from flights stay in the atmosphere and will warm it for several centuries. Because aircraft emissions are released high in the atmosphere, they have a potent climate impact, triggering chemical reactions and atmospheric effects that heat the planet.",
        "Did you know that always having the lights on is consuming a lot of energy and can get very expensive too?",
        "Maybe it's not necessary to buy new things frequently. Overconsumption is one of the biggest pollution creators.",
        "Did you know that some foods can be injected with chemicals in order for them to look good enough?",
        "Did you know by using the wrong type of light bulb, you are wasting more energy than you might think?",
        "It’s not just the affect oil has on wildlife; oil contamination can make water unsuitable for irrigation and damage how water treatment plants work.",
        "Did you know that coal originally comes from oil production but has to be heated up to become coal?"
    };

    private List<string> goodTip = new List<string>() {
        "Fantastic! Did you know that eating more vegetables is not only healthy for you but also uses less water than meat?",
        "Not only are you using less water than with meat production, but it also benefits the wildlife and animals!",
        "Did you know that taking the bike not only is good for the environment, it can also improve your mood?",
        "Movement is always good for you and it’s free!",
        "Stay hydrated! Glass bottles have a longer life-span than plastic and can be cleaned easier than plastic bottles.",
        "Using paper-, cotton- or recyclable plastic bags can be used multiple times and have different purposes.",
        "Did you know that your food waste could be used for boosting your garden? Give it a try, your flowers will love the nutrition!",
        "Compared to cars and airplanes, trains emit between 66 and 75 percent less carbon. In terms of energy consumption, use of space, and noise levels, trains are far more sustainable too.",
        "Public transportation inherently benefits the environment because it reduces the number of people driving single-occupancy vehicles.",
        "Sailboats aren't as damaging on the environment as boats that exclusively rely on a motor. And it’s a nice way to explore the sea.",
        "Did you know that turning off the lights saves up some energy that can be used for another time?",
        "Did you know by buying second hand or fixing something old, you are giving it a new life?",
        "Did you know that shopping locally not only helps local farmers, but it also reduces your carbon footprint?",
        "Did you know that it's 80% more positive to use LED bulbs and they require less energy?",
        "Did you know, even when it’s cloudy, the solar power can still produce energy?",
        "Did you know that wind is an emissions-free source of energy and only relies on the wind? Depending on wind conditions, the blades turn at rates between 10 and 20 revolutions per minute."
    };

    [Header("Menu")]
    public GameObject pauseMenu;
    public GameObject helpMenu;
    private bool menuOpen = false;
    private bool helpOpen = false;
    private int selectedOption = 0;
    private int maxButton = 3;
    private bool gameOver = false;
    public GameObject[] buttonsPause;
    public GameObject[] buttonsWin;
    public GameObject[] buttonsLose;
    public GameObject[] buttonsMedium;

    // audio

    [Header("QM - Audio")]
    public AudioSource backgroundMusic;
    public AudioClip correctAnswer;
    public AudioClip wrongAnswer;
    public AudioClip winGame;
    public AudioClip loseGame;
    public AudioClip pollutionHit;
    public AudioClip buttonScroll;
    public AudioClip buttonSelected;
    public AudioClip buttonBack;
    public AudioClip buttonClose;

    private AudioSource audioSource;
    private bool isSoundPlayed = false;

    [Header("QM - Tips beginning")]

    public GameObject moveTip;
    public GameObject menuTip;
    public float tipLength = 3.5f;
    private bool tipsShown = false;

    private void Start()
    {
        // We connect the phone with the computer by creating a OSCReceiver
        // This will have the values we determinated in the editor (port, uuid and operating system)
        OSCReceiver receiver = gameObject.AddComponent<OSCReceiver>();
        receiver.LocalPort = oscPortNumber;
        if (operatingSystem == "ios")
        {
            receiver.Bind("/ZIGSIM/" + oscDeviceUUID + "/gyro", OnMove);
            receiver.Bind("/ZIGSIM/" + oscDeviceUUID + "/touch0", OnClick);
        }
        else {
            receiver.Bind("/" + oscDeviceUUID + "/gyro", OnMove);
            receiver.Bind("/" + oscDeviceUUID + "/touch0", OnClick);
        }

        // We assign the rigidbody to the variable
        rb = GetComponent<Rigidbody>();

        // QM
        initialBarWidth = lifetimeBar.transform.localScale.x;
        worldRenderer = GetComponent<Renderer>();

        questionText.SetActive(false);
        box1.SetActive(false);
        box2.SetActive(false);
        timer = 0.0f;

        factUI.SetActive(false);

        positionBadBox = new Vector3(-5, 0.7f, 47.7f);
        positionGoodBox = new Vector3(5, 0.7f, 47.7f);

        Time.timeScale = 1;

        // We show the tips
        if (PlayerPrefs.HasKey("tipScreen"))
        {
            tipsShown = true;
            // Uncomment next line to test the showing of the tipscreen
            //PlayerPrefs.DeleteKey("tipScreen");
        }
        else
        {
            PlayerPrefs.SetString("tipScreen", "true");
            PlayerPrefs.Save();
            moveTip.SetActive(true);
        }
               
        // get audio

        audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        // We update the movement of the ball
        Vector3 movement = new Vector3(movementX, movementY, 0.0f);
        rb.AddForce(movement * speed);

        // Rotation of the earth
        transform.Rotate(new Vector3(0, 30, 0) * Time.deltaTime);
    }

    void Update()
    {
        // Changing size of the lifetime bar
        lifetimeBar.transform.localScale = new Vector3(lifetime * initialBarWidth / 100, 0.1561234f, 0);

        // Change the colors depending on the life
        lifetimeBar.color = greenGradient.Evaluate(lifetime / 100);
        worldRenderer.materials[1].SetColor("_Color", greenGradient.Evaluate(lifetime / 100));
        worldRenderer.materials[0].SetColor("_Color", blueGradient.Evaluate(lifetime / 100));
        cloudsRenderer.material.SetColor("_Color", whiteGradient.Evaluate(lifetime / 100));

        // Timer working
        if (timer > timeBetweenQuestions && isPossibleQuestion == false && tipsShown)
        {
            isPossibleQuestion = true;
            newQuestion();
        }
        else
        {
            // Moving the boxes near to the camera
            box1.transform.position = Vector3.MoveTowards(box1.transform.position, positionBadBox, 0.5f);
            box2.transform.position = Vector3.MoveTowards(box2.transform.position, positionGoodBox, 0.5f);
        }

        // Remove tip screens
        if (!tipsShown && timer > tipLength)
        {
            moveTip.SetActive(false);
            menuTip.SetActive(true);
        }
        if (!tipsShown && timer > 2*tipLength)
        {
            menuTip.SetActive(false);
            tipsShown = true;
            timer = 0.0f;
        }
        
        // check for game over
        if (numberOfQuestions == numberOfQuestionsToGet && lifetime > 50)
        {
            gameOver = true;
            winScreen.SetActive(true);
            if(isSoundPlayed == false)
            {
                audioSource.PlayOneShot(winGame);
                isSoundPlayed = true;
            }

            for (int i = 0; i < buttonsWin.Length; i++)
            {
                buttonsWin[i].GetComponent<Image>().color = new Color(0.2588235f, 0.3098039f, 0.254902f, 0.2f);
            }
            // last button has a different color
            buttonsWin[buttonsWin.Length - 1].GetComponent<Image>().color = new Color(0.60784313f, 0.36862745f, 0.24705882f, 0.35f);

            // selected button color
            if (selectedOption == maxButton)
            {
                // last button (exit) has different select color
                buttonsWin[selectedOption].GetComponent<Image>().color = new Color(0.60784313f, 0.36862745f, 0.24705882f, 0.7f);
            }
            else
            {
                buttonsWin[selectedOption].GetComponent<Image>().color = new Color(0.2588235f, 0.3098039f, 0.254902f, 0.7f);
            }
        }
        else if (numberOfQuestions == numberOfQuestionsToGet && lifetime <= 50 && lifetime > 0)
        {
            gameOver = true;
            mediumScreen.SetActive(true);

            if (isSoundPlayed == false)
            {
                audioSource.PlayOneShot(winGame);
                isSoundPlayed = true;
            }

            for (int i = 0; i < buttonsMedium.Length; i++)
            {
                buttonsMedium[i].GetComponent<Image>().color = new Color(0.9686275f, 0.9372549f, 0.8941177f, 0.2f);
            }
            // last button has a different color
            buttonsMedium[buttonsMedium.Length - 1].GetComponent<Image>().color = new Color(0.811764705f, 0.615686274f, 0.36862745f, 0.4f);

            // selected button color
            if (selectedOption == maxButton)
            {
                // last button (exit) has different select color
                buttonsMedium[selectedOption].GetComponent<Image>().color = new Color(0.811764705f, 0.615686274f, 0.36862745f, 0.7f);
            }
            else
            {
                buttonsMedium[selectedOption].GetComponent<Image>().color = new Color(0.9686275f, 0.9372549f, 0.8941177f, 0.4f);
            }
        }
        else if (factUI.activeInHierarchy == false && lifetime <= 0)
        {
            gameOver = true;
            gameOverScreen.SetActive(true);

            if (isSoundPlayed == false)
            {
                audioSource.PlayOneShot(loseGame);
                isSoundPlayed = true;
            }

            for (int i = 0; i < buttonsLose.Length; i++)
            {
                buttonsLose[i].GetComponent<Image>().color = new Color(0.9686275f, 0.9372549f, 0.8941177f, 0.2f);
            }
            
            // last button has a different color
            buttonsLose[buttonsLose.Length - 1].GetComponent<Image>().color = new Color(0.811764705f, 0.615686274f, 0.36862745f, 0.4f);

            // selected button color
            if (selectedOption == maxButton)
            {
                // last button (exit) has different select color
                buttonsLose[selectedOption].GetComponent<Image>().color = new Color(0.811764705f, 0.615686274f, 0.36862745f, 0.7f);
            }
            else
            {
                buttonsLose[selectedOption].GetComponent<Image>().color = new Color(0.9686275f, 0.9372549f, 0.8941177f, 0.4f);
            }
        }
        else {
            timer += Time.deltaTime;
        }

        if(gameOver)
        {
            Time.timeScale = 0;
            backgroundMusic.Stop();
        }

        // Menu
        if (menuOpen == true)
        {
            // not selected buttons color
            for (int i = 0; i < buttonsPause.Length - 1; i++)
            {
                buttonsPause[i].GetComponent<Image>().color = new Color(0.9686275f, 0.9372549f, 0.8941177f, 0.2f);
            }
            // last button has a different color
            buttonsPause[buttonsPause.Length-1].GetComponent<Image>().color = new Color(0.8117647058f, 0.6156862745f, 0.36862745098f, 0.2f);

            // selected button color
            if (selectedOption == maxButton)
            {
                // last button (exit) has different select color
                buttonsPause[selectedOption].GetComponent<Image>().color = new Color(0.8117647058f, 0.6156862745f, 0.36862745098f, 0.4f);
            }
            else
            {
                buttonsPause[selectedOption].GetComponent<Image>().color = new Color(0.6078432f, 0.654902f, 0.3333333f, 0.4f);
            }
        }

    }

    public void OnMove(OSCMessage message)
    {
        // We get the values we receive from the phone and assign them to the movement
        if (menuOpen == false && !gameOver) {
            movementX = message.Values[0].FloatValue;
            movementY = message.Values[1].FloatValue;
        } else {
            if (menuOpen == true)
            { 
                maxButton = 3;
            }
            else { 
                maxButton = 2;
            }

            if (!helpOpen)
            { 

                if (message.Values[1].FloatValue < -0.3) 
                {
                    selectedOption += 1;
                    if (selectedOption > maxButton) {
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
            if (factUI.activeInHierarchy == true && !gameOver) {
                factUI.SetActive(false);
                timer = 0.0f;
                isPossibleQuestion = false;

                audioSource.PlayOneShot(buttonClose);

                numberOfQuestions += 1;
            }
        }

        // Menu (right top)
        if (x > 0 && x < 1 && y > 0 && y < 1)
		{
            if (menuOpen == false && !gameOver)
            {
				menuOpen = true;
				pauseMenu.SetActive(true);
                Time.timeScale = 0;
                audioSource.PlayOneShot(buttonSelected);
            }
	    }

        // Back (left top)
        if (x > 0 && x < 1 && y < 0 && y > -1)
		{
            if (menuOpen == true)
            {
                audioSource.PlayOneShot(buttonBack);
                if (helpOpen == true)
                {
                    helpOpen = false;
                    helpMenu.SetActive(false);
                }
                else {
                    menuOpen = false;
                    pauseMenu.SetActive(false);
                    Time.timeScale = 1;
                }
            }
        }

        // Confirm
        if (x < 0 && x > -1 && y < 0 && y > -1)
        {
            if (menuOpen == true)
            {
                audioSource.PlayOneShot(buttonSelected);
                switch (selectedOption)
                {
                    case 0:
                        // RESUME
                        menuOpen = false;
                        pauseMenu.SetActive(false);
                        Time.timeScale = 1;
                        break;
                    case 1:
                        // HELP
                        helpOpen = true;
                        helpMenu.SetActive(true);
                        break;
                    case 2:
                        // MAIN MENU
                        SceneManager.LoadScene("MainMenu");
                        break;
                    case 3:
                        // EXIT GAME
                        Application.Quit();
                        break;
                }
            }
            else if (gameOver)
            {
                audioSource.PlayOneShot(buttonSelected);
                switch (selectedOption)
                {
                    case 0:
                        // PLAY AGAIN
                        // Dirty fix for select sound not playing
                        SceneManager.LoadScene("MainMenu");
                        SceneManager.LoadScene("GameScreen");
                        Time.timeScale = 1;
                        break;
                    case 1:
                        // MAIN MENU
                        SceneManager.LoadScene("MainMenu");
                        break;
                    case 2:
                        // EXIT GAME
                        Application.Quit();
                        break;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // We detect if we collide with the box and we deactivate it
        if (other.gameObject.CompareTag("box"))
        {
            other.gameObject.SetActive(false);
        }

        // We detect if we collide with the good or bad element
        if (other.gameObject.CompareTag("good"))
        {
            lifetime += 5; // we add 5 point to the lifetime
            endQuestion();
            audioSource.PlayOneShot(correctAnswer);
            other.transform.parent.gameObject.SetActive(false);
            
            if (lifetime > 100)
            {
                lifetime = 100;
            }

            GoodTip();
        }
        else if (other.gameObject.CompareTag("bad"))
        {
            lifetime -= 150; // we take 15 point to the lifetime
            endQuestion();
            audioSource.PlayOneShot(wrongAnswer);
            other.transform.parent.gameObject.SetActive(false);

            if (lifetime < 0)
            {
                lifetime = 0;
            }

            BadTip();
        }
    }

    private void newQuestion()
    {
        // we choose a random index
        index = UnityEngine.Random.Range(0, questions.Count);

        // we choose the question with that index
        questionText.SetActive(true);
        questionText.GetComponent<Text>().text = questions[index].ToUpper();
        questions.RemoveAt(index); // we remove from the list so it doesn't appear again

        GameObject correctAnswer = new GameObject();
        GameObject wrongAnswer = new GameObject();
        // we choose the objects with that index
        float randomElement = UnityEngine.Random.Range(0, 2);
        if (randomElement > 0.5)
        {
            correctAnswer.transform.SetParent(box1.transform);
            correctAnswer.transform.localPosition = Vector3.zero;
            wrongAnswer.transform.SetParent(box2.transform);
            wrongAnswer.transform.localPosition = Vector3.zero;
            correctpoint = 0;
            wrongpoint = 1;
        }
        else
        {
            wrongAnswer.transform.SetParent(box1.transform);
            wrongAnswer.transform.localPosition = Vector3.zero;
            correctAnswer.transform.SetParent(box2.transform);
            correctAnswer.transform.localPosition = Vector3.zero;
            correctpoint = 1;
            wrongpoint = 0;
        }
        correctAnswer.AddComponent<Rigidbody>();
        correctAnswer.AddComponent<BoxCollider>();
        correctAnswer.GetComponent<Collider>().isTrigger = true;
        correctAnswer.GetComponent<Rigidbody>().useGravity = false;
        correctAnswer.name = "Correct Answer";
        correctAnswer.tag = "good";
        wrongAnswer.AddComponent<Rigidbody>();
        wrongAnswer.AddComponent<BoxCollider>();
        wrongAnswer.GetComponent<Collider>().isTrigger = true;
        wrongAnswer.GetComponent<Rigidbody>().useGravity = false;
        wrongAnswer.name = "Wrong Answer";
        wrongAnswer.tag = "bad";

        GameObject badObject = Instantiate<GameObject>(badObjects[index], new Vector3(0, 0, 0), Quaternion.identity);
        badObject.transform.SetParent(wrongAnswer.transform);
        badObject.transform.localPosition = spawnPoint[wrongpoint].transform.localPosition;
        
        GameObject goodObject = Instantiate<GameObject>(goodObjects[index], new Vector3(0, 0, 0), Quaternion.identity);
        goodObject.transform.SetParent(correctAnswer.transform);
        goodObject.transform.localPosition = spawnPoint[correctpoint].transform.localPosition;

        removeArrayValue(ref badObjects, index);
        removeArrayValue(ref goodObjects, index);

        box1.transform.position = new Vector3(-5, 0.7f, 47.7f);
        box2.transform.position = new Vector3(5, 0.7f, 47.7f);
        positionBadBox = new Vector3(-5, 0.7f, 0f);
        positionGoodBox = new Vector3(5, 0.7f, 0f);
        box1.SetActive(true);
        box2.SetActive(true);
    }

    private void endQuestion()
    {
        questionText.SetActive(false);
        timer = 0.0f;

        positionBadBox = new Vector3(-5, 0.7f, -7f);
        positionGoodBox = new Vector3(5, 0.7f, -7f);

        Destroy(box1.GetComponent<Transform>().GetChild(3).gameObject);
        Destroy(box2.GetComponent<Transform>().GetChild(3).gameObject);
    }

    public static void removeArrayValue<T>(ref T[] arr, int index)
    {
        for (int a = index; a < arr.Length - 1; a++)
        {
            arr[a] = arr[a + 1];
        }
        Array.Resize(ref arr, arr.Length - 1);
    }

    private void BadTip()
    {
        factUI.SetActive(true);
        factUI.gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color(0.607f, 0.37f, 0.25f);
        factTitle.GetComponent<Text>().text = "NOT THE BEST CHOICE...";
        factText.GetComponent<Text>().text = badTip[index];
        badTip.RemoveAt(index);
        goodTip.RemoveAt(index);

        if (numberOfQuestions == 0) {
            quickTip.SetActive(true);
        }
        else {
            quickTip.SetActive(false);
        }
    }

    private void GoodTip()
    {
        factUI.SetActive(true);
        factUI.gameObject.transform.GetChild(0).GetComponent<Image>().color = new Color(0.607f, 0.65f, 0.33f);
        factTitle.GetComponent<Text>().text = "GOOD CHOICE!";
        factText.GetComponent<Text>().text = goodTip[index];
        badTip.RemoveAt(index);
        goodTip.RemoveAt(index);

        if (numberOfQuestions == 0)
        {
            quickTip.SetActive(true);
        }
        else
        {
            quickTip.SetActive(false);
        }
    }
}
