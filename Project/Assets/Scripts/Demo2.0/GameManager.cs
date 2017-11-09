using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public GameObject dinosaur;
	public GameObject human;
	public Count foodCount = new Count(3, 5);
	public GameObject[] foods;

	public List<Food> foodsAvailable = new List<Food>();

	private GroundGenerator groundGenerator;

	void Awake() {
		groundGenerator = GetComponent<GroundGenerator>();
		groundGenerator.GenerateLevel();
	}
	
	void Start () {
		Vector3 cavePos = groundGenerator.cave.transform.position;
		Vector3 tentPos = groundGenerator.tent.transform.position;

		dinosaur = Instantiate(dinosaur, cavePos, Quaternion.identity);
		human = Instantiate(human, tentPos, Quaternion.identity);
		foodsAvailable.Add(human.GetComponent<Food>());

		SpawnFood();
	}

	void SpawnFood() {
		Transform foodHolder = new GameObject("Foods").transform;

		int objCount = Random.Range(foodCount.minimum, foodCount.maximum + 1);
		List<Vector3> posAvailable = groundGenerator.gridPositions;

		for (int i = 0; i < objCount; i++)
		{
			GameObject toInstantiate = foods[Random.Range(0, foods.Length)];
			Vector3 randomPos = posAvailable[Random.Range(0, posAvailable.Count)];

			GameObject instance = Instantiate(toInstantiate, randomPos, Quaternion.identity);
			instance.transform.SetParent(foodHolder);

			foodsAvailable.Add(instance.GetComponent<Food>());

			posAvailable.Remove(randomPos);
		}
	}

	public void CharacterDied(GameObject characterGO)
	{
		BTreeExecutor bTreeExecutor = characterGO.GetComponent<BTreeExecutor>();
		if (bTreeExecutor != null)
			bTreeExecutor.enabled = false;
		Destroy(characterGO, 2f);
	}
}
