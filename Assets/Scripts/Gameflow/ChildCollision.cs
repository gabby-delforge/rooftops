using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCollision : MonoBehaviour {

	Rigidbody2D parentRB;

	// Use this for initialization
	void Start () {
		parentRB = GetComponentInParent<Rigidbody2D> ();
	}

	// Update is called once per frame
	void Update () {

	}

	void OnCollisionEnter2D( Collision2D collision) {
		this.parentRB.SendMessage("OnCollisionEnter2D",collision);
	}
}