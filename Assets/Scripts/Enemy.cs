using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : ChessPiece
{

	public GameObject EnemyHitCursorPrefab;
	public List<GameObject> EnemyHitCursorList = new List<GameObject>();

	public override void Start()
    {
		base.Start();

		for (int i = 0; i < HitCursorPrototypes.Count; i++)
		{
			EnemyHitCursorList.Add(Instantiate(EnemyHitCursorPrefab));
		}

		PlaceHitCursors();
	}

	void PlaceHitCursors()
	{
		Vector2Int unitCellAddress = GlobalManagement.GetCellAddressByPosition(transform.position.x, transform.position.z);
		Vector2Int newHitCursorCoords;

		for (int i = 0; i < EnemyHitCursorList.Count; i++)
		{
			newHitCursorCoords = unitCellAddress + HitCursorPrototypes[i] * new Vector2Int(1,-1); // * -1 по y

			Debug.Log(newHitCursorCoords.x + ", " + newHitCursorCoords.y + " (" + GlobalManagement.PathRadius + ")");

			if (Mathf.Abs(newHitCursorCoords.x) <= GlobalManagement.PathRadius)
			{
				EnemyHitCursorList[i].SetActive(true);
				EnemyHitCursorList[i].transform.position = GlobalManagement.GetPositionByCellAddress(newHitCursorCoords);
			}
			else
				EnemyHitCursorList[i].SetActive(false);
		}
	}

	void Update()
    {
        
    }
}
