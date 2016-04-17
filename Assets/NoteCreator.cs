using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class NoteCreator : MonoBehaviour {

	public enum NoteType{
		QUARTER_NOTE,
		EIGHTH_NOTE,
		TRIPLET_NOTE,
		QUARTER_REST,
		EIGHTH_REST,
	}

	[Header("Execute")]
	public bool CreateNotes;
	public bool RandomizeNotes;

	[Header("Parent properties")]
	public BarController BController;

	[Header("Prefab Properties")]
	public GameObject FullNotePrefab;
	public GameObject FullRestPrefab;
	public GameObject HalfNotePrefab;
	public GameObject HalfRestPrefab;
	public GameObject TripletPrefab;

	[Header("Position Properties")]
	public Transform StartPosition;
	public Transform EndPosition;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
//		if (CreateNotes) {
//			CreateNotes = false;
//		}
//
//		if (RandomizeNotes) {
//			RandomizeNotes = false;
//			GetRandomNotes ();
//		}
	}

	public List<BarModel> ExecuteCreateNotes(List<NoteModel> notesData) {

		List<BarModel> BarData = new List<BarModel>();

		List<NoteModel> NotesData = notesData;
		if (NotesData == null) {
			NotesData = new List<NoteModel> ();
		}

		BarData.Clear ();
		for (int i = 0; i < BController.BarImages.Length; i++) {
			BarModel barModel = new BarModel ();
			BarData.Add (barModel);
		}


		int currentBar = 0;
		BarModel currentBarModel = BarData [currentBar];

		for (int i = 0; i < NotesData.Count; i++) {

			if (currentBar < BarData.Count) {
				NoteModel currentNote = NotesData[i];

				
				// cek muat
				if (currentBarModel.CurrentBarValue - currentNote.GetValueNumber() >= 0) {
					// kalau muat masukin
					currentBarModel.CurrentBarValue -= currentNote.GetValueNumber();
					currentBarModel.Notes.Add (currentNote);
				} else {
					// nggak muat

					// TODO:
					// masukin sisanya ke dalam bar ini
					NoteModel newNote = new NoteModel ();
					newNote.Name = "Tambahan";
					float remainingValue = currentBarModel.CurrentBarValue;
					if (remainingValue == 0) {
						// langsung pindah bar

					} else if (Mathf.Approximately(remainingValue,0.5f)) {

						if (currentNote.Value == NoteModel.BarValue.FULL_REST) {
							newNote.Value = NoteModel.BarValue.HALF_REST;

							currentBarModel.Notes.Add (newNote);

							// terus sisanya pindahin ke bar selanjutnya
							currentNote.Value = NoteModel.BarValue.HALF_REST;

						} else if (currentNote.Value == NoteModel.BarValue.FULL) {
							newNote.Value = NoteModel.BarValue.HALF;
							currentBarModel.Notes.Add (newNote);

							// terus sisanya pindahin ke bar selanjutnya
							currentNote.Value = NoteModel.BarValue.HALF;
						}
					}

					// pindah bar
					currentBar++;
					if (currentBar < BarData.Count) {
						currentBarModel = BarData [currentBar];						
						currentBarModel.CurrentBarValue -= currentNote.GetValueNumber();
						currentBarModel.Notes.Add (currentNote);
					}


				}
			}
		}
			
		// ------- DRAW NOTES
		DrawNotes(BarData);

		return BarData;
	}

	void DrawNotes(List<BarModel> BarData) {

		List<GameObject> NotesObject = new List<GameObject> ();

		// hitung lebar bar individual
		float barWidth = (EndPosition.position.x - StartPosition.position.x)/BController.BarImages.Length;


		for (int i = 0; i < BarData.Count; i++) {
			BarModel currentBarModel = BarData[i];

			float currentXPos = (StartPosition.position.x + (i*barWidth));

			for (int j = 0; j < currentBarModel.Notes.Count; j++) {
				NoteModel currentNoteModel = currentBarModel.Notes [j];

				if (currentNoteModel.Value == NoteModel.BarValue.FULL) { 
					// create the prefab
					GameObject noteClone = Instantiate(FullNotePrefab);
					noteClone.transform.position = new Vector3 (currentXPos, StartPosition.position.y);
					noteClone.transform.parent = this.transform;
					currentNoteModel.NoteImage = noteClone;
					NotesObject.Add (noteClone);


					currentXPos += barWidth;
				} else if (currentNoteModel.Value == NoteModel.BarValue.FULL_REST) {
					// create the prefab
					GameObject noteClone = Instantiate(FullRestPrefab);
					noteClone.transform.position = new Vector3 (currentXPos, StartPosition.position.y);
					noteClone.transform.parent = this.transform;
					currentNoteModel.NoteImage = noteClone;
					NotesObject.Add (noteClone);

					currentXPos += barWidth;
				} else if (currentNoteModel.Value == NoteModel.BarValue.HALF) {

					// create the prefab
					GameObject noteClone = Instantiate(HalfNotePrefab);
					noteClone.transform.position = new Vector3 (currentXPos, StartPosition.position.y);
					noteClone.transform.parent = this.transform;
					currentNoteModel.NoteImage = noteClone;
					NotesObject.Add (noteClone);

					// for next prefab
					currentXPos += (barWidth / 2);
				}else if(currentNoteModel.Value == NoteModel.BarValue.HALF_REST) {

					// create the prefab
					GameObject noteClone = Instantiate(HalfRestPrefab);
					noteClone.transform.position = new Vector3 (currentXPos, StartPosition.position.y);
					noteClone.transform.parent = this.transform;
					currentNoteModel.NoteImage = noteClone;
					NotesObject.Add (noteClone);

					// for next prefab
					currentXPos += (barWidth / 2);
				}else if(currentNoteModel.Value == NoteModel.BarValue.TRIPLET) {

					// create the prefab
					GameObject noteClone = Instantiate(TripletPrefab);
					noteClone.transform.position = new Vector3 (currentXPos, StartPosition.position.y);
					noteClone.transform.parent = this.transform;
					currentNoteModel.NoteImage = noteClone;
					NotesObject.Add (noteClone);

					// for next prefab
					currentXPos += barWidth;
				}


			}
		}
	}

	public List<BarModel> GetRandomNotes(){
	
		List<NoteModel> NotesData = new List<NoteModel>();

		float barValue = 0;
		while (barValue <= 8) {
			int randomNumber = Random.Range (0, 4);

			NoteModel noteModel = new NoteModel ();

			switch(randomNumber) {
			case 0:
				noteModel.Value = NoteModel.BarValue.FULL;
				barValue += 1f;
				break;
			case 1:
				noteModel.Value = NoteModel.BarValue.FULL_REST;
				barValue += 1f;
				break;
			case 2:
				noteModel.Value = NoteModel.BarValue.HALF;
				barValue += 0.5f;
				break;
			case 3:
				noteModel.Value = NoteModel.BarValue.HALF_REST;
				barValue += 0.5f;
				break;
			case 4:
				noteModel.Value = NoteModel.BarValue.TRIPLET;
				barValue += 1f;
				break;
			}

			noteModel.Name = barValue.ToString();

			NotesData.Add (noteModel);

		}

		return ExecuteCreateNotes (NotesData);

	}
}
