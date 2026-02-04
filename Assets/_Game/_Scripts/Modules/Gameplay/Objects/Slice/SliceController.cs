using DG.Tweening;
using EzySlice;
using UnityEngine;

public class SliceController : MonoBehaviour
{
    [SerializeField] private CuttingBoard _cuttingBoard;
    [SerializeField] private float _slicePositionY;
    private Sequence _chopSequence;
    private Vector3 _initialPosition;

    private void Awake()
    {
        _initialPosition = transform.localPosition;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Chop();
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<SliceableObject>(out var sliceableObject))
        {
            Slice(sliceableObject);
        }
    }

    private void Slice(SliceableObject objToSlice)
    {
        SlicedHull sliceHull = objToSlice.gameObject.Slice(transform.position, transform.right, objToSlice.MeshRenderer.material);
        GameObject upperHull = sliceHull.CreateUpperHull(objToSlice.gameObject, objToSlice.MeshRenderer.material);
        upperHull.AddComponent<MeshCollider>().convex = true;
        var upperRb = upperHull.AddComponent<Rigidbody>();

        GameObject lowerHull = sliceHull.CreateLowerHull(objToSlice.gameObject, objToSlice.MeshRenderer.material);  
        lowerHull.AddComponent<MeshCollider>().convex = true;
        var lowerRb = lowerHull.AddComponent<Rigidbody>();

        Vector3 pushDirection = transform.right;
        upperRb.AddForce(pushDirection * 1f, ForceMode.Impulse);
        lowerRb.AddForce(-pushDirection * 1f, ForceMode.Impulse);
        Destroy(objToSlice.gameObject);
    }

    private void Chop()
    {
        if(_chopSequence != null)
        {
            return;
        }

        _chopSequence = DOTween.Sequence();
        _chopSequence.Append(transform.DOLocalMoveY(_slicePositionY, 0.2f).SetEase(Ease.InQuad).SetLink(gameObject));
        _chopSequence.Append(transform.DOLocalMoveY(_initialPosition.y, 0.2f).SetEase(Ease.OutQuad).SetLink(gameObject));
        _chopSequence.OnComplete(() => _chopSequence = null);   
    }
}
