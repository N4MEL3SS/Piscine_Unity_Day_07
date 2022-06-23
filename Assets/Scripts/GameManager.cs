using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
		
	public static GameManager gm;
	private AudioSource _audioSource;

	private void Start()
	{
		_audioSource = GetComponent<AudioSource>();
		if (gm == null)
			gm = this;
		Cursor.visible = false;
	}

	public void playDead()
	{
		_audioSource.Play();
	}
}
