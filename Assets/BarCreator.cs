using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class BarCreator : MonoBehaviour {

	[Header("Parent properties")]
	public BarController BController;

	[Header("Prefab Properties")]
	public GameObject Prefab;

	[Header("Position Properties")]
	public Transform StartPosition;
	public Transform EndPosition;

	public int BarCount = 8;

	[Header("Execute")]
	public bool CreateBars;

	public List<GameObject> PrefabsList = new List<GameObject>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
		if (CreateBars) {
			CreateBars = false;
			ExecuteCreateBars ();
		}

	}

	void ExecuteCreateBars() {
		// delete previous bars
		foreach (GameObject go in PrefabsList) {
			DestroyImmediate (go);
		}
		PrefabsList.Clear ();

		//create bars
		float xOffset = (EndPosition.position.x - StartPosition.position.x)/BarCount;
		
		for (int i = 0; i < BarCount; i++) {
			GameObject prefabClone = Instantiate (Prefab);
			prefabClone.transform.position = 
				new Vector3 (
					StartPosition.position.x + (xOffset * i) ,
					StartPosition.position.y
				);
			prefabClone.transform.parent = this.transform;
			PrefabsList.Add (prefabClone);
		}

		BController.BarImages = PrefabsList.ToArray ();
	}
}
