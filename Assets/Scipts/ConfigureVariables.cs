//Import the necessary namespaces
using UnityEngine;

//Define a public class that inherits from MonoBehaviour
public class ConfigureVariables : MonoBehaviour
{
    //Declare a type of ConfigureVariables property named Instance
    public static ConfigureVariables Instance { get; private set; }
    //Declare different public variables to store configuration variables
    public float CubeBeltSpeed;
    public float MiddleArmSpeed;
    public float UpperArmSpeed;
    public int NumOfCubes;
    public float BasketBeltSpeed;

    //Define a private method named Awake that is called automatically when the script is executed
    private void Awake()
    {
        //Check if Instance is null
        if (Instance == null)
        {
            //Set Instance to the current script 
            Instance = this;
            //Prevent gameObject from being destroyed when loading new scene
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //current gameObject is destroyed if Instance is not null
            Destroy(gameObject);
        }
    }
}