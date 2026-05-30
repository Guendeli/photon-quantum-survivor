using Quantum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledEntityViewUpdater : EntityViewUpdater
{
    public static PooledEntityViewUpdater Instance;
    private ViewPool _viewPool;
    [SerializeField]
    private HideFlags _flags;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        if (_viewPool == null)
        {
            _viewPool = GetComponent<ViewPool>();
        }
    }

    protected override EntityView CreateEntityViewInstance(EntityViewAsset asset, AssetGuid guid,
        Vector3? position = null, Quaternion? rotation = null)
    {
        Debug.Assert(asset.View != null);

        EntityView pooledView = _viewPool.GetPooledObject(guid);
        if (pooledView == null)
        {
            EntityView view = base.CreateEntityViewInstance(asset, guid, position, rotation);
            view.gameObject.hideFlags = _flags;
            return view;
        }
        else
        {
            return pooledView;
        }
    }

    protected override void DestroyEntityView(QuantumGame game, EntityView view)
    {
        Debug.Assert(view != null);
        view.OnEntityDestroyed.Invoke(game);

        if (view.ManualDisposal == false)
        {
            Debug.LogWarning("Not Pooled Entity! " + view.AssetGuid);
            if (view.AssetGuid.IsValid)
            {
                DestroyEntityViewInstance(view);
            }
            else
            {
                DisableMapEntityInstance(view);
            }
        }
        else
        {
            _viewPool.StoreObject(view);
        }
    }

    public void StoreOnPool(EntityView view)
    {
        _viewPool.StoreObject(view);
    }

}