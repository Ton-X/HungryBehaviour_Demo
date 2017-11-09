using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BTreeBlackboardRuntime {

	// Contains the dynamic status of the btree

	[SerializeField]
	private StringFloatDictionary blackboardDictionary = StringFloatDictionary.New<StringFloatDictionary>();
	public Dictionary<string, float> blackboard {
		get { return blackboardDictionary.dictionary; }
	}
}
