using Photon.Deterministic;

namespace Quantum
{
  public unsafe class CollectibleSystem: SystemMainThread
  {
    public override void Update(Frame f)
    {
      int collectibleCount = f.ComponentCount<Collectible>();
      FP timeMultiplier = f.DeltaTime * collectibleCount * 10 / f.RuntimeConfig.CollectibleBaseCount;

      int schedulePeriod = 10;
      foreach (var (entity, c) in f.Unsafe.GetComponentBlockIterator<Collectible>())
      {
        if (entity.Index % schedulePeriod == f.Number % schedulePeriod)
        {
          CheckCollectDistance(f, entity, c);
          if (c->TTL <= FP._0)
          {
            f.Destroy(entity);
          }

          c->TTL -= timeMultiplier * schedulePeriod;
        }
      }
    }

    private bool CheckCollectDistance(Frame f, EntityRef entity, Collectible* collectible)
    {
      f.Unsafe.TryGetPointerSingleton<TeamProgression>(out var team);

      for (int i = 0; i < Constants.MaxPlayers; i++)
      {
        
        var character = team->Characters[i];
        if (character == default || !f.Unsafe.TryGetPointer<Transform2D>(character, out var transform) || !f.Unsafe.TryGetPointer<CollectibleArea>(character, out var collectibleArea))
          continue;

        FPVector2 position = f.Get<Transform2D>(entity).Position;
        if (FPVector2.DistanceSquared(transform->Position, position) <= collectibleArea->SquareDist)
        {
          if (f.Has<MagneticLock>(entity) == false)
          {
            f.Add<MagneticLock>(entity, out var magneticLock);
            magneticLock->Speed = 10;
            magneticLock->Target = character;
            magneticLock->Time = FP._0_10;
          }
          return true;
        }
      }

      return false;
    }
  }
}