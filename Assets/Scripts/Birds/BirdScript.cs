using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdScript : MonoBehaviour {
	[Tooltip("Offset (in degrees) from directly to the right along X axis that represents the lowest to the ground the bird can fly")]
	public float lowestDegreeAngleOffset;
	[Tooltip("Size (in degrees) of range that birds can possibly fly, beginning at lowestDegreeAngleOffset")]
	public float rangeOfPossibleAngles;
	public float speedLowerBound;
	public float speedUpperBound;
	AudioClip flap;
	Rigidbody2D rb;
	Animator anim;
	bool flying;
    float randSpeed;
    float randDirection;
	Vector2 vecToFlyTo;
	// Use this for initialization
	void Start () {
		flying = false;
		rb = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
		randSpeed = Random.Range (speedLowerBound, speedUpperBound);
		float lowAngle = lowestDegreeAngleOffset * Mathf.PI / 180;
		float hiAngle = lowAngle + rangeOfPossibleAngles * Mathf.PI / 180;
		randDirection = Random.Range(lowAngle, hiAngle);
		if (randDirection * (180 / Mathf.PI) > 90) {
			transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
		}
		vecToFlyTo = new Vector2 (Mathf.Cos(randDirection) * randSpeed, Mathf.Sin(randDirection) * randSpeed);
    }
	
	// Update is called once per frame
	void Update () {
		if (rb.transform.position.y > 10
			|| rb.transform.position.y < -20) {
			Destroy (gameObject);
		}
		if (flying) {
			rb.velocity = vecToFlyTo;
		}

	}

	public void startFlying() {
		flying = true;
		gameObject.layer = LayerMask.NameToLayer ("FlyingBirds");
		int i = Random.Range (1, 3);
		flap = Resources.Load<AudioClip>("Sound/SFX/Birdflaps/" + i);
		AudioSource.PlayClipAtPoint (flap, transform.position, Random.Range(0.5f, 1));
		anim.SetTrigger ("flying");
		rb.velocity = vecToFlyTo;
	}
}
