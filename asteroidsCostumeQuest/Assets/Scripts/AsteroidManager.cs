using UnityEngine;
using System.Collections;

public class AsteroidManager : MonoBehaviour {

	public GameObject asteroid;

	private float startTime;
	private float asteroidGenDelta = 1f;
	private int asteroidsToGen = 1;
	private int asteroidsSpawned=0;
	private int asteroidsDestroyed=0;
	private static int level = 1;
	private int baseAsteroidValue = 100;
	private int baseLevelValue = 1000;

	void Start () {
		Asteroid.manager = this;
		init ();
	}

	public void init(){
		startTime = Time.time;
		asteroidsToGen = 1;
		asteroidsSpawned=0;
		asteroidsDestroyed=0;
		level = 1;

	}

	// Update is called once per frame
	void Update () {
		if (Ship.restarting) {
			destroyAsteroids();
			init();
			Ship.restarting = false;
		}
		if (asteroidsSpawned != 0 && asteroidsSpawned == asteroidsDestroyed) {
			level++;
			asteroidsSpawned=0;
			asteroidsDestroyed=0;
			asteroidsToGen = level*level;
			startTime= Time.time;
			//logic to calc different level value would go here if desired
			Ship.addPoints (baseLevelValue);
		}

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

	public void asteroidDestroyed(bool isDebris, bool awardPoints){
		asteroidsDestroyed++;
		if (!awardPoints)
			return;

		int pointVal = baseAsteroidValue;
		if(isDebris)
			pointVal *= 2;
		Ship.addPoints (pointVal);
	}

	//Generate 4 smaller asteroids in place
	public void generateDebris(Vector3 pos){
		for (int i=0; i<4; i++) {
			generateAsteroid(.5f,pos, true);
		}
	}

	void destroyAsteroids(){
		GameObject [] gameObjects =  GameObject.FindGameObjectsWithTag ("asteroid");
		
		for(var i = 0 ; i < gameObjects.Length ; i ++)
			Destroy(gameObjects[i]);
	}

	Vector3 generateRandomPosition(bool pinToScreenEdge){
		//Calculate bounds from the camera
		float yBound = Camera.main.orthographicSize;  
		float xBound = yBound * Screen.width / Screen.height;

		//Generate a random position within these bounds
		float yPos = Random.Range (-yBound, yBound);
		float xPos = Random.Range (-xBound, xBound);
		Vector3 randPos = new Vector3 (xPos, yPos, 1f);

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

}
