using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour {

	public float SPEED;
	public GameObject REF;
	private float prevRefX;
	private Vector3 velocity = Vector3.zero;
	private Renderer GO_render;

	void Start () {
		prevRefX = REF.gameObject.transform.position.x;
		GO_render = GetComponent<Renderer> ();
	}

	void Update () {
		float newRefX = REF.gameObject.transform.position.x;
		float difference = newRefX - prevRefX;
		float newX = transform.position.x - ((difference * SPEED) / 100);
		Vector3 target = new Vector3 (newX, transform.position.y, transform.position.z);
		transform.position = Vector3.SmoothDamp (transform.position, target, ref velocity, 0.01f);
//		transform.position = target;
		prevRefX = newRefX;
	}

	void OnBecameInvisible() {
		Vector3 curr = transform.position;
		curr.x += 2 * GO_render.bounds.size.x;
		transform.position = curr;
	}
}
