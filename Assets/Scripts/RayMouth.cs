using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RayMouth : MonoBehaviour {
    [System.ComponentModel.DefaultValue(0f)]

    public MicrophoneInput mic;
    public float micThreshold = 0.5f;
    
    public float Score
    {
        get;
        private set;
    }

    List<GameObject> _collidedPlanctons = new List<GameObject>();

    // Use this for initialization
    void Start ()
    {
        GetGlobalVars();
        Score = 0f;
        _collidedPlanctons.Clear();
	}

    public void GetGlobalVars()
    {
        // Get global vars
        maxAngle = GlobalVars.Instance.maxAngle;
        maxSteeringSpeed = GlobalVars.Instance.maxSteeringSpeed;
        speed = GlobalVars.Instance.speed;
        hp = GlobalVars.Instance.hp;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.tag == "plancton")
        {
            if(!_collidedPlanctons.Any(c => object.ReferenceEquals(collision.gameObject, c))) 
            {
                _collidedPlanctons.Add(collision.gameObject);
            }
        }
    }

    private void Update()
    {
        GetGlobalVars();

        // To later replace with the sound detection
        if (mic.loudness > micThreshold)
        {
            Debug.Log("Loud enough to eat");
            Score += _collidedPlanctons.Count;

            foreach (var go in _collidedPlanctons) Destroy(go);

            _collidedPlanctons.Clear();
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.tag == "plancton")
        {
            var idToDelete = _collidedPlanctons.Select((c, idx) => new { c, idx }).FirstOrDefault(o => object.ReferenceEquals(o.c, collision.gameObject));
            if (idToDelete != null) _collidedPlanctons.RemoveAt(idToDelete.idx);
        }
    }
}
