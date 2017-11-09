using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escape : BaseBehaviourNode {

	GroundGenerator ground;
	AStarPathfinding aStar;

	Tile targetTile;
	List<Tile> path;
	int currentIndex;
	Tile currentTile;

	BehaviourTreeRuntime btree;

	public override void Start()
	{
		GameManager gm = Object.FindObjectOfType<GameManager>();
		ground = gm.GetComponent<GroundGenerator>();
		aStar = gm.GetComponent<AStarPathfinding>();

		btree = go.GetComponent<BTreeExecutor>().btRuntime;
	}

	public override int DoLogic()
	{
		// Escape in the opposite direction of the threat incoming
		float trueSpeed = btree.btRuntimeBlackboard.blackboard["TrueSpeed"];
		float threatDist = btree.btRuntimeBlackboard.blackboard["ThreatDistance"];

		// Check in a range equals to the threat distance to get the direction
		Collider2D threat = Physics2D.OverlapCircle(go.transform.position, threatDist, LayerMask.GetMask("Dinosaur"));
		Vector3 threatDir = (threat.transform.position - go.transform.position).normalized;
		Vector3 threatDirRotateLeft = (Quaternion.Euler(0, 0, 115) * -threatDir).normalized;
		Vector3 threatDirRotateRight = (Quaternion.Euler(0, 0, -115) * -threatDir).normalized;
		Debug.DrawRay(go.transform.position, -threatDir);
		Debug.DrawRay(go.transform.position, threatDir);
		Debug.DrawRay(go.transform.position, threatDirRotateLeft, Color.yellow);
		Debug.DrawRay(go.transform.position, threatDirRotateRight, Color.green);

		if (targetTile != null)
		{
			// Keep moving to the target tile
			if (go.transform.position == currentTile.tilePosition)
			{
				if (currentTile == path[path.Count - 1])
				{
					// We have reached the safe place
					path = null;
					targetTile = null;
					btree.btRuntimeParams.boolParams["GoingSafe"] = false;
					btree.btRuntimeParams.boolParams["Escaping"] = false;
				} else
				{
					currentIndex++;
					currentTile = path[currentIndex];
				}
			}
			go.transform.position = Vector3.MoveTowards(go.transform.position, currentTile.tilePosition, trueSpeed * 1.8f * Time.deltaTime);
		} else
		{
			// Select a random tile at the opposite direction
			Tile[,] grid = ground.tiles;
			Tile myTile = ground.TileFromWorldPoint(go.transform.position);
			List<Tile> selectedTiles = new List<Tile>();
			int neighbourX;
			int neighbourY;
			for (int x = -2; x <= 2; x++)
			{
				for (int y = -2; y <= 2; y++)
				{
					neighbourX = myTile.xPos + x;
					neighbourY = myTile.yPos + y;
					if (x == 0 && y == 0 || neighbourX < 0 || neighbourX >= ground.columns || neighbourY < 0 || neighbourY >= ground.rows)
					{
						// Avoid our tile and those one that go outside the grid
						continue;
					}

					if (Random.value >= 0.5 && selectedTiles.Count < 5)
						selectedTiles.Add(grid[neighbourX, neighbourY]);
				}
				if (selectedTiles.Count == 5)
					break;
			}

			while (targetTile == null && selectedTiles.Count > 0)
			{
				targetTile = selectedTiles[selectedTiles.Count - 1];

				if (targetTile.tileType != TileType.Type.ROCK && targetTile.tileType != TileType.Type.HOME)
				{
					Vector3 tileDir = (targetTile.tilePosition - go.transform.position).normalized;
					float dotThreatTile = Vector3.Dot(-threatDir, tileDir);
					if (dotThreatTile > Mathf.Cos(Mathf.Deg2Rad * 115))
						// Good tile for escaping
						break;
				}
				// The tile selected isn't eligible
				targetTile = null;
				selectedTiles.RemoveAt(selectedTiles.Count - 1);

			}

			if (targetTile != null)
			{
				Debug.DrawLine(go.transform.position, targetTile.tilePosition, Color.cyan);

				btree.btRuntimeParams.boolParams["GoingSafe"] = true;
				btree.btRuntimeParams.boolParams["Escaping"] = true;
				Debug.Log(go.name.ToUpper() + " is escaping!");
				path = aStar.FindPath(go.transform.position, targetTile.tilePosition);
				currentIndex = 0;
				currentTile = path[currentIndex];
			}
		}
		// We return always success because we want to save our life
		return (int)Result.SUCCESS;
	}
}
