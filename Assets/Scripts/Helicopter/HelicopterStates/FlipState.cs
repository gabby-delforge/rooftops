using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipState : HelicopterState {
	//The helicopter that currently has this state
	private HeliScript helicopter;

	//These variables are used locally and necessary for correct behavior of FlipState
	private float timeIntoFlip;
	private float lengthOfFlipInSeconds;
	private float halfwayPoint;
	private bool haveFlipped;
	private Quaternion spotlightRotationBeforeFlip;
	private HelicopterState toReturnTo;
	private float initialSpeed;
	private Vector3 rootPosition;

	//These are the six variables needed for FlipState to be a valid HelicopterState
	private Vector3 targetPosition;
	private Quaternion targetSpotlightRotation;
	private float currSpeed;
	private float spotlightRotationSpeed;
	private bool flippedThisFrame;
	private bool justSawPlayer;


	/**
	 * The default constructor for FlipState.
	 * This makes the assumption that we can finish this FlipState with 
	 * the position of the helicopter after the flip as the root, and then
	 * begin the movement pattern from there (starting sine wave at t=0).
	 * Because of this, this shouldn't be called by IdleState, as it can
	 * result in the helicopter repeatedly changing it's anchor height after
	 * every flip, over time making the helicopter descend or ascend significantly
	 */
	public FlipState(HeliScript helicopter, float deltaTime, HelicopterState toReturnTo) {
		//Initializing default, independent vals for behavioral variables
		this.helicopter = helicopter;
		timeIntoFlip = 0f;
		haveFlipped = false;
		flippedThisFrame = false;
		rootPosition = toReturnTo.getTargetPosition();
		rootPosition.y -= helicopter.yOffsetMovement.indexedValue (); //since we adjust for bob, remove from root position
		this.toReturnTo = toReturnTo;
		justSawPlayer = false;
		initialSpeed = (helicopter.getMovingRight ()) ? helicopter.speed : -helicopter.speed;

		//initalize debug vars
		//currSpeedDebug = new List<float>();
		//targetPositionDebug = new List<float> ();

		//Shortcuts for accessing frequent variables without reaching back into helicopter each time
		lengthOfFlipInSeconds = helicopter.flipTime;
		halfwayPoint = lengthOfFlipInSeconds / 2;
		spotlightRotationBeforeFlip = helicopter.getSpotlight().transform.rotation;

		//Only need to calculate new spotlight rotation once, as it is simply the negated spotlight's rotation along the z axis
		targetSpotlightRotation = calculateNewSpotlightRotation ();

		//Similarly, only need to calculate spotlight rotation speed once, as it's just the value that is scaled by time 
		//to allow the spotlight to rotate smoothly and end exactly on the rotation point at helicopter.flipTime seconds
		spotlightRotationSpeed = calculateSpotlightRotationSpeed(targetSpotlightRotation.eulerAngles.z);
		//Must call this once during the constructor so that we have values initiated for the first frame of it's instantiation
		updateState (deltaTime);

	}
		

	/**
	 * Called once per frame when this state is active
	 * The five relevant state variables:
	 * currSpeed 								changes every frame
	 * targetPosition 							changes every frame
	 * flippedThisFrame (newDirection() r.v.) 	is true for one frame
	 * spotlightRotationSpeed 					never changes
	 * targetSpotlightRotation					never changes
	 */
	public HelicopterState updateState (float deltaTime) {
		//Begin by accounting for the time that's passed since the last frame
		timeIntoFlip += deltaTime;
		//If we just flipped the model last frame, we need to change this so it won't flip it back
		if (haveFlipped && flippedThisFrame) {
			flippedThisFrame = false;
		}
		if (justSawPlayer) {
			return new PursuitState (helicopter, deltaTime, new IdleState(helicopter, deltaTime));
		}
		//If this is the case, the flip is done
		if (timeIntoFlip > lengthOfFlipInSeconds) {
			return toReturnTo;
		} 

		//This flips the model at the halfway point
		if (!haveFlipped && timeIntoFlip > halfwayPoint) {
			haveFlipped = true;
			flippedThisFrame = true;
		}

		//updating target position depends on current speed, so must calculate currSpeed first
		updateTargetPosition (deltaTime);
		updateCurrSpeed (deltaTime);


		//We haven't changed from idle due to any factors we can calculate within this state,
		//So we return the current state to be called again next frame
		return this;
	}

	public void playerSeen() {
		justSawPlayer = true;
	}
		
	/**
	 * Technically speaking, the intended behavior is to continue idling but just slow until turned around, 
	 * but, so long as helicopter.flipTime is relatively small (t < 2.5s) the abrupt end of vertical movement
	 * isn't all that noticable, and this is much more workable
	 */
	void updateTargetPosition(float deltaTime) {
		targetPosition.x = rootPosition.x + (-2 * initialSpeed / lengthOfFlipInSeconds) * (timeIntoFlip * timeIntoFlip) + (2 * initialSpeed * timeIntoFlip);
		targetPosition.y = rootPosition.y + helicopter.yOffsetMovement.nextValue (deltaTime);
	}


	public void updateCurrSpeed(float deltaTime) {
		//Must scale by 1/dt because position is scaled by dt
		currSpeed =  Vector3.Distance (helicopter.transform.position, targetPosition) / deltaTime;
	}

	/**
	 * Explanation for what this does:
	 * The initial euler angle before the flip is always given as it's positive representation
	 * option1 is the initial angle negated 	(ex. angle 318 -> -318)
	 * option 2 is 360 - initial angle 			(ex. angle 318 -> 42)
	 * These both represent the same thing (-318 = 42)
	 * Since these are symmetrical across z=0 axis, the rotation needed is double of each option
	 * option1 represents rotation from 318 -> -318, so -636 degrees
	 * option2 represents rotation from 318 -> 42, so 84 degrees
	 * 
	 * We need to take the shorter rotation, as one will always be less than 
	 * 180 degrees of rotation, and the other will always be greater.
	 * 
	 */ 
	Quaternion calculateNewSpotlightRotation() {
		float option1 = -spotlightRotationBeforeFlip.eulerAngles.z;
		float option2 = 360 + option1;
		Quaternion rv = Quaternion.Euler (0, 0,(Mathf.Abs(option1) < 180) ? option1 : option2);
		return rv;
	}

	/**
	 * Since this value is scaled by Time.deltaTime in HeliScript, this function gives us the desired behavior
	 * When the sum of all deltaTimes == lengthOfFlipInSeconds, 
	 * we've moved 2 * z * (lengthOfFlipInSeconds/lengthOfFlipInSeconds) =  2 *z * 1 = 2z total degrees.
	 * This is exactly how much we need to move as long as 
	 * z=targetSpotlightRotation.eulerAngles.z and we choose representation s.t. -180 <= z <= 180
	 * 
	 * We must add in a very small constant value to this, as during the very last frame of rotation, we switch to idle state,
	 * and the rotation speed is then given by the idle state, so we want to finish the rotation slightly before the state changes.
	 * Our implementation of rotation will never overestimate, so we can safely have a greater speed than necessary
	 */
	float calculateSpotlightRotationSpeed(float z) {
		float correctiveOffset = 1f;
		z = (z > 180) ? 360 - z : z;
		return  2 * z / lengthOfFlipInSeconds + correctiveOffset;
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
