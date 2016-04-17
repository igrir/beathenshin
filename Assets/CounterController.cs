using UnityEngine;
using System.Collections;

public class CounterController : MonoBehaviour {

	public GameObject[] Counters;

	public void Show(int index) {
		GameObject targetCount = Counters [index];
		if (targetCount != null) {
			targetCount.SetActive (true);
		}
	}

	public void Hide(int index) {
		GameObject targetCount = Counters [index];
		if (targetCount != null) {
			targetCount.SetActive (false);
		}
	}

	public void HideAll() {
		foreach(GameObject go in Counters) {
			go.SetActive (false);
		}
	}

	public void ShowAll() {
		foreach(GameObject go in Counters) {
			go.SetActive (true);
		}
	}
}
