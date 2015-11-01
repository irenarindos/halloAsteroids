using UnityEngine;
using System.Collections;

enum shotType{singleShot, doubleShot, tripleShot, radialShot};

struct Weapon{
	string shotName;
	string textureName;
	float speed;
	shotType shotStyle;

	public void init(string s, string t, float sp, shotType styl){
		shotName = s;
		textureName = t;
		speed = sp;
		shotStyle = styl;
	}
}

/*
 * Class that is aware of different projectile classes and handles  
 * shooting for the ship
 */
public class WeaponsSystem : MonoBehaviour {

	private Weapon[] weapons;
	Rigidbody projectileRigidBody;

	// Use this for initialization
	void Start () {
		createWeaponTypes ();	
	}

	//Grab a rigidbody game object from the ship
	public void init(Rigidbody p){
		projectileRigidBody = p;
	}

	//check for projetile type and do this shit accordingly
	public void shoot(Rigidbody s, Transform t){
		Rigidbody shot = Instantiate(s, t.position+t.up, t.rotation) as Rigidbody;
		shot.AddForce (t.up * 500f);
		var rend = shot.GetComponent<Renderer>();
		rend.material.mainTexture = Resources.Load("mrBurns") as Texture;
	}

	/*
	 * Given more time, implement an xml loader that
	 * reads projectile properties 
	 */
	void createWeaponTypes(){
		weapons = new Weapon[4];

		weapons[0].init("candyShot", "candyCorn", 10f, shotType.singleShot);
		weapons[1].init("candyShot", "candyCorn", 10f, shotType.doubleShot);
		weapons[2].init("candyShot", "candyCorn", 10f, shotType.tripleShot);
		weapons[3].init("candyShot", "candyCorn", 10f, shotType.radialShot);
	}
	
}
