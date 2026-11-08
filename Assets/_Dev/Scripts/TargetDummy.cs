using UnityEngine;

public class TargetDummy : MonoBehaviour, IDamageable
{
    [SerializeField] private float health = 5f;
    [SerializeField] private GameObject deathEffect;

    AudioSource audioData;
    void Start()
    {
        audioData = GetComponent<AudioSource>();
    }

    public void OnDamage(float damage)
    {
        Debug.Log($"{gameObject.name} took {damage} damage.");
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        audioData.Play();
    }
     void OnDestroy()
    {
        Instantiate(deathEffect, transform.position, Quaternion.identity);
    }
}
