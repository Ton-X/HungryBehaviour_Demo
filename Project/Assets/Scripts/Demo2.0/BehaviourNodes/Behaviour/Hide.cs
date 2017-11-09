using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hide : BaseBehaviourNode {

	AStarPathfinding aStar;
	HungerManager hungerManager;
	HideEditor hideEditor;

	Vector3 hideoutPos = Vector3.negativeInfinity;
	List<Tile> path;
	Tile currentTile;
	int currentIndex;
	float hidingTime;
	float timePassed;

	BehaviourTreeRuntime btree;

	public override void Start()
	{
		GameManager gm = Object.FindObjectOfType<GameManager>();
		aStar = gm.GetComponent<AStarPathfinding>();
		hungerManager = go.GetComponent<HungerManager>();
		hideEditor = editorNode as HideEditor;

		hidingTime = hideEditor.hidingTime;

		btree = go.GetComponent<BTreeExecutor>().btRuntime;
	}

	public override int DoLogic()
	{
		bool escaping = btree.btRuntimeParams.boolParams["Escaping"];
		bool hidden = btree.btRuntimeParams.boolParams["Hidden"];
		float trueSpeed = btree.btRuntimeBlackboard.blackboard["TrueSpeed"];
		float threatDist = btree.btRuntimeBlackboard.blackboard["ThreatDistance"];

		if (escaping)
			return (int)Result.FAILURE;

		if (hidden)
		{
			timePassed += Time.deltaTime;
			timePassed = Mathf.Clamp(timePassed, 0, hidingTime);
		}

		// Detect all hideouts in range
		Collider2D[] hideouts = Physics2D.OverlapCircleAll(go.transform.position, hideEditor.hideRange, LayerMask.GetMask("Hideout"));
		if (hideouts.Length > 0 && threatDist > hideEditor.hideRange/2)
		{
			// There is at least one hideout in range then find the closest one
			float minDist = Vector3.Distance(go.transform.position, hideouts[0].transform.position);
			hideoutPos = Vector3.negativeInfinity;
			foreach (Collider2D hideout in hideouts)
			{
				float dist = Vector3.Distance(go.transform.position, hideout.transform.position);
				if (dist <= minDist)
				{
					minDist = dist;
					hideoutPos = hideout.transform.position;
				}
			}

			if (hideoutPos != Vector3.negativeInfinity)
			{
				// We don't want to eat in this situation
				hungerManager.closerFood = null;
				btree.btRuntimeParams.boolParams["GoingSafe"] = true;

				if (path != null && path.Count <= 0)
				{
					// Already hiding in a hideout
					path = null;
					if (timePassed == hidingTime)
					{
						btree.btRuntimeParams.boolParams["GoingSafe"] = false;
						timePassed = 0;
					}
					btree.btRuntimeParams.boolParams["Hidden"] = true;
					if (go.transform.position != hideoutPos)
						go.transform.position = Vector3.MoveTowards(go.transform.position, hideoutPos, trueSpeed * Time.deltaTime);
					return (int)Result.SUCCESS;
				}

				// Closest hideout found
				if (path == null)
				{
					// Find a path to reach the hideout found
					path = aStar.FindPath(go.transform.position, hideoutPos);
					currentIndex = 0;
					if (path.Count <= 0)
					{
						// Already reached destination
						if (timePassed == hidingTime)
						{
							btree.btRuntimeParams.boolParams["GoingSafe"] = false;
							timePassed = 0;
						}
						btree.btRuntimeParams.boolParams["Hidden"] = true;
						return (int)Result.SUCCESS;
					}
					currentTile = path[currentIndex];
				} else
				{
					// Move towards the hideout
					if (go.transform.position == currentTile.tilePosition)
					{
						if (currentTile == path[path.Count - 1])
						{
							// We have reached the hideout
							Debug.Log(go + " is hiding!");
							path = null;
							if (timePassed == hidingTime)
							{
								btree.btRuntimeParams.boolParams["GoingSafe"] = false;
								timePassed = 0;
							}
							btree.btRuntimeParams.boolParams["Hidden"] = true;
						} else
						{
							currentIndex++;
							currentTile = path[currentIndex];
						}
					}
					go.transform.position = Vector3.MoveTowards(go.transform.position, currentTile.tilePosition, trueSpeed * 1.8f * Time.deltaTime);
				}
				return (int)Result.SUCCESS;
			}
		}
		// No hideout found
		return (int)Result.FAILURE;
	}
}
