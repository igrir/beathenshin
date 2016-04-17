using UnityEngine;
using System.Collections;

[System.Serializable]
public class NoteModel{
	public enum BarValue{
		HALF,
		FULL,
		TRIPLET,
		HALF_REST,
		FULL_REST,
	}

	public BarValue Value;
	public string Name;

	public bool Tapped = false;

	public GameObject NoteImage;

	public float GetValueNumber()
	{
		if (Value == NoteModel.BarValue.FULL || 
			Value == NoteModel.BarValue.FULL_REST ||
			Value == NoteModel.BarValue.TRIPLET) {
			return 1f;
		}else if (	Value == NoteModel.BarValue.HALF || 
					Value == NoteModel.BarValue.HALF_REST) {
			return 0.5f;
		}
		return 0;
	}
}
