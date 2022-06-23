using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LifeScript : MonoBehaviour
{

	public int hp = 200;

	private void Update()
	{
		if (hp <= 0)
		{
			GameManager.gm.playDead();
			if (!gameObject.transform.CompareTag("Player"))
				Destroy (gameObject);
			else
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}

}
