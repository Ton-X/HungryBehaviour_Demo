using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BaseNodeEditor : ScriptableObject {

	public string title;
	public Rect nodeRect;

	public List<BaseNodeEditor> children;

	public BaseNodeEditor Parent {
		get { return parent; }
		set { parent = value; }
	}

	[SerializeField]
	private BaseNodeEditor parent = null;

	public BaseNodeRuntime runtimeNode;

	public virtual void Draw() {
	}

	public virtual void DrawConnections () {
	}

	public virtual void NodeDeleted(BaseNodeEditor node) {
	}

	public virtual void AddChild(BaseNodeEditor node) {
	}

	public abstract BaseNodeRuntime InstantiateRuntimeNode ();

	public abstract BaseNodeRuntime InstantiateRuntimeNode (GameObject gameObj, Type type);
}