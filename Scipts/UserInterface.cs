using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;



public class UserInterface : MonoBehaviour
{
    public GameObject LoginPanel;
    public GameObject InfoPanel;
    public GameObject GuidePanel;
    public GameObject StartPanel;

    //Login panel elements
    public TMP_InputField inputUserName;
    public TMP_Dropdown position;
    public Button loginNextButton;
    public Button loginBackButton;
    public TextMeshProUGUI invalidName;
    public TMP_Dropdown fixedSpeed;

    //Info panel
    public Button startButton;
    public Button backButton;
    public TMP_InputField cubeBeltSpeed;
    public TMP_InputField upperArmSpeed;
    public TMP_InputField middleArmSpeed;
    public TMP_InputField numberOfCubes;
    public TMP_InputField basketBeltSpeed;
    public TextMeshProUGUI invalidCubeBeltSpeed;
    public TextMeshProUGUI invalidUpperArmSpeed;
    public TextMeshProUGUI invalidMiddleArmSpeed;
    public TextMeshProUGUI invalidNumberOfCubes;
    public TextMeshProUGUI invalidBasketBeltSpeed;

    //Guide Panel
    public Button guideNextButton;
    public Button guideBackButton;
    public Button guideSkipButton;
    public Button guideStartButton;
    public Image robotArmBubble;
    public Image conveyorBeltBubble;
    public Image basketBubble;
    public Image cubesBubble;
    public Image towerlightBubble;

    private int currentGuideStep = 0;
    private Image[] guideBubbles;

    //StartPanel;
    public Button startPageButton;


    private ConfigureVariables configVar;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "UserInterface")
        {
            InitializeUI();
        }
    }

    public void InitializeUI()
    {
        Debug.Log("The scence has started");

        configVar = ConfigureVariables.Instance;

        guideBubbles = new Image[]
        {
            cubesBubble,
            basketBubble,
            conveyorBeltBubble,
            robotArmBubble,
            towerlightBubble
        };

        //info page
        backButton.onClick.AddListener(showLoginPanel);
        startButton.onClick.AddListener(onInfoPage);

        //guide page
        guideNextButton.onClick.AddListener(OnGuideNextButtonClick);
        guideBackButton.onClick.AddListener(OnGuideBackButtonClick);
        guideSkipButton.onClick.AddListener(showLoginPanel);
        guideStartButton.onClick.AddListener(showLoginPanel);

        //login page
        fixedSpeed.onValueChanged.AddListener(OnFixedSpeedDropdownChanged);
        loginNextButton.onClick.AddListener(onLoginPage);
        loginBackButton.onClick.AddListener(showGuidePanel);


        //start page
        startPageButton.onClick.AddListener(showGuidePanel);

        ResetAllFields();
        showStartPanel();


    }

    public void showStartPanel()
    {
        StartPanel.SetActive(true);
        GuidePanel.SetActive(false);
        LoginPanel.SetActive(false);
        InfoPanel.SetActive(false);
    }

    public void OnFixedSpeedDropdownChanged(int index)
    {
        switch (index)
        {
            case 0: // Fixed robotic arm speed
                cubeBeltSpeed.text = "2";
                basketBeltSpeed.text = "2";
                upperArmSpeed.text = "";
                middleArmSpeed.text = "";

                cubeBeltSpeed.interactable = false;
                basketBeltSpeed.interactable = false;
                upperArmSpeed.interactable = true;
                middleArmSpeed.interactable = true;
                break;

            case 1: // Fixed conveyor belt speed
                cubeBeltSpeed.text = "";
                basketBeltSpeed.text = "";
                upperArmSpeed.text = "115";
                middleArmSpeed.text = "105";
            
                cubeBeltSpeed.interactable = true;
                basketBeltSpeed.interactable = true;
                upperArmSpeed.interactable = false;
                middleArmSpeed.interactable = false;
                break;

            case 2: // Custom (all speeds)
                cubeBeltSpeed.text = "";
                basketBeltSpeed.text = "";
                upperArmSpeed.text = "";
                middleArmSpeed.text = "";

                cubeBeltSpeed.interactable = true;
                basketBeltSpeed.interactable = true;
                upperArmSpeed.interactable = true;
                middleArmSpeed.interactable = true;
                break;
        }
    }

    private void ResetAllFields()
    {
        // Reset input fields
        inputUserName.text = "";
        cubeBeltSpeed.text = "";
        upperArmSpeed.text = "";
        middleArmSpeed.text = "";
        numberOfCubes.text = "";
        basketBeltSpeed.text = "";

        // Reset dropdown
        position.value = 0;

        // Hide all error messages
        invalidName.gameObject.SetActive(false);
        invalidCubeBeltSpeed.gameObject.SetActive(false);
        invalidUpperArmSpeed.gameObject.SetActive(false);
        invalidMiddleArmSpeed.gameObject.SetActive(false);
        invalidNumberOfCubes.gameObject.SetActive(false);
        invalidBasketBeltSpeed.gameObject.SetActive(false);

        fixedSpeed.value = 0;
        OnFixedSpeedDropdownChanged(0);

        // Reset panels
        LoginPanel.SetActive(false);
        InfoPanel.SetActive(false);

        //Reset guide bubbles visibility
        ResetGuideBubbles();
        currentGuideStep = 0;
    }

    public void showGuidePanel()
    {
        StartPanel.SetActive(false);

        GuidePanel.SetActive(true);
        LoginPanel.SetActive(false);
        InfoPanel.SetActive(false);

        currentGuideStep = 0;
        ResetGuideBubbles();  // Reset all guide bubbles

        ShowCurrentGuideBubble();
        UpdateGuideButtonsState();
    }

    private void ResetGuideBubbles()
    {
        for (int i = 0; i < guideBubbles.Length; i++)
        {
            guideBubbles[i].gameObject.SetActive(false);
            Debug.Log($"Reset bubble {i}: {guideBubbles[i].name}");
        }
        currentGuideStep = 0;

    }


    public void showLoginPanel()
    {

        StartPanel.SetActive(false);
        LoginPanel.SetActive(true);
        InfoPanel.SetActive(false);
        GuidePanel.SetActive(false);
    }

    public void showInfoPanel()
    {
        StartPanel.SetActive(false);
        LoginPanel.SetActive(false);
        InfoPanel.SetActive(true);
        GuidePanel.SetActive(false);
        OnFixedSpeedDropdownChanged(fixedSpeed.value);


    }

    private void ShowCurrentGuideBubble()
    {
        for (int i = 0; i < guideBubbles.Length; i++)
        {
            bool shouldBeActive = (i == currentGuideStep);
            guideBubbles[i].gameObject.SetActive(shouldBeActive);
            Debug.Log($"Bubble {i} active: {shouldBeActive}");
        }
    }


    public void OnGuideNextButtonClick()
    {
        if (currentGuideStep < guideBubbles.Length - 1)
        {
            currentGuideStep++;
            ShowCurrentGuideBubble();
            UpdateGuideButtonsState();
        }
        else
        {
            Debug.Log("Reached end of guide bubbles");
        }
    }

    public void OnGuideBackButtonClick()
    {
        if (currentGuideStep > 0)
        {
            currentGuideStep--;
            ShowCurrentGuideBubble();
            UpdateGuideButtonsState();
        }

    }

    private void UpdateGuideButtonsState()
    {
        guideBackButton.interactable = (currentGuideStep > 0);
        guideNextButton.interactable = (currentGuideStep < guideBubbles.Length - 1);
        guideStartButton.gameObject.SetActive(currentGuideStep == guideBubbles.Length - 1);
    }

    public void onLoginPage()
    {
        string userName = inputUserName.text;
        if (string.IsNullOrEmpty(userName))
        {
            invalidName.gameObject.SetActive(true);

        }
        else
        {
            invalidName.gameObject.SetActive(false);
            showInfoPanel();

        }
        OnFixedSpeedDropdownChanged(fixedSpeed.value);

    }

    public void onInfoPage()
    {
        bool isValid = true;

        //Validate conveyor speed
        if (!ValidateFloatInput(cubeBeltSpeed.text, 1f, 4f))
        {
            invalidCubeBeltSpeed.gameObject.SetActive(true);
            isValid = false;
        }
        else
        {
            invalidCubeBeltSpeed.gameObject.SetActive(false);
        }
        //Validate upper arm speed
        if(!ValidateFloatInput(upperArmSpeed.text, 85f, 120f))
        {
            invalidUpperArmSpeed.gameObject.SetActive(true);
            isValid = false;

        }
        else
        {
            invalidUpperArmSpeed.gameObject.SetActive(false);
        }
        // Validate middle arm speed
        if (!ValidateFloatInput(middleArmSpeed.text, 85f, 120f))
        {
            invalidMiddleArmSpeed.gameObject.SetActive(true);
            isValid = false;
        }
        else
        {
            invalidMiddleArmSpeed.gameObject.SetActive(false);
        }

        // Validate number of cubes
        if (!ValidateIntInput(numberOfCubes.text, 1, 25))
        {
            invalidNumberOfCubes.gameObject.SetActive(true);
            isValid = false;
        }
        else
        {
            invalidNumberOfCubes.gameObject.SetActive(false);
        }

        // Validate speed of basket conveyor belt
        if (!ValidateFloatInput(basketBeltSpeed.text, 1f, 4f))
        {
            invalidBasketBeltSpeed.gameObject.SetActive(true);
            isValid = false;
        }
        else
        {
            invalidBasketBeltSpeed.gameObject.SetActive(false);
        }

        //if all valide load scene mode 
        if (isValid)
        {
            InfoPanel.SetActive(false);
            UpdateConfigurationManager();
            LoadSceneMode("SortingLine");
        }
    }

    public void LoadSceneMode(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }


    private bool ValidateFloatInput(string input, float min, float max)
    {
        if (float.TryParse(input, out float value))
        {
            return value >= min && value <= max;
        }
        return false;
    }

    private bool ValidateIntInput(string input, int min, int max)
    {
        if (int.TryParse(input, out int value))
        {
            return value >= min && value <= max;
        }
        return false;
    }

    private void UpdateConfigurationManager()
    {
        configVar.NumOfCubes = int.Parse(numberOfCubes.text);

        switch (fixedSpeed.value)
        {
            case 0: // Fixed robotic arm speed
                configVar.UpperArmSpeed = float.Parse(upperArmSpeed.text);
                configVar.MiddleArmSpeed = float.Parse(middleArmSpeed.text);
                configVar.CubeBeltSpeed = 2f;
                configVar.BasketBeltSpeed = 2f;
                break;

            case 1: // Fixed conveyor belt speed
                configVar.UpperArmSpeed = 115f;
                configVar.MiddleArmSpeed = 105f;
                configVar.CubeBeltSpeed = float.Parse(cubeBeltSpeed.text);
                configVar.BasketBeltSpeed = float.Parse(basketBeltSpeed.text);
                break;

            case 2: // Custom (all speeds)
                configVar.UpperArmSpeed = float.Parse(upperArmSpeed.text);
                configVar.MiddleArmSpeed = float.Parse(middleArmSpeed.text);
                configVar.CubeBeltSpeed = float.Parse(cubeBeltSpeed.text);
                configVar.BasketBeltSpeed = float.Parse(basketBeltSpeed.text);
                break;
        }
    }

}
