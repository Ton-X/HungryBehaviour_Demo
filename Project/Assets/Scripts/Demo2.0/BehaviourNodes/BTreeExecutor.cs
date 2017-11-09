using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTreeExecutor : MonoBehaviour {

	public BehaviourTreeEditor btAsset;
	public BehaviourTreeRuntime btRuntime;
	
	void Start () {
		btRuntime = new BehaviourTreeRuntime();
		btRuntime.Init(btAsset, gameObject);
	}
	
	void Update () {
		btRuntime.Run();
	}
}
