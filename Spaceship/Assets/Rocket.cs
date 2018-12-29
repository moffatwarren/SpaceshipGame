using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
		Rigidbody rigidBody;
	AudioSource audioSource;

	[SerializeField] int lastSceneIndex = 2; //should not have to put number in. Determine last scene by code

	[SerializeField] bool allowXrotation = false;
	[SerializeField] float thrustControlForce = 100f;
	[SerializeField] float rotationControlSpeed = 100f;
	[SerializeField] float levelLoadDelay = 2f;
	[SerializeField] AudioClip mainEngine;
	[SerializeField] AudioClip success;
	[SerializeField] AudioClip death;
	[SerializeField] ParticleSystem mainEngineParticle;
	[SerializeField] ParticleSystem successParticle;
	[SerializeField] ParticleSystem deathParticle;

	enum State { ALIVE, DEAD, FINISHED };
	State state = State.ALIVE;

	// Use this for initialization
	void Start()
	{
		rigidBody = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();

	}

  // Update is called once per frame
  void Update()
  {
		if (state == State.ALIVE)
		{
			GetThrustInput();
			GetRotateInput();
		}
  }

	private void GetThrustInput()
	{

		if (Input.GetKey(KeyCode.Space) || Input.GetButton("AButton"))
		{
			ApplyThrust();
		}
		else
		{
			audioSource.Stop();
			mainEngineParticle.Stop();
		}
	}

	private void ApplyThrust()
	{
		rigidBody.AddRelativeForce(Vector3.up * thrustControlForce);
		if (!audioSource.isPlaying)
		{
			audioSource.PlayOneShot(mainEngine);
		}
		mainEngineParticle.Play();
	}

	private void GetRotateInput()
	{
		float leftStickHorizontal = Input.GetAxis("LeftJoystickHorizontal");
		float leftStickVertical = Input.GetAxis("LeftJoystickVertical");
		rigidBody.freezeRotation = true;

		float rotationSpeed = rotationControlSpeed * Time.deltaTime;

		///// z axis rotation /////
		// keyboard input
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			transform.Rotate(Vector3.forward * rotationSpeed);
		}
		else if (Input.GetKey(KeyCode.RightArrow))
		{
			transform.Rotate(-Vector3.forward * rotationSpeed);
		}
		// controller input
		if (System.Math.Abs(leftStickHorizontal) > 0.1f)
		{
			transform.Rotate(Vector3.forward * rotationSpeed * leftStickHorizontal * -1f);
		}

		///// x axis rotation /////
		if (allowXrotation)
		{
			// keyboard input
			if (Input.GetKey(KeyCode.UpArrow))
			{
				transform.Rotate(Vector3.right * rotationSpeed);
			}
			else if (Input.GetKey(KeyCode.DownArrow))
			{
				transform.Rotate(-Vector3.right * rotationSpeed);
			}
			// controller input
			if (System.Math.Abs(leftStickVertical) > 0.1f)
			{
				print(leftStickVertical);
				transform.Rotate(Vector3.right * rotationSpeed * leftStickVertical * -1f);
			}
		}
		

		

		rigidBody.freezeRotation = false;
	}

	private void RestartCurrentLevel()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	private void GetNextLevel()
	{
		if (SceneManager.GetActiveScene().buildIndex == lastSceneIndex)
		{
			StartFirstLevel();
		}
		else
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		}
	}

	private static void StartFirstLevel()
	{
		SceneManager.LoadScene(0);
	}

	void OnCollisionEnter(Collision collision)
	{
		switch (collision.gameObject.tag)
		{
			case "Friendly":
				//do nothing
				break;
			case "Finish":
				if (state != State.DEAD)
				{
					StartSuccessSequence();
				}
				break;
			default:
				if (state != State.FINISHED)
				{
					StartDeathSequence();
				}
				break;
		}
	}

		private void StartSuccessSequence()
	{
		if (state != State.FINISHED)
		{
			state = State.FINISHED;
			audioSource.Stop();
			audioSource.PlayOneShot(success);
			successParticle.Play();
			Invoke("GetNextLevel", levelLoadDelay);
		}
	}

	private void StartDeathSequence()
	{
		if (state != State.DEAD)
		{
			state = State.DEAD;
			audioSource.Stop();
			audioSource.PlayOneShot(death);
			deathParticle.Play();
			Invoke("RestartCurrentLevel", levelLoadDelay);
		}
	}
}
