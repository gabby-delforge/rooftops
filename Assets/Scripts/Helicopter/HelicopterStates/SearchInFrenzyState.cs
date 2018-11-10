using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchInFrenzyState : HelicopterState {
    private const float searchDist = 15;
	//The helicopter that currently has this state
	private HeliScript helicopter;


	//These variables are used locally and necessary for correct behavior of SearchInFrenzyState
	private FrenzyMode frenzy;
	private GameObject player;
	private bool lastSeenToRight;
	private PursuitState comingFrom;
	private HelicopterState ifLost;
    private Vector3 rootPos;
    private Vector3 targetSpotPos;

	//These are the six variables needed  to be a valid HelicopterState
	private Vector3 targetPosition;
	private Quaternion targetSpotlightRotation;
	private float currSpeed;
	private float spotlightRotationSpeed;
	private bool flippedThisFrame;
	private bool goBackToPursuit;


	public SearchInFrenzyState(HeliScript helicopter, float deltaTime, PursuitState comingFrom, HelicopterState afterLosing) {
        Debug.Log("Searching");
		//Initializing default, independent vals for behavioral variables
		this.helicopter = helicopter;
		this.comingFrom = comingFrom;
		goBackToPursuit = false;
		ifLost = afterLosing;

        //Interacting with the frenzy controller
        frenzy = GameObject.Find("FrenzyController").GetComponent<FrenzyMode>();

        rootPos =  frenzy.getLastKnownPlayerPosition();
        rootPos.y += helicopter.vertDistanceFromPlayerInChase;

        //Getting the actual player we're chasing down
        player = GameObject.Find("Player");
		lastSeenToRight = player.transform.position.x > rootPos.x;


        targetPosition = rootPos + new Vector3(searchDist, 0, 0);
        targetSpotlightRotation = helicopter.getSpotlight().transform.rotation;
        //These two variables are never updated during pursuit state
        currSpeed = helicopter.speed * helicopter.frenzySpeedBonus;
		spotlightRotationSpeed = helicopter.spotlightRotationSpeedChase;

		//Updates to be ready for the first frame
		updateState (deltaTime);
	}


	public HelicopterState updateState (float deltaTime) {
       
        if (goBackToPursuit) {
			return comingFrom;
		}

        updateFlippedThisFrame();
        updateTargetPosition();
		updateTargetSpotlightRotation (deltaTime);
        
		return this;
	}

   
	void updateTargetPosition() {
        if (helicopter.transform.position.x == targetPosition.x)
        {
            Vector3 dir = rootPos - helicopter.transform.position;
            targetPosition.x += 2 * dir.x;
         
        }
    }

    void updateFlippedThisFrame()
    {
        if (flippedThisFrame)
        {
            flippedThisFrame = false;
        }
        if ((!helicopter.getMovingRight() && targetPosition.x > helicopter.transform.position.x)
            || (helicopter.getMovingRight() && targetPosition.x < helicopter.transform.position.x))
        {
            flippedThisFrame = true;
        }
    }


    void updateTargetSpotlightRotation(float deltaTime) {
        Vector3 spotPos = helicopter.getSpotlight().transform.position;
        if (helicopter.getSpotlight().transform.rotation == targetSpotlightRotation)
        { 
            //build new target position for helicopter to rotate to

            targetSpotPos = Vector3.zero;
            targetSpotPos.x = frenzy.getLastKnownPlayerPosition().x;
            targetSpotPos.x += helicopter.xOffsetLightSearch.nextValue(deltaTime);
            targetSpotPos.y = frenzy.getLastKnownPlayerPosition().y;

           
        }
        //Vector representing drawing directions to get from spotlight to player
        Vector3 hypotenuse = targetSpotPos - spotPos;
        hypotenuse.z = 0;
        //ratio to calculate angle through atan
        float ratio = -1 * hypotenuse.x / hypotenuse.y;
        //Offset from vertically down that the spotlight should point
        float radOffset = Mathf.Atan(ratio);
        //converted
        float degOffset = Mathf.Rad2Deg * radOffset;
        //The spotlight rotation should be quaternion representation of that offset
        targetSpotlightRotation = Quaternion.Euler(0, 0, degOffset);
    }

    public void playerSeen() {
		goBackToPursuit = true;
	}



	/**
	 * The getters that we need for each HelicopterState
	 */
	public Vector3 getTargetPosition() {return targetPosition;}
	public float getCurrSpeed() {return currSpeed;}
	public Quaternion getTargetSpotlightRotation() {return targetSpotlightRotation;}
	public float getSpotlightRotationSpeed() {return spotlightRotationSpeed;}
	public bool newDirection() {return flippedThisFrame;}
}
