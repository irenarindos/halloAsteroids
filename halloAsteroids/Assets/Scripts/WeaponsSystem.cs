using UnityEngine;
using System.Collections;

/*
 * Class that is aware of different projectile classes and handles  
 * shooting for the ship
 */
public class WeaponsSystem : MonoBehaviour {

	private Weapon[] weapons;
	private int activeWeapon;

	// Use this for initialization
	void Start () {
		createWeaponTypes ();	
		activeWeapon = 0;
	}

	public void setActiveWeapon(weaponType w){
		activeWeapon = (int)w;
	}

	//check for projetile type and do this shit accordingly
	public void shoot(Rigidbody s, Transform t){
		Texture shotTexture = Resources.Load(weapons [activeWeapon].textureName) as Texture;
		float shotSpeed = weapons [activeWeapon].speed;

		//Generate straight shot for single, triple, or tetra shot
		if (weapons [activeWeapon].shotStyle != shotType.doubleShot) {
			Rigidbody shot = Instantiate (s, t.position + t.up, t.rotation) as Rigidbody;
			shot.AddForce (t.up * shotSpeed);
			Renderer rend = shot.GetComponent<Renderer> ();
			rend.material.mainTexture = shotTexture;
			if (weapons [activeWeapon].shotStyle == shotType.singleShot)
				return;
		}

		//Generate dual forked shot for double, triple or tetra shot
		//First shot, at 10 degrees
		Rigidbody shot2 = Instantiate(s, t.position+t.up, t.rotation) as Rigidbody;
		Vector3 rotator2 = rotateVector (t.up, 10f);
		shot2.AddForce (rotator2 * shotSpeed);
		Renderer rend2 = shot2.GetComponent<Renderer>();
		rend2.material.mainTexture = shotTexture;

		//Second shot, at -10 degrees
		Rigidbody shot3 = Instantiate (s, t.position+t.up , t.rotation) as Rigidbody;
		Vector3 rotator3 = rotateVector (t.up, -10);
		shot3.AddForce (rotator3 * shotSpeed);
		Renderer rend3 = shot3.GetComponent<Renderer> ();
		rend3.material.mainTexture = shotTexture;

		//Generate backwards shot
		if (weapons [activeWeapon].shotStyle == shotType.tetraShot) {
			Vector3 down = new Vector3(-t.up.x, -t.up.y, -t.up.z);
			Rigidbody shot4 = Instantiate(s, t.position, t.rotation) as Rigidbody;
			shot4.AddForce (down * shotSpeed);
			Renderer rend4 = shot4.GetComponent<Renderer>();
			rend4.material.mainTexture = shotTexture;			
		}
	}

	/*
	 * TODO: implement an xml loader that reads projectile properties
	 * Currently this function loads weapon types with hardcoded values
	 */
	void createWeaponTypes(){
		weapons = new Weapon[4];

		weapons[0].init("candyCorn", 500f, shotType.singleShot);
		weapons[1].init("kiss", 515f, shotType.doubleShot);
		weapons[2].init("pbCup", 525f, shotType.tripleShot);
		weapons[3].init("moonbotShot", 550f, shotType.tetraShot);
	}

	//Given a vector and degrees, return a rotated vector
	Vector3 rotateVector(Vector3 vec, float angle){
		Vector3 result = vec;

		//Perform rotation matrix math 
		result.x= ( vec.x * Mathf.Cos(angle*Mathf.Deg2Rad) ) 
		            - ( vec.y * Mathf.Sin(angle*Mathf.Deg2Rad) );
		result.y= ( vec.x * Mathf.Sin(angle*Mathf.Deg2Rad) ) 
					+ ( vec.y * Mathf.Cos(angle*Mathf.Deg2Rad) );
		return result;
	}	
}


public enum shotType{singleShot, doubleShot, tripleShot, tetraShot};
public enum weaponType { candyShot,knightShot, ninjaShot, moonShot};

/*
 * Contains info on weapon texture, shot speed, 
 * and the shot mechanism
 */
public struct Weapon{
	public string textureName;
	public float speed;
	public shotType shotStyle;
	
	public void init(string t, float sp, shotType styl){
		textureName = t;
		speed = sp;
		shotStyle = styl;
	}
}
