using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {

	public GameObject asteroid;
	public GameObject ship;
	public Text levelCleared;

	private float startTime;
	private float levelClearedTime;
	private float asteroidGenDelta = 1f;
	private int asteroidsToGen = 1;
	private int asteroidsSpawned=0;
	private int asteroidsDestroyed=0;
	private static int level = 1;
	private int baseAsteroidValue = 100;
	private int baseLevelValue = 1000;
	private Ship shipScript;

	private ShipType[] ships;

	void Start () {
		Asteroid.manager = this;
		createShipTypes ();
		shipScript = ship.GetComponent<Ship> ();
		init ();
	}

	public void init(){
		startTime = Time.time;
		asteroidsToGen = 1;
		asteroidsSpawned=0;
		asteroidsDestroyed=0;
		level = 1;
		levelCleared.enabled = false;
	}

	// Update is called once per frame
	void Update () {
		//Destroy asteroids on screen for a restart
		if (Ship.restarting) {
			destroyAsteroids();
			init();			
			shipScript.upgradeShip (ships[0]);
			Ship.restarting = false;
		}

		checkIfLevelCleared ();

		//Check if we're done generating for this level
		if (asteroidsToGen <= 0)
			return;
		//Periodically generate asteroids with random pos and dir
		if (Time.time - startTime > asteroidGenDelta) {
			startTime= Time.time;
			generateAsteroid(1f,generateRandomPosition(true), false);
			asteroidsToGen--;
		}
	}

	/*
	 * If all asteroids generated have been destroyed, advance the level
	 * and award points to the player for clearing the level.
	 * Additionally, see if player has earned a new ship type
	 */
	void checkIfLevelCleared(){
		if (asteroidsSpawned != 0 && asteroidsToGen <= 0 && asteroidsSpawned == asteroidsDestroyed) {
			levelCleared.enabled= true;
			levelCleared.text= "Level "+level+" cleared";
			level++;

			checkIfShipUnlocked();

			//Reset asteroid counts
			asteroidsSpawned=0;
			asteroidsDestroyed=0;
			asteroidsToGen = level*level;

			startTime= levelClearedTime = Time.time;

			shipScript.addPoints (baseLevelValue);
		}

		//Check if we should still display the "level cleared" message
		if (Time.time - levelClearedTime > 2.5f) {
			levelCleared.enabled = false;
		}
	}

	//Generate an asteroid at a desired size
	void generateAsteroid(float scaleFactor, Vector3 pos, bool isDebris){
		GameObject a = Instantiate(asteroid, pos, Quaternion.identity) as GameObject;
		a.transform.localScale=new Vector3 (a.transform.localScale.x * scaleFactor, a.transform.localScale.y 
               * scaleFactor, a.transform.localScale.z * scaleFactor);

		Asteroid script = a.GetComponent<Asteroid>();
		script.generateRandomDirection();
		script.isDebris = isDebris;
		asteroidsSpawned++;
	}

	//Handle asteroid destruction and award points
	public void asteroidDestroyed(bool isDebris, bool awardPoints){
		asteroidsDestroyed++;
		if (!awardPoints)
			return;

		int pointVal = baseAsteroidValue;
		if(isDebris)
			pointVal *= 2;
		shipScript.addPoints (pointVal);
	}

	//Generate 4 smaller asteroids in place
	public void generateDebris(Vector3 pos){
		for (int i=0; i<4; i++) {
			generateAsteroid(.5f,pos, true);
		}
	}

	//Destroy all asteroids on screen
	void destroyAsteroids(){
		GameObject [] gameObjects =  GameObject.FindGameObjectsWithTag ("asteroid");
		
		for(var i = 0 ; i < gameObjects.Length ; i ++)
			Destroy(gameObjects[i]);
	}

	/*
	 * Generate a random position within the screen,
	 * with the option to pin position to the edge of the screen
	 */
	Vector3 generateRandomPosition(bool pinToScreenEdge){
		//Calculate bounds from the camera
		float yBound = Camera.main.orthographicSize;  
		float xBound = yBound * Screen.width / Screen.height;

		//Generate a random position within these bounds
		float yPos = Random.Range (-yBound, yBound);
		float xPos = Random.Range (-xBound, xBound);
		Vector3 randPos = new Vector3 (xPos, yPos, Wrappable.zCoord);

		//Pin point to screen edge
		if (pinToScreenEdge) {
			//Pin either on y or x component
			int truncY = Random.Range (0, 10) % 2;
			int sign = Random.Range (-10, 10);
			
			//Top or bottom edge
			if (truncY == 1) {
				randPos.y= (sign > 0) ? yBound : -yBound;
			}
			else { //Left or right edge
				randPos.x= (sign > 0) ? xBound : -xBound;
			}
		}
		return randPos;
	}

	public static int getLevel(){
		return level;
	}

	void checkIfShipUnlocked(){
		for (int i= 0; i<4; i++) {
			if(ships[i].levelUnlocked == level){
				shipScript.upgradeShip (ships[i]);
			}
		}
	}

	/*
	 * TODO: implement an xml loader that reads ship properties and associates what 
	 * level they're earned at.
	 * Currently this function loads ship types with hardcoded values
	 */
	void createShipTypes(){
		ships = new ShipType[4];

		ships [0].init ("Candy Corn" , 1, 8f, 200f, "candyCorn", weaponType.candyShot);
		ships [1].init ("Chocolate Kiss" , 3, 10f, 200f, "kiss", weaponType.knightShot);
		ships [2].init ("Peanut Butter Cup" , 6, 12f, 225f, "pbCup", weaponType.ninjaShot);
		ships [3].init ("Moonbot" , 10, 15f, 225f, "moonbot", weaponType.moonShot);
	}

}
