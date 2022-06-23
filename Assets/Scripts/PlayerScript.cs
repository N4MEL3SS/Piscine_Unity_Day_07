using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PlayerScript : MonoBehaviour
{

	public float defaultSpeed = 8f;
	public float boostSpeed = 15f;
	public float boostCharge = 0.05f;
	public float sensitivityY = 10f;
	public float playerAngle = 2f;
	public float radius = 150f;
	public float boost = 100f;
	public int maxAmmo = 20;
	
	public GameObject tower;
	public GameObject wheel;
	public AudioClip gunSound;
	public GameObject particleGun;
	public AudioClip missileSound;
	public GameObject particleMissile;
	
	public Text hpText;
	public Text ammoText;
	public Text boostText;
	public Image crosshair;
	public LayerMask mask;

	private float _speed;
	private float _rotationY;
	private float _rotationX;
	private int _ammo;
	private bool _recharge = false;
	private RaycastHit _raycastHit;
	private AudioSource _towerSound;
	private LifeScript _lifeScript;

	private void Start()
	{
		_lifeScript = GetComponent<LifeScript>();
		_towerSound = tower.GetComponent<AudioSource>();
		_ammo = maxAmmo;
	}
	
	// Update is called once per frame
	private void Update()
	{
		HpColor();
		AmmoColor();
		TowerControl();
		PlayerControl();
		SceneReload();

	}

	private void HpColor()
	{
		hpText.text = _lifeScript.hp.ToString() + " hp";
		hpText.color =  _lifeScript.hp switch {<= 50 => Color.red, <= 100 => Color.yellow, _ => Color.white};
	}

	private void AmmoColor()
	{
		ammoText.text = _ammo.ToString() + " / " + maxAmmo.ToString();
		ammoText.color =
			_ammo switch {<= 5 => Color.red, <= 10 => Color.yellow, _ => Color.white};
	}

	private static void SceneReload()
	{
		if (Input.GetKeyDown(KeyCode.R))
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	private void TowerControl()
	{
		_rotationX = tower.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityY;
		_rotationY = tower.transform.localEulerAngles.x + Input.GetAxis("Mouse Y") * sensitivityY;

		switch (_rotationY)
		{
			case >= 15 and <= 90f:
				_rotationY = 15;
				break;
			case <= 350 and >= 270:
				_rotationY = 350;
				break;
		}
		
		tower.transform.localEulerAngles = new Vector3(_rotationY, _rotationX, 0);

		Vector3 fwd = tower.transform.TransformDirection(Vector3.forward);
		if (Physics.Raycast(tower.transform.position, fwd, out _raycastHit, radius)) 
		{
			if (_raycastHit.transform.CompareTag("Tank"))
				crosshair.color = Color.red;
			else
				crosshair.color = Color.white;
			
			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				if (_raycastHit.transform.CompareTag("Tank"))
					_raycastHit.transform.GetComponent<LifeScript>().hp -= 5;
				Instantiate(particleGun, _raycastHit.point, Quaternion.identity);
				_towerSound.clip = gunSound;
				_towerSound.Play();
			}
			else if (Input.GetKeyDown(KeyCode.Mouse1) && _ammo > 0)
			{
				if (_raycastHit.transform.CompareTag("Tank"))
					_raycastHit.transform.GetComponent<LifeScript>().hp -= 40;
				Instantiate (particleMissile, _raycastHit.point, Quaternion.identity);
				_towerSound.clip = missileSound;
				_towerSound.Play();
				_ammo -= 1;
			}
			
			if (_raycastHit.transform != null)
				Debug.DrawLine(transform.position, _raycastHit.point, Color.red);
		}
		else
			crosshair.color = Color.white;
	}
	private void PlayerControl()
	{
		Boost();
		if (Input.GetKey(KeyCode.W))
			wheel.transform.Translate(Vector3.forward * (_speed * Time.deltaTime));
		if (Input.GetKey(KeyCode.S))
			wheel.transform.Translate(Vector3.back * (_speed * Time.deltaTime));
		if (Input.GetKey(KeyCode.D))
			wheel.transform.Rotate (0, playerAngle, 0);
		if (Input.GetKey(KeyCode.A))
			wheel.transform.Rotate (0, -playerAngle, 0);
	}
	
	private void Boost()
	{
		if (Input.GetKey(KeyCode.LeftShift) && (boost >= boostCharge * 2) && !_recharge)
		{
			boost -= boostCharge * 2;
			_speed = boostSpeed;
		}
		else
		{
			_speed = defaultSpeed;
			if (boost < 100)
				boost += boostCharge;
		}

		_recharge = boost switch {0 => true, 100 => false, _ => _recharge};

		boostText.color = _recharge switch {true => Color.red, false => Color.green};
		boostText.text = boost.ToString();
	}
	
}
