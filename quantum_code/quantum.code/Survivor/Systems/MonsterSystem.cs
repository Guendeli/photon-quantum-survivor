using Photon.Deterministic;
using Quantum;
using Quantum.Task;

namespace Quantum
{
  public unsafe class MonsterSystem : SystemMainThreadFilter<MonsterSystem.Filter>
  {
    public struct Filter
    {
      public EntityRef Entity;
      public Transform2D* Transform;
      public Monster* Monster;
    }

    public override void Update(Frame f, ref Filter filter)
    {
      if (filter.Monster->ShouldDie)
      {
        // move to signal
        if (filter.Monster->Drop != default)
        {
          var drop = f.Create(filter.Monster->Drop);
          if (f.Unsafe.TryGetPointer<Transform2D>(drop, out var t))
          {
            t->Position = filter.Transform->Position;
          }
        }
        f.Destroy(filter.Entity);
      }
      else
      {
        if (f.Exists(filter.Monster->Target) == false)
        {
          f.Signals.SetMonsterTarget(filter.Entity);
        }
      }
    }
  }
}
