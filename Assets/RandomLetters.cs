using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using System;

public class RandomLetters : MonoBehaviour {
	List<KeyValuePair<string, double>> alphabetList = new List<KeyValuePair<string, double>> ();
	List<KeyValuePair<string, double>> consonantList = new List<KeyValuePair<string, double>> ();
	List<KeyValuePair<string, double>> vowelList = new List<KeyValuePair<string, double>> ();
	string currentVowelLetters = "";
	string currentConsonantLetters = "";
	string consonantSet = "BCDFGHJKLMNPQRSTVWXYZ";
	string firstVowelSet = "AEOIU";
	public GameObject letterPrefab;
	public Transform letterSpawn;
	public static float movementMargin = 0.63f;
	public static HashSet<string> wordList = new HashSet<string>();
	public static bool isGameOver = false;
	public GameObject gameOverText;
	public GameObject gameOverPanel;
	public static int gameScore = 0;
	public static bool shouldPlaySound = false;
	public AudioClip crashSound;


	//public static List<string> wordList = new List<string>();
	
	// Use this for initialization
	void Start () {
		gameOverPanel.SetActive (false);
		PopulateValidWordList ();
		alphabetList.Add (new KeyValuePair<string, double>("AEI", 0.28));
		alphabetList.Add (new KeyValuePair<string, double>("OU", 0.2));
		alphabetList.Add (new KeyValuePair<string, double>("QXYZJ", 0.05));
		alphabetList.Add (new KeyValuePair<string, double>("LNRST", 0.22));
		alphabetList.Add (new KeyValuePair<string, double>("FHKPWV", 0.1));
		alphabetList.Add (new KeyValuePair<string, double>("BCDGM", 0.15));
//		alphabetList.Add (new KeyValuePair<string, double>("A", 0.5));
//		alphabetList.Add (new KeyValuePair<string, double>("BC", 0.5));
		vowelList.Add (new KeyValuePair<string, double>("AEI", 0.7));
		vowelList.Add (new KeyValuePair<string, double>("OU", 0.3));
//		vowelList.Add (new KeyValuePair<string, double>("A", 1));
		consonantList.Add (new KeyValuePair<string, double>("QXYZJ", 0.1));
		consonantList.Add (new KeyValuePair<string, double>("LNRST", 0.40));
		consonantList.Add (new KeyValuePair<string, double>("FHKPWV", 0.2));
		consonantList.Add (new KeyValuePair<string, double>("BCDGM", 0.30));
//		consonantList.Add (new KeyValuePair<string, double>("A", 1));
	}

	// Update is called once per frame
	void Update () {
		if (shouldPlaySound) {
			audio.PlayOneShot(crashSound);
			shouldPlaySound = false;
		}

		if (isGameOver) {
//			Text text = gameOverText.GetComponent<Text>();
//			text.text = "Game Over!!";
			gameOverPanel.SetActive (true);
		}
		else {
			var activeLettersList = FindActiveLetters();
			if (activeLettersList.Count == 0) {
				SpawnLetter();
			}
			UpdateScore();
		}
	}

	public void RestartGame()
	{
		var cubes = GameObject.FindGameObjectsWithTag ("InactiveLetter");
		for (int i = cubes.Length - 1; i >= 0; i--) {
			Destroy(cubes[i]);
		}
		LetterMovement.allLettersOnBoard.Clear ();
		gameScore = 0;
		gameOverPanel.SetActive (false);
		isGameOver = false;
	}

	public void UpdateScore()
	{
		Text text = gameOverText.GetComponent<Text>();
		text.text = "Score: " + gameScore.ToString ();
	}

	//Populate a list with valid words from a text file
	private void PopulateValidWordList()
	{


		TextAsset wordTextFile = Resources.Load("sowpods") as TextAsset;
		StringReader reader = new StringReader(wordTextFile.text);

		// Read each line from the file
		string validWord = string.Empty;
		while ((validWord = reader.ReadLine()) != null)
			wordList.Add (validWord);
	}

	//Find all the active letters on the screen and push them into a new list
	private List<LetterMovement> FindActiveLetters()
	{
		List<LetterMovement> activeLettersList = new List<LetterMovement>();
		GameObject[] objectsWithLetterTagArray = GameObject.FindGameObjectsWithTag ("Letter");

		foreach (var letterTagObject in objectsWithLetterTagArray) {
			var letterComponent = letterTagObject.GetComponent<LetterMovement>();
			if (letterComponent.isActive) {
				activeLettersList.Add(letterComponent);
			}
		}

		return activeLettersList;
	}

	//Creates a new letter at the specified spot
	private void SpawnLetter()
	{
		char randomLetter = GetRandomLetter();
		letterPrefab = (GameObject)Resources.Load(randomLetter + "Letter");
		Instantiate (letterPrefab, letterSpawn.position, letterSpawn.rotation);
	}

	/// <summary>
	/// Gets the random letter.
	/// </summary>
	/// <returns>The random letter.</returns>
	private char GetRandomLetter()
	{
		System.Random r = new System.Random();
		List<KeyValuePair<string, double>> listToUse = new List<KeyValuePair<string, double>> ();
		if (currentVowelLetters.Length >= 4) {
			listToUse = consonantList;
			currentVowelLetters = "";
		}
		else if (currentConsonantLetters.Length >= 3) {
			listToUse = vowelList;
			currentConsonantLetters = "";
		}
		else {
			listToUse = alphabetList;
		}
		double diceRoll = r.NextDouble ();
		char selectedElement = 'A';
		double cumulative = 0.0;
		for (int i = 0; i < listToUse.Count; i++)
		{
			cumulative += listToUse[i].Value;
			if (diceRoll < cumulative)
			{
				string selectedSet = listToUse[i].Key;
				selectedElement = selectedSet[UnityEngine.Random.Range(0, selectedSet.Length)];

				break;
			}
		}
		if (consonantSet.Contains(selectedElement)) {
			currentConsonantLetters += selectedElement;
		}
		else if (firstVowelSet.Contains(selectedElement)) {
			currentVowelLetters += selectedElement;
		}
		return selectedElement;
//		char randomChar = 'a';
//		int randomInt = Random.Range (1, 3);
//		if (randomInt == 1) {
//			randomChar = consonantSet[Random.Range(0, consonantSet.Length)];
//		} else if (randomInt == 2) {
//			randomChar = firstVowelSet[Random.Range(0, firstVowelSet.Length)];
//		} 

		//return randomChar;
	}
}
