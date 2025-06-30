using UnityEngine;
using UnityEngine.Windows;

public class CharacterMeshRotation : MonoBehaviour
{
    public float speedRotation = 10f;
    [SerializeField]
    private float rotationThreshold = 0.1f;
    private bool isRotation = false;
    private Quaternion targetRotation;

    private INetworkOwnership _networkOwnership;
    private INetworkTransform _networkTransform;
    public void Initialize(INetworkOwnership networkOwnership, INetworkTransform networkTransform)
    {
        if (networkOwnership == null)
        {
            Debug.LogError("CharacterMeshRotation: INetworkOwnership cannot be null. Disabling script.", this);
            this.enabled = false;
            return;
        }
        if (networkTransform == null)
        {
            Debug.LogError("CharacterMeshRotation: INetworkTransform cannot be null. Disabling script.", this);
            this.enabled = false;
            return;
        }

        _networkOwnership = networkOwnership;
        _networkTransform = networkTransform;

        this.enabled = _networkOwnership.IsLocalPlayer;
        if (_networkOwnership.IsLocalPlayer)
        {
            Debug.Log("CharacterMeshRotation: Initialized and Enabled for local player.");
        }
        else
        {
            Debug.Log("CharacterMeshRotation: Initialized and Disabled for remote player.");
        }
    }
    void OnEnable()
    {
        if (_networkOwnership != null)
        {
            KaventInputHandler.OnCharacterRotateChanged -= RotateTowardsDirection;
            KaventInputHandler.OnCharacterRotateChanged += RotateTowardsDirection;
            Debug.Log("CharacterMeshRotation OnEnable: Event registered.");
        }
    }

    void OnDisable()
    {
        if (_networkOwnership != null)
        {
            KaventInputHandler.OnCharacterRotateChanged -= RotateTowardsDirection;
            Debug.Log("CharacterMeshRotation OnDisable: Event unregistered.");
        }
    }
    public void RotateTowardsDirection(Vector2 targetDirection)
    {
        Vector3 _targetDirection = new Vector3(targetDirection.x, 0f, targetDirection.y).normalized;
        if (_targetDirection == Vector3.zero)
        {
            isRotation = false;
            return;
        }

        targetRotation = Quaternion.LookRotation(_targetDirection);
        isRotation = true;
    }
    private void Update()
    {
        if (!isRotation) return;
        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f) {
            transform.rotation = targetRotation;
            isRotation = false;
            if (_networkTransform != null)
            {
                _networkTransform.SendRotation(transform.rotation);
            }
            return;
        }
        
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speedRotation * Time.deltaTime);
        if (_networkTransform != null)
        {
            _networkTransform.SendRotation(transform.rotation);
        }
    }
}
