using DG.Tweening;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InteractableObjectBase : MonoBehaviour
{
    [Header("Base Properties")]
    [SerializeField] private Rigidbody _objectRb;
    private Transform _holdObjectPoint;
    private Collider _objectCollider;
    [SerializeField] private float _followSpeed = 15f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private Vector3 _grabRotation = Vector3.zero;
    private float _initialAngularDamping;
    private float _initialLinearDamping;
    private bool _canGrab = true;

    //Getter & Setter
    public Collider ObjectCollider => _objectCollider;
    public bool CanGrab => _canGrab;

    protected virtual void Awake()
    {
        _objectRb = GetComponent<Rigidbody>();
        _objectCollider = GetComponent<Collider>();
        SetUpRigidbody();
    }

    private void SetUpRigidbody()
    {
        _objectRb.interpolation = RigidbodyInterpolation.Interpolate;
        _objectRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _initialLinearDamping = _objectRb.linearDamping;
        _initialAngularDamping = _objectRb.angularDamping;
    }

    private void FixedUpdate()
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
        Quaternion rotationDelta = _holdObjectPoint.rotation * Quaternion.Inverse(_objectRb.rotation);
        rotationDelta.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);
        if (angleInDegrees > 180f) angleInDegrees -= 360f;

        if (Mathf.Abs(angleInDegrees) > 0.1f)
        {
            Vector3 angularDisplacement = rotationAxis * (angleInDegrees * Mathf.Deg2Rad);
            _objectRb.angularVelocity = angularDisplacement * _rotationSpeed;
        }
        else
        {
            _objectRb.angularVelocity = Vector3.zero;
        }
    }

    public virtual void OnPickup(Transform holdObjectPoint)
    {
        _objectRb.MoveRotation(Quaternion.Euler(_grabRotation));
        _canGrab = false;
        _holdObjectPoint = holdObjectPoint;
        _objectCollider.isTrigger = true;
        _objectRb.useGravity = false;
        _objectRb.linearDamping = 10f;
        _objectRb.angularDamping = 5f;
        _objectRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

    }
    public virtual void InteractWith(InteractableObjectBase otherObject, PickupHandler pickupHandler) 
    {

    }

    public void MoveToPlaceableSurface(Vector3 dropPosition)
    {
        _objectCollider.isTrigger = false;
        _holdObjectPoint = null;
        StartCoroutine(MoveToDropPosition(dropPosition));
    }

    private IEnumerator MoveToDropPosition(Vector3 dropPosition)
    {
        dropPosition += new Vector3(0f, 0.5f, 0f);
        yield return transform.DOMove(dropPosition, 0.25f).SetEase(Ease.Linear).SetLink(gameObject).WaitForCompletion();
        yield return new WaitForEndOfFrame();
        _objectRb.useGravity = true;
        _objectRb.linearDamping = _initialLinearDamping;
        _objectRb.angularDamping = _initialAngularDamping;
        _canGrab = true;
        _objectRb.constraints = RigidbodyConstraints.None;
    }
}
