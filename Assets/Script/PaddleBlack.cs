using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PaddleBlack : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float maxZ = 3.5f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void FixedUpdate()
    {
        float input = 0f;
        if (Keyboard.current.leftArrowKey.isPressed) input -= 1f;
        if (Keyboard.current.rightArrowKey.isPressed) input += 1f;

        Vector3 pos = rb.position;
        pos.z = Mathf.Clamp(pos.z + input * speed * Time.fixedDeltaTime, -maxZ, maxZ);
        rb.MovePosition(pos);
    }
}