using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Ship : Wrappable {
	private  CharacterController controller;
	private float speed = 10f;
	private float rotationSpeed = 200f;
	public Text infoUI;
	public Text gameOver;
	public int numLives = 3;
	public static int levelNum= 0;
	private bool dead = false;
	private bool invincible = false;
	private float invincibleStart;
	private float invinciblePeriod = .5f;

	public Rigidbody projectile;
	private float projectileSpeed = 500f;

	void Start(){
		controller = GetComponent<CharacterController>();
		setBounds();
		updateUI ();
		gameOver.enabled = false;
	}
	
	void Update() {
		updateUI();
		if (dead)
			return;

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

		if (other.gameObject.tag == "asteroid" || other.gameObject.tag == "shot") {
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
	}

	void endGame(){
		gameOver.enabled = true;
		dead = true;
	}

	
}
