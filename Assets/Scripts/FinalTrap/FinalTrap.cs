using UnityEngine;

public class FinalTrap : MonoBehaviour
{
    [SerializeField] GameObject finalCollider;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            finalCollider.SetActive(true);
            Destroy(gameObject);
        }
    }
}
