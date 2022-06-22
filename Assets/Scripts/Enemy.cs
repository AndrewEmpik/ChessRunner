using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : ChessPiece
{

	public GameObject EnemyHitCursorPrefab;
	public List<GameObject> EnemyHitCursorList = new List<GameObject>();


	private Management _management;

	public override void Start()
    {
		base.Start();

		_management = FindObjectOfType<Management>();

		for (int i = 0; i < HitCursorPrototypes.Count; i++)
		{
			EnemyHitCursorList.Add(Instantiate(EnemyHitCursorPrefab));
		}

		PlaceHitCursors();
	}

	void PlaceHitCursors()
	{
		Vector2Int unitCellAddress = _management.GetCellAddressByPosition(transform.position.x, transform.position.z);
		Vector2Int newHitCursorCoords;

		for (int i = 0; i < EnemyHitCursorList.Count; i++)
		{
			newHitCursorCoords = unitCellAddress + HitCursorPrototypes[i] * new Vector2Int(1,-1); // * -1 по y

			Debug.Log(newHitCursorCoords.x + ", " + newHitCursorCoords.y + " (" + _management.PathRadius + ")");

			if (Mathf.Abs(newHitCursorCoords.x) <= _management.PathRadius)
			{
				EnemyHitCursorList[i].SetActive(true);
				EnemyHitCursorList[i].transform.position = _management.GetPositionByCellAddress(newHitCursorCoords);
			}
			else
				EnemyHitCursorList[i].SetActive(false);
		}
	}

	void Update()
    {
        
    }
}
