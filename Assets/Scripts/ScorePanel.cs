using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePanel : MonoBehaviour {

	// Speed in m/s
	public float speed = 20.0f;
	public float deathDelay = 60.0f;
	public ParticleSystem bubbles;
	public ParticleSystem dustOnGroundHit;

    public TextMesh text;

	private bool hitGround = false;
	float time;
	// Use this for initialization
	void Start () {
		dustOnGroundHit.Stop ();
	}

	// Update is called once per frame
	void Update () {
		if (this.transform.position.y > 0) {
			this.transform.Translate (new Vector3 (0f, -speed * Time.deltaTime, 0f), Space.World);
		} else {
			if (!hitGround) {
				hitGround = true;
				// Emit one burst
				dustOnGroundHit.Emit ((int)dustOnGroundHit.emission.GetBurst (0).count.constant);
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

    public void SetScore(int s)
    {
        text.text = s.ToString();
    }
}
