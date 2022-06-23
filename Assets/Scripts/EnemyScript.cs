using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{

	public float radius = 150f;
	public Collider[] colliders;
	public LayerMask mask;
	public Image hpBar;
	public Text hpText;
	
	public GameObject tower;
	public AudioClip gunSound;
	public GameObject particleGun;
	public AudioClip missileSound;
	public GameObject particleMissile;
	
	private float _nearestDistance;
	private float _distance;
	private GameObject _nearestTank;
	private RaycastHit _raycastHit;
	private AudioSource _audioSource;

	private void Start()
	{
		_audioSource = tower.GetComponent<AudioSource>();
		StartCoroutine(nameof(StartAI));
	}

	private void Update()
	{
		hpBar.color = GetComponent<LifeScript>().hp switch { >= 100 => Color.green, >= 50 => Color.yellow, _ => Color.red};
		hpText.text = GetComponent<LifeScript>().hp.ToString();
		
		colliders = Physics.OverlapSphere(transform.position, radius, mask);
		_nearestDistance = 1000f;
		
		foreach(Collider tank in colliders)
		{
			float distance = Vector3.Distance(transform.position, tank.gameObject.transform.position);
			
			if (distance != 0 && distance < _nearestDistance)
			{
				_nearestDistance = distance;
				_nearestTank = tank.gameObject;
				tower.transform.LookAt(_nearestTank.transform);
			}
		}
		
		if (_raycastHit.transform != null)
			Debug.DrawLine (transform.position, _raycastHit.point, Color.red);
	}
	
	void fire()
	{
		if (_raycastHit.transform != null && _raycastHit.transform.CompareTag("Tank") || _raycastHit.transform.CompareTag("Player"))
		{
			var randomNum = Random.Range(1, 70);

			if (randomNum is > 20  and <= 60)
			{
				_raycastHit.transform.GetComponent<LifeScript>().hp -= 5;
				Instantiate(particleGun, _raycastHit.point, Quaternion.identity);
				_audioSource.clip = gunSound;
				_audioSource.Play();
			}
			else if (randomNum is > 0  and <= 20)
			{
				_raycastHit.transform.GetComponent<LifeScript>().hp -= 40;
				Instantiate(particleMissile, _raycastHit.point, Quaternion.identity);
				_audioSource.clip = missileSound;
				_audioSource.Play();
			}
		}
	}

	private IEnumerator StartAI()
	{
		while (true)
		{
			if (_nearestTank != null)
			{
				Vector3 fwd = tower.transform.TransformDirection(new Vector3 (0, 0, 2));
				GetComponent<NavMeshAgent>().destination = _nearestTank.transform.position;
				
				if (Physics.Raycast(tower.transform.position, fwd, out _raycastHit, radius))
					fire();
			}
			yield return new WaitForSeconds (0.8f);
		}
	}
}
