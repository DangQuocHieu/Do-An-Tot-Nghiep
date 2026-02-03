using Cinemachine;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class CuttingBoard : InteractableObjectBase
{
    [Header("Cutting Board Properties")]
    [SerializeField] private Transform[] _cameraTransforms;
    [SerializeField] private CinemachineVirtualCamera _cuttingCamera;
    [SerializeField] private SliceController _slicer;
    private Vector3 _characterPosition;
    public void EnterCutMode(KnifeObject knife, PickupHandler pickupHandler)
    {
        knife.gameObject.SetActive(false);
        _characterPosition = pickupHandler.transform.position;
        TurnOnCamera(knife);
        _slicer.gameObject.SetActive(true);
        StartCoroutine(WaitForExitInput(knife));
        
    }

    public void ExitCutMode(KnifeObject knifeObject)
    {
        _cuttingCamera.gameObject.SetActive(false);
        _cuttingCamera.transform.SetParent(transform);
        _slicer.gameObject.SetActive(false);
        knifeObject.gameObject.SetActive(true);
    }

    private void TurnOnCamera(KnifeObject knife)
    {
        float currentDistanceToKnife = float.MaxValue;
        foreach(var point in _cameraTransforms)
        {
            if(Vector3.Distance(point.position, _characterPosition) < currentDistanceToKnife)
            {
                currentDistanceToKnife = Vector3.Distance(point.position, _characterPosition);
                _cuttingCamera.transform.SetParent(point);
                _cuttingCamera.transform.localPosition = Vector3.zero;
                _cuttingCamera.transform.localRotation = Quaternion.Euler(Vector3.zero);
                _slicer.transform.rotation = Quaternion.Euler(0f, point.transform.rotation.eulerAngles.y, 0f);
            }
        }
        _cuttingCamera.gameObject.SetActive(true);
    }

    private IEnumerator WaitForExitInput(KnifeObject knifeObject)
    {
        yield return new WaitUntil(() => Input.GetMouseButton(1));
        ExitCutMode(knifeObject);
    }
}
