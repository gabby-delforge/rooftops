using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Glow : MonoBehaviour {

	public bool text;
	public float min = 0f;
	public float max = 1f;
	public float fadeSpeed = 0.5f;

	private float scale = 1;
	private Color color;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update() {
		if (text) {
			color = gameObject.GetComponent<Text> ().color;
			if (color.a < min && scale < 0) {
				scale = 1;
			} else if (color.a > max && scale > 0) {
				scale = -1;
			}
			gameObject.GetComponent<Text> ().color = new Color (color.r, color.g, color.b, color.a + scale * Time.deltaTime * fadeSpeed);

		} else {
			color = gameObject.GetComponent<Image> ().color;
			if (color.a < min && scale < 0) {
				scale = 1;
			} else if (color.a > max && scale > 0) {
				scale = -1;
			}
			gameObject.GetComponent<Image> ().color = new Color (color.r, color.g, color.b, color.a + scale * Time.deltaTime * fadeSpeed);

		}


	}
}
