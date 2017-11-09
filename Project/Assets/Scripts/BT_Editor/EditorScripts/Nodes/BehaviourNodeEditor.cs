using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Reflection;
using System;
using System.Linq;

public class BehaviourNodeEditor : BaseLeafNodeEditor
{
	private GameObject goSelected;
	private bool phNode = false;
#if UNITY_EDITOR
	SerializedObject serialObj;
#endif
	private List<FieldInfo> fields = new List<FieldInfo>();

	public string phNodeName = "";

	public BehaviourNodeEditor() {
		title = "Behaviour Node";
	}

	private void OnEnable()
	{
		FieldInfo[] allFields = this.GetType().GetFields();
		foreach (FieldInfo field in allFields)
		{
			List<object> attributes = field.GetCustomAttributes(true)
				.Where(attr => attr.GetType().Equals(typeof(NodeField)))
				.ToList();
			if (field.IsPublic && attributes.Count > 0)
			{
				fields.Add(field);
			}
		}

		nodeRect.height = 60 + (20 * fields.Count);
	}

	public void SetPlaceHolder(bool isPlaceHolder) {
		phNode = isPlaceHolder;
	}

#if UNITY_EDITOR
	public override void Draw ()
	{
		base.Draw();

		GUIStyle style = new GUIStyle (GUI.skin.label);
		style.alignment = TextAnchor.MiddleCenter;
		style.fontStyle = FontStyle.Bold;
		style.fontSize = 14;

		EditorGUILayout.BeginVertical ();
		GUILayout.Label (nodeName, style, GUILayout.ExpandWidth(true));
		if (phNode)
			phNodeName = EditorGUILayout.TextField (phNodeName, GUILayout.ExpandWidth(true));
		GUILayout.Space(5);
		EditorGUILayout.EndVertical ();

		EditorGUILayout.BeginVertical();
		serialObj = new SerializedObject(this);
		EditorGUI.BeginChangeCheck();
		EditorGUIUtility.labelWidth = 80f;
		foreach (FieldInfo field in fields) {
			SerializedProperty serialProp = serialObj.FindProperty(field.Name);
			if (serialProp != null)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PropertyField(serialProp, true);
				EditorGUILayout.EndHorizontal();
			}
		}
		EditorGUILayout.EndVertical();
		if (EditorGUI.EndChangeCheck()) {
			serialObj.ApplyModifiedProperties();
		}
		
		AssetDatabase.Refresh ();
	}
#endif

	public override void NodeDeleted (BaseNodeEditor node)
	{
		if (Parent && Parent.Equals (node)) {
			Parent = null;
		}
	}

	public override BaseNodeRuntime InstantiateRuntimeNode () {
		return null;
	}

	public override BaseNodeRuntime InstantiateRuntimeNode (GameObject gameObj, Type type)
	{
		BaseBehaviourNode node = Activator.CreateInstance (type) as BaseBehaviourNode;
		node.go = gameObj;
		node.editorNode = this;

		runtimeNode = node;

		return runtimeNode;
	}
}

