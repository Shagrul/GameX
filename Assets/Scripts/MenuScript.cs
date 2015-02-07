using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {

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

	public void OnClickQuit()
	{
		Application.Quit ();
	}
}
