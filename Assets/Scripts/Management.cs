using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Management : MonoBehaviour
{
	private float _fps;
	[SerializeField] GUIStyle style;

	public Vector2 WorldZeroPoint = Vector2.zero;
	public int PathWidthInCells = 5; // пока не меняем
	private int _pathRadius;
	public float CellSize = 1f;

	void Start()
    {
		_pathRadius = (PathWidthInCells - 1) / 2; // считаем, что PathWidthInCells нечётный

#if !UNITY_EDITOR
		Application.targetFrameRate = 60;
#endif

#if UNITY_EDITOR
		style.fontSize = (int)(style.fontSize / 2.5f);
#endif
	}

	void OnGUI()
	{
		//float newFPS = 1.0f / Time.smoothDeltaTime;
		_fps = 1.0f / Time.smoothDeltaTime;  //Mathf.Lerp(fps, newFPS, 0.0005f);
		GUI.Label(new Rect(10, 10, 100, 100), "FPS: " + ((int)_fps).ToString(), style);
	}

	void Update()
    {
		if (Input.GetKey(KeyCode.Escape))
			QuitGame();
	}

	public Vector2Int GetCellAddressByPosition(float x, float y)
	{
		float xRelative = x - WorldZeroPoint.x;
		int _x = (int)((Mathf.Abs(xRelative) + CellSize / 2) / CellSize * Mathf.Sign(xRelative));
		int _y = (int)((y-WorldZeroPoint.y	+ CellSize / 2) / CellSize);
		return new Vector2Int(_x,_y);
	}
	public Vector2Int GetCellAddressByPosition(Vector2 point)
	{
		return GetCellAddressByPosition(point.x, point.y);
	}

	public Vector2 GetPositionByCellAddress(Vector2Int cellAddress)
	{
		return new Vector2(cellAddress.x, cellAddress.y)*CellSize + WorldZeroPoint;
	}

	public Vector3 GetPositionByCellAddress(int x, int y)
	{
		Vector2 flatPosition = new Vector2(x, y) * CellSize + WorldZeroPoint;
		return new Vector3(flatPosition.x, 0, flatPosition.y);
	}

	public void QuitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit(); 
#endif
	}
}
