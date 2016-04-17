using UnityEngine;
using System.Collections;

public class CharImageController : MonoBehaviour {

	public Sprite[] HenshinImages;
	public Sprite[] FailImages;
	public Sprite[] IdleImages;
	public Sprite[] TransformedImages;

	SpriteRenderer sr;

	int _CurrentIdleIndex = 0;

	// Use this for initialization
	void Start () {
		sr = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void RandomHenshin() {
		int randomImage = Random.Range (0, HenshinImages.Length);
		sr.sprite = HenshinImages [randomImage];
		RandomFlipX ();
	}

	public void RandomFail() {
		int randomImage = Random.Range (0, FailImages.Length);
		sr.sprite = FailImages [randomImage];
		RandomFlipX ();
	}

	public void ContinueIdle() {

		if (_CurrentIdleIndex >= IdleImages.Length) {
			_CurrentIdleIndex = 0;
		}
			
		sr.sprite = IdleImages [_CurrentIdleIndex];

		_CurrentIdleIndex++;

	}

	void RandomFlipX() {
		if (Random.Range (0, 2) == 0) {
			sr.flipX = true;
		} else {
			sr.flipX = false;
		}
	}


}
