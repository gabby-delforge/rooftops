using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : HelicopterState {
	//The helicopter that currently has this state
	private HeliScript helicopter;

	//Variables needed for calculating behavior in IdleState
	private Vector3 rootPosition;
	private float traversalDistance;
	private float horizontalSpeed;

	//These are the six variables needed for IdleState to be a valid HelicopterState
	private Vector3 targetPosition;
	private Quaternion targetSpotlightRotation;
	private float currSpeed;
	private float spotlightRotationSpeed;
	private bool flippedThisFrame;
	private bool justSawPlayer;

	//Constructor run when creating IdleState
	public IdleState(HeliScript helicopter, float deltaTime) {
		this.helicopter = helicopter;
		flippedThisFrame = false;
		horizontalSpeed = helicopter.speed;
		targetSpotlightRotation = helicopter.getSpotlight().transform.rotation;
		spotlightRotationSpeed = 0f;
		rootPosition = helicopter.transform.position;
		traversalDistance = helicopter.idleRoamingDistanceCovered;
		justSawPlayer = false;
		updateState (deltaTime);

	}

	/**
	 * This is called once per frame and updates the variables necessary for its state
	 * It also checks to see if a new state is required, and if so, returns that new state
	 */
	public HelicopterState updateState(float deltaTime) {
		if (justSawPlayer) {
			return new PursuitState (helicopter, deltaTime, this);
		}
		//Logic for checking whether there should be a new state
		if ((helicopter.getMovingRight() && helicopter.transform.position.x - rootPosition.x > traversalDistance)
			|| (!helicopter.getMovingRight() && rootPosition.x - helicopter.transform.position.x > traversalDistance)) {

			return new FlipState (helicopter, deltaTime, this);
		}
			
		//Update the values for each important field
		//targetSpotlightRotation and spotlightRotationSpeed never change
		targetPosition = calculateTargetPosition (deltaTime);
		currSpeed = calculateNewSpeed (deltaTime);

		//We haven't changed from idle due to any factors we can calculate within this state,
		//So we return the current state to be called again next frame
		return this;

	}

	public float calculateNewSpeed(float deltaTime) {
		//Must scale by 1/dt because position is scaled by dt
		return Vector3.Distance (helicopter.transform.position, targetPosition) / deltaTime;
	}

	public void playerSeen() {
		justSawPlayer = true;

	}

	/**
	 * Helper function that is called by updateState each frame to calculate the next location it should be given
	 */
	Vector3 calculateTargetPosition(float deltaTime) {
		//This updates the horizontal component of the target position
		Vector3 newTargetPosition = helicopter.transform.position;
		newTargetPosition.x += (helicopter.getMovingRight()) ? horizontalSpeed * deltaTime : -horizontalSpeed * deltaTime;


		//This updates the vertical component of the target position
		newTargetPosition.y = rootPosition.y + helicopter.yOffsetMovement.nextValue(deltaTime);


		return newTargetPosition;
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
