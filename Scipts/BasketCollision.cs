using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BasketCollision : MonoBehaviour
{

    public int basketCount1 { get; private set; }
    public int basketCount2 { get; private set; }
    public int cubeCount { get; private set; }
    private Vector3 cubesdirection = new Vector3(1, 0, -1);

    private bool cubesMovedInBasket1 = false;
    private bool cubesMovedInBasket2 = false;

    private List<GameObject> cubeinbelt1 = new List<GameObject>();
    private List<GameObject> cubeinbelt2 = new List<GameObject>();

    //will run once collision occur 
    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Cube1")) 
        {

            //stop the cube from moving in the basket
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            cubeinbelt1.Add(collision.gameObject);
            if (!cubesMovedInBasket1 && basketCount1 == 2)
            {
                StartCoroutine(MoveCubeinBasket(1));
            }
        }
        else if (collision.gameObject.CompareTag("Cube2"))
        {
            //stop the cube from moving in the basket
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            cubeinbelt2.Add(collision.gameObject);

            if (!cubesMovedInBasket2 && basketCount2 == 2)
            {

                StartCoroutine(MoveCubeinBasket(2));
            }
        }
    }

    //move the cubes in the basket when there is 2 cube
    public IEnumerator MoveCubeinBasket(int basketType)
    {
        yield return new WaitForSeconds(2f);

        if (basketType == 1 && basketCount1 == 2 && !cubesMovedInBasket1)
        {

            MoveCubes(cubeinbelt1);
            cubesMovedInBasket1 = true;
        }
        else if (basketType == 2 && basketCount2 == 2 && !cubesMovedInBasket2)
        {
            MoveCubes(cubeinbelt2);
            cubesMovedInBasket2 = true;
        }
    }


    //move the cubes in the basket a little to make space for other cubes
    private void MoveCubes(List<GameObject> cubesList)
    {

        foreach (GameObject cube in cubesList)
        {
            Rigidbody rb = cube.GetComponent<Rigidbody>();

            // calculate the target position based on current position and direction
            Vector3 targetPosition = rb.position + cubesdirection.normalized * 0.1f;

            // apply the translation using Translate in local space
            rb.transform.Translate(targetPosition - rb.position, Space.Self);
        }
    }


    //will run once there is trigger
    void OnTriggerEnter(Collider other)
    {
        //when the cube triggered is cube1 
        if (other.gameObject.CompareTag("Cube1"))
        {
            //increment the count
            basketCount1++;
            ///Debug.Log("The current basket1 count " + basketCount1);
            cubeCount++;
        }
        if (other.gameObject.CompareTag("Cube2"))
        {
            basketCount2++;
            //Debug.Log("The current basket2 count " + basketCount1);
            cubeCount++;
        }
    }

    //will run once the cube exit the trigger
    //cube fall unsuccessfull then decrement the count 
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Cube1"))
        {
            basketCount1--;
            cubeCount--;


        }
        if (other.gameObject.CompareTag("Cube2"))
        {
            basketCount2--;
            cubeCount--;

        }
    }

    //resetting the count
    public void ResetCount(string basket)
    {
        if(basket == "basket1")
        {
            basketCount1 = 0;
        }
        else if(basket == "basket2")
        {
            basketCount2 = 0;
        }
    }


}
