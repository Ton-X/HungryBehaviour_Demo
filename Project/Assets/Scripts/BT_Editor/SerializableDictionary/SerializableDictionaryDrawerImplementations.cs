using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
// ---------------
//  String => Int
// ---------------
[UnityEditor.CustomPropertyDrawer(typeof(StringIntDictionary))]
public class StringIntDictionaryDrawer : SerializableDictionaryDrawer<string, int> {
    protected override SerializableKeyValueTemplate<string, int> GetTemplate() {
        return GetGenericTemplate<SerializableStringIntTemplate>();
    }
}
internal class SerializableStringIntTemplate : SerializableKeyValueTemplate<string, int> {}

// ---------------
//  String => Float
// ---------------
[UnityEditor.CustomPropertyDrawer(typeof(StringFloatDictionary))]
public class StringFloatDictionaryDrawer : SerializableDictionaryDrawer<string, float> {
	protected override SerializableKeyValueTemplate<string, float> GetTemplate() {
		return GetGenericTemplate<SerializableStringFloatTemplate>();
	}
}
internal class SerializableStringFloatTemplate : SerializableKeyValueTemplate<string, float> { }

// ---------------
//  String => Bool
// ---------------
[UnityEditor.CustomPropertyDrawer(typeof(StringBoolDictionary))]
public class StringBoolDictionaryDrawer : SerializableDictionaryDrawer<string, bool> {
	protected override SerializableKeyValueTemplate<string, bool> GetTemplate() {
		return GetGenericTemplate<SerializableStringBoolTemplate>();
	}
}
internal class SerializableStringBoolTemplate : SerializableKeyValueTemplate<string, bool> { }

// ---------------
//  GameObject => Float
// ---------------
[UnityEditor.CustomPropertyDrawer(typeof(GameObjectFloatDictionary))]
public class GameObjectFloatDictionaryDrawer : SerializableDictionaryDrawer<GameObject, float> {
    protected override SerializableKeyValueTemplate<GameObject, float> GetTemplate() {
        return GetGenericTemplate<SerializableGameObjectFloatTemplate>();
    }
}
internal class SerializableGameObjectFloatTemplate : SerializableKeyValueTemplate<GameObject, float> {}
#endif