using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public Transform head, left, right;
    private Rigidbody rb;
    public BoxCollider mouthCollider, bodyCollider;
    public float maxAngle = 45f;
    public float maxSteeringForce = 1f;
    public float speedForce = 3f;
    bool faceDown = true;

	// Use this for initialization
	void Start () {
        
        // Set vars
        rb = this.GetComponent<Rigidbody>();

        // Check if left controller is really on the left, if not invert...USELESS ?
        /*
        if(left.position.x < right.position.x)
        {
            Transform tempTr = left;
            left = right;
            right = tempTr;
            Debug.LogWarning("controllers inverted");
        }
        */
		
	}
	
	// Update is called once per frame
	void Update () {

        float angle = GetAngle();

        //Debug
        if (Input.GetKey("d"))
        {
            Debug.Log("angle : " + angle );
        }

        // Determine if we're face up or down
        if(angle < 90 && angle > -90)
        {
            faceDown = true;
        } else
        {
            faceDown = false;
        }

        ApplySteering(angle, faceDown);
        ApplySpeed();
        
	}
    

    void ApplySteering(float angle, bool down)
    {
        float force = 0f;
        if (!down)
        {
            if(angle < -90)
            {
                angle = angle + 180f;
            } else
            {
                angle = angle - 180f;
            }
        }
        // Compute force
        force = angle*maxSteeringForce/ maxAngle;
        // apply
        rb.AddForce(new Vector3(force, 0, 0));
    }

    private void ApplySpeed()
    {
        rb.AddForce(new Vector3(0, 0, speedForce));
    }

    public float GetAngle()
    {
        Vector3 handsVec = right.position - left.position;
        handsVec = new Vector3(handsVec.x, handsVec.y, 0f);
        float angle = Vector3.Angle(handsVec, Vector3.right);
        // Add angle sign back
        if(handsVec.y > 0f)
        {
            angle *= -1;
        }
        return angle;
    }
}
