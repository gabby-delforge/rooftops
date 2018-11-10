using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursuitState : HelicopterState {

	//The helicopter that currently has this state
	private HeliScript helicopter;

	//Necessary to restore correct behavior on change of state
	private HelicopterState cameFrom;

	//These variables are used locally and necessary for correct behavior of PursuitState
	private FrenzyMode frenzy;
	private GameObject player;
	private float heightFromPlayer;
	private float deadZoneForModelFlip = 0.1f;
	private float timeSincePlayerSeen;

	//These are the five variables needed for PursuitState to be a valid HelicopterState
	private Vector3 targetPosition;
	private Quaternion targetSpotlightRotation;
	private float currSpeed;
	private float spotlightRotationSpeed;
	private bool flippedThisFrame;

	//rootState needs to be the staet that the helicopter will return to after the player escapes
	public PursuitState(HeliScript helicopter, float deltaTime, HelicopterState rootState) {
		//Initializing default, independent vals for behavioral variables
		this.helicopter = helicopter;
		this.cameFrom = rootState;
		heightFromPlayer = helicopter.vertDistanceFromPlayerInChase;
		timeSincePlayerSeen = 0f;

		//Getting the actual player we're chasing down
		player = GameObject.Find("Player");

		//Interacting with the frenzy controller
		frenzy = GameObject.Find ("FrenzyController").GetComponent<FrenzyMode>();
		frenzy.canSeePlayer(this.helicopter);

		//These two variables are never updated during pursuit state
		currSpeed = helicopter.speed * helicopter.frenzySpeedBonus;
		spotlightRotationSpeed = helicopter.spotlightRotationSpeedChase;

		//Updates to be ready for the first frame
		updateState (deltaTime);
	}
		
	

	public HelicopterState updateState (float deltaTime) {
		timeSincePlayerSeen += deltaTime;
        Debug.Log(timeSincePlayerSeen);
		if (timeSincePlayerSeen > helicopter.timeToHideLocation) {
			return new SearchInFrenzyState (helicopter, deltaTime, this, cameFrom);
		}


		updateTargetPosition ();
		updateFlippedThisFrame ();
		updateTargetSpotlightRotation (deltaTime);
		return this;
	}
		
	void updateTargetPosition() {
        //During pursuit, we should be moving towards the player
        targetPosition = frenzy.getLastKnownPlayerPosition();
		//but we should be trying to fly heightFromPlayer above the player
		targetPosition.y += heightFromPlayer; 
	}

	void updateFlippedThisFrame() {
		if (flippedThisFrame) {
			flippedThisFrame = false;
		}
		if (((!helicopter.getMovingRight() && targetPosition.x > helicopter.transform.position.x)
			|| (helicopter.getMovingRight() && targetPosition.x < helicopter.transform.position.x))
			&& (Mathf.Abs(helicopter.transform.position.x - targetPosition.x) > deadZoneForModelFlip)){
			flippedThisFrame = true;
		}
	}

	void updateTargetSpotlightRotation(float deltaTime) {
		//Absolute position of spotlight
		Vector3 spotPos = helicopter.getSpotlight().transform.position;
		//Abs pos of player
		Vector3 playerPos = player.transform.position;
		Vector3 xOffset = Vector3.right * helicopter.xOffsetLightChase.nextValue (deltaTime);
		Vector3 targetLocation = playerPos + xOffset;



		//Vector representing drawing directions to get from spotlight to player
		Vector3 hypotenuse = targetLocation - spotPos;
		hypotenuse.z = 0;
		//ratio to calculate angle through atan
		float ratio = -1 * hypotenuse.x/hypotenuse.y;
		//Offset from vertically down that the spotlight should point
		float radOffset = Mathf.Atan(ratio);
		//converted
		float degOffset = Mathf.Rad2Deg * radOffset;
		//The spotlight rotation should be quaternion representation of that offset
		targetSpotlightRotation = Quaternion.Euler (0, 0, degOffset);

	}


	//Called by heliscript every frame that the player is in helicopter
	public void playerSeen() {
		timeSincePlayerSeen = 0f;
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
