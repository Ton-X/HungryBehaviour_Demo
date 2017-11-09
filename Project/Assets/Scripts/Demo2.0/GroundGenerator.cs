using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundGenerator : MonoBehaviour {

	public LayerMask groundMask;

	public int rows = 6;
	public int columns = 6;

	public Count obstacleCount = new Count(3, 5);
	public GameObject[] groundTiles;
	public GameObject[] obstacleTiles;
	public GameObject tent;
	public GameObject cave;

	public Tile[,] tiles;

	public List<Vector3> gridPositions = new List<Vector3>();
	private Vector3 tentPosition;
	private Vector3 cavePosition;
	
	void InitPositions() {
		for (int x = 0; x < columns; x++) {
			for (int y = 0; y < rows; y++) {
				gridPositions.Add(new Vector3(x, y, 0f));
			}
		}

		tiles = new Tile[columns, rows];
		tentPosition = new Vector3(columns - 1, 0, 0);
		cavePosition = new Vector3(0, rows - 1, 0);
	}

	void GenerateGround() {
		Transform groundHolder = new GameObject("Level").transform;

		foreach (Vector3 pos in gridPositions) {
			GameObject toInstantiate = groundTiles[Random.Range(0, groundTiles.Length)];
			TileType.Type type;
			type = TileType.Type.GRASS;

			Tile tile = new Tile(toInstantiate, pos, Quaternion.identity, type);

			int xPos = Mathf.RoundToInt(pos.x);
			int yPos = Mathf.RoundToInt(pos.y);
			tiles[xPos, yPos] = tile;
			tile.tileGO.transform.SetParent(groundHolder);
		}
	}

	void GenerateObstacles() {
		int objectCount = Random.Range(obstacleCount.minimum, obstacleCount.maximum+1);

		for (int i = 0; i < objectCount; i++) {
			Vector3 randomPos = gridPositions[Random.Range(0, gridPositions.Count)];
			GameObject toInstantiate = obstacleTiles[Random.Range(0, obstacleTiles.Length)];
			TileType.Type type;
			if (toInstantiate.tag == "Rock")
				type = TileType.Type.ROCK;
			else if (toInstantiate.tag == "Bush")
				type = TileType.Type.BUSH;
			else
				type = TileType.Type.WATER;
			Tile tile = new Tile(toInstantiate, randomPos, Quaternion.identity, type);

			int xPos = Mathf.RoundToInt(randomPos.x);
			int yPos = Mathf.RoundToInt(randomPos.y);
			tiles[xPos, yPos] = tile;
			gridPositions.Remove(randomPos);
		}
	}

	public void GenerateLevel() {

		InitPositions();

		GenerateGround();

		gridPositions.Remove(tentPosition);
		gridPositions.Remove(cavePosition);

		GenerateObstacles();
		// Instantiate the tent at the lower right corner
		Tile tentTile = new Tile(tent, tentPosition, Quaternion.identity, TileType.Type.HOME);
		tent = tentTile.tileGO;
		// Instantiate the cave at upper left corner
		Tile caveTile = new Tile(cave, cavePosition, Quaternion.identity, TileType.Type.HOME);
		cave = caveTile.tileGO;


		int xPos = Mathf.RoundToInt(tentPosition.x);
		int yPos = Mathf.RoundToInt(tentPosition.y);
		tiles[xPos, yPos] = tentTile;
		xPos = Mathf.RoundToInt(cavePosition.x);
		yPos = Mathf.RoundToInt(cavePosition.y);
		tiles[xPos, yPos] = caveTile;

	}

	public Tile TileFromWorldPoint(Vector3 worldPos)
	{
		int xPos = Mathf.RoundToInt(worldPos.x);
		int yPos = Mathf.RoundToInt(worldPos.y);

		return tiles[xPos, yPos];
	}

	public List<Tile> GetNeighbours(Tile tile)
	{
		List<Tile> neighbours = new List<Tile>();

		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0)
					continue;

				int neighbourX = tile.xPos + x;
				int neighbourY = tile.yPos + y;

				if (neighbourX >= 0 && neighbourX < columns && neighbourY >= 0 && neighbourY < rows)
					neighbours.Add(tiles[neighbourX, neighbourY]);
			}
		}

		return neighbours;
	}

	void OnDrawGizmos()
	{
		Color color;
		if (tiles != null)
		{
			foreach (Tile t in tiles)
			{
				if (t.tileType == TileType.Type.GRASS)
					color = Color.green;
				else if (t.tileType == TileType.Type.BUSH)
					color = Color.blue;
				else if (t.tileType == TileType.Type.ROCK)
					color = Color.red;
				else if (t.tileType == TileType.Type.WATER)
					color = Color.yellow;
				else
					color = Color.magenta;

				Gizmos.color = color;
				Gizmos.DrawWireCube(t.tilePosition, new Vector3(.5f,.5f,0));
			}

		}
	}
}
