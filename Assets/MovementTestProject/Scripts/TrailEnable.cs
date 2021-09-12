using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailEnable : MonoBehaviour
{
	PlayerLocomotion playerLocomotion;
	TrailRenderer trail;

	void Awake()
	{
		trail = GetComponent<TrailRenderer>();
		playerLocomotion = FindObjectOfType<PlayerLocomotion>();
	}

	void Update()
	{
		if (playerLocomotion.isFlying)
		{
			trail.enabled = true;
		}
		else
		{
			trail.enabled = false;
		}
	}
}
