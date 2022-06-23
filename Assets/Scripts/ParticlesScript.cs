using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesScript : MonoBehaviour
{

	// Use this for initialization
	private void Start()
	{
		StartCoroutine ("destroy");
	}

	IEnumerator destroy()
	{
		yield return new WaitForSeconds (1);
		Destroy (gameObject);
	}
}
