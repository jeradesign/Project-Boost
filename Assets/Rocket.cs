using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    new Rigidbody rigidbody;
    AudioSource audioSource;
    
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip levelSound;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem levelParticles;

    [SerializeField] bool detectCollisions = true;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state != State.Alive) {
            return;
        }
        Thrust();
        Rotate();
        DebugKeys();
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch(collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                StartLevelSequence();
                break;
            default:
                if (detectCollisions)
                {
                    audioSource.Stop();
                    StartDeathSequence();
                }
                break;
        }
    }

    private void StartLevelSequence()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(levelSound);
        levelParticles.Play();
        state = State.Transcending;
        Invoke("LoadNextScene", 1f);
    }

    private void StartDeathSequence()
    {
        audioSource.PlayOneShot(deathSound);
        deathParticles.Play();
        state = State.Dying;
        Invoke("GoBackToStart", 1f);
    }

    private void GoBackToStart()
    {
        SceneManager.LoadScene(0);
        state = State.Alive;
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(1);
        state = State.Alive;
    }

    private void Thrust()
    {
        float thrustThisFrame = mainThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.Space))
        {
            rigidbody.AddRelativeForce(Vector3.up * thrustThisFrame);
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(mainEngine);
                mainEngineParticles.Play();
            }
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void Rotate()
    {
        rigidbody.freezeRotation = true; // take manual control of rotation

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidbody.freezeRotation = false; // resume physics control of rotation
    }

    private void DebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L)) {
            LoadNextScene();
        }
        if (Input.GetKeyDown(KeyCode.C)) {
            detectCollisions = !detectCollisions;
        }
    }
}
