using UnityEngine;
using System.Collections;

public class GameMapScript : MonoBehaviour {

	public AudioClip clip;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnClickPlay()
	{
		Application.LoadLevel ("GameStart");
	}





}
