using UnityEngine;
public interface IMovable
{
    void SetInputLeft(Vector2 input);
    void Move();
    void ApplySpeedMultiplier(float multiplier, float duration);
    void ResetSpeed();
}