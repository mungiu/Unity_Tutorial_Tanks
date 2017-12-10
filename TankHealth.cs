using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;          
    public Slider m_Slider;                        
    public Image m_FillImage;                      
    public Color m_FullHealthColor = Color.green;  
    public Color m_ZeroHealthColor = Color.red;    
    public GameObject m_ExplosionPrefab;
    
    
    private AudioSource m_ExplosionAudio;          
    private ParticleSystem m_ExplosionParticles;   
    private float m_CurrentHealth;  
    private bool m_Dead;            


    private void Awake()
    {
        //storing the explosion prefab components in a variable and instantiating the explosion
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        //making sure tank does not explode from the start
        m_ExplosionParticles.gameObject.SetActive(false);
    }

    //making sure that a new tank can spawn full health only
    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        //updating UI according to changes
        SetHealthUI();
    }


    /// <summary>
    /// Damages tank's health, updates UI health values, check's if tank is dead.
    /// </summary>
    /// <param name="amount">Amount of damage taken.</param>
    public void TakeDamage(float amount)
    {
        m_CurrentHealth -= amount;
        //updating UI according to changes
        SetHealthUI();

        if (m_CurrentHealth <= 0f && !m_Dead)
            OnDeath();
    }


    // Adjust the value and colour of the slider.
    private void SetHealthUI()
    {
        m_Slider.value = m_CurrentHealth;

        //interpolating between color a & b by c amount
        m_FillImage.color = Color.Lerp(
            m_ZeroHealthColor, 
            m_FullHealthColor,
            //current health ratio
            m_CurrentHealth / m_StartingHealth);
    }


    // Play the effects for the death of the tank and deactivate it.
    private void OnDeath()
    {
        m_Dead = true;
        //setting explosion position to tanks position
        m_ExplosionParticles.transform.position = transform.position;
        //activating explosion particles
        m_ExplosionParticles.gameObject.SetActive(true);

        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        //deactivating Tank gameobject
        gameObject.SetActive(false);
    }
}