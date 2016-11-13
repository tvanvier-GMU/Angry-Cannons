using UnityEngine;
using System.Collections;

public class Cannon : MonoBehaviour {
	public Transform cannonPivot;
	public Transform cannonBase;
	public Transform gunPoint;

	[Header("Cannon Options")]
	[Range(0, 100000)]
	public float cannonForce = 1000f;
	[Range(1, 50)]
	public float cannonBallWeight = 10f;
	public float rotationSpeed = 5f;

	public GameObject bulletPrefab;

	public enum GunType{
		cannon,
		machineGun
	}

	public GunType activeGun = GunType.cannon;

	[Header("Machine Gun Options")]
	public float machineGunRange = 100f;
	[Range(0, 50000)]
	public float hitForce = 100f;
	public bool explosiveRounds = false;
	public float explosionRadius = 1f;
	[Range(0, 50000)]
	public float explosionForce = 100f;
	public GameObject hitParticle;
	public float particleDestroyTime = .5f;

		

	[System.Serializable]
	public class CannonControls{
		public KeyCode fireKey = KeyCode.Space;
		public KeyCode rotateLeftKey = KeyCode.LeftArrow;
		public KeyCode rotateRightKey = KeyCode.RightArrow;
		public KeyCode rotateUpKey = KeyCode.UpArrow;
		public KeyCode rotateDownKey = KeyCode.DownArrow;
	}

	public CannonControls controls = new CannonControls();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Debug.DrawRay (gunPoint.position, gunPoint.forward * machineGunRange, Color.red);
	}

	void LateUpdate(){
		Vector3 rotation = Vector3.zero;
		if (activeGun == GunType.cannon && Input.GetKeyDown(controls.fireKey)) {
			Fire ();
		}
		if (activeGun == GunType.machineGun && Input.GetKey (controls.fireKey)) {
			Fire ();
		}

		if (Input.GetKey(controls.rotateLeftKey)) {
			rotation += Vector3.left;
		}
		if (Input.GetKey (controls.rotateRightKey)) {
			rotation += Vector3.right;
		}
		if (Input.GetKey(controls.rotateUpKey)) {
			rotation += Vector3.up;
		}
		if (Input.GetKey (controls.rotateDownKey)) {
			rotation += Vector3.down;
		}

		cannonBase.Rotate (new Vector3 (0, rotation.x, 0) * Time.deltaTime * rotationSpeed, Space.World);
		//cannonPivot.Rotate (new Vector3 (-rotation.y, 0, 0) * Time.deltaTime * rotationSpeed, Space.World);
		cannonPivot.Rotate(cannonPivot.right * -rotation.y * Time.deltaTime * rotationSpeed, Space.World);
	}

	void Fire(){
		if (activeGun == GunType.cannon) {
			GameObject newBullet = Instantiate (bulletPrefab, gunPoint.position, gunPoint.rotation) as GameObject;
			Rigidbody nbrb = newBullet.GetComponent<Rigidbody> ();
			nbrb.mass = cannonBallWeight;
			nbrb.AddForce (gunPoint.forward * cannonForce);
		}
		if (activeGun == GunType.machineGun) {
			RaycastHit hit;
			if (Physics.Raycast (gunPoint.position, gunPoint.forward, out hit, machineGunRange)) {
				if (explosiveRounds) {
					Collider[] colliders = Physics.OverlapSphere (hit.point, explosionRadius);
					foreach (Collider hitCollider in colliders) {
						Rigidbody rb = hitCollider.GetComponent<Rigidbody> ();
						if (rb != null) {
							rb.AddExplosionForce (explosionForce, hit.point, explosionRadius, 2f);
							GameObject particle = (GameObject) Instantiate (hitParticle, hit.point, Quaternion.identity);
							Destroy (particle, particleDestroyTime);
						}
					}
				} 
				else {
					Collider col = hit.collider;
					Rigidbody rb = col.GetComponent<Rigidbody> ();
					if (rb != null) {
						rb.AddForce (gunPoint.forward * hitForce);
						GameObject particle = (GameObject) Instantiate (hitParticle, hit.point, Quaternion.identity);
						Destroy (particle, particleDestroyTime);
					}
				}
			}
		}
	}
}
