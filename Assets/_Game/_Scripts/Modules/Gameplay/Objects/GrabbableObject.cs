using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabbableObject : MonoBehaviour
{
    private Rigidbody _objectRb;
    private Transform _holdObjectPoint;

    [Header("Physics Settings")]
    [SerializeField] private float _followSpeed = 15f;
    [SerializeField] private float _rotationSpeed = 20f;
    private float _initialDrag;
    private float _initialAngularDrag;
    private Quaternion _initialRotation;

    private void Awake()
    {
        _objectRb = GetComponent<Rigidbody>();
        _initialDrag = _objectRb.linearDamping;
        _initialAngularDrag = _objectRb.angularDamping;
        _initialRotation = transform.rotation;
    }

    private void Update()
    {
        if (_holdObjectPoint != null)
        {
            MoveToTarget();
            RotateToTarget();
        }
    }

    private void MoveToTarget()
    {
        Vector3 positionDifference = _holdObjectPoint.position - transform.position;
        _objectRb.linearVelocity = positionDifference * _followSpeed;
    }

    private void RotateToTarget()
    {
        Quaternion rotationDifference = _holdObjectPoint.rotation * Quaternion.Inverse(transform.rotation);
        rotationDifference.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);
        if (angleInDegrees > 180f)
        {
            angleInDegrees -= 360f;
        }
        if (Mathf.Abs(angleInDegrees) > Mathf.Epsilon)
        {
            Vector3 angularVelocity = (rotationAxis * (angleInDegrees * Mathf.Deg2Rad)) * _rotationSpeed;
            _objectRb.angularVelocity = angularVelocity;
        }
    }

    public void OnPickup(Transform holdObjectPoint)
    {
        _holdObjectPoint = holdObjectPoint;

        _objectRb.useGravity = false;
        _objectRb.linearDamping = 10f;
        _objectRb.angularDamping = 5f;
        _objectRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        _objectRb.MoveRotation(_initialRotation);
    }

    public void OnDrop()
    {
        _holdObjectPoint = null;
        _objectRb.useGravity = true;
        _objectRb.linearDamping = _initialDrag;
        _objectRb.angularDamping = _initialAngularDrag;
        _objectRb.constraints = RigidbodyConstraints.None;
    }
}