using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTreeState : ScriptableObject {
	[SerializeField]
	public BTreeParameters parameters;

	[SerializeField]
	public StringFloatDictionary floatDictionary = StringFloatDictionary.New<StringFloatDictionary>();
	public Dictionary<string, float> floatParams {
		get { return floatDictionary.dictionary; }
	}
	[SerializeField]
	public StringIntDictionary intDictionary = StringIntDictionary.New<StringIntDictionary>();
	public Dictionary<string, int> intParams {
		get { return intDictionary.dictionary; }
	}
	[SerializeField]
	public StringBoolDictionary boolDictionary = StringBoolDictionary.New<StringBoolDictionary>();
	public Dictionary<string, bool> boolParams {
		get { return boolDictionary.dictionary; }
	}

	void OnEnable() {
		parameters = ScriptableObject.CreateInstance<BTreeParameters>();
	}
}
