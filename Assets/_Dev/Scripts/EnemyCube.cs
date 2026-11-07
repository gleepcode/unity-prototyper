using UnityEngine;

public class EnemyCube : MonoBehaviour, IClickable
{
    [SerializeField] private int health = 1;
    [SerializeField] private GameObject deathEffect;

    public void OnClicked()
    {
        health--;
        Debug.Log("Ow!");
        
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        Instantiate(deathEffect, transform.position, Quaternion.identity);
    }
}