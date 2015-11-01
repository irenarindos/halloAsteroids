using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;
using System.Xml;
using System.IO;

struct ShipType{
	string shipName;
	bool enabled;
	int levelUnlocked;
	float speed; 
	float rotationSpeed;
	string textureName;
	string weaponName;

	public void init(string n, bool e, int l, float s, float rs, string t, string sh){
		shipName = n;
		enabled = e;
		levelUnlocked= l;
		speed = s;
		rotationSpeed = rs;
		textureName = t;
		weaponName = sh;
	}
}

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

	private bool dead;
	private float deathTime;
	private bool invincible;
	private float invincibleStart;
	private float invinciblePeriod = .5f;

	//Scorekeeping
	private static int points;
	private static int pointsToExtraLife = 100000;
	private static int extraLivesEarned;

	private WeaponsSystem weaponsSystem;

	void Start(){
		controller = GetComponent<CharacterController>();
		setBounds();
		init ();
		weaponsSystem = gameObject.AddComponent<WeaponsSystem>();
		weaponsSystem.init (projectile);	}

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

		if(Input.GetButtonUp("Shoot")){
			weaponsSystem.shoot(projectile, transform);
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
		infoUI.text = "Level: "+ LevelManager.getLevel().ToString ()+ "\t Lives: "+ numLives.ToString();
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
