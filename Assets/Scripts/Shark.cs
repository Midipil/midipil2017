using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark : MonoBehaviour {

	// Speed in m/s
	public float speed = 10.0f;
	public float deathDelay = 10.0f;
	public ParticleSystem bubbles;
	public ParticleSystem dustOnGroundHit;
	public AudioClip[] sndClips; // 0;Attack   1;HitGround
    public AudioClip HitGroundSound;
	AudioSource audioSource;
	private bool hitGround = false;
	float time;
	// Use this for initialization
	void Start () {
		dustOnGroundHit.Stop ();
		audioSource = GetComponent<AudioSource> ();
		audioSource.clip = sndClips [Random.Range(0, sndClips.Length - 1)];
	}

	// Update is called once per frame
	void Update () {
		if (this.transform.position.y > 0) {
			this.transform.Translate (new Vector3 (0f, -speed * Time.deltaTime, 0f));

            if(this.transform.position.y / speed <= audioSource.clip.length / 2f && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
		} else {
			if (!hitGround) {
				hitGround = true;
				// Emit one burst
				dustOnGroundHit.Emit ((int)dustOnGroundHit.emission.GetBurst (0).count.constant);
				// Play Sound Hit Ground
				audioSource.Stop();
				audioSource.PlayOneShot(HitGroundSound);
			}
			// Shark is in the ground
			if (bubbles.isEmitting) {
				bubbles.Stop ();
			}
		}
		time += Time.deltaTime;
		if (time > deathDelay)
			Destroy (gameObject);
	}
}
