using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTreeParameters : ScriptableObject {
	
	[SerializeField]
	private StringFloatDictionary floatDictionary = StringFloatDictionary.New<StringFloatDictionary>();
	public Dictionary<string, float> floatParams {
		get { return floatDictionary.dictionary; }
	}
	[SerializeField]
	private StringIntDictionary intDictionary = StringIntDictionary.New<StringIntDictionary>();
	public Dictionary<string, int> intParams {
		get { return intDictionary.dictionary; }
	}
	[SerializeField]
	private StringBoolDictionary boolDictionary = StringBoolDictionary.New<StringBoolDictionary>();
	public Dictionary<string, bool> boolParams {
		get { return boolDictionary.dictionary; }
	}
}
