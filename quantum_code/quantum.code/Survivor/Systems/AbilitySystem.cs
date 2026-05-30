using System.Reflection;
using Photon.Deterministic;

namespace Quantum
{
  public unsafe class AbiltiySystem : SystemMainThreadFilter<AbiltiySystem.Filter>
  {
    public struct Filter
    {
      public EntityRef Entity;
      public Abilities* Abilities;
      public Transform2D* Transform;
    }

    public override void Update(Frame f, ref Filter filter)
    {
      if (f.Unsafe.TryGetPointerSingleton<TeamProgression>(out var progression))
      {
        if (progression->State != GameState.Playing) return;
      }

      for (int i = 0; i < filter.Abilities->Slot.Length; i++)
      {
        Ability* ability = filter.Abilities->Slot.GetPointer(i);
        if (ability->IsEnabled == false) {
          continue;
        }
        ability->Time -= f.DeltaTime;

        if (ability->Time <= FP._0)
        {
          ability->Time = ability->Cooldown;
          var spec = f.Assets.ProjectileSpec(ability->Spec);
          FPVector2 direction = spec.GetDirection(f, filter.Entity, filter.Transform, ability);
          spec.Spawn(f, filter.Transform->Position, direction, ability->Damage);
        }
      }
    }
  }
}