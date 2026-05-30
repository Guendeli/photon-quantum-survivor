using System;
using Photon.Deterministic;

namespace Quantum
{

  [Serializable]
  public unsafe class WhipProjectileSpec : ProjectileSpec
  {
    public Shape2DConfig ShapeConfig;
    public FPVector2 Offset = new FPVector2(0, 1);
    public override unsafe void Spawn(Frame f, FPVector2 position, FPVector2 direction, FP damage)
    {
      base.Spawn(f, position + Offset, FPVector2.Left, damage);
      base.Spawn(f, position - Offset, FPVector2.Right, damage);
    }

    public override void InjectQuery(Frame f, ref ProjectileQueryInjectSystem.Filter filter)
    {
      Shape2D shape = ShapeConfig.CreateShape(f);
      filter.Projectile->QueryID = f.Physics2D.AddOverlapShapeQuery(filter.Transform->Position, filter.Transform->Rotation, shape, Mask, QueryOptions);
    }
  }
}
