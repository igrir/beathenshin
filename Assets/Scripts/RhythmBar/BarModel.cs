using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BarModel{
	// value between 1 to 0
	public float CurrentBarValue = 1;
	public List<NoteModel> Notes = new List<NoteModel>();

	public NoteModel GetNoteOnNormalizedTime (float normalizedTime) {
		float value = 0;
		int index = 0;
		while (value <= 1 && index < Notes.Count) {
			NoteModel currentNoteModel = Notes [index];


			if (currentNoteModel.Value == NoteModel.BarValue.FULL || 
				currentNoteModel.Value == NoteModel.BarValue.FULL_REST ||
				currentNoteModel.Value == NoteModel.BarValue.TRIPLET) {
				value += 1f;
			}else if (currentNoteModel.Value == NoteModel.BarValue.HALF || 
				currentNoteModel.Value == NoteModel.BarValue.HALF_REST) {
				value += 0.5f;
			}

			if (value >= normalizedTime) {
				return currentNoteModel;
			}

			index++;

		}
		return null;
	}

	public float GetNoteBarValue (float normalizedTime) {
		float value = 0;
		int index = 0;
		while (value <= 1 && index < Notes.Count) {
			NoteModel currentNoteModel = Notes [index];


			if (currentNoteModel.Value == NoteModel.BarValue.FULL || 
				currentNoteModel.Value == NoteModel.BarValue.FULL_REST ||
				currentNoteModel.Value == NoteModel.BarValue.TRIPLET) {
				value += 1f;
			}else if (currentNoteModel.Value == NoteModel.BarValue.HALF || 
				currentNoteModel.Value == NoteModel.BarValue.HALF_REST) {
				value += 0.5f;
			}

			if (value >= normalizedTime) {
				return value;
			}

			index++;

		}
		return 0;
	}
}
