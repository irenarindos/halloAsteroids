using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;
using System.Xml;
using System.IO;

public class Ship : Wrappable {

	public static bool restarting = false;

	public Rigidbody projectile;
	public Text infoUI;
	public Text scoreUI;
	public Text gameOver;
	public Text shipUpgrade;

	private static int numLives;
	private  CharacterController controller;
	private float rotationSpeed = 200f;
	private float speed = 10f;

	private bool dead;
	private float deathTime;
	private bool invincible;
	private float invincibleStart;
	private float invinciblePeriod = .5f;
	private bool shipUnlocked;
	private float unlockTime;

	//Scorekeeping
	private static int points;
	private static int pointsToExtraLife = 100000;
	private static int extraLivesEarned;

	private static WeaponsSystem weaponsSystem;
	private static ShipType shipType;

	void Start(){
		controller = GetComponent<CharacterController>();
		setBounds();
		init ();
		weaponsSystem = gameObject.AddComponent<WeaponsSystem>();
	}

	void init(){
		numLives = 3;
		dead = false;
		invincible = false;
		shipUnlocked = false;
		shipUpgrade.enabled = false;
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

		if (other.gameObject.tag == "asteroid" ){
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
		if (shipUnlocked) {
			if(Time.time - unlockTime > 2.5f){
				shipUnlocked= false;
				shipUpgrade.enabled= false;
				return;
			}
		}
	}

	//Register game over and set timer for inactivity
	void endGame(){
		gameOver.enabled = true;
		dead = true;
		deathTime = Time.time;
	}	

	public void addPoints(int pointVal){
		points+= pointVal;
		//Check if player earned an extra life
		if(points > (pointsToExtraLife* (extraLivesEarned+1))){
			numLives++;
			extraLivesEarned++;
		}
	}

	public void upgradeShip(ShipType st){
		shipType = st;
		weaponsSystem.setActiveWeapon (st.weapon);
		Texture text = Resources.Load(st.textureName) as Texture;
		Renderer rend = GetComponent<Renderer>();
		rend.material.mainTexture = text;
		shipUnlocked = true;
		unlockTime = Time.time;
		shipUpgrade.text = st.shipName + " unlocked!";
		shipUpgrade.enabled = true;
	}
}


/*
 * Ship type indicates name of ship type, the level it's unlocked at,
 * its acceleration speed and rotation speed, in addition to the
 * texture and its weapon type
 */
public struct ShipType{
	public string shipName;
	public int levelUnlocked;
	public float speed; 
	public float rotationSpeed;
	public string textureName;
	public weaponType weapon;
	
	public void init(string n, int l, float s, float rs, string t, weaponType w){
		shipName = n;
		levelUnlocked= l;
		speed = s;
		rotationSpeed = rs;
		textureName = t;
		weapon= w;
	}
}
