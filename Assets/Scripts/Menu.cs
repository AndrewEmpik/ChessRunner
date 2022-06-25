using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// взято из Платформера полностью

public class Menu : MonoBehaviour
{ 
	public GameObject MenuButton;
	public GameObject MenuWindow;

	public List<MonoBehaviour> ComponentsToDisable;

	private bool _menuIsActive;
	public bool GameInProgress = true;

	[SerializeField] GameObject _musicCrossOut;

	public AudioSource Music;

	private void Start()
	{
		_menuIsActive = GlobalManagement.FirstLoad;

		if (_menuIsActive)
			OpenMenuWindow();
		else
			Time.timeScale = 1f;

		GlobalManagement.FirstLoad = false;

		Music.enabled = GlobalManagement.MusicEnabled;
		_musicCrossOut.SetActive(!GlobalManagement.MusicEnabled);
	}

	public void OpenMenuWindow()
	{
		_menuIsActive = true;
		//MenuButton.SetActive(false);
		MenuWindow.SetActive(true);
		foreach (MonoBehaviour C in ComponentsToDisable)
			C.enabled = false;
		Time.timeScale = 0f;
	}

	public void CloseMenuWindow()
	{
		_menuIsActive = false;
		//MenuButton.SetActive(true);
		MenuWindow.SetActive(false);
		foreach (MonoBehaviour C in ComponentsToDisable)
			C.enabled = true;
		Time.timeScale = 1f;
	}

	public void ToggleMenu()
	{
		if (_menuIsActive)
			CloseMenuWindow();
		else
			OpenMenuWindow();
	}

	public void ToggleGameInProgress()
	{
		GameInProgress = !GameInProgress;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (GameInProgress)
				ToggleMenu();
			else
				QuitGame();
		}
	}

	public void Restart()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		//CloseMenuWindow();

	}

	public void ToggleMusic()
	{
		GlobalManagement.MusicEnabled = !GlobalManagement.MusicEnabled;
		Music.enabled = GlobalManagement.MusicEnabled;
		_musicCrossOut.SetActive(!GlobalManagement.MusicEnabled);
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
