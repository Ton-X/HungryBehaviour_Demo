using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HungerManager : MonoBehaviour {

	public int totalFoodPoints;
	public int pointsDrop;
	public int hungerTolerance;

	public LayerMask foodMask;
	public List<FoodType.Type> favoriteFoods = new List<FoodType.Type>();
	public List<Food> myFoods = new List<Food>();
	public Food closerFood;

	public GameManager gm;
	public Slider foodBar;
	public Image fillImage;

	private Coroutine lastCoroutine;

	private void Awake()
	{
		gm = FindObjectOfType<GameManager>();
		foodBar.value = 1;
	}
	
	void Start () {
		lastCoroutine = StartCoroutine("DecreaseFoodPoints");
	}

	public void IncreaseFoodPoints()
	{
		if (totalFoodPoints < 100 && closerFood != null)
		{
			if (closerFood.tag == "Human")
			{
				closerFood.GetComponent<BTreeExecutor>().enabled = false;
				transform.position = Vector3.MoveTowards(transform.position, closerFood.transform.position, 0.3f);
			}
			StartCoroutine("WaitForEat");
		}
	}

	IEnumerator WaitForEat()
	{
		StopCoroutine(lastCoroutine);
		gameObject.GetComponent<BTreeExecutor>().enabled = false;
		yield return new WaitForSeconds(closerFood.timeToEat);

		totalFoodPoints += closerFood.foodPoints;
		GameObject.Destroy(closerFood.gameObject);
		closerFood = null;
		lastCoroutine = StartCoroutine("DecreaseFoodPoints");
		gameObject.GetComponent<BTreeExecutor>().enabled = true;
		yield break;
	}

	IEnumerator DecreaseFoodPoints()
	{
		while (true)
		{
			if (totalFoodPoints > 0)
			{
				Debug.Log(gameObject.name.ToUpper() + ": " + totalFoodPoints);
				totalFoodPoints -= pointsDrop;
				foodBar.value = (float)totalFoodPoints/100;
				if (totalFoodPoints <= 50)
					fillImage.color = Color.yellow;
				if (totalFoodPoints <= 30)
					fillImage.color = Color.red;
			} else
			{
				foodBar.gameObject.SetActive(false);
				Debug.Log(gameObject.name.ToUpper() + " DIED!");
				gm.CharacterDied(gameObject);
				yield break;
			}
			yield return new WaitForSeconds(1);
		}
	}
}
