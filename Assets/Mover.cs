using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {
	public float speed;
	public Transform bossSpawn;
	
	// Use this for initialization
	void Start () {
		GameObject[] cubes;
		cubes = GameObject.FindGameObjectsWithTag ("select");
		foreach (GameObject cube in cubes) 
		{
			var s = cube.GetComponent<ZombieController>();
			if (s.isSelected) {
				rigidbody.velocity = (s.targetEnemy.position - transform.position).normalized * speed;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
