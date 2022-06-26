using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public AudioSource EnemyWhooshSound;

    void Start()
    {
		//Enemy.OnEnemyRush.AddListener(PlayEnemyWhooshSound);
    }

	public void PlayEnemyWhooshSound()
	{
		EnemyWhooshSound.Play();
	}
}
