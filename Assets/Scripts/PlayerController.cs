using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Transform head, left, right;
    private Rigidbody rb;
    public BoxCollider mouthCollider, bodyCollider;
    //Movement
    public float playerHeight = 1.5f;
    public float maxAngle = 45f;
    public float maxSteeringSpeed = 1f;
    public float speedForce = 3f;
    bool faceDown = true;
    // Health points
    private int hp = 1;

    // sounds
    public float _minimumForceForSound = 1f;
    public AudioClip[] _waterSounds;
    public AudioSource _leftWaterSound;
    public AudioSource _rightWaterSound;


	public float widthLimit = 4;

    private float _decceleratorDividor = 1f;

    // Use this for initialization
    void Start()
    {
        // Destroy the controllers if needed
#if !UNITY_EDITOR
        foreach (Transform t in left) Destroy(t);
        foreach (Transform t in right) Destroy(t);
#endif

        GetGlobalVars();

        CalibratePlayer();

        // Set vars
        rb = this.GetComponent<Rigidbody>();

        rb.inertiaTensorRotation = Quaternion.identity;
        rb.inertiaTensor = Vector3.one;
        rb.centerOfMass = Vector3.zero;

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

    public void StartGame()
    {
        CalibratePlayer();
    }

    // Update is called once per frame
    void Update()
    {

        GetGlobalVars();

        float angle = GetAngle();

        //Debug
        if (Input.GetKey("d"))
        {
            Debug.Log("angle : " + angle);
        }
        if (Input.GetKey("a"))
        {
            rb.MovePosition(new Vector3(rb.position.x + -1.0f * Time.deltaTime, rb.position.y, rb.position.z));
        }
        if (Input.GetKey("z"))
        {
            rb.MovePosition(new Vector3(rb.position.x + 1.0f * Time.deltaTime, rb.position.y, rb.position.z));
        }

        // Determine if we're face up or down
        if (angle < 90 && angle > -90)
        {
            faceDown = true;
        }
        else
        {
            faceDown = false;
        }

        if (GameManager.Instance.State == GameManager.GameState.EATING || GameManager.Instance.State == GameManager.GameState.FIGHTING)
        {
            ApplySteering(angle, faceDown);
        }

        ApplySpeed();
    }

    public void GetGlobalVars()
    {
        // Get global vars
        playerHeight = GlobalVars.Instance.playerHeight;
        maxAngle = GlobalVars.Instance.maxAngle;
        maxSteeringSpeed = GlobalVars.Instance.maxSteeringSpeed;
        speedForce = GlobalVars.Instance.speed;
        hp = GlobalVars.Instance.hp;
    }

    public void CalibratePlayer()
    {
        Vector3 difVec = new Vector3(0, playerHeight, 0) - head.position;
        this.transform.Find("VR player").position += difVec;
        Debug.Log("dif vec : " + difVec);
    }

    void ApplySteering(float angle, bool down)
    {
        if (!down)
        {
            if (angle < -90)
            {
                angle = angle + 180f;
            }
            else
            {
                angle = angle - 180f;
            }
        }
        // Compute force
        float force = angle * maxSteeringSpeed / maxAngle;

        if (this.transform.position.x < -widthLimit && force < 0 ||
            this.transform.position.x > widthLimit && force > 0)
        {
            return;
        }

        // apply
        rb.MovePosition(new Vector3(rb.position.x + force * Time.deltaTime, rb.position.y, rb.position.z));
        //rb.AddForce(new Vector3(force, 0, 0));

        if (Mathf.Abs(force) > _minimumForceForSound && !_rightWaterSound.isPlaying && !_leftWaterSound.isPlaying)
        {
            if (force > 0) // 
            {
                _rightWaterSound.clip = _waterSounds[Random.Range(0, _waterSounds.Length - 1)];
                _rightWaterSound.volume = Mathf.Min(1f, force / 4f);
                _rightWaterSound.Play();
            }
            else
            {
                _leftWaterSound.clip = _waterSounds[Random.Range(0, _waterSounds.Length - 1)];
                _leftWaterSound.volume = Mathf.Min(1f, -force / 4f);
                _leftWaterSound.Play();
            }
        }
    }

    private void ApplySpeed()
    {
        //rb.AddForce(new Vector3(0, 0, speedForce));
        if (GameManager.Instance.State == GameManager.GameState.EATING || GameManager.Instance.State == GameManager.GameState.FIGHTING)
        {
            _decceleratorDividor = 1f;
            rb.MovePosition(new Vector3(rb.position.x, rb.position.y, rb.position.z + speedForce * Time.deltaTime));
        }
        else if (GameManager.Instance.State == GameManager.GameState.GAME_OVER && _decceleratorDividor < 20f)
        {
            rb.MovePosition(new Vector3(rb.position.x, rb.position.y, rb.position.z + speedForce * Time.deltaTime / _decceleratorDividor));
            _decceleratorDividor += 3 * Time.deltaTime;
        }
    }

    public float GetAngle()
    {
        Vector3 handsVec = right.position - left.position;
        handsVec = new Vector3(handsVec.x, handsVec.y, 0f);
        float angle = 0f;
        if (handsVec.magnitude > 0.3f)
        {
            angle = Vector3.Angle(handsVec, Vector3.right);
            // Add angle sign back
            if (handsVec.y > 0f)
            {
                angle *= -1;
            }
        }
        return angle;
    }

    public int Hit(int damage = 1)
    {
        hp -= damage; // 1 is default
                      // Rumble
        StartCoroutine(Rumble(1.5f, 1f));
        if (hp <= 0)
        {
            GameOver();
        }
        return hp; // for info
    }

    private void GameOver()
    {
        Debug.LogError("GAME OVER");
        GameManager.Instance.GameOver(GetComponentInChildren<RayMouth>().Score);
    }

    IEnumerator Rumble(float length, float strength)
    {
        for (float i = 0; i < length; i += Time.deltaTime)
        {
            SteamVR_Controller.Input((int)left.GetComponent<SteamVR_TrackedObject>().index).TriggerHapticPulse((ushort)Mathf.Lerp(0, 3999, strength));
            SteamVR_Controller.Input((int)right.GetComponent<SteamVR_TrackedObject>().index).TriggerHapticPulse((ushort)Mathf.Lerp(0, 3999, strength));
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("obstacle") || collision.CompareTag("shark"))
        {
            Hit();
        }
    }
}
