using UnityEngine;

public class Ingredient : InteractableObjectBase
{
    public override void InteractWith(InteractableObjectBase otherObject, PickupHandler handler)
    {
        if(otherObject.TryGetComponent<PlaceableSurface>(out var surface))
        {
            MoveToPlaceableSurface(surface.SnapPoint == null ? surface.transform.position : surface.SnapPoint.position);
            handler.DropObjectInHand();
            
        }
    }
}
