using UnityEngine;

public class Blade : MonoBehaviour
{
    [Header("Blade Settings")]
    [SerializeField] private float bladeForce = 5f;
    [SerializeField] private float minSpeed = 0.01f;
    [SerializeField] private float hitRadius = 1f;

    public Vector3 Direction { get; private set; }
    public bool Slicing { get; private set; }
    public float SliceForce => bladeForce;

    private Camera camera;
    private Collider hitCollider;
    private TrailRenderer trail;

    private void Awake()
    {
        camera = Camera.main;

        if (camera == null)
        {
            camera = FindAnyObjectByType<Camera>();
        }

        hitCollider = GetComponent<Collider>();
        trail = GetComponentInChildren<TrailRenderer>();
    }

    private void OnEnable()
    {
        StopSlice();
    }

    private void OnDisable()
    {
        StopSlice();
    }

    private void Update()
    {
        if (camera == null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            StartSlice();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopSlice();
        }
        else if (Slicing)
        {
            ContinueSlice();
        }
    }

    private void StartSlice()
    {
        Vector3 position = camera.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0f;
        transform.position = position;

        Direction = Vector3.zero;

        Slicing = true;
        hitCollider.enabled = true;
        trail.enabled = true;
        trail.Clear();
    }

    private void StopSlice()
    {
        Slicing = false;
        hitCollider.enabled = false;
        trail.enabled = false;
    }

    private void ContinueSlice()
    {
        Vector3 newPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        newPosition.z = 0f;
        Vector3 movement = newPosition - transform.position;
        Direction = movement;

        float speed = movement.magnitude / Time.deltaTime;
        bool fastEnough = speed > minSpeed;
        hitCollider.enabled = fastEnough;

        if (fastEnough)
        {
            SliceAlongPath(movement);
        }

        transform.position = newPosition;
    }

    private void SliceAlongPath(Vector3 movement)
    {
        float distance = movement.magnitude;

        if (distance <= Mathf.Epsilon)
        {
            return;
        }

        RaycastHit[] hits = Physics.SphereCastAll(
            transform.position,
            hitRadius,
            movement / distance,
            distance,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Collide);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.TryGetComponent(out Fruit fruit))
            {
                fruit.Cut(this);
                continue;
            }

            if (hit.collider.TryGetComponent(out Bomb bomb))
            {
                bomb.Detonate();
            }
        }
    }

}
