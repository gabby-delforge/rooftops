using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSpawner : MonoBehaviour {


	public GameObject[] buildings;
	public GameObject[] checkPointBuildings;
	public float buildingGap;

	public GameObject player;
	public float verticalChange;

	private float maxX;
	public float lastY;
	private GameObject lastBuilding;

	private float distTraveled;
	private float start;
	private bool resetCheckpoint;


	// Use this for initialization
	void Start () {
		lastBuilding = GameObject.Find ("building5");
		maxX = lastBuilding.GetComponent<SpriteRenderer> ().bounds.max.x;
		lastY = lastBuilding.GetComponent<SpriteRenderer> ().bounds.max.y;
		start = player.transform.position.x;
		distTraveled = 0;
		resetCheckpoint = false;
	}

	// Update is called once per frame
	void Update () {
		if(player.transform.position.x > maxX - 40) {
			AddBuilding();
		}
	}

	void AddBuilding() {
		int index = Random.Range (0, buildings.Length);
		Vector3 pos = new Vector3 (maxX + Random.Range (5, buildingGap), -20);

		BuildingConstants bc = lastBuilding.GetComponent<BuildingConstants> ();

		lastBuilding = Instantiate (buildings [index], pos, Quaternion.identity);
		BuildingConstants newBc = lastBuilding.GetComponent<BuildingConstants> ();
		float yMod = Random.Range (-verticalChange, verticalChange/2.0f);
		newBc.SetModifier (bc.modifier + bc.entranceHeight - bc.exitHeight + yMod);

		maxX = lastBuilding.GetComponent<SpriteRenderer> ().bounds.max.x;
		lastY = lastBuilding.GetComponent<SpriteRenderer> ().bounds.max.y;
	}
}
