using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ISeeHuman : BaseBehaviourNode
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
		float humanPriority = btree.btRuntimeBlackboard.blackboard["HumanPriority"];
		foreach (Food f in hungerManager.myFoods)
		{
			if (f.tag == "Human")
			{
				// Human is in sight
				if (hungerManager.closerFood.priority <= humanPriority)
					hungerManager.closerFood = f;
				return (int)Result.SUCCESS;
			}
		}

		return (int)Result.SUCCESS;
	}
}
