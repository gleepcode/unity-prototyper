using UnityEngine;

public class DestroyAfterDelay : MonoBehaviour
{
    [SerializeField] private float delayTime;

    private void Start()
    {
        Destroy(gameObject, delayTime);
    }
}
