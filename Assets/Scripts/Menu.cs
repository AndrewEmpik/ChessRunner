using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ����� �� ����������� ���������

public class Menu : MonoBehaviour
{ 
	public GameObject MenuButton;
	public GameObject MenuWindow;

	public List<MonoBehaviour> ComponentsToDisable;

	private bool _menuIsActive;

	[SerializeField] Toggle MusicToggle;
	[SerializeField] Slider VolumeSlider;

	private void Start()
	{
		_menuIsActive = GlobalManagement.FirstLoad;

		if (_menuIsActive)
			OpenMenuWindow();
		else
			Time.timeScale = 1f;

		GlobalManagement.FirstLoad = false;

		//MusicToggle.isOn = GlobalManagement.MusicEnabled;
		//VolumeSlider.value = GlobalManagement.Volume;
	}

	public void OpenMenuWindow()
	{
		_menuIsActive = true;
		MenuButton.SetActive(false);
		MenuWindow.SetActive(true);
		foreach (MonoBehaviour C in ComponentsToDisable)
			C.enabled = false;
		Time.timeScale = 0f;
	}

	public void CloseMenuWindow()
	{
		_menuIsActive = false;
		MenuButton.SetActive(true);
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

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			ToggleMenu();
		}
	}

	public void Restart()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		//CloseMenuWindow();

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