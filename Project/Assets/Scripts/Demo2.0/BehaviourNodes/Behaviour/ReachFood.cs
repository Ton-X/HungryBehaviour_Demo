using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReachFood : BaseBehaviourNode
{
	AStarPathfinding aStar;
	HungerManager hungerManager;
	GroundGenerator ground;

	List<Tile> path;
	int currentIndex;
	Tile currentTile;
	Food targetFood;

	BehaviourTreeRuntime btree;

	public override void Start()
	{
		GameManager gm = Object.FindObjectOfType<GameManager>();
		aStar = gm.GetComponent<AStarPathfinding>();
		hungerManager = go.GetComponent<HungerManager>();
		ground = gm.GetComponent<GroundGenerator>();
		btree = go.GetComponent<BTreeExecutor>().btRuntime;
	}

	public override int DoLogic()
	{
		float trueSpeed = btree.btRuntimeBlackboard.blackboard["TrueSpeed"];

		if (hungerManager.closerFood == null || path != null && path.Count == 0)
		{
			path = null;
			btree.btRuntimeParams.boolParams["WanderingEnabled"] = true;
			return (int)Result.FAILURE;
		}
		
		// Move towards the closest food in the view field if is reachable
		bool targetMoved = false;
		if (path != null)
		{
			Tile closerFoodTile = ground.TileFromWorldPoint(hungerManager.closerFood.transform.position);
			if (closerFoodTile != path[path.Count - 1])
				targetMoved = true;
		}

		if (path == null || targetFood != hungerManager.closerFood || targetMoved)
		{
			// Find a path to the closer food and disable wandering behaviour
			targetFood = hungerManager.closerFood;
			Vector3 closerFoodPos = targetFood.transform.position;
			path = aStar.FindPath(go.transform.position, closerFoodPos);
			currentIndex = 0;
			if (path.Count <= 0)
			{
				// Already reached destination
				return (int)Result.SUCCESS;
			}
			currentTile = path[currentIndex];
			btree.btRuntimeParams.boolParams["WanderingEnabled"] = false;
		} else
		{
			// Actual movement following the path found
			if (go.transform.position == currentTile.tilePosition)
			{
				if (currentTile == path[path.Count - 1])
				{
					// Destination reached
					path = null;
					return (int)Result.SUCCESS;
				} else
				{
					currentIndex++;
					currentTile = path[currentIndex];
				}
			}
			go.transform.position = Vector3.MoveTowards(go.transform.position, currentTile.tilePosition, trueSpeed * 1.65f * Time.deltaTime);
		}
		// Keep moving
		return (int)Result.FAILURE;
	}
}
