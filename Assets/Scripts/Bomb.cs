using UnityEngine;

public class Bomb : MonoBehaviour
{
    private bool hasDetonated;

    public void Detonate()
    {
        if (hasDetonated)
        {
            return;
        }

        hasDetonated = true;

        GetComponent<Collider>().enabled = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.Explode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Detonate();
        }
    }

}
