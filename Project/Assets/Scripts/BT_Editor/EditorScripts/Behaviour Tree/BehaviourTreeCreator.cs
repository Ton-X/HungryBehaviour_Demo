using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

public class BehaviourTreeCreator : ScriptableObject {
#if UNITY_EDITOR
	private static int dupNum = 0;
	private static string assetPath = "Assets";

	[MenuItem("Assets/Create/Behaviour Tree", priority = 1)]
	public static BehaviourTreeEditor Create() {
		BehaviourTreeEditor bt = ScriptableObject.CreateInstance<BehaviourTreeEditor>();
		bt.btEditorParams = ScriptableObject.CreateInstance<BTreeParameters>();
		bt.btEditorBlackboard = ScriptableObject.CreateInstance<BTreeBlackboardEditor> ();

		string assetName = "NewBehaviourTree.asset";
		if (File.Exists (assetPath + "/" + assetName)) {
			dupNum++;
			assetName = "NewBehaviourTree(" + dupNum + ").asset";
		}

		AssetDatabase.CreateAsset(bt, assetPath + "/" + assetName);
		AssetDatabase.AddObjectToAsset(bt.btEditorParams, bt);
		AssetDatabase.AddObjectToAsset(bt.btEditorBlackboard, bt);
		AssetDatabase.SaveAssets();

		EditorUtility.FocusProjectWindow();
		Selection.activeObject = bt;

		return bt;
	}
#endif
}