using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{ 
	public GameObject MenuButton;
	public GameObject MenuWindow;

	public List<MonoBehaviour> ComponentsToDisable;

	private bool _menuIsActive;
	public bool GameInProgress = true;

	[SerializeField] GameObject _musicCrossOut;
	[SerializeField] GameObject _helpWindow;
	[SerializeField] GameObject _optionsWindow;

	[SerializeField] GameObject _globalVolume;
	[SerializeField] GameObject _globalLight;
	//[SerializeField] GameObject _globalVolume;

	public AudioSource Music;

	private void Start()
	{
		_menuIsActive = GlobalManagement.FirstLoad;

		if (_menuIsActive)
			OpenMenuWindow();
		else
		{
			CloseMenuWindow();
			Time.timeScale = 1f;
		}

		GlobalManagement.FirstLoad = false;

		Music.enabled = GlobalManagement.MusicEnabled;
		_musicCrossOut.SetActive(!GlobalManagement.MusicEnabled);

		if (GameObject.FindGameObjectsWithTag("Music").Length > 1)
		{
			Debug.Log("Больше");
			Destroy(Music.gameObject);
		}

		DontDestroyOnLoad(Music.gameObject); 
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
		if (GameInProgress)
		{
			if (_menuIsActive)
				CloseMenuWindow();
			else
				OpenMenuWindow();
		}
	}

	public void ToggleGameInProgress()
	{
		GameInProgress = !GameInProgress;
	}

	public void TogglePostEffects(bool value)
	{
		_globalVolume.SetActive(value);
	}

	public void ToggleShadows(bool value)
	{
		_globalLight.GetComponent<Light>().shadows = value ? LightShadows.Hard : LightShadows.None;
	}

	public void ToggleOcclusion(bool value)
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (GameInProgress)
				ToggleMenu();
			else
				QuitGame();

			_helpWindow.SetActive(false);
			_optionsWindow.SetActive(false);
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
		Music = GameObject.FindGameObjectsWithTag("Music")[0].GetComponent<AudioSource>();
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
