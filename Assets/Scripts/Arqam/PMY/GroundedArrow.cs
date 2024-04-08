using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedArrow : MonoBehaviour
{
    public Transform[] nodes; // Array to hold the node points
    public GameObject arrowPrefab; // Prefab of the arrow
    public float speed = 5f; // Adjust the speed as needed
    public float delayBetweenArrows = 1f; // Delay between each arrow's movement

    void Start()
    {
        // Start spawning arrows
        InvokeRepeating("SpawnArrow", delayBetweenArrows, delayBetweenArrows);
    }

    void SpawnArrow()
    {
        // Instantiate a new arrow
        GameObject newArrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        newArrow.transform.parent = this.transform;
        // Start moving the arrow
        newArrow.GetComponent<Arrow>().StartMovement(nodes, speed);
    }



}
