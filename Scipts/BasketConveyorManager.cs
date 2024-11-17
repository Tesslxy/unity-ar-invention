using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketConveyorManager : MonoBehaviour
{
    public GameObject currentbasket1;
    public GameObject currentbasket2;
    public Vector3 basket1Position;
    public Vector3 basket2Position;
    public Vector3 direction;
    public Vector3 cubesdirection;

    public Vector3 rotation;

    public GameObject basket1Prefab;
    public GameObject basket2Prefab;

    private List<GameObject> basketsOnBelt1 = new List<GameObject>();
    private List<GameObject> basketsOnBelt2 = new List<GameObject>();


    public int totalBasket1;  //total of cubes in basket 1
    public int totalBasket2;  //total of cubes in basket 2
    public int currentBasket1Count;  //current number of cubes in basket 1
    public int currentBasket2Count; //current number of cubes in basket 2
    public int totalCube;  //total number of cubes


    public BasketCollision basket1Collision;
    public BasketCollision basket2Collision;

    private ConfigureVariables configVar;


    public GameObject groundPlaneStage;

    public float basketMoveSpeed; // speed at which the basket moves
    public float basketMoveDistance = 4.5f; // distance the basket should move

    public bool isBasket1Moving = false;
    public bool isBasket2Moving = false;

    private bool isBasket1Spawning = false;
    private bool isBasket2Spawning = false;



    private void Start()
    {
        basketsOnBelt1.Add(currentbasket1);
        basketsOnBelt2.Add(currentbasket2);

        configVar = ConfigureVariables.Instance;
        basketMoveSpeed = configVar.BasketBeltSpeed;

    }


    void Update()
    {
        CheckFull();

    }

    private void OnCollisionEnter(Collision collision)
    {
  
        if (collision.gameObject.CompareTag("Cube1"))
        {
            collision.transform.SetParent(currentbasket1.transform);

        }
        else if (collision.gameObject.CompareTag("Cube2"))
        {
            collision.transform.SetParent(currentbasket2.transform);    

        }
    }

    //check whether the basket is full
    private void CheckFull()
    {
 

        //if the basket is full and is isBasketMoving false
        if (basket1Collision.basketCount1 == 5 && !isBasket1Moving && !isBasket1Spawning)
        {
            isBasket1Moving = true; //set it basketmoving to true
            isBasket1Spawning = true; // Set the spawning flag

            StartCoroutine(MoveBasketCoroutine(currentbasket1, "SpawnBasket1")); //move the basket

        }
        if (basket2Collision.basketCount2 == 5 && !isBasket2Moving && !isBasket2Spawning)
        {
            //Debug.Log("Basket2 is FULL");
            isBasket2Moving = true;
            isBasket2Spawning = true; // Set the spawning flag
            StartCoroutine(MoveBasketCoroutine(currentbasket2, "SpawnBasket2"));
        }

    }


    //method to move the basket 
    private IEnumerator MoveBasketCoroutine(GameObject basket, string spawnBasketTag)
    {

        yield return new WaitForSeconds(2f); //delay

        List<GameObject> basketsToMove = (spawnBasketTag == "SpawnBasket1") ? basketsOnBelt1 : basketsOnBelt2;

        List<Vector3> targetPositions = new List<Vector3>();

        // Calculate target positions for all baskets
        foreach (GameObject basketObj in basketsToMove)
        {
            // Use the ground plane's transform to calculate the movement direction
            Vector3 localDirection = groundPlaneStage.transform.TransformDirection(direction.normalized);
            Vector3 targetPosition = basketObj.transform.position + localDirection * basketMoveDistance;
            targetPositions.Add(targetPosition);
        }

        bool allBasketsReachedTarget = false;

        while (!allBasketsReachedTarget)
        {
            allBasketsReachedTarget = true;

            for (int i = 0; i < basketsToMove.Count; i++)
            {
                GameObject basketObj = basketsToMove[i];
                Vector3 targetPosition = targetPositions[i];
                Transform basketTransform = basketObj.transform;

                // Calculate the distance to the target position
                if ((basketTransform.position - targetPosition).sqrMagnitude > 0.01f)
                {
                    Vector3 direction = (targetPosition - basketTransform.position).normalized;
                    Vector3 movement = direction * basketMoveSpeed * Time.fixedDeltaTime;
                    basketTransform.Translate(movement, Space.World);
                    allBasketsReachedTarget = false;
                }
            }

            yield return new WaitForFixedUpdate();
        }

        //spawn the basket 
        SpawnBasket(spawnBasketTag);

        if (spawnBasketTag == "SpawnBasket1")
        {
            isBasket1Moving = false;
            isBasket1Spawning = false;
        }
        else
        {
            isBasket2Moving = false;
            isBasket2Spawning = false;
        }
    }


    private void SpawnBasket(string baskets)
    {
        if (baskets == "SpawnBasket1" && isBasket1Moving)
        {
            Debug.Log("Spawning basket 1");
            totalBasket1 += basket1Collision.basketCount1;
            isBasket1Spawning = false;

            // Use the ground plane's transform to set the spawn position and rotation
            Vector3 localSpawnPosition = new Vector3(basket1Position.x, basket1Position.y, basket1Position.z);
            Vector3 worldSpawnPosition = groundPlaneStage.transform.TransformPoint(localSpawnPosition);

            Quaternion desiredRotation = Quaternion.Euler(rotation);
            Quaternion finalRotation = groundPlaneStage.transform.rotation * desiredRotation;
            GameObject newBasket = Instantiate(basket1Prefab, worldSpawnPosition, finalRotation);

            newBasket.transform.SetParent(groundPlaneStage.transform, true);
            basketsOnBelt1.Add(newBasket);
            currentbasket1 = newBasket;
            basket1Collision = newBasket.GetComponent<BasketCollision>();

            if (basket1Collision.basketCount1 == 5)
            {
                basket1Collision.ResetCount("basket1");
            }
        }
        else if (baskets == "SpawnBasket2" && isBasket2Moving)
        {
            Debug.Log("Spawning basket 2");

            totalBasket2 += basket2Collision.basketCount2;
            isBasket2Spawning = false;


            // Use the ground plane's transform to set the spawn position and rotation
            Vector3 localSpawnPosition = new Vector3(basket2Position.x, basket2Position.y, basket2Position.z);
            Vector3 worldSpawnPosition = groundPlaneStage.transform.TransformPoint(localSpawnPosition);


            Quaternion desiredRotation = Quaternion.Euler(rotation);
            Quaternion finalRotation = groundPlaneStage.transform.rotation * desiredRotation;
            GameObject newBasket = Instantiate(basket2Prefab, worldSpawnPosition, finalRotation);

            newBasket.transform.SetParent(groundPlaneStage.transform, true);
            basketsOnBelt2.Add(newBasket);
            currentbasket2 = newBasket;
            basket2Collision = newBasket.GetComponent<BasketCollision>();

            if (basket2Collision.basketCount2 == 5)
            {
                basket2Collision.ResetCount("basket2");
            }
        }
    }

    //method to get the total number of cubes
    public int GetTotalCubeCount()
    {
    
        int currentBasket1Count = basket1Collision != null ? basket1Collision.basketCount1 : 0;
        int currentBasket2Count = basket2Collision != null ? basket2Collision.basketCount2 : 0;


        totalCube = totalBasket1 + totalBasket2 + currentBasket1Count + currentBasket2Count;

        return totalCube;
    }
}
