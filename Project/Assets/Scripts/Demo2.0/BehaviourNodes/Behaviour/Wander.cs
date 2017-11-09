using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : BaseBehaviourNode {

	GameManager gm;
	GroundGenerator ground;
	AStarPathfinding aStar;
	WanderEditor wanderEditor;

	Tile targetTile;
	List<Tile> path;
	Queue<Tile> recentTilesQueue = new Queue<Tile>();
	int currentIndex;
	Tile currentTile;

	BehaviourTreeRuntime btree;

	public override void Start()
	{
		gm = Object.FindObjectOfType<GameManager>();
		ground = gm.GetComponent<GroundGenerator>();
		aStar = gm.GetComponent<AStarPathfinding>();
		wanderEditor = editorNode as WanderEditor;
		btree = go.GetComponent<BTreeExecutor>().btRuntime;
		btree.btRuntimeParams.boolParams["WanderingEnabled"] = true;
	}

	public override int DoLogic()
	{
		bool wanderingEnabled = btree.btRuntimeParams.boolParams["WanderingEnabled"];
		if (wanderingEnabled)
		{
			btree.btRuntimeBlackboard.blackboard["TrueSpeed"] = wanderEditor.wanderSpeed;
			float trueSpeed = btree.btRuntimeBlackboard.blackboard["TrueSpeed"];

			if (targetTile != null && go.transform.position != targetTile.tilePosition)
			{
				if (currentTile == null)
					Debug.LogError("currentTile is NULL!!");

				// Keep moving towards target tile
				if (go.transform.position == currentTile.tilePosition)
				{
					currentIndex++;
					currentTile = path[currentIndex];
				}
				go.transform.position = Vector3.MoveTowards(go.transform.position, currentTile.tilePosition, trueSpeed * Time.deltaTime);
			} else
			{
				// Select another target tile and find a path to reach it
				Tile[,] grid = ground.tiles;
				int randomX = Random.Range(0, ground.columns);
				int randomY = Random.Range(0, ground.rows);
				targetTile = grid[randomX, randomY];
				if (recentTilesQueue.Contains(targetTile) || targetTile.tileType == TileType.Type.ROCK || targetTile.tileType == TileType.Type.HOME)
				{
					// The tile selected isn't eligible
					targetTile = null;
					return (int)Result.FAILURE;
				}
				// A target tile has been found
				path = aStar.FindPath(go.transform.position, targetTile.tilePosition);
				if (path.Count == 0)
				{
					// Already reached destination
					targetTile = null;
					return (int)Result.SUCCESS;
				}
				currentIndex = 0;
				currentTile = path[currentIndex];
				recentTilesQueue.Enqueue(targetTile);
				if (recentTilesQueue.Count > 4)
					recentTilesQueue.Dequeue();
			}
		} else
		{
			// Wandering was disabled then reset target tile to recalculate a new path
			targetTile = null;
		}

		return (int)Result.SUCCESS;
	}
}
