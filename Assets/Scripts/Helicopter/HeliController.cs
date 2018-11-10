using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliController : MonoBehaviour
{
    [Tooltip("The Helicopter prefab")]
    public GameObject heli;
    [Tooltip("Horizontal distance representing how far the player must travel before new helicopters are spawned")]
    public float xDistanceToTriggerSpawn;
    [Tooltip("How far from the camera the helicopter will spawn when its spawn is triggered")]
    public float distance;
    [Tooltip("How much the distance from trigger varies")]
    public float variabilityDistance;
    public float despawnDistance;
    float locNextHeliSpawn;
    List<GameObject> helicopters;
    FrenzyMode frenzy;

    // Use this for initialization
    void Start()
    {
        locNextHeliSpawn = Camera.main.transform.position.x;
        helicopters = new List<GameObject>();

        frenzy = GameObject.Find("FrenzyController").GetComponent<FrenzyMode>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main.transform.position.x > locNextHeliSpawn)
        {
            addHeli();
        }
        checkDespawn();
    }

    public void sawPlayer()
    {
        foreach (GameObject h in helicopters)
        {
            h.GetComponent<HeliScript>().findPlayer();
        }

    }

    void addHeli()
    {
        Vector2 pos = new Vector2(locNextHeliSpawn + distance + Random.Range(0, variabilityDistance), Camera.main.transform.position.y + 10);
        GameObject instance = Instantiate(heli, pos, Quaternion.identity);
        locNextHeliSpawn = pos.x + xDistanceToTriggerSpawn + Random.Range(0, variabilityDistance);

        helicopters.Add(instance);
        if (frenzy.isActive())
        {
            instance.GetComponent<HeliScript>().findPlayer();
        }
    }

    void checkDespawn()
    {
        bool[] marked = new bool[helicopters.Count];
        for (int i = 0; i < helicopters.Count; i++)
        {
            GameObject h = helicopters[i];
            if (Camera.main.transform.position.x - h.transform.position.x > despawnDistance)
            {
                marked[i] = true;
            }

        }
        for (int j = 0; j < helicopters.Count; j++)
        {
            if (marked[j])
            {
                Destroy(helicopters[j]);
                helicopters.RemoveAt(j);

            }
        }

    }
}

