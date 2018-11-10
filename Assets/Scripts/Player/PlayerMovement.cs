using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {

	public Animator anim;
    public LayerMask groundLayer;
    public int maxJumps;

	private GameScript gameController;
	private GameObject buildingSpawner;
	private BuildingSpawner spawnScript;
	private FrenzyMode frenzyController;
	private Vector3 start;

    //Physics
    private float speed;
	private float crouchConstant;
	private bool crouching;
    private bool rolling;
	private float runTime;
	private bool jumping;
	private bool grounded;
	public float maxSpeed;
	public float acceleration;
	public float decceleration;
	private Vector3 lastPosition;
	private int hitCount;
	public float jumpThrust;
    public int jumpCount;

	//Death
	private bool dead;
	private Image b_img;
	Rigidbody2D rb;
	Transform trans;
	private float distTraveled;
	private bool resetCheckpoint;


	// Use this for initialization
	void Start () {
		trans = GetComponent<Transform>();
		anim = transform.GetChild (0).GetComponent<Animator>();
		gameController = GameObject.Find ("GameController").GetComponent<GameScript>();
		buildingSpawner = GameObject.Find ("BuildingSpawner");
		spawnScript = buildingSpawner.GetComponent<BuildingSpawner> ();
		frenzyController = GameObject.Find ("FrenzyController").GetComponent<FrenzyMode>();
		b_img = GameObject.Find("BlackFade").GetComponent<Image>();
		rb = GetComponent<Rigidbody2D>();
		start = transform.position;
		speed = 0;
		runTime = 0;
        jumpCount = 0;
		jumping = false;
        rolling = false;
		grounded = true;
		dead = false;
		hitCount = 0;
		lastPosition = this.transform.position;
			
		anim.SetFloat("xSpeed", speed);
	}
	void flip(){
		trans.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
	}

    bool IsGrounded()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;
        float distance = 0.7f;
        Debug.DrawRay(position, direction, Color.green);
        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
        if (hit.collider != null)
        {
            return true;
        }

        return false;
    }

	void OnCollisionEnter2D(Collision2D col){
		float smallest = float.MaxValue;
		float largest = float.MinValue;
		lastPosition = this.transform.position;
		foreach (ContactPoint2D contact in col.contacts) {
			if (contact.point.x < smallest) {
				smallest = contact.point.x;
			}
			if (contact.point.x > largest) {
				largest = contact.point.x;
			}
		}
		if (speed > 0) {
			if (smallest - lastPosition.x > 0) {
				speed = Mathf.Min(0.5f, speed);
				hitCount = 3;
			}
		} else if (speed < 0) {
			if (largest - this.transform.position.x < 0) {
				speed = Mathf.Max(-0.5f, speed);
				hitCount = 3;
			}
		}
	}

	//Bird trigger
	void OnTriggerEnter2D(Collider2D col) {
		string interactingWith = col.gameObject.tag;
		switch (interactingWith) {
			case "Bird":
				col.gameObject.GetComponent<BirdScript> ().startFlying ();
				break;
			default:
				break;
		}

	}
	bool isRunning(){
		return Mathf.Abs(speed) >= 2;
	}
	bool isCrouching(){
		return Input.GetAxis("Vertical") < 0;
	}
	bool isSkidding(){
		return Input.GetAxis("Horizontal") == 0 && Mathf.Abs(speed) > maxSpeed/4;
	}
	public void gameOver(){
		transform.SetPositionAndRotation(start, transform.rotation);
		dead = false;
		gameController.setScore(0);
		StartCoroutine ("Respawn");

	}

	IEnumerator Death() {
		yield return new WaitForSeconds(1.0f);
		frenzyController.deactivate();
		gameOver();
	}

	IEnumerator Respawn() {
		yield return new WaitForSeconds(1.0f);
		var tempColor = b_img.color;
		tempColor.a = 0f;
		b_img.color = tempColor;
	}
		

	public void fade(Image img) {
		var tempColor = img.color;
		tempColor.a += 3.0e-02F;
		img.color = tempColor;
	}

	public void fallDeath() {
		if (b_img.color.a < 1.0f) {
			fade (b_img);
		} else {
			StartCoroutine("Death");
			gameOver();

		}
	}

	public void setSpawn(GameObject checkpoint) {
		start = checkpoint.transform.position;
	}

    // Update is called once per frame
    void FixedUpdate() {

        if (dead) {
            fallDeath();
        }

        float buildingPos = spawnScript.lastY;
        if (transform.position.y < buildingPos - 10) {
            dead = true;
        }

        crouchConstant = isCrouching() ? 0.5f : 1f;

        grounded = IsGrounded() ? true : false;


        float moveAxis = Input.GetAxis("Horizontal");
        if (moveAxis != 0) {
            if (moveAxis < 0 && trans.localScale.x > 0 || moveAxis > 0 && trans.localScale.x < 0) {
                flip();
            }
            runTime = Mathf.Clamp(runTime + Time.fixedDeltaTime, 0, 2);
            if ((speed > 0 && moveAxis < 0) || (speed < 0 && moveAxis > 0)) {
                speed = speed > 0 ? speed - decceleration * Time.fixedDeltaTime : speed + decceleration * Time.fixedDeltaTime;
            } else {
                speed += acceleration * Time.fixedDeltaTime * moveAxis;
            }
            speed = Mathf.Clamp(speed, -maxSpeed * crouchConstant, maxSpeed * crouchConstant);

        } else {
            runTime = 0;
            if (Mathf.Abs(speed) > .5f) {
                speed = speed > 0 ? speed - decceleration * Time.fixedDeltaTime : speed + decceleration * Time.fixedDeltaTime;
            } else {
                speed = 0;
            }
        }

        if (Input.GetAxis("Vertical") < 0)
        {
            if (speed >= maxSpeed * (1.0f / 2.0f))
            {
                anim.SetTrigger("roll");
                rolling = true;
                crouching = false;
            }
            else
            {
                crouching = true;
            }
        }
        else
        {
            crouching = false;
        }

        Vector2 newVelocity = rb.velocity;
        if (hitCount > 0) {
            hitCount -= 1;
            if (hitCount == 0 && Mathf.Abs(lastPosition.x - this.transform.position.x) < 0.1f && Mathf.Abs(lastPosition.y - this.transform.position.y) < 0.1f) {
                speed = 0;
            }
            lastPosition = this.transform.position;
        }

        newVelocity.x = rolling ? speed : speed * 2.0f;
        rb.velocity = newVelocity;


        if (IsGrounded() && Input.GetButtonDown("Jump") && !crouching) {
            rb.AddForce(new Vector3(0, Input.GetAxis("Jump") * jumpThrust));
            jumping = true;
            jumpCount += 1;
        } else if (Input.GetButtonDown("Jump") && jumpCount < maxJumps) {
            Vector2 vel = rb.velocity;
            vel.y = 0;
            rb.velocity = vel;
            rb.AddForce(new Vector3(0, Input.GetAxis("Jump") * jumpThrust));
            jumping = true;
            jumpCount += 1;
        } else {
			jumping = false;
            jumpCount = 0;
		}

		
		anim.SetBool("crouching", crouching);
		anim.SetBool("grounded", IsGrounded());
		anim.SetBool("jumping", jumping);
		anim.SetBool("skidding", isSkidding ());
		anim.SetBool("running", isRunning());
		anim.SetFloat("xSpeed", Mathf.Abs(speed));
		anim.SetFloat("ySpeed", Mathf.Abs(rb.velocity.y));
		anim.SetFloat("yVel", rb.velocity.y);
	}
}
