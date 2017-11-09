using System;
 
using UnityEngine;
 
// ---------------
//  String => Int
// ---------------
[Serializable]
public class StringIntDictionary : SerializableDictionary<string, int> {}

// ---------------
//  String => Float
// ---------------
[Serializable]
public class StringFloatDictionary : SerializableDictionary<string, float> { }

// ---------------
//  String => Bool
// ---------------
[Serializable]
public class StringBoolDictionary : SerializableDictionary<string, bool> { }

// ---------------
//  GameObject => Float
// ---------------
[Serializable]
public class GameObjectFloatDictionary : SerializableDictionary<GameObject, float> {}
