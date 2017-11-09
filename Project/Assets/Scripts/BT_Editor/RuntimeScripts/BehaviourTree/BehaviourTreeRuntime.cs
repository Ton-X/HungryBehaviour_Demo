using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

[System.Serializable]
public class BehaviourTreeRuntime {
	[SerializeField]
	public List<BaseNodeRuntime> nodes;
	[SerializeField]
	public BTreeParameters btRuntimeParams;
	[SerializeField]
	public BTreeBlackboardRuntime btRuntimeBlackboard;

	BaseNodeRuntime root;

	public void Init(BehaviourTreeEditor btAsset, GameObject go) {
		List<Type> behavNodes = GetAllBehaviourNode ();

		nodes = new List<BaseNodeRuntime> ();
        List<BaseBehaviourNode> nodesToStart = new List<BaseBehaviourNode>();
		BaseNodeRuntime nodeToAdd = null;
		foreach (BaseNodeEditor node in btAsset.nodes) {
			if (node.GetType ().Equals (typeof(BehaviourNodeEditor)) || node.GetType().BaseType.Equals(typeof(BehaviourNodeEditor))) {
				BehaviourNodeEditor bNode = node as BehaviourNodeEditor;
				foreach (var behav in behavNodes) {
					if (behav.Name.Equals (bNode.nodeName)) {
						nodeToAdd = node.InstantiateRuntimeNode (go, behav);
                        nodesToStart.Add((BaseBehaviourNode)nodeToAdd);
					}
				}
			} else {
				nodeToAdd = node.InstantiateRuntimeNode ();
			}

			nodes.Add (nodeToAdd);
		}
		if (nodes.Count > 0)
			root = nodes [0];

		// Initialize parameters
		btRuntimeParams = btAsset.btEditorParams;

		// Initialize blackboard
		btRuntimeBlackboard = new BTreeBlackboardRuntime();
		foreach (string item in btAsset.btEditorBlackboard.blackboard) {
			btRuntimeBlackboard.blackboard.Add(item, 0f);
		}

        // Call the overrided Start method of all BehaviourNodes
        foreach (BaseBehaviourNode node in nodesToStart)
        {
            node.Start();
        }
	}

	public void Run() {

		foreach (BaseNodeRuntime node in nodes) {
			node.ResultValue = BaseNodeRuntime.Result.RUNNING.ToString ();
		}

		root.Tick ();
	}

	List<Type> GetAllBehaviourNode() {
		return Assembly.GetExecutingAssembly ().GetTypes ()
			.Where (t => t.BaseType.Equals (typeof(BaseBehaviourNode)))
			.ToList ();
	}
}
