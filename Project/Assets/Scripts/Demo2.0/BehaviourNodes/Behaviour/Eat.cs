using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eat : BaseBehaviourNode
{
	HungerManager hungerManager;

	BehaviourTreeRuntime btree;

	public override void Start()
	{
		hungerManager = go.GetComponent<HungerManager>();
		btree = go.GetComponent<BTreeExecutor>().btRuntime;
	}

	public override int DoLogic()
	{
		// Eat the food reached in the previous behaviour
		hungerManager.IncreaseFoodPoints();
		btree.btRuntimeParams.boolParams["WanderingEnabled"] = true;
		return (int)Result.SUCCESS;
	}
}
