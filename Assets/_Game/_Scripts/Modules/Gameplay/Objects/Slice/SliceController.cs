using DG.Tweening;
using EzySlice;
using UnityEngine;

public class SliceController : MonoBehaviour
{
    [SerializeField] private CuttingBoard _cuttingBoard;
    [SerializeField] private float _slicePositionY;
    [SerializeField] private float _minSizeToSlice = 0.1f;
    [SerializeField] private float _sensitivity = 1f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private Transform _slicerModel;
    [SerializeField] private float _meshVolumeMultiplier = 1000f;   
    private Sequence _chopSequence;
    private Vector3 _initialPosition;

    private void Awake()
    {
        _initialPosition = transform.localPosition;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
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
        SlicedHull sliceHull = objToSlice.gameObject.Slice(_slicerModel.position, _slicerModel.right, objToSlice.MeshRenderer.material);
        if(sliceHull != null)
        {
            Mesh upperMesh = sliceHull.upperHull;
            Mesh lowerMesh = sliceHull.lowerHull;

            if(!CheckMeshSize(upperMesh) || !CheckMeshSize(lowerMesh))
            {
                return;
            }

            SliceableObject upperHull = Instantiate(objToSlice, null);
            SliceableObject lowerHull = Instantiate(objToSlice, null); 
           
            lowerHull.ReplaceMesh(lowerMesh, objToSlice.MeshRenderer.materials);
            upperHull.ReplaceMesh(upperMesh, objToSlice.MeshRenderer.materials);

            lowerHull.Rb.AddForce(-_slicerModel.right * 0.5f, ForceMode.Impulse);
            upperHull.Rb.AddForce(_slicerModel.right * 0.5f, ForceMode.Impulse);

            Destroy(objToSlice.gameObject);

        }
    }

    private bool CheckMeshSize(Mesh mesh)
    {
        float volume = mesh.bounds.size.x * mesh.bounds.size.y * mesh.bounds.size.z;
        Debug.Log(volume * _meshVolumeMultiplier);
        return volume * _meshVolumeMultiplier >= _minSizeToSlice;
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

    private void HandleMovement()
    {
        float inputX = Input.GetAxis("Mouse X");
        float inputY = Input.GetAxis("Mouse Y");
        Vector3 moveX = transform.right * inputX * _sensitivity * Time.deltaTime;
        Vector3 moveZ = transform.forward * inputY * _sensitivity * Time.deltaTime;
        transform.position += (moveX + moveZ);
    }

    private void HandleRotation()
    {
        if(Input.GetKey(KeyCode.A))
        {
            _slicerModel.Rotate(transform.up, _rotationSpeed * Time.deltaTime);
        }

        else if(Input.GetKey(KeyCode.D))
        {
            _slicerModel.Rotate(transform.up, -_rotationSpeed * Time.deltaTime);
        }
    }
}
