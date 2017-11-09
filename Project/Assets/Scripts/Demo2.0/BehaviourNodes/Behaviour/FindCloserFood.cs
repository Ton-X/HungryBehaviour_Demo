using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindCloserFood : BaseBehaviourNode
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
		// Find the closest food in field of view
		float minDist = Vector3.Distance(go.transform.position, hungerManager.myFoods[0].transform.position);
		int maxPriority = hungerManager.myFoods[0].priority;
		Transform humanTransform = null;
		foreach (Food f in hungerManager.myFoods)
		{
			if (f.tag == "Human")
				humanTransform = f.transform;
			float dist = Vector3.Distance(go.transform.position, f.transform.position);
			int priority = f.priority;
			if (hungerManager.totalFoodPoints <= 30)
			{
				if (dist <= minDist && priority >= maxPriority)
				{
					minDist = dist;
					maxPriority = priority;
					hungerManager.closerFood = f;
				}
			} else
			{
				if (priority > maxPriority)
				{
					maxPriority = priority;
					minDist = dist;
					hungerManager.closerFood = f;
				} else if (priority == maxPriority)
				{
					if (dist <= minDist)
					{
						minDist = dist;
						hungerManager.closerFood = f;
					}
				}
			}
		}

		if (hungerManager.closerFood != null)
		{
			if (humanTransform != null)
			{
				float distFood = Vector3.Distance(go.transform.position, hungerManager.closerFood.transform.position);
				float distHuman = Vector3.Distance(go.transform.position, humanTransform.position);
				if (distFood >= distHuman)
					btree.btRuntimeBlackboard.blackboard["HumanPriority"] = hungerManager.closerFood.priority * 2;
				else
					btree.btRuntimeBlackboard.blackboard["HumanPriority"] = hungerManager.closerFood.priority / 2;
			}
			return (int)Result.SUCCESS;
		}

		return (int)Result.FAILURE;
	}
}
