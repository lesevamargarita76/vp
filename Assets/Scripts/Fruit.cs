using UnityEngine;

public class Fruit : MonoBehaviour
{
    [Header("Fruit Settings")]
    [SerializeField] private GameObject whole;
    [SerializeField] private GameObject sliced;

    [SerializeField] private int points = 1;

    private Rigidbody body;
    private Collider trigger;
    private ParticleSystem juice;
    private bool isSliced;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        trigger = GetComponent<Collider>();
        juice = GetComponentInChildren<ParticleSystem>();
    }

    public void Cut(Blade blade)
    {
        Slice(blade.Direction, blade.transform.position, blade.SliceForce);
    }

    private void Slice(Vector3 direction, Vector3 hitPoint, float force)
    {
        if (isSliced)
        {
            return;
        }

        isSliced = true;

        GameManager.Instance.IncreaseScore(points);

        trigger.enabled = false;
        whole.SetActive(false);

        sliced.SetActive(true);

        if (juice != null)
        {
            juice.Play();
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        sliced.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Rigidbody[] slices = sliced.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody slice in slices)
        {
            slice.linearVelocity = body.linearVelocity;
            slice.AddForceAtPosition(direction * force, hitPoint, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Blade blade = other.GetComponent<Blade>();

            if (blade != null)
            {
                Cut(blade);
            }
        }
    }

}
