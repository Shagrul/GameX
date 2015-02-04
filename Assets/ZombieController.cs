using UnityEngine;
using System.Collections;

public class ZombieController : MonoBehaviour {
	public bool isSelected;
	private bool isActive;
	public float speed = 1.5f;
	private Vector3 target;
	public float floatHeight;
	public float liftForce;
	public float damping;
	private Rigidbody2D rocket;
	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;
	private float nextFire;
	public Transform bossSpawn;
	public Transform targetEnemy;
	
	// Use this for initialization
	void Start () {

		target = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonUp(0)) {
			Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);
			camPos.y = CameraOperator.InvertMouseY(camPos.y);
			if (CameraOperator.selection.width > 2) {
				isSelected = CameraOperator.selection.Contains(camPos);
			}

		}

		if (isSelected) {

			renderer.material.color = Color.blue;

			if (Input.GetMouseButtonDown(1)) {
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				
				if (Physics.Raycast (ray.origin, ray.direction, Mathf.Infinity)) {
					target = transform.position;
				} else {
					isActive = true;
					target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					target.z = transform.position.z;
				}
			}
			transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
		}
		else {
			renderer.material.color = Color.white;
				}

		if (isActive) {
			transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
			if (transform.position == target) {
				isActive = false;
			}
		}
		else {
			if (targetEnemy != null) {
				Fire ();
			}

		}


	}
	
	void OnMouseDown() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit2D hit = Physics2D.Raycast (ray.origin, ray.direction, Mathf.Infinity);
		if (hit) {
			if (hit.transform.tag == "select")
			{
				hit.transform.tag="Untagged";
				hit.transform.renderer.material.color=Color.blue;
				GameObject[] cubes;
				cubes = GameObject.FindGameObjectsWithTag ("select");
				foreach (GameObject cube in cubes) 
				{
					cube.renderer.material.color=Color.white;
					var s = cube.GetComponent<ZombieController>();
					s.isSelected = false;
				}
				isSelected = true;
				hit.transform.tag="select";
			}
		}
	}

	void Fire()
	{
		if (Time.time > nextFire) {
			nextFire = Time.time + fireRate;
			//Instantiate (shot, shotSpawn.position, shotSpawn.rotation);
			GameObject bulletClone;
			bulletClone = (GameObject)Instantiate (shot, shotSpawn.position, shotSpawn.rotation);
			bulletClone.rigidbody.velocity = (targetEnemy.position - transform.position).normalized * 10F;
			//bulletClone.parent = bullets; // bullet is child of "Bullets"
		}

	}
}

