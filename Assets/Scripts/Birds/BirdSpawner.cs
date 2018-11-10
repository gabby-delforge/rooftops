using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSpawner : MonoBehaviour {
	[Tooltip("The Bird prefab we're spawning")]
	public GameObject bird;
	[Tooltip("How far from the camera the birds will spawn when their spawn is triggered")]
	public float distance;
	[Tooltip("How much the distance from trigger varies")]
	public float variabilityDistance;
	[Tooltip("Horizontal distance representing how far the player must travel before new birds are spawned")]
	public float xDistanceToTriggerSpawn;
	[Tooltip("Density of bird cluster that spawns each time the spawn is triggered")]
	public int density;

	float locationNextBirdTrigger;

	// Use this for initialization
	void Start () {
		locationNextBirdTrigger = xDistanceToTriggerSpawn;

	}

	// Update is called once per frame
	void Update () {
		if (Camera.main.transform.position.x > locationNextBirdTrigger) {
			AddBird ();
			locationNextBirdTrigger += Random.Range (distance / 2, distance);
		}
	}


	void AddBird() {
		Vector2 pos = new Vector2 (locationNextBirdTrigger + distance + Random.Range(0, variabilityDistance), 10);
		int num_birds = density/2 + Random.Range (0, density);
		for (int i = 0; i < num_birds; i++) {
			Instantiate (bird, pos, Quaternion.identity);
			pos.x += 0.2f;
		}
	}
}

