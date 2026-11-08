using UnityEngine;

public class TargetDummy : MonoBehaviour, IDamageable
{
    [SerializeField] private int health = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDamage(float damage)
    {
        Debug.Log($"{gameObject.name} took {damage} damage.");
        
        health--;
        
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
