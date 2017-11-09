using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.IO;
using System.Reflection;
using System.Linq;

#if UNITY_EDITOR
public class NodeEditorWindow : EditorWindow {

	private BehaviourTreeEditor bt;
	private Vector2 mousePos;
	private BaseNodeEditor selectedNode;
	bool isNodeSelected;

	private BaseNodeEditor root = null;

	private Rect leftSideRect;

	EditorGUISplitView horizontalSplitView = new EditorGUISplitView(EditorGUISplitView.Direction.Horizontal);
	EditorGUISplitView verticalSplitView = new EditorGUISplitView(EditorGUISplitView.Direction.Vertical);

	[MenuItem("Tools/BT Editor")]
	private static void ShowWindow() {
		NodeEditorWindow editor = GetWindow<NodeEditorWindow>();
		editor.titleContent = new GUIContent("BT Editor");

	}

	void OnEnable() {
		if (EditorPrefs.HasKey("AssetPath") && File.Exists(EditorPrefs.GetString("AssetPath"))) {
			string assetPath = EditorPrefs.GetString("AssetPath");
			bt = (BehaviourTreeEditor) AssetDatabase.LoadAssetAtPath(assetPath, typeof(BehaviourTreeEditor));
			if (bt != null && bt.nodes == null) {
				bt.nodes = new List<BaseNodeEditor> ();
			}

			if (bt != null && bt.nodes.Count > 0)
				root = bt.nodes [0];
		}


		AssetDatabase.Refresh ();
		AssetDatabase.SaveAssets ();
	}

	void OnDestroy() {
		AssetDatabase.SaveAssets();
	}

	void Update() {
		if (bt == null)
			return;

		Repaint ();
	}

	private void OnGUI() {

		if (bt == null) {
			DrawToolbar();
			return;
		}

		Event e = Event.current;
		mousePos = e.mousePosition;

		if (mousePos.x >= 250 && mousePos.y >= 20 && e.button == 1 && e.type == EventType.MouseDown) {
			CheckMousePos ();

			if (!isNodeSelected) {
				
				if (root == null) {
					GenericMenu menu = new GenericMenu ();
					menu.AddItem(new GUIContent("Add root/Sequence"), false, AddSequence);
					menu.AddItem(new GUIContent("Add root/Selector"), false, AddSelector);
					menu.AddItem(new GUIContent("Add root/Inverter"), false, AddInverter);

					menu.ShowAsContext ();
					e.Use ();
				} 
			} else {
				var behavNodes = Assembly.GetExecutingAssembly ().GetTypes ()
					.Where (t => t.BaseType.Equals (typeof(BaseBehaviourNode)))
					.ToList ();

				GenericMenu menu = new GenericMenu ();
				if (!selectedNode.GetType ().Equals (typeof(BehaviourNodeEditor)) && !selectedNode.GetType().BaseType.Equals(typeof(BehaviourNodeEditor))) {
					menu.AddItem (new GUIContent ("Add child/Sequence"), false, AddSequenceChild);
					menu.AddItem (new GUIContent ("Add child/Selector"), false, AddSelectorChild);
					menu.AddItem (new GUIContent ("Add child/Inverter"), false, AddInverterChild);
					menu.AddItem (new GUIContent ("Add child/Behaviour/PlaceHolder"), false, AddBehaviour, "PlaceHolder");
					foreach (var nodeType in behavNodes) {
						menu.AddItem (new GUIContent ("Add child/Behaviour/" + nodeType.Name), false, AddBehaviour, nodeType);
					}

					menu.AddSeparator ("");
				}
				menu.AddItem(new GUIContent("Delete Node"), false, DeleteNode);

				menu.ShowAsContext ();
				e.Use ();
			}
		}

		if (bt.nodes.Count > 0 && root == null) {
			root = bt.nodes [0];
		}
			
		leftSideRect = horizontalSplitView.BeginSplitView ();
		verticalSplitView.BeginSplitView ();

		DrawParameters ();

		verticalSplitView.Split ();

		DrawBlackboard ();

		verticalSplitView.EndSplitView ();
		horizontalSplitView.Split ();
		DrawToolbar();

		foreach (BaseNodeEditor node in bt.nodes) {
			node.DrawConnections ();
		}
	
		BeginWindows();
		{
			for (int i = 0; i < bt.nodes.Count; i++) {
				Rect prevRect = bt.nodes[i].nodeRect;
				bt.nodes[i].nodeRect = GUI.Window(i, bt.nodes[i].nodeRect, DrawNode, new GUIContent(bt.nodes[i].title));
				if (bt.nodes [i].nodeRect.x < leftSideRect.xMax - bt.nodes [i].nodeRect.x + 10)
					bt.nodes [i].nodeRect.x = prevRect.x;
				if (bt.nodes [i].nodeRect.y < 20)
					bt.nodes [i].nodeRect.y = prevRect.y;
			}
		}
		EndWindows();

		horizontalSplitView.EndSplitView ();
	}

	private void DrawNode(int id) {
		bt.nodes[id].Draw();
		GUI.DragWindow();
	}

	private void DrawToolbar ()
	{
		GUILayout.BeginHorizontal();
		if (bt != null)
			GUILayout.Space (5);
		if (GUILayout.Button("New...", EditorStyles.toolbarButton)) {
			CreateNewBT();
		}
		if (GUILayout.Button ("Save Tree", EditorStyles.toolbarButton)) {
			SaveBT();
		}
		if (GUILayout.Button ("Load Tree", EditorStyles.toolbarButton)) {
			LoadBT();
		}
		GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();
	}

	private void DrawParameters() {
		EditorGUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		GUILayout.Label ("PARAMETERS", EditorStyles.boldLabel);
		GUILayout.FlexibleSpace ();
		EditorGUILayout.EndHorizontal ();

		Dictionary<string, float> floatPar = bt.btEditorParams.floatParams;
		Dictionary<string, int> intPar = bt.btEditorParams.intParams;
		Dictionary<string, bool> boolPar = bt.btEditorParams.boolParams;

		GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
		labelStyle.fontSize = 11;
		labelStyle.fontStyle = FontStyle.BoldAndItalic;

		EditorGUILayout.BeginVertical();

		EditorGUI.BeginChangeCheck();

		string keyToRemove;
		// Float parameters management
		if (floatPar.Count > 0) {
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Float Params", labelStyle);
			GUILayout.FlexibleSpace ();
			EditorGUILayout.EndHorizontal ();
			GUILayout.Space (5);
		}

		keyToRemove = null;
		Dictionary<string,float> modifiedFloatEntries = new Dictionary<string,float> ();
		foreach (KeyValuePair<string,float> kvp in floatPar) {
			EditorGUILayout.BeginHorizontal();
			string key = EditorGUILayout.TextField(kvp.Key, GUILayout.Width(120));
			float value = EditorGUILayout.FloatField(kvp.Value, GUILayout.Width(40));

			// If the key was modified, remove the old one and add the new with the same value
			if (!key.Equals (kvp.Key)) {
				keyToRemove = kvp.Key;
				modifiedFloatEntries.Add (key, value);
			}

			// If the value was modified, update the value of the related key
			if (!value.Equals (kvp.Value)) {
				modifiedFloatEntries.Add(key, value);
			}

			GUILayout.FlexibleSpace();
			if (GUILayout.Button ("-", GUILayout.ExpandWidth(false))) {
				keyToRemove = kvp.Key;
			}
			EditorGUILayout.EndHorizontal();
		}
		if (keyToRemove != null)
		{
			Undo.RecordObject(bt.btEditorParams, "Remove parameter");
			floatPar.Remove(keyToRemove);
		}
		foreach (var e in modifiedFloatEntries) {
			if (floatPar.ContainsKey(e.Key))
			{
				Undo.RecordObject(bt.btEditorParams, "Modify value");
				floatPar[e.Key] = e.Value;
			} else
			{
				Undo.RecordObject(bt.btEditorParams, "Modify key");
				floatPar.Add(e.Key, e.Value);
			}
		}

		GUILayout.Space (10);

		// Int parameters management
		if (intPar.Count > 0) {
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Int Params", labelStyle);
			GUILayout.FlexibleSpace ();
			EditorGUILayout.EndHorizontal ();
			GUILayout.Space (5);
		}

		keyToRemove = null;
		Dictionary<string,int> modifiedIntEntries = new Dictionary<string,int> ();
		foreach (KeyValuePair<string,int> kvp in intPar) {
			EditorGUILayout.BeginHorizontal();
			string key = EditorGUILayout.TextField(kvp.Key, GUILayout.Width(120));
			int value = EditorGUILayout.IntField(kvp.Value, GUILayout.Width(40));

			// If the key was modified, remove the old one and add the new with the same value
			if (!key.Equals (kvp.Key)) {
				keyToRemove = kvp.Key;
				modifiedIntEntries.Add (key, value);
			}

			// If the value was modified, update the value of the related key
			if (!value.Equals (kvp.Value)) {
				modifiedIntEntries.Add(key, value);
			}

			GUILayout.FlexibleSpace();
			if (GUILayout.Button ("-", GUILayout.ExpandWidth(false))) {
				keyToRemove = kvp.Key;
			}
			EditorGUILayout.EndHorizontal();
		}
		if (keyToRemove != null)
		{
			Undo.RecordObject(bt.btEditorParams, "Remove parameter");
			intPar.Remove(keyToRemove);
		}
		foreach (var e in modifiedIntEntries) {
			if (intPar.ContainsKey(e.Key))
			{
				Undo.RecordObject(bt.btEditorParams, "Modify value");
				intPar[e.Key] = e.Value;
			} else
			{
				Undo.RecordObject(bt.btEditorParams, "Modify key");
				intPar.Add(e.Key, e.Value);
			}
		}

		GUILayout.Space (10);

		// Bool parameters management
		if (boolPar.Count > 0) {
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Bool Params", labelStyle);
			GUILayout.FlexibleSpace ();
			EditorGUILayout.EndHorizontal ();
			GUILayout.Space (5);
		}

		keyToRemove = null;
		Dictionary<string,bool> modifiedBoolEntries = new Dictionary<string,bool> ();
		foreach (KeyValuePair<string,bool> kvp in boolPar) {
			EditorGUILayout.BeginHorizontal();
			string key = EditorGUILayout.TextField(kvp.Key, GUILayout.Width(120));
			bool value = EditorGUILayout.Toggle(kvp.Value);

			// If the key was modified, remove the old one and add the new with the same value
			if (!key.Equals (kvp.Key)) {
				keyToRemove = kvp.Key;
				modifiedBoolEntries.Add (key, value);
			}

			// If the value was modified, update the value of the related key
			if (!value.Equals (kvp.Value)) {
				modifiedBoolEntries.Add(key, value);
			}

			GUILayout.FlexibleSpace();
			if (GUILayout.Button ("-", GUILayout.ExpandWidth(false))) {
				keyToRemove = kvp.Key;
			}
			EditorGUILayout.EndHorizontal();
		}

		if (keyToRemove != null)
		{
			Undo.RecordObject(bt.btEditorParams, "Remove parameter");
			boolPar.Remove(keyToRemove);
		}
		foreach (var e in modifiedBoolEntries) {
			if (boolPar.ContainsKey(e.Key))
			{
				Undo.RecordObject(bt.btEditorParams, "Modify value");
				boolPar[e.Key] = e.Value;
			} else
			{
				Undo.RecordObject(bt.btEditorParams, "Modify key");
				boolPar.Add(e.Key, e.Value);
			}
		}

		GUILayout.Space (10);
		EditorGUILayout.LabelField("ParamCount: " + (floatPar.Count+intPar.Count+boolPar.Count), EditorStyles.label);

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("+")) {
			// Adding a parameter float/int/bool
			int keyCode = (int) (-GetHashCode () * UnityEngine.Random.value);
			string dictKey = "Param" + keyCode;
			GenericMenu menu = new GenericMenu();
			menu.AddItem(new GUIContent("Float"), false, () => {
				Undo.RecordObject(bt.btEditorParams, "Add parameter");
				floatPar.Add(dictKey, 0f);
			});
			menu.AddItem(new GUIContent("Int"), false, () => {
				Undo.RecordObject(bt.btEditorParams, "Add parameter");
				intPar.Add(dictKey, 0);
			});
			menu.AddItem(new GUIContent("Bool"), false, () => {
				Undo.RecordObject(bt.btEditorParams, "Add parameter");
				boolPar.Add(dictKey, false);
			});

			menu.ShowAsContext();
		}

		if (EditorGUI.EndChangeCheck())
		{
			EditorUtility.SetDirty(bt.btEditorParams);
		}

		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
	}

	private void DrawBlackboard() {
		EditorGUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		GUILayout.Label ("BLACKBOARD", EditorStyles.boldLabel);
		GUILayout.FlexibleSpace ();
		EditorGUILayout.EndHorizontal ();

		List<string> blackboardList = bt.btEditorBlackboard.blackboard;
		List<string> modifiedItems = new List<string>();

		EditorGUILayout.BeginVertical();
		string itemToRemove = null;

		EditorGUI.BeginChangeCheck();

		for (int i = 0; i < blackboardList.Count; i++) {
			EditorGUILayout.BeginHorizontal();
			modifiedItems.Add(blackboardList[i]);
			modifiedItems[i] = EditorGUILayout.TextField(modifiedItems[i], GUILayout.Width(120));
			
			if (!modifiedItems[i].Equals(blackboardList[i])) {
				Undo.RecordObject(bt.btEditorBlackboard, "Modify blackboard var");
				blackboardList[i] = modifiedItems[i];
			}

			if (GUILayout.Button("-", GUILayout.ExpandWidth(false))) {
				itemToRemove = blackboardList[i];
			}
			
			EditorGUILayout.EndHorizontal();
		}

		if (itemToRemove != null) {
			Undo.RecordObject(bt.btEditorBlackboard, "Remove blackboard var");
			blackboardList.Remove(itemToRemove);
		}


		GUILayout.Space(10);
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("+")) {
			// Adding a static variable to the blackboard
			int keyCode = (int) (-GetHashCode() * UnityEngine.Random.value);
			string varItem = "Var" + keyCode;
			Undo.RecordObject(bt.btEditorBlackboard, "Add blackboard var");
			blackboardList.Add(varItem);
		}

		if (EditorGUI.EndChangeCheck())
		{
			EditorUtility.SetDirty(bt.btEditorBlackboard);
		}

		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
	}

	private void CreateNewBT() {
		
		bt = BehaviourTreeCreator.Create();
		if (bt != null) {
			root = null;
			bt.nodes = new List<BaseNodeEditor>();
			string relPath = AssetDatabase.GetAssetPath(bt);
			EditorPrefs.SetString("AssetPath", relPath);
		}
	}

	private void SaveBT() {
		if (root != null) {
			string relPath = AssetDatabase.GetAssetPath(bt);
			EditorPrefs.SetString("AssetPath", relPath);

			foreach (var node in bt.nodes) {
				Debug.Log(node.title);
			}

			Debug.Log("BT Saved!");
		} else
			Debug.Log("No BT to save!");
		AssetDatabase.SaveAssets ();
	}

	private void LoadBT() {
		string absPath = EditorUtility.OpenFilePanel("Select a Behaviour Tree...", Application.dataPath, "asset");
		if (absPath.StartsWith(Application.dataPath)) {
			string relPath = absPath.Substring(Application.dataPath.Length - "Assets".Length);
			bt = (BehaviourTreeEditor) AssetDatabase.LoadAssetAtPath(relPath, typeof(BehaviourTreeEditor));

			if (bt != null && bt.nodes == null) {
				bt.nodes = new List<BaseNodeEditor> ();
				EditorPrefs.SetString("AssetPath", relPath);
			}
			root = null;

		}
		Debug.Log("BT Loaded!");
	}


	void AddSequence() {
		SequenceNodeEditor seqNode = CreateInstance<SequenceNodeEditor>();
		seqNode.nodeRect = new Rect (mousePos.x, mousePos.y, 120, 50);
		bt.nodes.Add (seqNode);

		AssetDatabase.AddObjectToAsset (seqNode, bt);
	}

	void AddSequenceChild() {
		SequenceNodeEditor seqNode = CreateInstance<SequenceNodeEditor>();
		seqNode.nodeRect = new Rect (selectedNode.nodeRect.x, selectedNode.nodeRect.y + (selectedNode.nodeRect.height * 2), 120, 50);
		bt.nodes.Add (seqNode);
		selectedNode.AddChild (seqNode);

		AssetDatabase.AddObjectToAsset (seqNode, bt);
	}

	void AddSelector() {
		SelectorNodeEditor selNode = CreateInstance<SelectorNodeEditor>();
		selNode.nodeRect = new Rect (mousePos.x, mousePos.y, 120, 50);
		bt.nodes.Add (selNode);

		AssetDatabase.AddObjectToAsset (selNode, bt);
	}

	void AddSelectorChild() {
		SelectorNodeEditor selNode = CreateInstance<SelectorNodeEditor>();
		selNode.nodeRect = new Rect (selectedNode.nodeRect.x, selectedNode.nodeRect.y + (selectedNode.nodeRect.height * 2), 120, 50);;
		bt.nodes.Add (selNode);
		selectedNode.AddChild (selNode);

		AssetDatabase.AddObjectToAsset (selNode, bt);
	}

	void AddInverter() {
		InverterNodeEditor invNode = CreateInstance<InverterNodeEditor>();
		invNode.nodeRect = new Rect (mousePos.x, mousePos.y, 120, 50);
		bt.nodes.Add (invNode);

		AssetDatabase.AddObjectToAsset (invNode, bt);
	}

	void AddInverterChild() {
		InverterNodeEditor invNode = CreateInstance<InverterNodeEditor>();
		invNode.nodeRect = new Rect (selectedNode.nodeRect.x, selectedNode.nodeRect.y + (selectedNode.nodeRect.height * 2), 120, 50);
		bt.nodes.Add (invNode);
		selectedNode.AddChild (invNode);

		AssetDatabase.AddObjectToAsset (invNode, bt);
	}

	void AddBehaviour(object obj) {
		BehaviourNodeEditor behavNode;
		if (obj.ToString() != "PlaceHolder") {
			Type runtimeType = obj as Type;
			List<Type> bNodesEditor = Assembly.GetExecutingAssembly().GetTypes()
				.Where(t => t.BaseType.Equals(typeof(BehaviourNodeEditor)))
				.ToList();
			
			Type targetType = null;
			foreach (Type editorType in bNodesEditor) {
				if (editorType.Name.Contains(runtimeType.Name))
					targetType = editorType;
			}

			behavNode = ScriptableObject.CreateInstance(targetType) as BehaviourNodeEditor;
		} else {
			behavNode = CreateInstance<BehaviourNodeEditor>();
			behavNode.SetPlaceHolder(true);
		}
		
		behavNode.nodeName = obj.ToString();
		behavNode.nodeRect = new Rect (selectedNode.nodeRect.x, selectedNode.nodeRect.y + (selectedNode.nodeRect.height * 2), 200, behavNode.nodeRect.height);
		bt.nodes.Add (behavNode);
		selectedNode.AddChild (behavNode);

		AssetDatabase.AddObjectToAsset (behavNode, bt);
	}

	void DeleteNode() {
		BaseNodeEditor toRemove = selectedNode;
		bt.nodes.Remove(toRemove);
		if (toRemove.Equals (root)) {
			root = null;
			foreach (BaseNodeEditor node in bt.nodes) {
				UnityEngine.Object.DestroyImmediate (node, true);
			}
			bt.nodes.Clear ();
		} else {
			foreach (BaseNodeEditor node in bt.nodes) {
				node.NodeDeleted (toRemove);
			}
		}
		UnityEngine.Object.DestroyImmediate (toRemove, true);
		AssetDatabase.SaveAssets ();
	}

	public static void DrawNodeLines(Rect start, Rect end) {
		Handles.color = Color.black;
		Vector3 startPos = start.center;
		Vector3 endPos = end.center;
		Handles.DrawLine(startPos, endPos);
	}

	void CheckMousePos () {
		isNodeSelected = false;
		selectedNode = null;

		for (int i = 0; i < bt.nodes.Count; i++) {
			if (bt.nodes [i].nodeRect.Contains (mousePos)) {
				isNodeSelected = true;
				selectedNode = bt.nodes [i];
				break;
			}
		}
	}

}
#endif
