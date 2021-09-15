using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailEnable : MonoBehaviour
{
	StateManager stateManager;
	TrailRenderer trail;

	void Awake()
	{
		trail = GetComponent<TrailRenderer>();
		stateManager = FindObjectOfType<StateManager>();
	}

	void Update()
	{
		if (stateManager.isFlying)
		{
			trail.enabled = true;
		}
		else
		{
			trail.enabled = false;
		}
	}
}
