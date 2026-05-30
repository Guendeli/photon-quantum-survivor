using Photon.Deterministic;

namespace Quantum
{
  unsafe partial struct CharacterController
  {
    // dangerous, runs in parallel (do NOT query from here)
    public void Update(FrameThreadSafe f, ref CharacterMovementSystem.Filter filter)
    {
      var frameUNSAFE = (Frame)f;
      if (frameUNSAFE.RuntimeConfig.UseSnapMovementation)
      {
        SnapMovementation(f, filter);
      }
      else
      {
        SmoothedMovementation(f, filter);
      }
    }

    private void SnapMovementation(FrameThreadSafe f, CharacterMovementSystem.Filter filter)
    {

      var cc = filter.CC;
      var transform = filter.Transform;
      var direction = filter.InputContainer->Input.Direction;
      var moving = direction != default;
      if (moving)
      {
        cc->Velocity = direction.Normalized * MaxSpeed;
      }
      else
      {
        cc->Velocity = FPVector2.Zero;
      }

      transform->Position += cc->Velocity * f.DeltaTime;

      f.TryGetSingleton<GameplayArea>(out var gameplayArea);
      transform->Position = gameplayArea.NormalizedPosition(transform->Position);
    }

    private void SmoothedMovementation(FrameThreadSafe f, CharacterMovementSystem.Filter filter)
    {

      var cc = filter.CC;
      var transform = filter.Transform;
      var direction = filter.InputContainer->Input.Direction;
      var moving = direction != default;
      if (moving)
      {
        cc->Velocity += direction * Acceleration * f.DeltaTime;
        if (cc->Velocity.Magnitude > MaxSpeed)
        {
          cc->Velocity = cc->Velocity.Normalized * MaxSpeed;
        }
        transform->Rotation = FPMath.Atan2(cc->Velocity.Y, cc->Velocity.X);
      }
      else
      {
        cc->Velocity *= (1 - Brake * f.DeltaTime);
      }

      transform->Position += cc->Velocity * f.DeltaTime;
    }
  }
}