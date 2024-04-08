using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Transform[] nodes; // Array to hold the node points
    public int currentNodeIndex = 0; // Index of the current node being followed
    private float speed; // Speed of the arrow

    public void StartMovement(Transform[] _nodes, float _speed)
    {
        nodes = _nodes;
        speed = _speed;
        // Start moving the arrow
        InvokeRepeating("MoveArrow", 0f, Time.fixedDeltaTime);
    }

    void MoveArrow()
    {
        if (nodes == null || currentNodeIndex >= nodes.Length)
        {
            // Stop movement if nodes are not set or index exceeds the length
            CancelInvoke("MoveArrow");
            return;
        }

        // Move towards the current node
        transform.position = Vector3.MoveTowards(transform.position, nodes[currentNodeIndex].position, speed * Time.deltaTime);

        // Rotate towards the current node
        Vector3 direction = (nodes[currentNodeIndex].position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);

        // Check if the arrow has reached the current node
        if (Vector3.Distance(transform.position, nodes[currentNodeIndex].position) < 0.1f)
        {
            // Move to the next node
            currentNodeIndex = (currentNodeIndex + 1) % nodes.Length;
            if (currentNodeIndex == 0) Destroy(this.gameObject);
        }
    }

}
