using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private GameObject[] fruits;
    [SerializeField] private GameObject bomb;

    [Range(0f, 1f)]
    [SerializeField] private float bombChance = 0.05f;

    [SerializeField] private float minDelay = 0.25f;
    [SerializeField] private float maxDelay = 1f;

    [SerializeField] private float minAngle = -15f;
    [SerializeField] private float maxAngle = 15f;

    [SerializeField] private float minForce = 18f;
    [SerializeField] private float maxForce = 22f;

    [SerializeField] private float lifeTime = 5f;

    private Collider area;

    private void Awake()
    {
        area = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        StartCoroutine(Spawn());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(2f);

        while (enabled)
        {
            if (fruits == null || fruits.Length == 0)
            {
                yield return new WaitForSeconds(1f);
                continue;
            }

            GameObject prefab = fruits[Random.Range(0, fruits.Length)];

            if (bomb != null && Random.value < bombChance)
            {
                prefab = bomb;
            }

            Vector3 position = new Vector3
            {
                x = Random.Range(area.bounds.min.x, area.bounds.max.x),
                y = Random.Range(area.bounds.min.y, area.bounds.max.y),
                z = Random.Range(area.bounds.min.z, area.bounds.max.z)
            };

            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));

            GameObject fruit = Instantiate(prefab, position, rotation);
            Destroy(fruit, lifeTime);

            Rigidbody body = fruit.GetComponent<Rigidbody>();

            if (body != null)
            {
                float force = Random.Range(minForce, maxForce);
                body.AddForce(fruit.transform.up * force, ForceMode.Impulse);
            }

            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
        }
    }

}
