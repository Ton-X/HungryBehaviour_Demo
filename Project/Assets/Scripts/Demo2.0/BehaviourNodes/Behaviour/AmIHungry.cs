	using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmIHungry : BaseBehaviourNode {

	HungerManager hungerManager;

	public override void Start()
	{
		hungerManager = go.GetComponent<HungerManager>();
	}

	public override int DoLogic()
	{
		if (hungerManager.totalFoodPoints <= hungerManager.hungerTolerance)
		{
			return (int)Result.SUCCESS;
		}

		return (int)Result.FAILURE;
	}
}
