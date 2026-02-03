using UnityEngine;

public class KnifeObject : InteractableObjectBase
{
    public override void InteractWith(InteractableObjectBase otherObject, PickupHandler pickupHandler)
    {
         if(otherObject is CuttingBoard cuttingBoard)
         {
            cuttingBoard.EnterCutMode(this, pickupHandler);
         }
    }
}
