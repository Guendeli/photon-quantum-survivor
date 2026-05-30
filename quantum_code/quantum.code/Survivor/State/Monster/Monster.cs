using Photon.Deterministic;

namespace Quantum
{
  unsafe partial struct Monster
  {
    public void UpdateInput(FrameThreadSafe f, ref MonsterInputSystem.Filter filter)
    {
      filter.InputContainer->Input = default;
      if (f.Exists(filter.Monster->Target))
      {
        var characterTransform = f.GetPointer<Transform2D>(filter.Monster->Target);
        PhysicsCollider2D characterCollider = f.Get<PhysicsCollider2D>(filter.Monster->Target);
        FP characterRadius = characterCollider.Shape.BroadRadius;

        var direction = characterTransform->Position - filter.Transform->Position;

        if (direction.Magnitude >= characterRadius)
        {
          filter.InputContainer->Input.Direction = direction.Normalized;
        }
      }
      else
      {
        filter.Monster->ShouldDie = true;
      }
    }
    public void SetMonsterTarget(Frame f)
    {
      FindTarget(f, out Target);
    }

    private bool FindTarget(Frame f, out EntityRef target)
    {
      target = default;
      if (f.Unsafe.TryGetPointerSingleton<TeamProgression>(out var team))
      {
        var rndIndex = f.RNG->Next(0, team->Characters.Length);
        target = team->Characters[rndIndex];
        if (f.Exists(target))
        {
          return true;
        }
      }
      return false;
    }
  }
}