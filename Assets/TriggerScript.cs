using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


public class TriggerScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) 
	{
		//Destroy(other.gameObject);
		//var letterComponent = this.GetComponent<LetterMovement>();
		//letterComponent.rigidbody.velocity = Vector3.zero;
		//letterComponent.isActive = false;
		//letterComponent.tag = "InactiveLetter";
	}

	void OnMouseDown()
	{
		//Destroy(gameObject);
		Debug.Log (transform.position.ToString());
	}
}