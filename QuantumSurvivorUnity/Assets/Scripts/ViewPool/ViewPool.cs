using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quantum;
using UnityEngine.Pool;

public class ViewPool : MonoBehaviour
{
    Dictionary<AssetGuid, List<EntityView>> _pool = new Dictionary<AssetGuid, List<EntityView>>();

    public EntityView GetPooledObject(AssetGuid guid)
    {
    if (_pool.TryGetValue(guid, out var list))
        {
            for (int i = 0; i < list.Count; i++)
            {
                EntityView view = list[i];
                if (view.gameObject.activeInHierarchy == false)
                {
                    view.gameObject.SetActive(true);
                    list.RemoveAt(i);
                    return view;
                }
            }

        }
        return null;
    }

    public void StoreObject(EntityView view)
    {
        if (_pool.TryGetValue(view.AssetGuid, out var list))
        {
            view.gameObject.SetActive(false);
            list.Add(view);
        }
        else
        {
            List<EntityView> newList = new List<EntityView>();
            newList.Add(view);
            _pool.Add(view.AssetGuid, newList);
            view.gameObject.SetActive(false);
        }
    }
}
