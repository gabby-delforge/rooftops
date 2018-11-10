using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BuildingConstants : MonoBehaviour {
	public float entranceHeight;
	public float exitHeight;
	public float destroyDistance = 200;
  
    public float H = 0;
    public float S = 0;
    public float V = 100;

    public Color c;
	public float modifier;

	private GameObject player;

	// void Update() {
	// 	Vector3 newHeight = this.transform.position;
	// 	newHeight.y = entranceHeight;
	// 	this.transform.position = newHeight;

	// 	this.entranceHeight = newHeight.y;
	// 	this.exitHeight = exitHeight;
	// }

	void Start() {
		player = GameObject.Find("Player");
        chooseColor();

    }

	void Update() {
		if(player.transform.position.x > gameObject.transform.position.x + destroyDistance) {
			Destroy(gameObject);
		}
	}


	public void SetModifier(float mod) {
		modifier = mod;

//		print ("EntranceHeight: " + entranceHeight.ToString () + " ExitHeight: " + exitHeight.ToString () + " Mod: " + modifier.ToString ());

		Vector3 newHeight = this.transform.position;
		newHeight.y = entranceHeight + modifier;
		this.transform.position = newHeight;

		this.entranceHeight = newHeight.y;
		this.exitHeight = exitHeight + modifier;

	}

    public void chooseColor() {
        H = (H >= 0) ? H : Random.Range(0, 360.0f);
        S = (S >= 0) ? S : Random.Range(0, 100.0f);
        V = (V >= 0) ? V : Random.Range(0, 100.0f);
        Debug.Log(H.ToString() + " " + S.ToString() + " " + V.ToString());

        c = Color.HSVToRGB(H, S, V);
        gameObject.GetComponent<SpriteRenderer>().color = c;

    }
}