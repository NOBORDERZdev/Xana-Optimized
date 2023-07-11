using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCRandomMovement : MonoBehaviour
{
    // Start is called before the first frame update
    Animator animator;
    CharacterController controller;
    public enum movement { Walk,idel,sprint,dance };
    movement currentmovement;
    
    int TimeMovement=0;
    System.Random random;

    public bool stopanimation = true;

    Quaternion rotatn;
    bool isGrounded = false;
    

    void Start()
    {

        Vector3 trans = transform.position;
        trans.y = -0.081f;
        transform.position = trans;
        animator = gameObject.GetComponent<Animator>();
        random = new System.Random();
        controller = GetComponent<CharacterController>();
        Invoke("startanimation", 5);

    }
    public void startanimation()
    {
        stopanimation = false;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            //Debug.Log("Did Hit");

            if (transform.position.y > hit.point.y)
            {
               // transform.localPosition = new Vector3(0, -0.081f, 0);
                Vector3 vector3 = transform.position;
                vector3.y = hit.point.y;
                transform.position = vector3;
                //Debug.Log("hit y==="+ hit.transform.localPosition.y);
                //Debug.Log("not grounded");
                isGrounded = false;
                
            }
            else
            {
                //Debug.Log("grounded");
                isGrounded = true;
            }
                

        }
       
        if (stopanimation) return;

        if (TimeMovement <= 0)
        {
            TimeMovement = Random.Range(80,100);
           
            var tempRotation = Quaternion.identity;
            var tempVector = Vector3.zero;
            tempVector = tempRotation.eulerAngles;
            tempVector.y = Random.Range(0, 359);
            tempRotation.eulerAngles = tempVector;
            rotatn = tempRotation;
            transform.rotation=Quaternion.SlerpUnclamped(transform.rotation,rotatn,0.5f);

            int move = Random.Range(0, 2);
         
            switch (move)
            {
                case 0:
                    {
                        currentmovement = movement.Walk;
                        break;
                       
                    }
                case 1:
                    {
                        currentmovement = movement.idel;
                        break;
                    }
                case 2:
                    {
                        currentmovement = movement.sprint;
                        break;
                    }

            }

            return;
        }

        Vector3 data = transform.forward;
       // data.y = 0;


        switch (currentmovement)
        {
            case movement.Walk:
                {
                    float tempVariable = data.y;
                    Vector3 vector3 = data * 2 * Time.deltaTime;
                    vector3.y = -0.081f;
                    animator.SetFloat("Blend", 0.5f, 0.1f, Time.deltaTime);
                    controller.Move(vector3);

                    break;
                }
            case movement.idel:
                {

                    animator.SetFloat("Blend", 0, 0.1f, Time.deltaTime);
                    break;
                }
            case movement.sprint:
                {
                    float tempVariable = data.y;
                    Vector3 vector3 = data * 5 * Time.deltaTime;
                    vector3.y = -0.081f;
                    controller.Move(vector3);
                    animator.SetFloat("Blend", 1.1f, 0.1f, Time.deltaTime);
                    break;
                }
        }
        TimeMovement -= 1;

        
    }
    private void OnCollisionEnter(Collision collision)
    {
        var tempRotation = Quaternion.identity;
        var tempVector = Vector3.zero;
        tempVector = tempRotation.eulerAngles;

        
            tempVector.y = Random.Range(0 + transform.rotation.y + 60,359 - (transform.rotation.y+60));
        
        tempRotation.eulerAngles = tempVector;
        transform.rotation =Quaternion.SlerpUnclamped(transform.rotation, tempRotation,0.5f);
    }
}
