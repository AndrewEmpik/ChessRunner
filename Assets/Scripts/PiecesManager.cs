using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecesManager : MonoBehaviour
{
	public List<Enemy> Enemies = new List<Enemy>();

	[SerializeField] Enemy[] _piecePrefabs;
	[SerializeField] int _enemiesNumber = 12;
	[SerializeField] int _offsetBetweenEnemies = 4;

	Management _management;

	void Start()
    {

		_management = FindObjectOfType<Management>();

		for (int i = 0; i < _enemiesNumber; i++)
		{
			//Enemy E = new Enemy();
			int _newTypeNumber;
			if (i < _enemiesNumber - 1)
				_newTypeNumber = Random.Range(1, 6);
			else
				_newTypeNumber = 0; // короля в конец

			Enemies.Add(
				Instantiate(
					_piecePrefabs[_newTypeNumber], 
					GlobalManagement.GetPositionByCellAddress(Random.Range(-2,3), 7 + i * _offsetBetweenEnemies), 
					Quaternion.Euler(-90,180,0)
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
