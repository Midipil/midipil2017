using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RayMouth : MonoBehaviour {
    [System.ComponentModel.DefaultValue(0f)]

    public MicrophoneInput mic;
    public float micThreshold = 0.5f;

    public float MaxEatingSoundLvl = 0.5f;
    public AudioClip[] _eatingSounds;
    public AudioSource _eatingPlanktonSound;

    public int Score
    {
        get;
        private set;
    }

    List<GameObject> _collidedPlanctons = new List<GameObject>();

    // Use this for initialization
    void Start ()
    {
        GetGlobalVars();
        Score = 0;
        _collidedPlanctons.Clear();
	}

    public void GetGlobalVars()
    {
        // Get global vars
        micThreshold = GlobalVars.Instance.micThreshold;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.CompareTag("plancton"))
        {
            if(!_collidedPlanctons.Any(c => object.ReferenceEquals(collision.gameObject, c))) 
            {
                _collidedPlanctons.Add(collision.gameObject);
            }

            _eatingPlanktonSound.volume = Random.Range(0f, MaxEatingSoundLvl);
            _eatingPlanktonSound.clip = _eatingSounds[Random.Range(0, _eatingSounds.Length - 1)];
            _eatingPlanktonSound.Play();
        }
    }

    private void Update()
    {
        GetGlobalVars();

        // To later replace with the sound detection
        if (mic.loudness > micThreshold || GlobalVars.Instance.voiceCheat)
        {
            Score += _collidedPlanctons.Count;
            
            //Debug.Log("Score = "+Score);
            foreach (var go in _collidedPlanctons)
            {
                GameManager.Instance.NewScore(Score);
                Destroy(go);
            }
            _collidedPlanctons.Clear();
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("plancton"))
        {
            var idToDelete = _collidedPlanctons.Select((c, idx) => new { c, idx }).FirstOrDefault(o => object.ReferenceEquals(o.c, collision.gameObject));
            if (idToDelete != null) _collidedPlanctons.RemoveAt(idToDelete.idx);
        }
    }
}
