using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour {
	public int health;

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(1)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


			RaycastHit hit;

			if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
				GameObject[] cubes;
				cubes = GameObject.FindGameObjectsWithTag ("select");
				foreach (GameObject cube in cubes) 
				{
					var s = cube.GetComponent<ZombieController>();
					if (s.isSelected) {
						s.targetEnemy = hit.transform;
					}
				}
			}
		}
	}

	void OnTriggerEnter(Collider other) 
	{
		if (other.tag == "Boundary")
		{
			return;
		}
		Destroy(other.gameObject);

		health -= 5;

		if (health <= 0) {
			Destroy(gameObject);
		}
	}

	void OnMouseDown() {



	}
}
