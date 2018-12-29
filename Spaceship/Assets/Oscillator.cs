using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour
{
	[SerializeField] Vector3 movementVector = new Vector3(10f, 0f, 0f);
	[SerializeField] float period = 2f;

	Vector3 startingPosition;

	// Use this for initialization
	void Start()
	{
		startingPosition = transform.position;
	}

	// Update is called once per frame
	void Update()
	{
		if (period <= Mathf.Epsilon) { return; }

		float cycles = Time.time / period;
		float rawSinWave = Mathf.Sin(cycles * Mathf.PI * 2f);
		float movementFactor = rawSinWave / 2f + 0.5f;
		transform.position = startingPosition + movementVector * movementFactor;

	}
}
