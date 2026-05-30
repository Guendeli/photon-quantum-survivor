using System;
using UnityEngine;

public class SmoothedEntityView : EntityView
{
  [Header("Smoothing Parameters")]
  [Range(0, 1)] public Single InterpolatePositionFactor = 0.05f;
  [Range(0, 1)] public Single InterpolateRotationFactor = 0.05f;


  public void OnDestroyEntity()
  {
    PooledEntityViewUpdater.Instance.StoreOnPool(this);
  }

  protected override void ApplyTransform(ref UpdatePostionParameter param)
  {
    var framePredicted = QuantumRunner.Default?.Game?.Frames.Predicted;
    if (framePredicted == null)
    {
      base.ApplyTransform(ref param);
      return;
    }

    if (framePredicted.IsCulled(EntityRef))
    {
      transform.position = Vector3.Lerp(transform.position, param.NewPosition, Time.deltaTime / InterpolatePositionFactor);
      transform.rotation = Quaternion.Slerp(transform.rotation, param.NewRotation, Time.deltaTime / InterpolateRotationFactor);
    }
    else
    {
      base.ApplyTransform(ref param);
    }
  }
}

