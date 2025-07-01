using UnityEngine;
public class MovableSlowDecorator : IMovable
{
    private IMovable wrapped;

    public MovableSlowDecorator(IMovable wrapped)
    {
        this.wrapped = wrapped;
    }

    public void SetInputLeft(Vector2 input)
    {
        wrapped.SetInputLeft(input);
    }

    public void Move()
    {
        wrapped.Move();
    }

    public void ApplySpeedMultiplier(float multiplier, float duration)
    {
        wrapped.ApplySpeedMultiplier(multiplier, duration);
    }

    public void ResetSpeed()
    {
        wrapped.ResetSpeed();
    }
}
