using UnityEngine;

public class PickupHandler : MonoBehaviour
{
    [SerializeField] private Transform _grabObjectPoint;
    [SerializeField] private Transform _camera;
    private GrabbableObject _objectInHand;

    [Header("Pickup Settings")]
    [SerializeField] private float _pickupRange = 3f;
    

    private void Update()
    {
        HandlePickUpAndDropObject();
    }

    private void HandlePickUpAndDropObject()
    {
        if(Input.GetMouseButtonDown(1))
        {
            if(_objectInHand == null)
            {
                if(Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit, _pickupRange))
                {
                    if(hit.collider.TryGetComponent<GrabbableObject>(out GrabbableObject obj))
                    {
                        _objectInHand = obj;
                        _objectInHand.OnPickup(_grabObjectPoint);
                    }
                }
            }
            else
            {
                _objectInHand.OnDrop();
                _objectInHand = null;
            }
        }
    }
}