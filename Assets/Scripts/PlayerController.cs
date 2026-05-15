using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Lanes")]
    [SerializeField] private float laneOffset = 2f;
    [SerializeField, Min(1)] private int laneCount = 3;
    [SerializeField] private float laneSwitchSpeed = 14f;

    [Header("Jump")]
    [SerializeField] private float jumpVelocity = 8f;
    [SerializeField] private float gravity = -25f;

    private int _laneIndex;
    private float _y;
    private float _yVel;
    private Vector2 _prevMove;

    void Awake()
    {
        Debug.Log("PlayerController Awake called");

        if (TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            Debug.Log("Rigidbody set to kinematic");
        }
        else
        {
            Debug.LogWarning("No Rigidbody found on Player!");
        }
    }

    void Start()
    {
        Debug.Log("PlayerController Start called");
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        Vector2 v = ctx.ReadValue<Vector2>();
        if (v.x > 0.5f && _prevMove.x <= 0.5f) ChangeLane(+1);
        else if (v.x < -0.5f && _prevMove.x >= -0.5f) ChangeLane(-1);
        if (v.y > 0.5f && _prevMove.y <= 0.5f && _y <= 0f) _yVel = jumpVelocity;
        _prevMove = v;
    }

    private void ChangeLane(int delta)
    {
        int half = laneCount / 2;
        _laneIndex = Mathf.Clamp(_laneIndex + delta, -half, half);
    }

    void Update()
    {
        _yVel += gravity * Time.deltaTime;
        _y += _yVel * Time.deltaTime;
        if (_y < 0f) { _y = 0f; _yVel = 0f; }

        Vector3 pos = transform.position;
        pos.x = Mathf.MoveTowards(pos.x, _laneIndex * laneOffset, laneSwitchSpeed * Time.deltaTime);
        pos.y = _y;
        pos.z = 0f;
        transform.position = pos;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"COLLISION DETECTED with: {collision.gameObject.name}, Tag: {collision.gameObject.tag}");

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("*** OBSTACLE HIT! GAME OVER ***");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
            else
            {
                Debug.LogError("GameManager.Instance is NULL!");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"TRIGGER ENTERED with: {other.gameObject.name}, Tag: {other.gameObject.tag}");

        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("*** OBSTACLE TRIGGER! GAME OVER ***");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
            else
            {
                Debug.LogError("GameManager.Instance is NULL!");
            }
        }
    }
}