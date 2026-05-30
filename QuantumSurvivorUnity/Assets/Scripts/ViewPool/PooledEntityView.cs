using System;
using System.Collections;
using UnityEngine;

public class PooledEntityView : EntityView
{
  public void OnDestroyEntity()
  {
    PooledEntityViewUpdater.Instance.StoreOnPool(this);
  }
}

