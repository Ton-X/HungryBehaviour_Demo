using UnityEngine;

[System.Serializable]
public class Tile {

	public GameObject tileGO;
	public Vector3 tilePosition;
	public TileType.Type tileType;
	public int xPos;
	public int yPos;

	public Tile parent; // The tile which this tile came from
	public int gScore; // The cost to move from tileA to tileB
	public int hScore; // The cost to move from a tile to the end tile

	public int fScore {
		get {
			return gScore + hScore;
		}
	}

	public int movementCost {
		get {
			if (tileType == TileType.Type.GRASS)
				return 5;
			else if (tileType == TileType.Type.WATER)
				return 8;
			else if (tileType == TileType.Type.BUSH)
				return 5;
			else if (tileType == TileType.Type.HOME)
				return 3;
			else
				return 50;
		}
	}

	public Tile(GameObject toInstantiate, Vector3 position, Quaternion rotation, TileType.Type type) {
		tileGO = Object.Instantiate(toInstantiate, position, rotation);
		tilePosition = position;
		tileType = type;
		xPos = (int)tileGO.transform.position.x; // Maybe useless
		yPos = (int)tileGO.transform.position.y; // Maybe useless
	}
}
