using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class BarController : MonoBehaviour {
	[Header("Sound")]
	public AudioSource BeatSound;
	public AudioSource HitSound;

	[Header("UI")]
	public Text CaughtText;
	public Text PerfectionText;

	[Header("Particle")]
	public ParticleSystem Particle;

	public GameObject StartPosition;
	public GameObject EndPosition;

	public CounterController Counter;

	public GameObject[] BarImages;

	public List<PassageModel> PassageData = new List<PassageModel>();
	PassageModel _CurrentPassageModel;

	public float RhythmSpeed = 0.7f;

	BarModel _CurrentBarModel;
	float _BarTime;
	int _CurrentBarIdx;

	int _TotalGoodRhythm;

	public float TapCalibration = 0f;

	NoteCreator _NoteCreator;

	public bool GameOver = false;

	[Header("Counter")]
	public int CounterNum = 3;

	public delegate void BarEvent();
	public BarEvent OnBeat;
	public BarEvent OnMisbeat;
	public BarEvent OnBeating;
	public BarEvent OnPassageSucceed;

	// Use this for initialization
	void Start () {
		_NoteCreator = GetComponent<NoteCreator>();

		CreatePassages ();
		RepositionPassages ();

//
//		CreatePassages ();
//
		HideAllBar ();
		StartCoroutine (UpdateBars ());
	}
	
	// Update is called once per frame
	void Update () {
	
		if (!GameOver) {
			_BarTime += Time.deltaTime;
			if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Slash) || 
				Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
				EvaluateNote ();
			}
		}


	}

	void EvaluateNote() {
		float normalizedCaughtTime = (_BarTime + TapCalibration) / RhythmSpeed;
		if (_CurrentBarModel != null) {

			// get note on normalized time
			NoteModel caughtNote =  _CurrentBarModel.GetNoteOnNormalizedTime(normalizedCaughtTime);
			float noteBarValue = _CurrentBarModel.GetNoteBarValue (normalizedCaughtTime);

			if (caughtNote != null) {
				CaughtText.text = caughtNote.Name;

				// check not tapped before
				if (!caughtNote.Tapped) {
					caughtNote.Tapped = true;
					
					float missTime = Mathf.Abs (normalizedCaughtTime - (noteBarValue - caughtNote.GetValueNumber ()));
					
					if (missTime < 0.2f) {
						
						PerfectionText.text = "PERFECT";
						Sequence pseq = DOTween.Sequence ();
						pseq
						// punch scale
							.Append (
								PerfectionText.transform.DOPunchScale (new Vector3 (1.5f, 1.5f), 0.5f, 10, 0.2f)
							)
							//scale back
							.Append (
								PerfectionText.transform.DOScale (new Vector3 (1, 1), 0f)
							);



						Sequence seq = DOTween.Sequence ();
						seq
						// punch scale
							.Append (
							caughtNote.NoteImage.transform.DOPunchScale (new Vector3 (1.5f, 1.5f), 0.5f, 10, 0.2f)
						)
						//scale back
							.Append (
							caughtNote.NoteImage.transform.DOScale (new Vector3 (1, 1), 0f)
						);

						caughtNote.NoteImage.transform.position = 
							new Vector3 (
								caughtNote.NoteImage.transform.position.x,
								caughtNote.NoteImage.transform.position.y + 0.5f
							);
						
						if (caughtNote.Value == NoteModel.BarValue.FULL ||
						    caughtNote.Value == NoteModel.BarValue.HALF) {
							_TotalGoodRhythm++;

							PlayHitSound ();
							if (OnBeat != null) {
								OnBeat ();
							}
						} else {
							_TotalGoodRhythm--;
							// mencet yang kosong
							if (OnMisbeat != null) {
								OnMisbeat ();
							}
						}
						
					} else if (0.2f <= missTime && missTime < 0.45f) {
						PerfectionText.text = "GOOD";
						Sequence pseq = DOTween.Sequence ();
						pseq
						// punch scale
							.Append (
								PerfectionText.transform.DOPunchScale (new Vector3 (1.5f, 1.5f), 0.5f, 10, 0.2f)
							)
							//scale back
							.Append (
								PerfectionText.transform.DOScale (new Vector3 (1, 1), 0f)
							);

						Sequence seq = DOTween.Sequence ();
						seq
						// punch scale
							.Append (
							caughtNote.NoteImage.transform.DOPunchScale (new Vector3 (1.2f, 1.2f), 0.5f, 10, 0.2f)
						)
						//scale back
							.Append (
							caughtNote.NoteImage.transform.DOScale (new Vector3 (1, 1), 0f)
						);

						caughtNote.NoteImage.transform.position = 
							new Vector3 (
								caughtNote.NoteImage.transform.position.x,
								caughtNote.NoteImage.transform.position.y + 0.3f
							);
						
						if (caughtNote.Value == NoteModel.BarValue.FULL ||
						    caughtNote.Value == NoteModel.BarValue.HALF) {
							_TotalGoodRhythm++;

							PlayHitSound ();
							if (OnBeat != null) {
								OnBeat ();
							}
						} else {
							_TotalGoodRhythm--;
							//mencet yang kosong
							if (OnMisbeat != null) {
								OnMisbeat ();
							}
						}
						
					} else if (0.45f <= missTime && missTime < 1f) {
						PerfectionText.text = "BAD";
						Sequence seq = DOTween.Sequence ();
						seq
						// punch scale
							.Append (
							caughtNote.NoteImage.transform.DOScale (new Vector3 (0.2f, 0.2f), 0.2f).SetEase (Ease.OutQuad)
						)
						//scale back
							.Append (
							caughtNote.NoteImage.transform.DOScale (new Vector3 (1, 1), 0f)
						);

						if (OnMisbeat != null) {
							OnMisbeat ();
						}

						caughtNote.NoteImage.transform.position = 
							new Vector3 (
								caughtNote.NoteImage.transform.position.x,
								caughtNote.NoteImage.transform.position.y - 0.5f
							);

					} else {
						// we shouldn't come in here
						PerfectionText.text = "anying~";
					}

				// have tapped
				} else {
					PerfectionText.text = "Misbeat~";
					Sequence seq = DOTween.Sequence ();
					seq
					// punch scale
						.Append (
							caughtNote.NoteImage.transform.DOScale (new Vector3 (0.2f, 0.2f), 0.2f).SetEase (Ease.OutQuad)
						)
						//scale back
						.Append (
							caughtNote.NoteImage.transform.DOScale (new Vector3 (1, 1), 0f)
						);
					
					_TotalGoodRhythm--;

					caughtNote.NoteImage.transform.position = 
						new Vector3 (
							caughtNote.NoteImage.transform.position.x,
							caughtNote.NoteImage.transform.position.y - 0.5f
						);

					if (OnMisbeat != null) {
						OnMisbeat ();
					}
				}

			}
		}
	}


	IEnumerator UpdateBars() {

		_CurrentPassageModel = PassageData [0];

		_CurrentBarIdx = 0;
		int counterIndex = 0;
		bool counting = true;

		HideAllBar ();

		bool beginningCounting = true;

		int totalNotes = 0;
		while(!GameOver) {


			if (_CurrentBarIdx >= BarImages.Length) {
				_CurrentBarIdx = 0;

				HideAllBar ();

				counterIndex = 0;
				counting = true;
				beginningCounting = true;

				// TODO: Kalau sudah perfect pindah ke passage berikutnya
				Debug.Log("Good rhythm:"+_TotalGoodRhythm + ">= totalNotes:" + totalNotes);
				if (_TotalGoodRhythm >= totalNotes) {
					Debug.Log ("ALL GOOD!");

					Particle.Play ();

					if (OnPassageSucceed != null) {
						OnPassageSucceed ();
					}


					// destroy all game object
					DestroyPassageNotes (_CurrentPassageModel);
					PassageData.Remove (_CurrentPassageModel);
					RepositionPassages ();

					if (PassageData.Count > 0) {
						_CurrentPassageModel = PassageData [0];

						//add new passage
						CreatePassage();
						RepositionPassages ();

					} else {
						GameOver = true;
					}


				} else {
					ResetTappedNotes (_CurrentPassageModel);
				}
			}

			if (!GameOver) {
				if (counterIndex < CounterNum && counting == true) {
					
					//TODO: Do it only once
					if (counterIndex <= 0) {
						totalNotes = GetNoteCount (_CurrentPassageModel);
						_TotalGoodRhythm = 0;
					}

					if (beginningCounting) {
						beginningCounting = false;
						Counter.ShowAll ();
					} else {
						Counter.Hide (CounterNum - counterIndex-1);
						counterIndex++;
					}


					counting = true;
					
				} else {
					counting = false;
					counterIndex = 0;
					Counter.HideAll ();
					
					_CurrentBarModel = _CurrentPassageModel.BarData[_CurrentBarIdx];
					
					_BarTime = 0;				
					LitBar (_CurrentBarIdx);
				}

				if (OnBeating != null) {
					OnBeating ();
				}
				// play sound
				BeatSound.Play ();
				yield return new WaitForSeconds (RhythmSpeed);
				
				if (!counting) {
					_CurrentBarIdx++;
				}
			}

		}
	}

	void HideAllBar() {
		foreach (GameObject go in BarImages){
			go.SetActive (false);
		}
	}

	void LitBar(int index) {
		for (int i = 0; i < BarImages.Length; i++) {
			if (i == index) {
				BarImages [i].SetActive (true);
			} else {
				BarImages [i].SetActive (false);
			}
		}
	}

	void CreatePassages () {
		for (int i = 0; i < 3; i++) {
			PassageModel newPassageModel = new PassageModel ();
			newPassageModel.BarData = _NoteCreator.GetRandomNotes ();

			PassageData.Add (newPassageModel);

		}
	}

	void CreatePassage () {
		PassageModel newPassageModel = new PassageModel ();
		newPassageModel.BarData = _NoteCreator.GetRandomNotes ();

		PassageData.Add (newPassageModel);
	}

	void RepositionPassages() {
		for (int i = 0; i < PassageData.Count; i++) {
			List<BarModel> barData = PassageData [i].BarData;
			for (int j = 0; j < barData.Count; j++) {
				List<NoteModel> noteData = barData [j].Notes;
				for (int k = 0; k < noteData.Count; k++) {
					NoteModel noteModel = noteData[k];
					noteModel.NoteImage.transform.position = 
						new Vector3(
							noteModel.NoteImage.transform.position.x,
							StartPosition.transform.position.y - (i * 1.5f));
				}
			}
		}
	}

	int GetNoteCount(PassageModel passageModel) {
		int noteCount = 0;
		List<BarModel> barData = passageModel.BarData;
		for (int j = 0; j < barData.Count; j++) {
			List<NoteModel> noteData = barData [j].Notes;
			for (int k = 0; k < noteData.Count; k++) {
				NoteModel noteModel = noteData [k];

				if (noteModel.Value == NoteModel.BarValue.FULL ||
					noteModel.Value == NoteModel.BarValue.HALF) {
					noteCount++;
				}

			}
		}
		return noteCount;
	}
		

	void DestroyPassageNotes(PassageModel passageModel) {
		List<BarModel> barData = passageModel.BarData;
		for (int j = 0; j < barData.Count; j++) {
			List<NoteModel> noteData = barData [j].Notes;
			for (int k = 0; k < noteData.Count; k++) {
				NoteModel noteModel = noteData [k];
				Destroy (noteModel.NoteImage);
			}
		}	
	}

	void ResetTappedNotes(PassageModel passageModel) {
		List<BarModel> barData = passageModel.BarData;
		for (int j = 0; j < barData.Count; j++) {
			List<NoteModel> noteData = barData [j].Notes;
			for (int k = 0; k < noteData.Count; k++) {
				NoteModel noteModel = noteData [k];
				noteModel.Tapped = false;

				noteModel.NoteImage.transform.position = new 
					Vector3(
						noteModel.NoteImage.transform.position.x,
						StartPosition.transform.position.y
					);

			}
		}	
	}

	void PlayHitSound() {
		HitSound.pitch = Random.Range (1, 2f);
		HitSound.Play ();
	}
}
