using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RayMouth : MonoBehaviour {
    [System.ComponentModel.DefaultValue(0f)]
    public float Score
    {
        get;
        private set;
    }

    List<GameObject> _collidedPlanctons = new List<GameObject>();

    // Use this for initialization
    void Start ()
    {
        Score = 0f;
        _collidedPlanctons.Clear();
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
        // To later replace with the sound detection
        if (true)
        {
            foreach (var go in _collidedPlanctons)
            {
                if (go != null)
                {
                    Score++;
                    Debug.Log("score : " + Score);
                    Destroy(go);
                }
            }
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
