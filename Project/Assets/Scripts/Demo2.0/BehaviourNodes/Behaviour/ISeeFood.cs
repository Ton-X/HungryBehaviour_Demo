using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ISeeFood : BaseBehaviourNode
{
	HungerManager hungerManager;
	ISeeFoodEditor seeFoodEditor;

	List<Food> myFoods;

	BehaviourTreeRuntime btree;

	public override void Start()
	{
		hungerManager = go.GetComponent<HungerManager>();
		if (hungerManager == null)
			Debug.LogError("HungerManager has to be attached to this GO!");

		seeFoodEditor = editorNode as ISeeFoodEditor;
		btree = go.GetComponent<BTreeExecutor>().btRuntime;
	}

	public override int DoLogic()
	{
		myFoods = hungerManager.myFoods;

		// Retrieve the list of all foods in range
		Collider2D[] objects = Physics2D.OverlapCircleAll(go.transform.position, seeFoodEditor.range, hungerManager.foodMask);
		myFoods.Clear();
		if (objects.Length > 0)
		{
			RaycastHit2D hit;
			foreach (Collider2D c in objects)
			{
				// Filter to take only those foods that are favorites
				Food f = c.GetComponent<Food>();
				if (hungerManager.favoriteFoods.Contains(f.foodType))
				{
					// Check if the current food is in sight
					if (go.tag == "Human")
						hit = Physics2D.Linecast(go.transform.position, f.transform.position, LayerMask.GetMask("Food", "Obstacle"));
					else
						hit = Physics2D.Linecast(go.transform.position, f.transform.position, LayerMask.GetMask("Food", "Obstacle", "Human"));

					Debug.DrawLine(go.transform.position, f.transform.position);
					if (hit.collider != null)
					{
						if (hit.collider.tag == "Rock")
						{
							Debug.DrawLine(go.transform.position, f.transform.position, Color.red);
							continue;
						} else
						{
							// The current food is in sight
							if (f.tag == "Human")
							{
								bool hidden = f.GetComponent<BTreeExecutor>().btRuntime.btRuntimeParams.boolParams["Hidden"];
								float dist = Vector3.Distance(go.transform.position, f.transform.position);
								if (hidden && dist > (seeFoodEditor.range/2))
									continue;
							}
							myFoods.Add(f);
						}
					}
				}
			}

			if (myFoods.Count > 0)
				// At least one food is in field of view
				return (int)Result.SUCCESS;
		}

		// No food found then reset AI to wandering mode
		hungerManager.closerFood = null;
		btree.btRuntimeParams.boolParams["WanderingEnabled"] = true;
		return (int)Result.FAILURE;
	}
}
