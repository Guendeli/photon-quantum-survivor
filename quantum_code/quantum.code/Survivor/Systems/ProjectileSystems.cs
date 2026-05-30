using Photon.Deterministic;
using Quantum.Task;

namespace Quantum
{
  public unsafe class ProjectileQueryInjectSystem : SystemMainThreadFilter<ProjectileQueryInjectSystem.Filter>
  {

    public struct Filter
    {
      public EntityRef Entity;
      public Transform2D* Transform;
      public Projectile* Projectile;
    }


    public override void Update(Frame f, ref Filter filter)
    {
      filter.Projectile->QueryID = -1;

      var spec = f.Assets.ProjectileSpec(filter.Projectile->Spec);
      spec.InjectQuery(f, ref filter);
    }


  }

  public unsafe class ProjectileQueryUpdateSystem : SystemMainThreadFilter<ProjectileQueryUpdateSystem.Filter>, ISignalOnComponentAdded<Projectile>
  {

    public void OnAdded(Frame f, EntityRef entity, Projectile* projectile)
    {
      projectile->QueryID = -1;
    }
    public struct Filter
    {
      public EntityRef Entity;
      public Transform2D* Transform;
      public Projectile* Projectile;
    }

    public override void Update(Frame f, ref Filter filter)
    {
      Projectile* projectile = filter.Projectile;
      var spec = f.Assets.ProjectileSpec(filter.Projectile->Spec);
      if (projectile->TTL <= FP._0)
      {
        spec.OnDestroy(f, filter.Entity, filter.Projectile);
      }
      else
      {
        spec.UpdateQuery(f, ref filter);

        if (projectile->TTL != spec.TTL)
        {
          spec.Move(f, ref filter);
        }
      }
      projectile->TTL -= f.DeltaTime;
    }
  }
}