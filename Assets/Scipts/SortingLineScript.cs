using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SortingLineScript : MonoBehaviour
{
    public Vector3 Direction;  //Direction of the conveyer belt should move 
    public List<GameObject> onBelt;  //List of object on conveyor belt

    public GameObject Basket1Prefab;  
    public GameObject Basket2Prefab;
    public GameObject Cube1;
    public GameObject Cube2;
    private GameObject selectedCube;
    public GameObject groundPlaneStage; 
    public Vector3 spawnPosition;  //The position the cube is created
 
    public int numOfCubes;   //Set the number of cubes
    public float middleArmSpeed;  //Speed of middle arm 
    public float upperArmSpeed;   //Speed of upper arm
    public float basketBeltSpeed;//Speed of basket conveyer belt
    public float cubeBeltSpeed;  //Speed of cube conveyer belt

    public int basket1Total;   //Total number of cube in basket 1
    public int basket2Total;//Total number of cube in basket 2

    //Axis position of the robotic arm 
    public Transform r1MiddleArm;   
    public Transform r1UpperArm;
    public Transform r2MiddleArm;
    public Transform r2UpperArm;

    //The box collider of the detection zone in the pushpart
    public BoxCollider robot1DetectionZone;
    public BoxCollider robot2DetectionZone;

    //Set the target angle for the robotic arms
    private float middleTargetAngle = 45f;
    private float upperTargetAngle = 60f;

    //The orgin axis of the robotic arm 
    private float middleOriginZAxis;
    private float upperOriginZAxis;

    //Boolean to decide whether the arm should move 
    private bool r1Move = false;
    private bool r2Move = false;
    
    //Results Panel
    public BasketCollision basket1Collision;
    public BasketCollision basket2Collision;
    public TextMeshProUGUI finalResults;
    public GameObject resultsPanel;
    public Button restartButton;
    public Button settingsHistory;
    public Button resultsBackButton;
    public Button feedbackButton;
    private ConfigureVariables configVar;

    //Simulation Panel
    public Button stopButton;
    public Button continueButton;
    public Button restartSimulationButton;
    public TextMeshProUGUI Timer;
    public TextMeshProUGUI ProgressText;
    public Image progressBar;
    float progress, maxProgress;

    //Confirmation Panel for Results
    public GameObject resultsConfirmPanel;
    public Button resultsConfirmRestartButton;
    public Button resultsConfirmBackButton;

    //Confirmation Panel for simulation
    public GameObject simulationConfirmPanel;
    public Button simulationConfirmRestartButton;
    public Button simualtionConfirmBackButton;

    //Feedback Panel
    public GameObject feedbackPanel;
    public Button feedbackBackButton;
    public Button feedbackRestartButton;
    public Button moreInfoButton;
    public TextMeshProUGUI feedbackText;

    //MoreInfoPanel
    public GameObject infoPanel;
    public Button infoBackButton;
    public Button infoCloseButton;
    public Button infoNextButton;
    public TextMeshProUGUI infoText;
 

    private string[] infoTexts = new string[]
   {
        "1. System Components:\n\n- Conveyor Belt: Moves items through the system\n- Robotic Arms: Sort items into correct baskets\n- Baskets: Collect sorted items\n-Tower Light: Indicates the status of the sorting line",
        "2. Speed Relationships:\n\n- Conveyor Belt Speed: Affects overall throughput\n- Arm Speeds: Impact sorting accuracy\n- Balance is crucial: Too fast reduces accuracy, too slow decreases efficiency",
        "3. Optimization Challenges:\n\n- Speed vs. Accuracy: Faster speeds may increase errors\n- Product Variety: Different items may require different speeds\n- System Harmony: All speeds must work well together",
        "4. Real-world Impact:\n\n- Efficiency: Proper settings reduce waste and increase output\n- Cost Savings: Optimized systems save time and resources\n- Quality Control: Accurate sorting ensures product quality",
        "5. Performance Metrics:\n\n- Error Rate: Percentage of incorrectly sorted items\n- Accuracy: Percentage of correctly sorted items\n- These metrics guide system improvements",
        "6. Relationship Between Arm Speeds:\n\n- Upper Arm: Needs higher speed for larger movements\n- Middle Arm: Moves slower for fine adjustments\n- Balanced speeds ensure efficiency and accuracy"

   };

   
    public BasketConveyorManager basketConveyorManager;

    //Boolean conditions to check whether simulation is running
    private bool isSimulationRunning = true;

    private Coroutine cubeSpawnerCoroutine;
    private int spawnedCubesCount = 0;  //Start spawned cubes count 
    private float desiredDistance = 4f;  //Distance between spawned cubes

    private int currentInfoIndex = 0;
    private float timerValue = 0.0f;  //Timer initial valeu


    void Start()
    {

        //set all panel as false
        simulationConfirmPanel.SetActive(false);
        resultsConfirmPanel.SetActive(false);
        resultsPanel.SetActive(false);
        feedbackPanel.SetActive(false);
        infoPanel.SetActive(false);


        //results panel
        restartButton.onClick.AddListener(loadResultsConfirmPanel);
        settingsHistory.onClick.AddListener(LoadSettings);
        resultsBackButton.onClick.AddListener(finalResultUI);
        feedbackButton.onClick.AddListener(loadFeedbackPanel);

        //simualtion panels
        stopButton.onClick.AddListener(StopSimulation);
        continueButton.onClick.AddListener(ContinueSimulation);
        restartSimulationButton.onClick.AddListener(loadSimualtionRestartConfirmation);

        //simulation restart confirmation panel 
        simualtionConfirmBackButton.onClick.AddListener(ContinueSimulation);
        simulationConfirmRestartButton.onClick.AddListener(LoadSceneMode);

        //results restart confimration panel;
        resultsConfirmRestartButton.onClick.AddListener(LoadSceneMode);
        resultsConfirmBackButton.onClick.AddListener(finalResultUI);

        //feedback panel
        feedbackBackButton.onClick.AddListener(finalResultUI);
        feedbackRestartButton.onClick.AddListener(loadResultsConfirmPanel);
        moreInfoButton.onClick.AddListener(loadMoreInfoPanel);

        //moreInfo Panel
        infoBackButton.onClick.AddListener(ShowPreviousInfo);
        infoNextButton.onClick.AddListener(ShowNextInfo);
        infoCloseButton.onClick.AddListener(finalResultUI);

        //set contimue button to false because the simulation is not stopped yet
        continueButton.interactable = false; 
         

        configVar = ConfigureVariables.Instance; 

        // retrieve the configuration
        cubeBeltSpeed = configVar.CubeBeltSpeed;
        middleArmSpeed = configVar.MiddleArmSpeed;
        upperArmSpeed = configVar.UpperArmSpeed;
        numOfCubes = configVar.NumOfCubes;
        basketBeltSpeed = configVar.BasketBeltSpeed;


        //get the original axis so that it can retract arm back to origin position
        middleOriginZAxis = NormalizeAngle(r1MiddleArm.localEulerAngles.z);
        upperOriginZAxis = NormalizeAngle(r1UpperArm.localEulerAngles.z);

        //start adding cube onto belt
        cubeSpawnerCoroutine = StartCoroutine(SpawnCubes());

        //for the cube progress panel
        progress = 0;
        maxProgress = numOfCubes;

        //update the bubble text
        UpdateInfoText();


    }

    // Update is called once per frame
    void Update()
    {

        if (isSimulationRunning)
        {
            timerValue += Time.deltaTime;
            Timer.text = "Timer: " + timerValue.ToString("F2") + " s";

            MoveBelt();
            CheckDetectionZones();
            RobotArmDecision();
        }

        //To show the progress of the cube 
        ProgressText.text = "Cube Progress: " + spawnedCubesCount.ToString() + " / " + numOfCubes.ToString();
        progress = spawnedCubesCount;
        ProgressFiller();

    }

    //Show the simulation restart panel
    public void loadSimualtionRestartConfirmation()
    {
        simulationConfirmPanel.SetActive(true);
    }

    //Show the results restart confirmation panel
    public void loadResultsConfirmPanel()
    {
        resultsConfirmPanel.SetActive(true);
        resultsPanel.SetActive(false);
    }

    //Show the feedback panel
    public void loadFeedbackPanel()
    {
        simulationConfirmPanel.SetActive(false);
        resultsConfirmPanel.SetActive(false);
        resultsPanel.SetActive(false);
        feedbackPanel.SetActive(true);

        ProvideFeedback();
    }

    //Show the more infomation panel 
    public void loadMoreInfoPanel()
    {
        infoPanel.SetActive(true);
        resultsConfirmPanel.SetActive(false);
        resultsPanel.SetActive(false);
        feedbackPanel.SetActive(false);
        currentInfoIndex = 0;
        UpdateInfoText();
    }

    //Setting the index of the information bubble
    private void UpdateInfoText()
    {
        infoText.text = infoTexts[currentInfoIndex];
        infoBackButton.interactable = (currentInfoIndex > 0);
        infoNextButton.interactable = (currentInfoIndex < infoTexts.Length - 1);
    }

    //Method to show the previous explanation bubble
    private void ShowPreviousInfo()
    {
        if (currentInfoIndex > 0)
        {
            currentInfoIndex--;
            UpdateInfoText();
        }
    }

    //Method to show the next explanation bubble
    private void ShowNextInfo()
    {
        if (currentInfoIndex < infoTexts.Length - 1)
        {
            currentInfoIndex++;
            UpdateInfoText();
        }
    }

    //Method to fill the progress bar 
    void ProgressFiller()
    {
        if (maxProgress > 0)
        {
            progressBar.fillAmount = progress / maxProgress;

        }
        else
        {
            progressBar.fillAmount = 0;
        }
    }

    //When stop button is clicked it will stop 
    public void StopSimulation()
    {
        isSimulationRunning = false;
        if (cubeSpawnerCoroutine != null)
        {
            StopCoroutine(cubeSpawnerCoroutine);
        }

        if (selectedCube != null)
        {
            Rigidbody rb = selectedCube.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
            }
        }

        continueButton.interactable = true;
        stopButton.interactable = false;
    }

    //When continue button is clicked it will stop 
    public void ContinueSimulation()
    {
        simulationConfirmPanel.SetActive(false);

        isSimulationRunning = true;
        cubeSpawnerCoroutine = StartCoroutine(SpawnCubes());
        MoveBelt();

        continueButton.interactable = false;
        stopButton.interactable = true;

    }

    //Check whether the cube has been fully sorted
    private IEnumerator CheckSimulationStatus()
    {
        if (onBelt.Count == 0 && spawnedCubesCount==numOfCubes)
        {
            yield return new WaitForSeconds(3f);
            Debug.Log("Results....");
            isSimulationRunning = false;

            Timer.text  = "Timer: " + timerValue.ToString("F2");
            finalResultUI();
        }
      
        else { 
             resultsConfirmPanel.gameObject.SetActive(false);
        }
    }

    //Method to show the final result 
    void finalResultUI()
    {
        Debug.Log("Loading resultts");

        infoPanel.SetActive(false);
        resultsConfirmPanel.SetActive(false);
        feedbackPanel.SetActive(false);

        resultsPanel.SetActive(true);
        settingsHistory.gameObject.SetActive(true);
        resultsBackButton.gameObject.SetActive(false);

        int sortedCubes = basketConveyorManager.GetTotalCubeCount();
        int totalErrors = numOfCubes - sortedCubes;
        float errorRate = ((float)totalErrors / numOfCubes) * 100;
        float accuracy = ((float)sortedCubes / numOfCubes) * 100;

        //Setting up the timer value 
        int minutes = Mathf.FloorToInt(timerValue / 60F);
        int seconds = Mathf.FloorToInt(timerValue % 60F);
        string formattedTime = string.Format("{0:0} min {1:00} s", minutes, seconds);

        //Set the final results output 
        finalResults.text = string.Format(
            "Sorted Cubes: {0}\n" +
            "Total Cubes: {1}\n" +
            "Errors: {2}\n" +
            "Error Rate: {3:F1} %\n" +  
            "Accuracy: {4:F1} %\n" +
           "Time to Complete: {5:F2} s ({6})",  // Include the formatted time
            sortedCubes, numOfCubes, totalErrors, errorRate, accuracy, timerValue, formattedTime);
    }

    //Load the settings in the result panel
    public void LoadSettings()
    {
        Debug.Log("Loading settingss");
        resultsBackButton.gameObject.SetActive(true);
        resultsConfirmPanel.gameObject.SetActive(false);
        settingsHistory.gameObject.SetActive(false);

        finalResults.text = string.Format(
              "Cube Conveyor Belt Speed (m/s): {0}\n" +
              "Basket Conveyor Belt Speed (m/s): {1}\n" +
              "Robot Upper Arm Speed (m/s): {2}\n" +
              "Robot Middle Arm Speed (m/s): {3}\n" +
              "Number of Cubes: {4}\n",
              cubeBeltSpeed, basketBeltSpeed, upperArmSpeed, middleArmSpeed, numOfCubes);

    }


    //Load the user interface scene
    public void LoadSceneMode()
    {
        SceneManager.LoadScene("UserInterface");
    }


    //Spawn the cube onto the belt
    private IEnumerator SpawnCubes()
    {
        while (spawnedCubesCount < numOfCubes)
        {
            while (!isSimulationRunning)
            {
                yield return null; // Wait until the simulation is running
            }

            float delay = desiredDistance / cubeBeltSpeed;
            yield return new WaitForSeconds(delay); // Wait for a few seconds before spawning the cube

            selectedCube = Random.value > 0.5f ? Cube1 : Cube2;  // Randomly select the cube


            Vector3 localSpawnPosition = new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z);
            Vector3 worldSpawnPosition = groundPlaneStage.transform.TransformPoint(localSpawnPosition);

            GameObject spawnedCube = Instantiate(selectedCube, worldSpawnPosition, groundPlaneStage.transform.rotation);
            spawnedCube.transform.SetParent(groundPlaneStage.transform, true);

            SetupCubePhysics(spawnedCube);

            spawnedCubesCount++; // Increment the count of spawned cubes
        }
        
    }

    //Set up the physics of the cube
    private void SetupCubePhysics(GameObject cube)
    {
        Rigidbody rb = cube.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = cube.AddComponent<Rigidbody>();
        }
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Set the gravity direction to be perpendicular to the ground plane
        Physics.gravity = -groundPlaneStage.transform.up * 9.81f;
    }

    //Move the cube on the belt
    void MoveBelt()
    {
        for (int i = 0; i < onBelt.Count; i++)
        {
            onBelt[i].transform.Translate(cubeBeltSpeed * Direction * Time.fixedDeltaTime, Space.Self);

            if (onBelt[i].transform.parent != groundPlaneStage.transform)
            {
                onBelt[i].transform.SetParent(groundPlaneStage.transform, true);
            }
        }
    }

    //Change the angle to range -180 to 180 easier to work with 
    private float NormalizeAngle(float angle)
    {
        angle = angle % 360;
        if (angle > 180)
        {
            angle -= 360;
        }
        return angle;
    }


    //When the collision between cube and belt is detected
    //box collider added into the belt that detects the collision
    private void OnCollisionEnter(Collision collision)
    {
        onBelt.Add(collision.gameObject); //add the cube onto the belt
    }

    // When something leaves the belt, remove the cube from the belt
    private void OnCollisionExit(Collision collision)
    {
        onBelt.Remove(collision.gameObject); //remove the cube on the belt
        StartCoroutine(CheckSimulationStatus()); //check whether simulation is complete
    }


    //Method to check whether the cube is near the arm
    void CheckDetectionZones()
    {
        Collider[] robot1Colliders = Physics.OverlapBox(robot1DetectionZone.bounds.center, robot1DetectionZone.bounds.extents, robot1DetectionZone.transform.rotation);
        Collider[] robot2Colliders = Physics.OverlapBox(robot2DetectionZone.bounds.center, robot2DetectionZone.bounds.extents, robot2DetectionZone.transform.rotation);

        r1Move = false;
        r2Move = false;

        foreach (Collider collider in robot1Colliders)
        {
            if (collider.gameObject.name.StartsWith("Cube1"))
            {
                // Check if the cube is near the center of the detection zone
                if (IsNearCenter(collider.transform.position, robot1DetectionZone.bounds.center))
                {
                    r1Move = true;
                    break;
                }
            }
        }

        foreach (Collider collider in robot2Colliders)
        {
            if (collider.gameObject.name.StartsWith("Cube2"))
            {
                // Check if the cube is near the center of the detection zone
                if (IsNearCenter(collider.transform.position, robot2DetectionZone.bounds.center))
                {
                    r2Move = true;
                    break;
                }
            }
        }
    }

    bool IsNearCenter(Vector3 cubePosition, Vector3 zoneCenter)
    {
        float threshold = 0.8f; 
        return Vector3.Distance(cubePosition, zoneCenter) < threshold;
    }

    private bool isR1Retracting = false;
    private bool isR2Retracting = false;
    //Method to decide whether the arm should move
    void RobotArmDecision()
    {
        if (r1Move && !isR1Retracting)
        {
            MoveRobotArms(r1UpperArm, r1MiddleArm);
            if (isArmExtended)
            {
                isR1Retracting = true;
            }
        }
        else
        {
            RetractRobotArm(r1UpperArm, r1MiddleArm);
            if (!isArmExtended)
            {
                isR1Retracting = false;
            }
        }

        // Similar for r2Move
        if (r2Move && !isR2Retracting)
        {
            MoveRobotArms(r2UpperArm, r2MiddleArm);
            if (isArmExtended)
            {
                isR2Retracting = true;
            }
        }
        else
        {
            RetractRobotArm(r2UpperArm, r2MiddleArm);
            if (!isArmExtended)
            {
                isR2Retracting = false;
            }
        }
    }


    bool isArmExtended = false;

    void MoveRobotArms(Transform upperArm, Transform middleArm)
    {
        float middleCurrentZRotation = NormalizeAngle(middleArm.localEulerAngles.z);
        float upperCurrentZRotation = NormalizeAngle(upperArm.localEulerAngles.z);

        bool middleArmComplete = false;
        bool upperArmComplete = false;

        if (middleCurrentZRotation > middleTargetAngle)
        {
            middleArm.localRotation *= Quaternion.Euler(0, 0, -middleArmSpeed * Time.deltaTime);
        }
        else
        {
            middleArm.localEulerAngles = new Vector3(middleArm.localEulerAngles.x, middleArm.localEulerAngles.y, middleTargetAngle);
            middleArmComplete = true;
        }

        if (upperCurrentZRotation < upperTargetAngle)
        {
            upperArm.localRotation *= Quaternion.Euler(0, 0, upperArmSpeed * Time.deltaTime);
        }
        else
        {
            upperArm.localEulerAngles = new Vector3(upperArm.localEulerAngles.x, upperArm.localEulerAngles.y, upperTargetAngle);
            upperArmComplete = true;
        }

        if (middleArmComplete && upperArmComplete)
        {
            isArmExtended = true;
        }
    }

 

    //Method to retract the robtic arm 
    void RetractRobotArm(Transform upperArm, Transform middleArm)
    {
        float middleCurrentZRotation = NormalizeAngle(middleArm.localEulerAngles.z);
        float upperCurrentZRotation = NormalizeAngle(upperArm.localEulerAngles.z);


        if (middleCurrentZRotation < middleOriginZAxis)
        {
            middleArm.localRotation *= Quaternion.Euler(0, 0, middleArmSpeed * Time.deltaTime);
        }
        else
        {
            middleArm.localEulerAngles = new Vector3(middleArm.localEulerAngles.x, middleArm.localEulerAngles.y, middleOriginZAxis);
        }

        if (upperCurrentZRotation > upperOriginZAxis)
        {
            upperArm.localRotation *= Quaternion.Euler(0, 0, -upperArmSpeed * Time.deltaTime);
        }
        else
        {
            upperArm.localEulerAngles = new Vector3(upperArm.localEulerAngles.x, upperArm.localEulerAngles.y, upperOriginZAxis);
        }
        if (middleCurrentZRotation == middleOriginZAxis && upperCurrentZRotation == upperOriginZAxis)
        {
            isArmExtended = false;
        }
    }

    //Method that provides simulation feedback 
    public void ProvideFeedback()
    {
        int sortedCubes = basketConveyorManager.GetTotalCubeCount();
        float accuracy = ((float)sortedCubes / numOfCubes) * 100;

        string feedback = "";

        // Analyze arm speeds
        feedback += AnalyzeArmSpeeds();

        // Analyze conveyor belt speeds
        feedback += AnalyzeConveyorSpeeds();

        // Overall performance feedback
        feedback += AnalyzeOverallPerformance(accuracy);

        // Set the feedback text
        feedbackText.text = feedback;
    }

    //Method that provides overall performance feedback 
    private string AnalyzeOverallPerformance(float accuracy)
    {
        string feedback = "<color=#00FF00>Overall Performance:</color>\n";

        if (numOfCubes <= 5)
        {
            // For very small numbers of cubes, we expect perfect or near-perfect sorting
            if (accuracy == 100)
                feedback += "Perfect sorting achieved! Excellent job with this small batch.";
            else if (accuracy >= 80)
                feedback += "Good performance, but with small batches, we aim for perfect sorting.";
            else
                feedback += "There's significant room for improvement, especially with small batches.";
        }
        else if (numOfCubes <= 10)
        {
            if (accuracy >= 90)
                feedback += "Excellent performance! High accuracy achieved with this batch.";
            else if (accuracy >= 80)
                feedback += "Good performance, but there's room for improvement.";
            else
                feedback += "Your current settings need adjustment to improve accuracy.";
        }
        else
        {
            // For larger batches, we can use the original thresholds
            if (accuracy >= 95)
                feedback += "Excellent performance! Your settings achieved high accuracy with a large batch.";
            else if (accuracy >= 85)
                feedback += "Good performance, but there's room for improvement, especially with larger batches.";
            else
                feedback += "Your current settings need adjustment to improve accuracy for large batches.";
        }

        return feedback;
    }

    //Method that provides arm speed feedback 
    private string AnalyzeArmSpeeds()
    {
        string feedback = "<color=#00FF00>Arm Speed Analysis:</color>\n";

        if (middleArmSpeed >= 100 && middleArmSpeed <= 110 && upperArmSpeed >= 110 && upperArmSpeed <= 120)
        {
            feedback += "Both arm speeds are within the optimal range. Great job!\n";
        }
        else
        {
            feedback += "Arm speeds could be optimized:\n";
            if (middleArmSpeed < 100)
                feedback += "- Middle arm speed is lower than optimal. Consider increasing it to <color=#FF4500>100-110°</color>./s.\n";
            else if (middleArmSpeed > 110)
                feedback += "- Middle arm speed is higher than optimal. The best range is <color=#FF4500>100-110°/s</color>.\n";

            if (upperArmSpeed < 110)
                feedback += "- Upper arm speed is lower than optimal. Consider increasing it to <color=#FF4500>110-120°/s</color>..\n";
            else if (upperArmSpeed > 120)
                feedback += "- Upper arm speed is at maximum. The optimal range is <color=#FF4500>110-120°/s</color>.\n";
        }

        if (upperArmSpeed <= middleArmSpeed)
        {
            feedback += "- For best results, the upper arm speed should be higher than the middle arm speed.\n";
        }

        return feedback;
    }

    private string AnalyzeConveyorSpeeds()
    {
        string feedback = "<color=#00FF00>Conveyor Belt Speeds:</color>\n";

        if (cubeBeltSpeed >= 1.5f && cubeBeltSpeed <= 2.5f && basketBeltSpeed >= 1.5f && basketBeltSpeed <= 2.5f)
        {
            feedback += "Your conveyor belt speeds are within a good range.\n";
        }
        else
        {
            if (cubeBeltSpeed < 1.5f || cubeBeltSpeed > 2.5f)
                feedback += $"- Cube belt speed ({cubeBeltSpeed} m/s) is outside the optimal range of <color=#FF4500>1.5-2.5 m/s</color>.\n";

            if (basketBeltSpeed < 1.5f || basketBeltSpeed > 2.5f)
                feedback += $"- Basket belt speed ({basketBeltSpeed} m/s) is outside the optimal range of <color=#FF4500>1.5-2.5 m/s</color>.\n";
        }

        return feedback;
    }
}

