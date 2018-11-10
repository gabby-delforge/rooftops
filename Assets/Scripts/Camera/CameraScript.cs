using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

	public float xOffset;
	public float yOffset;

	private float minY;
	private float maxY;
	private float camHeight;
	private GameObject follow;

	// Use this for initialization
	void Start () {
		follow = GameObject.Find ("Player");

		Camera cam = Camera.main;
		camHeight = cam.orthographicSize;

		GameObject background = GameObject.Find("Background");
		SpriteRenderer sr = background.GetComponent<SpriteRenderer> ();
		minY = sr.bounds.min.y;
		maxY = sr.bounds.max.y;
	}
	
	void Update() {
		Vector3 newPos = transform.position;
		newPos.x = follow.transform.position.x + xOffset;
		newPos.y = follow.transform.position.y + yOffset;

		if (newPos.y + camHeight > maxY) {
			newPos.y = maxY - camHeight;
		}
		if (newPos.y - camHeight < minY) {
			newPos.y = minY + camHeight;
		}

		transform.position = newPos;
	}
}
