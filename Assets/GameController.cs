using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	public int PassageToWin = 10;
	public string NextScene;

	[Header("References")]
	public Text PassageLeft;
	public CharImageController CharController;
	public BarController BarController;

	// Use this for initialization
	void Start () {

		BarController.OnMisbeat += CharController.RandomFail;
		BarController.OnBeat += CharController.RandomHenshin;
		BarController.OnBeating += CharController.ContinueIdle;
		BarController.OnPassageSucceed += DecreasePassageLeft;

		// set the text for the first time
		PassageLeft.text = PassageToWin.ToString();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void DecreasePassageLeft() {
		PassageToWin--;
		PassageLeft.text = PassageToWin.ToString();

		if (PassageToWin <= 0) {
			SceneManager.LoadScene (NextScene);
		}
	}
}
