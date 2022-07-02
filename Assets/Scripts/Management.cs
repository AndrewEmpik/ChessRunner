using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Management : MonoBehaviour
{
	private float _fps;
	private GUIStyle style = new GUIStyle();

	public UnityEvent OnWin;
	public UnityEvent OnLose;

	public int Score = 0;

	[SerializeField] TMP_Text _scoreTextUI;
	[SerializeField] TMP_Text _versionText;
	[SerializeField] string _gameVersion = "1.xx";

	void Start()
	{
		_versionText.text = "v " + _gameVersion;

		style.fontSize = 32;
		style.normal.textColor = Color.white;

		UpdateScoreUI();

#if !UNITY_EDITOR
		Application.targetFrameRate = 60;
#endif

#if UNITY_EDITOR
		style.fontSize = (int)(style.fontSize / 2.5f);
#endif
	}

	void OnGUI()
	{
		float newFPS = 1.0f / Time.smoothDeltaTime;
		_fps = 1.0f / Time.smoothDeltaTime;  //Mathf.Lerp(fps, newFPS, 0.0005f);
		GUI.Label(new Rect(10, 10, 100, 100), "FPS: " + ((int)_fps).ToString(), style);
	}

	void Update()
	{
		//if (Input.GetKey(KeyCode.Escape))
		//	GlobalManagement.QuitGame();
	}

	private bool _win = false;

	public void Win()
	{
		if (!_win)
		{
			_win = true;
			OnWin.Invoke();
		}
		Time.timeScale = 0f;
	}

	public void Lose()
	{
		OnLose.Invoke();
		Time.timeScale = 0f;
	}

	public void AddScore(int value)
	{
		Score += value;
		UpdateScoreUI();
	}

	void UpdateScoreUI()
	{
		_scoreTextUI.text = Score.ToString("00");
	}
}
