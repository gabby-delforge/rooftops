using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToStateState : HelicopterState {
	//The helicopter that currently has this state
	private HeliScript helicopter;
	private HelicopterState toDoFirst;
	private HelicopterState nextState;

	//These are the variables needed for ReturnToTransformState to be a valid HelicopterState
	private Vector3 targetPosition;
	private Quaternion targetSpotlightRotation;
	private float currSpeed;
	private float spotlightRotationSpeed;

	//Constructor run when creating IdleState
	public ReturnToStateState(HeliScript helicopter, float deltaTime, HelicopterState toReturnTo) {
		this.helicopter = helicopter;
		nextState = toReturnTo;
		targetPosition = nextState.getTargetPosition ();
		spotlightRotationSpeed = nextState.getSpotlightRotationSpeed ();
		targetSpotlightRotation = nextState.getTargetSpotlightRotation ();
		currSpeed = helicopter.speed;

		if ((helicopter.getMovingRight () && toReturnTo.getTargetPosition().x < helicopter.transform.position.x)
			|| (!helicopter.getMovingRight () && toReturnTo.getTargetPosition().x > helicopter.transform.position.x)) {
			//We gotta turn around first
			toDoFirst = new FlipState (helicopter, deltaTime, this);
		}

		updateState(deltaTime);
	}

	/**
	 * Checks to see if a new state is required, and if so, returns that new state
	 */
	public HelicopterState updateState(float deltaTime) {
		//Logic for checking whether there should be a new state
		if (toDoFirst != null) {
			HelicopterState temp = toDoFirst;
			toDoFirst = null;
			return temp;
		}
		if (helicopter.transform.position.Equals (targetPosition)) {
			return nextState;
		}
		return this;
	}

	public void playerSeen() {
		//Do nothing
	}


	/**
	 * The getters that we need for each HelicopterState
	 */
	public Vector3 getTargetPosition() {return targetPosition;}
	public float getCurrSpeed() {return currSpeed;}
	public Quaternion getTargetSpotlightRotation() {return targetSpotlightRotation;}
	public float getSpotlightRotationSpeed() {return spotlightRotationSpeed;}
	public bool newDirection() {return false;}

}
