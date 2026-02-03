using UnityEngine;

public class PickupHandler : MonoBehaviour
{
    [SerializeField] private Transform _grabObjectPoint;
    [SerializeField] private Transform _camera;
    [SerializeField] private InteractableObjectBase _objectInHand;
    [SerializeField] private Collider _characterCollider;

    [Header("Pickup Settings")]
    [SerializeField] private float _pickupRange = 3f;
    
 
    private void Update()
    {
        HandlePickUpAndDropObject();
    }

    private void HandlePickUpAndDropObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_objectInHand == null)
            {
                if (Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit, _pickupRange))
                {
                    if(hit.collider.TryGetComponent<InteractableObjectBase>(out InteractableObjectBase interactableObject))
                    {
                        if (interactableObject.CanGrab)
                        {
                            _objectInHand = interactableObject;
                            interactableObject.OnPickup(_grabObjectPoint);
                        }
                    }
                }
            }
            else
            {
                if (Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit, _pickupRange))
                {
                    if(hit.collider.TryGetComponent<InteractableObjectBase>(out InteractableObjectBase other))
                    {
                        _objectInHand.InteractWith(other, this);
                    }
                    else if(hit.collider.TryGetComponent<PlaceableSurface>(out var surface))
                    {
                        if (hit.collider.gameObject == _objectInHand.gameObject) return; 
                        _objectInHand.MoveToPlaceableSurface(surface.SnapPoint == null ? hit.point : surface.SnapPoint.position);
                        DropObjectInHand();
                    }
                }
            }
        }
    }

    public void DropObjectInHand()
    {
        _objectInHand = null;
    }
}