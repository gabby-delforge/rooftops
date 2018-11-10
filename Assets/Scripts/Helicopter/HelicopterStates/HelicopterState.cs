using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface HelicopterState{
	HelicopterState updateState (float deltaTime);
	Vector3 getTargetPosition();
	float getCurrSpeed();
	Quaternion getTargetSpotlightRotation();
	float getSpotlightRotationSpeed();
	bool newDirection ();
	void playerSeen ();
}

