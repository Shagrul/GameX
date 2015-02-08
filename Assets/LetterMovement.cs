using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using UnityEngine.UI;

public class LetterMovement : MonoBehaviour {
	public static List<LetterMovement> allLettersOnBoard = new List<LetterMovement>();
	public static List<KeyValuePair<string, List<LetterMovement>>> activeWords = new List<KeyValuePair<string, List<LetterMovement>>>();
	public Vector3 targetPosition;
	private Vector2 startPos;
	public bool shouldDestroy = false;
	public bool shouldRemove = false;
	public bool isActive;
	public float speed;
	private float minSwipeDistX = 115f;
	public string letterInAlphabet;
	public bool isSwipedDown = false;
	public int letterScore;
	private SpriteRenderer sprite;
	public bool hasMoved = false;
	public Sprite otherSprite;
	public AnimationClip anim;
	public string word;
	public bool isHighlightActive = false;
	private UnityEngine.Object letterHighlight;

	// Use this for initialization
	void Start () {
		//SomeFunction ();
		InitializeMovementParameters ();
	}
	
	// Update is called once per frame
	void Update () {
		RemoveWord ();
		DoubleTap ();
		DetermineIfLetterShouldStop ();
		MoveActiveLetter();
		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			DestroyLetters();	
		}
		DestroyLetters();	
		HighlightLetters ();

		if (!isHighlightActive) {
			isHighlightActive = true;
			var test = (GameObject)Resources.Load("ALetterHighlight");
			letterHighlight = Instantiate (test, targetPosition, transform.rotation);	
		}
	}

	private void RemoveWord()
	{
		if (Input.GetMouseButtonDown(1)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			RaycastHit hit;
			
			if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
				var obj = hit.transform.gameObject;
				var letterMovement = obj.GetComponent<LetterMovement>();
				DetermineIfValidWord1(obj);
			}
		}
	}

	private void InitializeMovementParameters()
	{
		sprite = GetComponent<SpriteRenderer>();
		speed = 0.8f;
		isActive = true;
		targetPosition = new Vector3 (2.5f, 0.8f);
	}

	public void DetermineIfLetterShouldStop()
	{
		if (isActive) {
			if (transform.position == targetPosition) {
				DestroyImmediate(letterHighlight);

				sprite.sortingOrder = (int)(transform.position.y * -100);
				RandomLetters.shouldPlaySound = true;
				DeactivateLetter();
				allLettersOnBoard.Add(gameObject.GetComponent<LetterMovement>());
				DetermineIfValidWord();
				DetermineIfYValidWord();
				DetermineIfGameOver();
				GameObject[] cubes;
				cubes = GameObject.FindGameObjectsWithTag ("HighlightedLetter");
			}
		}
		
		if (tag == "IsScrambling") {
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
			if (transform.position == targetPosition) {

				DeactivateLetter();
				allLettersOnBoard.Add(gameObject.GetComponent<LetterMovement>());
				DetermineIfValidWord();
				DetermineIfYValidWord();
			}
		}
	}

	public void DetermineIfGameOver()
	{
		if (transform.position.y >= 7f) {
			RandomLetters.isGameOver = true;
		}
	}

	public void MoveActiveLetter()
	{
		if (isActive) {
			Move();
			DetermineTargetYPosition ();
		}

		if (tag == "InactiveLetter") {
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
			Rescramble();
		}
	}

	private void Move ()
	{
		if (isSwipedDown) {
			transform.position = Vector3.MoveTowards (transform.position, targetPosition, 0.07f);
		} else {
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
		}

		MoveByTouch();
		MoveByKeyboard();
	}

	private void DeactivateLetter()
	{
		tag = "InactiveLetter";
		isActive = false;
	}

	public void MoveByTouch()
	{
		float swipeDistHorizontal = 0;
		float swipeDistVerticle = 0;

		if (Input.touchCount > 0) {
			Touch touch = Input.touches [0];
			switch (touch.phase) {
			case TouchPhase.Began:
				startPos = touch.position;
				break;
			case TouchPhase.Moved:
				swipeDistVerticle = (new Vector3 (touch.position.y, 0, 0) - new Vector3 (startPos.y, 0, 0)).magnitude;
				swipeDistHorizontal = (new Vector3 (touch.position.x, 0, 0) - new Vector3 (startPos.x, 0, 0)).magnitude;
				if (swipeDistHorizontal > minSwipeDistX) {
					float swipeValue = Mathf.Sign (touch.position.x - startPos.x);
					
					if (swipeValue > 0) {
						if (transform.position.x + RandomLetters.movementMargin < 5f) {
							var isLeft = allLettersOnBoard.Where(x => Convert.ToInt32(x.transform.position.x * 10) == Convert.ToInt32((transform.position.x + RandomLetters.movementMargin) * 10) &&
							                                     Convert.ToInt32((x.transform.position.y + RandomLetters.movementMargin + 0.00f) * 100) > Convert.ToInt32(transform.position.y * 100)).FirstOrDefault();
							
							if (isLeft == null) {
								isHighlightActive = false;
								DestroyImmediate(letterHighlight);
								Vector3 currentPosition = transform.position;
								currentPosition.x += RandomLetters.movementMargin;
								targetPosition.x += RandomLetters.movementMargin;
								transform.position = currentPosition;
							}
						}
					} else if (swipeValue < 0) {
						if (transform.position.x - RandomLetters.movementMargin > 0.4f) {
							var isLeft = allLettersOnBoard.Where(x => Convert.ToInt32(x.transform.position.x * 10) == Convert.ToInt32((transform.position.x - RandomLetters.movementMargin) * 10) &&
							                                     Convert.ToInt32((x.transform.position.y + RandomLetters.movementMargin + 0.00f) * 100) > Convert.ToInt32(transform.position.y * 100)).FirstOrDefault();

							if (isLeft == null) {
								isHighlightActive = false;
								DestroyImmediate(letterHighlight);
								Vector3 currentPosition = transform.position;
								currentPosition.x -= RandomLetters.movementMargin;
								targetPosition.x -= RandomLetters.movementMargin;
								transform.position = currentPosition;
							}
						}
					}
					swipeDistHorizontal = 0f;
					startPos = touch.position;
				}

				if (swipeDistVerticle > 300f) {
					float swipeValue = Mathf.Sign (touch.position.y - startPos.y);
					
					if (swipeValue < 0) {
						isSwipedDown = true;
					}
				}
				break;
			}
		}
	}
	
	public void MoveByKeyboard()
	{
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			if (transform.position.x - RandomLetters.movementMargin > 0.4f) {
				var isLeft = allLettersOnBoard.Where(x => Convert.ToInt32(x.transform.position.x * 10) == Convert.ToInt32((transform.position.x - RandomLetters.movementMargin) * 10) &&
				                                     Convert.ToInt32((x.transform.position.y + RandomLetters.movementMargin + 0.00f) * 100) > Convert.ToInt32(transform.position.y * 100)).FirstOrDefault();

				if (isLeft == null) {
					isHighlightActive = false;
					DestroyImmediate(letterHighlight);
					Vector3 position = this.transform.position;
					position.x -= RandomLetters.movementMargin;
					targetPosition.x -= RandomLetters.movementMargin;
					this.transform.position = position;
				}

			}
		} else if (Input.GetKeyDown(KeyCode.RightArrow)) {
			var isLeft = allLettersOnBoard.Where(x => Convert.ToInt32(x.transform.position.x * 10) == Convert.ToInt32((transform.position.x + RandomLetters.movementMargin) * 10) &&
			                                     Convert.ToInt32((x.transform.position.y + RandomLetters.movementMargin + 0.00f) * 100) > Convert.ToInt32(transform.position.y * 100)).FirstOrDefault();
			
			if (isLeft == null) {
				if (transform.position.x + RandomLetters.movementMargin < 5f) {
					isHighlightActive = false;
					DestroyImmediate(letterHighlight);
					Vector3 position = this.transform.position;
					position.x += RandomLetters.movementMargin;
					targetPosition.x += RandomLetters.movementMargin;
					this.transform.position = position;
				}
			}
		} else if (Input.GetKeyDown(KeyCode.DownArrow)) {
			isSwipedDown = true;
		}
	}

	private void Rescramble()
	{
		bool doesExist = false;
		float newYPosition = Mathf.Round ((transform.position.y - RandomLetters.movementMargin) * 100f) / 100f;
		var letter = allLettersOnBoard.Where (x => x.transform.position.x == transform.position.x &&
		                                      Convert.ToInt32(x.transform.position.y * 100) == Convert.ToInt32(newYPosition * 100)).FirstOrDefault();
		if (letter != null) {
			doesExist = true;
		}

		if (doesExist == false) {
			if (targetPosition.y > 0.9f) {
				targetPosition.y -= RandomLetters.movementMargin;
				tag = "IsScrambling";
				allLettersOnBoard.Remove(gameObject.GetComponent<LetterMovement>());
			}
			else if (targetPosition.y == 0.8f && (transform.position.y != 0.8f || transform.position.y < 0.8f)) {
				targetPosition.y = 0.8f;
				tag = "IsScrambling";
				allLettersOnBoard.Remove(gameObject.GetComponent<LetterMovement>());
			}
			
			doesExist = true;
		}
	}

	public void DetermineTargetYPosition()
	{
		bool isNextPositionEmpty = false;
		float startingYPosition = 0.8f;
		while (!isNextPositionEmpty) {
			int count = 0;
			var letter = allLettersOnBoard.Where (x => x.transform.position.x == targetPosition.x &&
			                                      Convert.ToInt32(x.transform.position.y * 100) == Convert.ToInt32(startingYPosition * 100)).FirstOrDefault();
			if (letter != null) {
				startingYPosition += RandomLetters.movementMargin;
				count++;
			}

			if (count == 0) {
				isNextPositionEmpty = true;
			}
		}

		targetPosition.y = startingYPosition;

	}

	void DetermineIfValidWord ()
	{
		if (tag == "InactiveLetter") {
			var letterListToFormWord = new List<LetterMovement>();
			var inactiveLettersOnBoard = allLettersOnBoard.Where(x => Convert.ToInt32(x.transform.position.y * 100) == Convert.ToInt32(transform.position.y * 100))
				.OrderBy(x => x.transform.position.x).ToList();
			//allLettersOnBoard = allLettersOnBoard.OrderBy(x => x.transform.position.x).ToList();

			float tilePositionLeft = transform.position.x;
			bool hasValue = true;
			while (hasValue) {
				tilePositionLeft -= RandomLetters.movementMargin;
				var inactiveLetter = inactiveLettersOnBoard.Where(x => x.transform.position.x == tilePositionLeft).FirstOrDefault();
				if (inactiveLetter != null) {
					letterListToFormWord.Add (inactiveLetter);
				} else {
					hasValue = false;
				}
			}

			float tilePositionRight = transform.position.x;
			hasValue = true;
			while (hasValue) {
				tilePositionRight += RandomLetters.movementMargin;
				var inactiveLetter = inactiveLettersOnBoard.Where(x => x.transform.position.x == tilePositionRight).FirstOrDefault();
				if (inactiveLetter != null) {
					letterListToFormWord.Add (inactiveLetter);
				} else {
					hasValue = false;
				}
			}

			letterListToFormWord.Add(gameObject.GetComponent<LetterMovement>());
			letterListToFormWord = letterListToFormWord.OrderBy(x => x.transform.position.x).ToList();


			var validLetterListToFormWord = new List<LetterMovement>();
			var validWords = new List<List<LetterMovement>>();
			for (int i = 0; i < letterListToFormWord.Count; i++) {
				string formedWord = "";
				var isTrue = false;
				validLetterListToFormWord = new List<LetterMovement>();
				for (int t = i; t < letterListToFormWord.Count; t++) {
					formedWord += letterListToFormWord[t].letterInAlphabet;
					validLetterListToFormWord.Add(letterListToFormWord[t]);
					if (RandomLetters.wordList.Contains(formedWord) && formedWord.Length >= 3) {
						var lst = new List<LetterMovement>();
						foreach (var item in validLetterListToFormWord) {
							item.word = formedWord;
							lst.Add(item);
						}
						validWords.Add(lst);
						Debug.Log (formedWord);
						isTrue = true;
					}
				}
				
				if (isTrue) {
					//break;
				}
				else {
					validLetterListToFormWord.Clear();
				}
			}

			if (validWords.Count > 0) {
				var orderedWords = validWords
					.GroupBy(str => str.Count)
					.OrderByDescending(grp => grp.Key)
					.First()
					.ToList();

				foreach (var orderedWord in orderedWords) {
					List<LetterMovement> vectorList = new List<LetterMovement>();
					foreach (var individualLetter in orderedWord) {
						individualLetter.shouldDestroy = true;
						vectorList.Add (individualLetter);
						//						var cubes = GameObject.FindGameObjectsWithTag ("InactiveLetter").ToList ();
						//						var cube = cubes.Where(x => x.gameObject == individualLetter.gameObject).FirstOrDefault();
						//						if (cube != null) {
						//							Destroy(cube);
						//						}
						
						//Destroy(allLettersOnBoard.Where(x => x == individualLetter).First().gameObject);
						//allLettersOnBoard.Remove(individualLetter);
					}
					activeWords.Add(new KeyValuePair<string, List<LetterMovement>>(orderedWord[0].word, vectorList));
				}
			}
		}
	}

	void DetermineIfYValidWord ()
	{
		
		if (tag == "InactiveLetter") {
			var letterListToFormWord = new List<LetterMovement>();
			var inactiveLettersOnBoard = allLettersOnBoard.Where(x => Convert.ToInt32(x.transform.position.x * 100) == Convert.ToInt32(transform.position.x * 100))
				.OrderBy(x => x.transform.position.y).ToList();
			//allLettersOnBoard = allLettersOnBoard.OrderBy(x => x.transform.position.x).ToList();

			float tilePositionLeft = transform.position.y;
			bool hasValue = true;
			while (hasValue) {
				tilePositionLeft -= RandomLetters.movementMargin;
				var inactiveLetter = inactiveLettersOnBoard.Where(x => Convert.ToInt32(x.transform.position.y * 100) == Convert.ToInt32(tilePositionLeft * 100)).FirstOrDefault();
				if (inactiveLetter != null) {
					letterListToFormWord.Add (inactiveLetter);
				} else {
					hasValue = false;
				}
			}
			
			float tilePositionRight = transform.position.y;
			hasValue = true;
			while (hasValue) {
				tilePositionRight += RandomLetters.movementMargin;
				var inactiveLetter = inactiveLettersOnBoard.Where(x => Convert.ToInt32(x.transform.position.y * 100) == Convert.ToInt32(tilePositionRight * 100)).FirstOrDefault();
				if (inactiveLetter != null) {
					letterListToFormWord.Add (inactiveLetter);
				} else {
					hasValue = false;
				}
			}
			
			letterListToFormWord.Add(gameObject.GetComponent<LetterMovement>());
			letterListToFormWord = letterListToFormWord.OrderBy(x => x.transform.position.y).ToList();
			
			
			var validLetterListToFormWord = new List<LetterMovement>();
			var validWords = new List<List<LetterMovement>>();
			for (int i = 0; i < letterListToFormWord.Count; i++) {
				string formedWord = "";
				var isTrue = false;
				validLetterListToFormWord = new List<LetterMovement>();
				for (int t = i; t < letterListToFormWord.Count; t++) {
					formedWord += letterListToFormWord[t].letterInAlphabet;
					validLetterListToFormWord.Add(letterListToFormWord[t]);
					if (RandomLetters.wordList.Contains(formedWord) && formedWord.Length >= 3) {
						var lst = new List<LetterMovement>();
						foreach (var item in validLetterListToFormWord) {
							item.word = formedWord;
							lst.Add(item);
						}
						validWords.Add(lst);
						Debug.Log (formedWord);
						isTrue = true;
					}
				}
				
				if (isTrue) {
					//break;
				}
				else {
					validLetterListToFormWord.Clear();
				}
			}
			
			if (validWords.Count > 0) {
				var orderedWords = validWords
					.GroupBy(str => str.Count)
						.OrderByDescending(grp => grp.Key)
						.First()
						.ToList();
				
				foreach (var orderedWord in orderedWords) {
					List<LetterMovement> vectorList = new List<LetterMovement>();
					foreach (var individualLetter in orderedWord) {
						individualLetter.shouldDestroy = true;
						vectorList.Add (individualLetter);
						//						var cubes = GameObject.FindGameObjectsWithTag ("InactiveLetter").ToList ();
						//						var cube = cubes.Where(x => x.gameObject == individualLetter.gameObject).FirstOrDefault();
						//						if (cube != null) {
						//							Destroy(cube);
						//						}
						
						//Destroy(allLettersOnBoard.Where(x => x == individualLetter).First().gameObject);
						//allLettersOnBoard.Remove(individualLetter);
					}
					activeWords.Add(new KeyValuePair<string, List<LetterMovement>>(orderedWord[0].word, vectorList));
				}
			}
		}
	}


	private void DetermineScore(LetterMovement letter)
	{
		float scoreMultiplier = 1;
		switch (word.Length) {
		case 3:
			scoreMultiplier = RandomLetters.threeLetterScore;
			break;
		case 4:
			scoreMultiplier = RandomLetters.fourLetterScore;
			break;
		case 5:
			scoreMultiplier = RandomLetters.fiveLetterScore;
			break;
		case 6:
			scoreMultiplier = RandomLetters.sixLetterScore;
			break;
		case 7:
			scoreMultiplier = RandomLetters.sevenLetterScore;
			break;
		default:
				break;
		}
		RandomLetters.gameScore += letter.letterScore * scoreMultiplier;
	}

	void HighlightLetters ()
	{
		if (shouldDestroy) {
			sprite.sprite = otherSprite;
//			var t = gameObject.GetComponent<Animator>();
//			var k = t.GetNextAnimatorStateInfo(0);
//			//t.StartPlayback();
//			t.Play("New Animation", -1, Time.deltaTime);
//			var z = animation;
//			z.Play();

			//anim.
			//anim = gameObject.GetComponent< Animator >();
			//anim.Play();
//			var obj =  gameObject.transform.GetChild(0).gameObject;
//			obj.renderer.material.color = Color.grey;
//			if (!hasMoved) {
//				Vector3 position = this.transform.position;
//				position.y += 1f;
//				this.transform.position = position;
//				hasMoved = true;
//			}

			//sprite.color = Color.cyan;

		}

//		var cubes = GameObject.FindGameObjectsWithTag ("InactiveLetter");
//		for (int i = cubes.Length - 1; i >= 0; i--) {
//			var s = cubes[i].GetComponent<LetterMovement>();
//			if (s.shouldDestroy) {
//				allLettersOnBoard.Remove(s);
//				Destroy (cubes[i]);
//			}
//		}
	}

	public void DestroyLetters()
	{
		if (shouldRemove) {
			var letterMovement = gameObject.GetComponent<LetterMovement>();
			DetermineScore(letterMovement);
			allLettersOnBoard.Remove(letterMovement);
			Destroy (gameObject);

			GameObject[] cubes;
			cubes = GameObject.FindGameObjectsWithTag ("Letter");
			foreach (var item in cubes) {
				var k = item.GetComponent<LetterMovement>();
				//Destroy (k.letterHighlight);
			}
		}
	}











	void DetermineIfValidWord1 (GameObject gameObj)
	{
		var letterMovement = gameObj.GetComponent<LetterMovement>();
		var dict = activeWords.Where (x => x.Key == letterMovement.word).ToList ();
		//var dict = activeWords.Where(x => x.Key == letterMovement.word).ToDictionary (x => x.Key, y => y.Value);
		foreach (var item in dict) {
			var itemVal = item.Value;
			foreach (var item2 in itemVal) {
				item2.shouldRemove = true;
				foreach (var d in activeWords) {
					if (d.Value.Contains(item2)) {
						foreach (var item3 in d.Value) {
							item3.shouldRemove = true;
						}
					}
				}

			}
		}
	}

	public void DoubleTap()
	{
		foreach (var item in Input.touches) {
			if (item.tapCount == 2) {
				Ray ray = Camera.main.ScreenPointToRay(item.position);
				
				RaycastHit hit;
				
				if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
					var obj = hit.transform.gameObject;
					var letterMovement = obj.GetComponent<LetterMovement>();
					DetermineIfValidWord1(obj);
				}
			}
		}

//		foreach(touch in Input.touches){
//			if(touch.tapCount == 2)
//				//DoSomething();
//		}
	}

	private void SomeFunction()
	{
		var lr = gameObject.GetComponent<LineRenderer>();
		
//		var gun = GameObject.Find("Gun");
//		var projectile = GameObject.Find("Projectile");
		
		lr.SetPosition(0, transform.position);
		lr.SetPosition(1, targetPosition);
	}


}
