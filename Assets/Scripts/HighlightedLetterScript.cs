using UnityEngine;
using System.Collections;

public class HighlightedLetterScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		DoubleTap ();
	}

	public void DoubleTap()
	{
		foreach (var item in Input.touches) {
			if (item.tapCount == 1) {
				Ray ray = Camera.main.ScreenPointToRay(item.position);
				
				RaycastHit hit;
				
				if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
					var obj = hit.transform.gameObject;
					if (obj.tag == "HighlightedLetter") {
						GameObject[] cubes;
						cubes = GameObject.FindGameObjectsWithTag ("Letter");
						foreach (var item2 in cubes) {
							var k = item2.GetComponent<LetterMovement>();
							k.isSwipedDown = true;
						}
					}
				}
			}
		}

		if (Input.GetMouseButtonDown(1)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			
			RaycastHit hit;
			
			if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
				var obj = hit.transform.gameObject;
				if (obj.tag == "HighlightedLetter") {
					GameObject[] cubes;
					cubes = GameObject.FindGameObjectsWithTag ("Letter");
					foreach (var item2 in cubes) {
						var k = item2.GetComponent<LetterMovement>();
						k.isSwipedDown = true;
					}
				}
			}
		}
	}
}
