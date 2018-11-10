using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliScript : MonoBehaviour {
	/**
	 * Public variables for modification in the inspector
	 */
	//Globally useful variables
	[Header("State-impartial variables")]
	[Tooltip("Baseline speed, the maximum horizontal distance the helicopter will cover per second during idle.")]
	public float speed;

	//Variables for HeliScript
	[Header("Detection variables")]
	[Tooltip("Time that the spotlight must illuminate the player before it counts as him being seen")]
	public float timeInLightToBeNoticed;
	[Tooltip("Time that the player has to be out of the light to reset the timeInLight counter to 0")]
	public float timeToCountAsOutOfLight;

	//Variables for IdleState
	[Header("IdleState variables")]
	[Tooltip("How far the helicopter travels horizontally during idle before turning around")]
	public float idleRoamingDistanceCovered;
	[Tooltip("How long, in seconds, it takes for the helicopter to fully loop through the primary sine wave of vertical movement")]
	[SerializeField]
	private float primaryPeriod;
	[Tooltip("The maximum vertical offset from its starting position, both positive and negative")]
	[SerializeField]
	private float waveformMagnitude;
	[Tooltip("How many times per period of primary sine wave that the fluctuation sine wave is composed into it")]
	[SerializeField]
	private float fluctuationFrequency;
	[Tooltip("How much the fluctuation of the secondary sine wave affects the primary one.")]
	[SerializeField]
	private float fluctuationIntensity;

	//Variables for FlipState
	[Header("FlipState variables")]
	[Tooltip("Time it takes in seconds for helicopter to turn around")]
	public float flipTime;

	//Variables for PursuitState
	[Header("Frenzy variables")]
	[Tooltip("How much we scale the helicopter's maximum speed by when it is chasing the player")]
	public float frenzySpeedBonus;
	[Tooltip("Lowest height that the helicopter will go when chasing player")]
	public float vertDistanceFromPlayerInChase;
	[Tooltip("Speed the light turns towards player during chase")]
	public float spotlightRotationSpeedChase;
	[Tooltip("The time necessary for the player to be out of the spotlight for the helicopter to no longer know his position")]
	public float timeToHideLocation;
	[Tooltip("How many seconds it takes for the light to complete one oscillation around the player while chasing him")]
	[SerializeField]
	private float oscillationTime;
	[Tooltip("How much the position of the spotlight fluctuates when chasing the player")]
	[SerializeField]
	private float oscillationIntensity;

	/**
	 * Variables not exposed to the inspector
	 */
	//State Variables
	private HelicopterState state;
	public SinusoidalHandler.Sinusoidal yOffsetMovement;
	public SinusoidalHandler.Sinusoidal xOffsetLightChase;
    public SinusoidalHandler.Sinusoidal xOffsetLightSearch;

	//Movement Variables
	private bool movingRight = false;

	//Graphical variables
	private SpriteRenderer sr;

	//Various behavioral variables
	private float timeInLight;
	private float timeSinceLight;
	private bool seenThisFrame;
    private FrenzyMode frenzy;

	//Variables for controlling the behavior of the spotlight
	private GameObject spotlight;

	void Start() { 
        frenzy = GameObject.Find("FrenzyController").GetComponent<FrenzyMode>();

        //Gets important components for behavior
        sr = GetComponent<SpriteRenderer> ();
		spotlight = GameObject.Find ("2DLight");
		initializeSinusoidals ();


		//begins helicopter behavior
		state = new IdleState (this, Time.deltaTime);
    }
		
	void FixedUpdate () {
		updateState ();
		updateSprite ();
		checkForPlayer ();
	}


	/**
	 * Each frame, the current state must provide five things:
	 * 
	 * 	Vector3 targetPosition = the position in the world that the helicopter should fly to
	 * 	float currSpeed = the speed that the helicopter should move towards that position
	 * 	Quaternion desiredSpotlightRotation = the Quaternion representation of the rotation that the spotlight should rotate towards
	 * 	float spotlightMoveSpeed = the speed that the spotlight should rotate towards that rotation
	 * 	bool newDirection = For this frame, newDirection = 1 if movingRight should be changed, 0 if it should stay the same.
	 */
	private void updateState() {
		state = state.updateState (Time.deltaTime);
		movingRight = (state.newDirection()) ? !movingRight : movingRight;
		transform.position = Vector3.MoveTowards(transform.position, state.getTargetPosition(), state.getCurrSpeed() * Time.deltaTime);
		spotlight.transform.rotation = Quaternion.RotateTowards (spotlight.transform.rotation, state.getTargetSpotlightRotation(), state.getSpotlightRotationSpeed() * Time.deltaTime);
	}

	/**
	 * Adjusts the sprite as needed after the state has been updated
	 */
	private void updateSprite() {
		//Makes the sprite face the correct direction
		sr.flipX = movingRight;
	}

	/**
	 * Two parts:
	 * 	1) Adjust variables based on whether the player is currently standing in the light
	 * 	2) Check to see if these new adjustments call for taking any action
	 */
	private void checkForPlayer() {
		if (seenThisFrame) {
			timeSinceLight = 0f;
			timeInLight += Time.deltaTime;
			seenThisFrame = false;
		} else {
			timeSinceLight += Time.deltaTime;
		}
        if (timeSinceLight > timeToCountAsOutOfLight)
        {
            frenzy.cannotSeePlayer(this);
            timeSinceLight = timeToCountAsOutOfLight;
            // This ^^ was only done to keep from incrementing the value forever
            timeInLight = 0f;
        } else if (timeInLight > timeInLightToBeNoticed) {
            frenzy.canSeePlayer(this);
			state.playerSeen ();
		}  
	}

	/**
	 * This is called when the raycast of the lighting hits the player
	 */
	public void foundPlayer() {
		seenThisFrame = true;
	}
		

	/**
	 * Don't use this unless it's an emergency!
	 * State behavior should *always* be handled by the states themselves
	 * The only time this is called is when the player dies.
	 * Messing with the states through HeliScript breaks 
	 * the discretized finite state machine logic that controls its AI
	 */
	public void resetState() {
		state = new IdleState (this, Time.deltaTime);
	}

    public void findPlayer()
    {
        state = new SearchInFrenzyState(this, 0.1f, new PursuitState(this, 0.1f, state), state);
    }
		
	private void initializeSinusoidals() {
		//Idle State sinusoidal
		float B1 = 2 * Mathf.PI / primaryPeriod;
		SinusoidalHandler.Sinusoidal primary = SinusoidalHandler.create (waveformMagnitude, B1, 0f);
		float B2 = fluctuationFrequency * B1;
		SinusoidalHandler.Sinusoidal secondary = SinusoidalHandler.create (fluctuationIntensity, B2, 0f);
		yOffsetMovement = SinusoidalHandler.compose (primary, secondary);

		//Pursuit State sinusoidal
		xOffsetLightChase = SinusoidalHandler.create(oscillationIntensity, 2 * Mathf.PI / oscillationTime, 0f);

        xOffsetLightSearch = SinusoidalHandler.create(oscillationIntensity * 5, 2 * Mathf.PI / 8, 0f);
	}

	/**
	 * Getters and Setters
	 */
	public bool getMovingRight() {
		return movingRight;
	}
	public GameObject getSpotlight() {
		return spotlight;
	}




}
