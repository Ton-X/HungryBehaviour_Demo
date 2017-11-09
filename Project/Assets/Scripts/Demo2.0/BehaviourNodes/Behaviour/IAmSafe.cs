using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAmSafe : BaseBehaviourNode {

	IAmSafeEditor safeEditor;

	Collider2D threat;

	BehaviourTreeRuntime btree;

	public override void Start()
	{
		safeEditor = editorNode as IAmSafeEditor;

		btree = go.GetComponent<BTreeExecutor>().btRuntime;
		btree.btRuntimeParams.boolParams["GoingSafe"] = false;
		btree.btRuntimeParams.boolParams["Hidden"] = false;
		btree.btRuntimeParams.boolParams["Escaping"] = false;
	}

	public override int DoLogic()
	{
		bool goingSafe = btree.btRuntimeParams.boolParams["GoingSafe"];
		if (goingSafe)
		{
			// We are in danger until hideout or a safe place wasn't reached
			btree.btRuntimeBlackboard.blackboard["ThreatDistance"] = Vector3.Distance(go.transform.position, threat.transform.position);
			return (int)Result.FAILURE;
		}

		btree.btRuntimeParams.boolParams["Hidden"] = false;
		btree.btRuntimeParams.boolParams["Escaping"] = false;

		// Check if there are threats in range
		threat = Physics2D.OverlapCircle(go.transform.position, safeEditor.safeRange, LayerMask.GetMask("Dinosaur"));
		if (threat != null)
		{
			// A threat is in range and now check if is also in sight
			RaycastHit2D hit = Physics2D.Linecast(go.transform.position, threat.transform.position, LayerMask.GetMask("Obstacle", "Dinosaur"));
			if (hit.collider != null && hit.collider.tag != "Rock")
			{
				Debug.DrawLine(go.transform.position, threat.transform.position, Color.magenta);
				btree.btRuntimeParams.boolParams["WanderingEnabled"] = false;
				btree.btRuntimeBlackboard.blackboard["ThreatDistance"] = hit.distance;
				return (int)Result.FAILURE;
			} else
			{
				Debug.DrawLine(go.transform.position, threat.transform.position, Color.red);
				return (int)Result.SUCCESS;
			}
		} else
		{
			// No threat in range
			btree.btRuntimeParams.boolParams["GoingSafe"] = false;
			btree.btRuntimeParams.boolParams["Hidden"] = false;
			btree.btRuntimeParams.boolParams["Escaping"] = false;
			return (int)Result.SUCCESS;
		}
	}
}
