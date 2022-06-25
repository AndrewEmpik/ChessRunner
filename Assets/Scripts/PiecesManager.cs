using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecesManager : MonoBehaviour
{
	public List<Enemy> Enemies = new List<Enemy>();

	[SerializeField] Enemy[] _piecePrefabs;
	[SerializeField] int _enemiesNumber = 12;

	Management _management;

	void Start()
    {

		_management = FindObjectOfType<Management>();

		for (int i = 0; i < _enemiesNumber; i++)
		{
			//Enemy E = new Enemy();
			int _newTypeNumber = Random.Range(0, 6);
			Enemies.Add(
				Instantiate(
					_piecePrefabs[_newTypeNumber], 
					GlobalManagement.GetPositionByCellAddress(Random.Range(-2,3), 7 + i * 6), 
					Quaternion.identity
				)
			);
		}
    }

	public void CheckPiecesListEmpty()
	{
		if (Enemies.Count <= 0)
			_management.Win();
	}
}
