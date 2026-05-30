using Photon.Deterministic;

namespace Quantum
{
  public unsafe class MagneticLockSystem : SystemMainThreadFilter<MagneticLockSystem.Filter>
  {
    public struct Filter
    {
      public EntityRef Entity;
      public Transform2D* Transform;
      public MagneticLock* MagneticLock;
    }


    public override void Update(Frame f, ref Filter filter)
    {
      EntityRef target = filter.MagneticLock->Target;
      if (f.TryGet<Transform2D>(target, out var targetTransform))
      {
        FP speed = filter.MagneticLock->Time > 0 ? -filter.MagneticLock->Speed : filter.MagneticLock->Speed;
        filter.MagneticLock->Time -= f.DeltaTime;

        FPVector2 targetPosition = FPVector2.MoveTowards(filter.Transform->Position, targetTransform.Position, speed * f.DeltaTime);
        filter.Transform->Position = targetPosition;
        if (FPVector2.DistanceSquared(targetTransform.Position, filter.Transform->Position) < FP._0_10)
        {
          Collectible* collectible = f.Unsafe.GetPointer<Collectible>(filter.Entity);
          CollectibleSpec collectibleSpec = f.FindAsset<CollectibleSpec>(collectible->Spec.Id);
          collectibleSpec.PerformCollectibleEffect(f, filter.Entity, target);
          f.Destroy(filter.Entity);
        }
      }
    }
  }
}