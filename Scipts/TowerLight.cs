//Import the necessary namespaces
using UnityEngine;

//Define a public class named TowerLight that inherits from MonoBehaviour
public class TowerLight : MonoBehaviour
{
    //Declare public variables to reference Light components in the scene
    public Light greenlight1;
    public Light greenlight2;
    public Light redlight1;
    public Light redlight2;
    //Declare a public variable to reference a BasketConveyorManager
    public BasketConveyorManager conveyorManager;


    //The Start() function is called once before the first frame update
    void Start()
    {
        //Initialize the state of all lights, in which the green lights are turned on while turning off the red lights
        greenlight1.enabled = true;
        greenlight2.enabled = true;
        redlight1.enabled = false;
        redlight2.enabled = false;

    }

    //The Update() function is called once per frame
    void Update()
    {
        //UpdateLightStatus() function is called to update the light status
        UpdateLightStatus();

    }

    //An UpdateLightStatus() function is created to update the light status based on the basket filling status
    public void UpdateLightStatus()
    {
       
        //Check the value of basketCount from both baskets 
        //If a basket is equal to 5, the red light is turned on and the green light is turned off
        if (conveyorManager.basket1Collision.basketCount1 == 5)
        {
            greenlight1.enabled = false;
            redlight1.enabled = true;
        }
        //If a basket is equal to 0, the green light is turned on and the red light is turned off
        else if (conveyorManager.basket1Collision.basketCount1 == 0)
        {
            greenlight1.enabled = true;
            redlight1.enabled = false;
        }

        // Update lights for basket 2
        if (conveyorManager.basket2Collision.basketCount2 == 5)
        {
            greenlight2.enabled = false;
            redlight2.enabled = true;
        }
        else if (conveyorManager.basket2Collision.basketCount2 == 0)
        {
            greenlight2.enabled = true;
            redlight2.enabled = false;
        }
    }
}