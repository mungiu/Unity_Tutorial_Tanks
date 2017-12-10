using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;
    public ParticleSystem m_ExplosionParticles;
    public AudioSource m_ExplosionAudio;
    public float m_MaxDamage = 100f;
    public float m_ExplosionForce = 1000f;
    public float m_MaxLifeTime = 2f;
    public float m_ExplosionRadius = 5f;



    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime);
    }


    // Find all the tanks in an area around the shell and damage them.
    private void OnTriggerEnter(Collider other)
    {
        //shells don't collide with shells
        if (other.CompareTag("Shell"))
            return;

        //gathering all involved colliders to an array
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            //defining targetRigidbody
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();
            if (!targetRigidbody)
                //moving on to next collider
                continue;

            //simulating explosion force effect
            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            //defining targetHealth
            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();
            if (!targetHealth)
                continue;

            float damage = CalculateDamage(targetRigidbody.position);

            targetHealth.TakeDamage(damage);
        }

        //detaching from destroyed object
        m_ExplosionParticles.transform.parent = null;
        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);
        Destroy(gameObject);
    }


    // Calculate the amount of damage a target should take based on it's position.
    private float CalculateDamage(Vector3 targetPosition)
    {
        //calculating how close is the target to the explosion as Vector3
        Vector3 explosionToTarget = targetPosition - transform.position;
        //transforming the above Vector3 into distance
        float explosionDistance = explosionToTarget.magnitude;
        //transforming the above into a distance relative to explosionRadius with value between 0 and 1
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;
        //setting damage based on distance from explosion
        float damage = relativeDistance * m_MaxDamage;
        //making sure damage is not negative (does not heal player)
        damage = Mathf.Max(0f, damage);

        return damage;
    }
}