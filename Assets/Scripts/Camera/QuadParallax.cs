using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadParallax : MonoBehaviour {

	public float order;
	private MeshRenderer mr;
	private Material mat;
	private Vector2 offset;
	public Transform player;

	// Use this for initialization
	void Start () {
		mr = GetComponent<MeshRenderer>();
		mat = mr.material;
		offset = mat.mainTextureOffset;

	}
	
	// Update is called once per frame
	void Update () {
		offset.x = order * player.position.x / 8000;
		offset.y = order * player.position.y / 20000;
		mat.mainTextureOffset = offset;

	}
}
	