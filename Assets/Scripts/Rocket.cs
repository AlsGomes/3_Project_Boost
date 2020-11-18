using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 300f;
    [SerializeField] float mainThrust = 3000f;
    [SerializeField] float levelLoadDelay;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip sucess;
    [SerializeField] AudioClip death;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    Rigidbody rigidbody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending }
    State state;

    enum CollisionState { On, Off };
    CollisionState currentCollisionState;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        state = State.Alive;
        levelLoadDelay = 1f;
        currentCollisionState = CollisionState.On;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotatetInput();

            if (Debug.isDebugBuild)
            {
                RespondToNextLevelInput();
                RespondToTurnOnOrOffCollisionInput();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive)
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                if (currentCollisionState == CollisionState.On)
                {
                    StartDeathSequence();
                }
                break;
        }
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke(nameof(LoadCurrentLevel), levelLoadDelay);
    }

    private void StartSuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(sucess);
        successParticles.Play();
        Invoke(nameof(LoadNextLevel), levelLoadDelay);
    }

    private void LoadCurrentLevel()
    {
        state = State.Alive;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadNextLevel()
    {
        state = State.Alive;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextScene = currentSceneIndex == SceneManager.sceneCountInBuildSettings - 1 ? 0 : ++currentSceneIndex;
        SceneManager.LoadScene(nextScene);
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidbody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    private void RespondToRotatetInput()
    {
        rigidbody.freezeRotation = true; //We control the physics when rotating
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame, Space.Self);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame, Space.Self);
        }

        rigidbody.freezeRotation = false; //Restore the physics to default
    }

    private void RespondToNextLevelInput()
    {
        if (Input.GetKey(KeyCode.L))
        {
            Invoke(nameof(LoadNextLevel), levelLoadDelay);
        }
    }

    private void RespondToTurnOnOrOffCollisionInput()
    {
        if (Input.GetKey(KeyCode.C))
        {
            if (currentCollisionState == CollisionState.On)
            {
                currentCollisionState = CollisionState.Off;
            }
            else
            {
                currentCollisionState = CollisionState.On;
            }
        }
    }
}
