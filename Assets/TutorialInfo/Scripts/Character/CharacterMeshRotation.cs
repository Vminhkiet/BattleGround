using UnityEngine;
using UnityEngine.Windows;

public class CharacterMeshRotation : MonoBehaviour
{
    public float speedRotation = 10f;
    [SerializeField]
    private float rotationThreshold = 0.1f;
    private bool isRotation = false;
    private Quaternion targetRotation;
    void OnEnable()
    {
        KaventInputHandler.OnCharacterRotateChanged += RotateTowardsDirection;
    }

    void OnDisable()
    {
        KaventInputHandler.OnCharacterRotateChanged -= RotateTowardsDirection;
    }
    public void RotateTowardsDirection(Vector2 targetDirection)
    {
        Vector3 _targetDirection = new Vector3(targetDirection.x, 0f, targetDirection.y).normalized;
        targetRotation = Quaternion.LookRotation(_targetDirection);
        isRotation = true;
    }
    private void Update()
    {
        if (!isRotation) return;
        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f) {
            transform.rotation = targetRotation;
            isRotation = false;
            return;
        }
        
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speedRotation * Time.deltaTime);
    }
}
