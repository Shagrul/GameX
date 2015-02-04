using UnityEngine;
using System.Collections;

public class TestMovementScript : MonoBehaviour {
	private Vector3 targetPosition = new Vector3(0f, 0f);

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		//transform.position = Vector3.Slerp(transform.position, targetPosition, 1 * Time.deltaTime);
		if (Input.GetKeyDown(KeyCode.DownArrow)) {

			//transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.smoothDeltaTime * 5);
			//transform.position = Vector3.Lerp(transform.position, new Vector3(0f, 0f), 0.1f);
			
		}
	}

	void FixedUpdate()
	{
		transform.position = Vector3.Lerp(transform.position, new Vector3(0f, 0f), Time.fixedDeltaTime);
		//transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.fixedDeltaTime);
		//rigidbody2D.velocity = new Vector2 (0f, -1f);
	}
}
