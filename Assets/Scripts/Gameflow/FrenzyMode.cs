 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrenzyMode : MonoBehaviour {

    // Use this for initialization
    public int heatLevel;
    public GameObject frenzyGUI;
    public GameObject player;
    private bool active;
    private float time;
    private Text text;
    private ArrayList searchers;
    private ArrayList seePlayer;
    private Vector3 lastKnownPlayerLoc;
    void Start() {
        active = false;
        time = 15f;
        text = frenzyGUI.GetComponentInChildren<Text>();
        frenzyGUI.SetActive(false);
        searchers = new ArrayList();
        seePlayer = new ArrayList();
    }
    private void activate() {
        active = true;
        frenzyGUI.SetActive(true);
        heatLevel = 0;
        GameObject.Find("HelicopterController").GetComponent<HeliController>().sawPlayer();
    }

    public bool isActive() {
        return active;
    }

    public void deactivate() {
        active = false;
        time = 15f;
        frenzyGUI.SetActive(false);
        for (int i = 0; i < searchers.Count; i += 1) {
            HeliScript h = (HeliScript)searchers[i];
            h.resetState();
        }
    }

    public Vector3 getLastKnownPlayerPosition()
    {
        return lastKnownPlayerLoc;
    }

    public void canSeePlayer(HeliScript heli) {
        if (!seePlayer.Contains(heli))
        {
            seePlayer.Add(heli);

        }
        if (!searchers.Contains(heli))
        {
            addSearcher(heli);
        }
    }
    
	public void cannotSeePlayer(HeliScript heli){
	    if (seePlayer.Contains(heli))
        {
            seePlayer.Remove(heli);

        }	
	}

    private void addSearcher(HeliScript heli) {
        if (searchers.Count == 0)
        {
            activate();
        }
        searchers.Add(heli);
        if (searchers.Count == 1)
        {
            heatLevel += 1;
        }
        else if (searchers.Count == 2)
        {
            heatLevel += 2;
        }
    }

    private void removeSearcher(HeliScript heli)
    {
        searchers.Remove(heli);
        if (searchers.Count <= 0)
        {
            deactivate();
        }

    }


	// Update is called once per frame
	void Update () {
		if(active){
			if(time > 0){
				time = Mathf.Clamp(time - Time.deltaTime, 0, 15);
			} else
            {
                deactivate();
                if (seePlayer.Count > 0)
                {
                    player.GetComponent<PlayerMovement>().gameOver();

                } else
                {
                    foreach (HeliScript searcher in searchers)
                    {
                        removeSearcher(searcher);
                    }

                }
			}
			//text.text = time.ToString();

		}
		text.text = time.ToString();

        if (seePlayer.Count > 0)
        {
            lastKnownPlayerLoc = player.transform.position;

        }
	}
}
