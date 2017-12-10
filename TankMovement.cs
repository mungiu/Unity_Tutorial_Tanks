using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;         
    public float m_Speed = 12f;            
    public float m_TurnSpeed = 180f;       
    public AudioSource m_MovementAudio;    
    public AudioClip m_EngineIdling;       
    public AudioClip m_EngineDriving;      
    public float m_PitchRange = 0.2f;
    

    private string m_MovementAxisName;     
    private string m_TurnAxisName;         
    private Rigidbody m_Rigidbody;         
    private float m_MovementInputValue;    
    private float m_TurnInputValue;        
    private float m_OriginalPitch = 0.2f;         


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    //called when script is turned on (after awake but before any updates)
    private void OnEnable ()
    {
        //bringing tank to life with 0 movement
        m_Rigidbody.isKinematic = false;
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }


    private void OnDisable ()
    {
        //tank death (no forces can be applied to kinematic object)
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        //reading player input and sorting by player number
        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

        m_OriginalPitch = m_MovementAudio.pitch;
    }

    // Store the player's input and make sure the audio for the engine is playing.
    private void Update()
    {
        //storing tank movement
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);

        EngineAudio();
    }


    private void EngineAudio()
    {
        //(Mathf.Abs turns any number to +, EG: negative "x-axis" movement)
        if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f)
        {
            if (m_MovementAudio.clip == m_EngineDriving)
            {
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range(
                    m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                
                m_MovementAudio.Play();
            }
        }
        else if (m_MovementAudio.clip == m_EngineIdling)
        {
            m_MovementAudio.clip = m_EngineDriving;
            m_MovementAudio.pitch = Random.Range(
                m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);

            m_MovementAudio.Play();
        }
    }

    //running every physics step (0.2 ms?)
    private void FixedUpdate()
    {
        Move();
        Turn();
    }


    private void Move()
    {
        // move the tank based on the player's input.
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }


    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
        //creating the amount of turn degrees
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
        //transforming the turn degree into a rotation
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        //applying the rotation
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }
}