using System;
using Photon.Deterministic;

namespace Quantum
{

  [Serializable]
  public unsafe class BombProjectileSpec : ProjectileSpec
  {
    public FP Radius;


    public override void InjectQuery(Frame f, ref ProjectileQueryInjectSystem.Filter filter)
    {
      Shape2D shape = Shape2D.CreateCircle(Radius);
      filter.Projectile->QueryID = f.Physics2D.AddOverlapShapeQuery(*filter.Transform, shape, Mask, QueryOptions);
    }
  }
}
