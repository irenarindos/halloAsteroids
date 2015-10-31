using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Ship : Wrappable {

	public static bool restarting = false;

	public Rigidbody projectile;
	public Text infoUI;
	public Text scoreUI;
	public Text gameOver;

	private static int numLives;
	private  CharacterController controller;
	private float rotationSpeed = 200f;
	private float speed = 10f;
	private float projectileSpeed = 500f;
	private bool dead;
	private bool invincible;
	private float invincibleStart;
	private float invinciblePeriod = .5f;
	private float deathTime;

	//Scorekeeping
	private static int points;
	private static int pointsToExtraLife = 100000;
	private static int extraLivesEarned;

	void Start(){
		controller = GetComponent<CharacterController>();
		setBounds();
		init ();
	}

	void init(){
		numLives = 3;
		dead = false;
		invincible = false;
		gameOver.enabled = false;
		points = 0;
		extraLivesEarned = 0;
		updateUI ();
	}

	void Update() {
		updateUI();
		if (dead) {
			//Wait a second so the player can react to "Game Over"
			if(Time.time- deathTime < 1f)
				return;

			if (Input.GetButtonUp ("Shoot")) {
				init ();
				restarting = true;
			}
			return;
		}
		if (invincible) {
			if((Time.time - invincibleStart) >invinciblePeriod )
				invincible = false;
		}
		Vector3 velocity = Vector3.zero;

		//Rotate around x-axis
		transform.Rotate(Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime, 0.0f, 0.0f);

		//Calculate velocity along local ship's y-axis
		velocity = transform.up * Input.GetAxis("Vertical") * speed;
		controller.Move (velocity * Time.deltaTime);   
		checkBounds ();

		if(Input.GetButtonUp("Shoot"))
		{
			Rigidbody shot = Instantiate(projectile, transform.position+transform.up, transform.rotation) as Rigidbody;
			shot.AddForce(transform.up * projectileSpeed);
		}
	}

	//Deal with asteroid and ship collisions
	void OnTriggerEnter(Collider other) {
		if (invincible)
			return;

		if (other.gameObject.tag == "asteroid" ){//|| other.gameObject.tag == "shot") {
			numLives--;
			respawn();

			if(numLives <= 0){
				endGame();
				numLives = 0;
			}
			updateUI();
		}
	}

	//Place player at center of screen & make invincible for a bit
	void respawn(){
		this.transform.position = new Vector3(0f,0f,1f) ;
		invincible = true;
		invincibleStart = Time.time;
	}

	void updateUI(){
		infoUI.text = "Level: "+ AsteroidManager.getLevel().ToString ()+ "\t Lives: "+ numLives.ToString();
		scoreUI.text = "Score: " + points.ToString ();
	}

	void endGame(){
		gameOver.enabled = true;
		dead = true;
		deathTime = Time.time;
	}	

	public static void addPoints(int pointVal){
		points+= pointVal;
		//Check if player earned an extra life
		if(points > (pointsToExtraLife* (extraLivesEarned+1))){
			numLives++;
			extraLivesEarned++;
		}
	}
}
