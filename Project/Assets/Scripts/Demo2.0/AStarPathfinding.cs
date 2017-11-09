using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour {

	private GroundGenerator generator;
	private List<Tile> path;

	void Awake()
	{
		generator = FindObjectOfType<GroundGenerator>();
	}
	
	public List<Tile> FindPath(Vector3 startPos, Vector3 endPos)
	{
		Tile startTile = generator.TileFromWorldPoint(startPos);
		Tile endTile = generator.TileFromWorldPoint(endPos);

		List<Tile> openSet = new List<Tile>();
		HashSet<Tile> closedSet = new HashSet<Tile>();
		openSet.Add(startTile);

		while (openSet.Count > 0)
		{
			Tile currentTile = openSet[0];
			foreach (Tile t in openSet)
			{
				if (t.fScore < currentTile.fScore || t.fScore == currentTile.fScore && t.hScore < currentTile.hScore)
					currentTile = t;
			}

			openSet.Remove(currentTile);
			closedSet.Add(currentTile);

			if (currentTile == endTile)
			{
				ReconstructPath(startTile, endTile);
				return path;
			}

			List<Tile> neighbours = generator.GetNeighbours(currentTile);
			foreach (Tile neighbour in neighbours)
			{
				if (neighbour.tileType == TileType.Type.ROCK || closedSet.Contains(neighbour))
					continue;

				int newGScore = currentTile.gScore + GetDistance(currentTile, neighbour);
				if (newGScore < currentTile.gScore || !openSet.Contains(neighbour))
				{
					neighbour.gScore = newGScore;
					neighbour.hScore = GetDistance(neighbour, endTile);
					neighbour.parent = currentTile;

					if (!openSet.Contains(neighbour))
						openSet.Add(neighbour);
				}
			}
		}

		return null;
	}

	void ReconstructPath(Tile startTile, Tile endTile)
	{
		List<Tile> reconstructedPath = new List<Tile>();
		Tile currentTile = endTile;

		while (currentTile != startTile)
		{
			reconstructedPath.Add(currentTile);
			currentTile = currentTile.parent;
		}

		reconstructedPath.Reverse();

		path = reconstructedPath;
	}

	int GetDistance(Tile tileA, Tile tileB)
	{
		int distX = Mathf.Abs(tileA.xPos - tileB.xPos);
		int distY = Mathf.Abs(tileA.yPos - tileB.yPos);

		// distance = (diagonal movement cost from A to B) * less distance + straight movement cost * difference of distances
		int distance; 
		int totalMovementCost = tileA.movementCost + tileB.movementCost;
		if (distX > distY)
			distance = (totalMovementCost * 2) * distY + totalMovementCost * (distX - distY);
		else
			distance = (totalMovementCost * 2) * distX + totalMovementCost * (distY - distX);

		return distance;
	}
}
