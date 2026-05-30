using Quantum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : QuantumCallbacks
{
  private EntityView _targetView;
  private EntityViewUpdater _updater;

  private void Start()
  {
    _updater = GameObject.FindObjectOfType<EntityViewUpdater>();
    QuantumEvent.Subscribe<EventOnCharacterCreated>(this, OnCharacterCreated);
  }

  private void OnCharacterCreated(EventOnCharacterCreated e)
  {
    if (e.Game.PlayerIsLocal(e.Link.Player))
    {
      _targetView = _updater.GetView(e.Entity);
    }
  }

  private void FindTargetView(QuantumGame game)
  {
    Frame f = game.Frames.Verified;
    if (f.TryGetSingleton<TeamProgression>(out var team))
    {
      for (int i = 0; i < team.Characters.Length; i++)
      {
        if (f.Exists(team.Characters[i]) == false)
        {
          continue;
        }
        PlayerLink link = f.Get<PlayerLink>(team.Characters[i]);
        if (game.PlayerIsLocal(link.Player))
        {
          _targetView = _updater.GetView(team.Characters[i]);
        }
      }
      if (_targetView == null)
      {
        for (int i = 0; i < team.Characters.Length; i++)
        {
          if (f.Exists(team.Characters[i]) == false)
          {
            continue;
          }
          _targetView = _updater.GetView(team.Characters[i]);
          return;
        }
      }
    }
  }

  public override void OnUpdateView(QuantumGame game)
  {
    if (_targetView == null)
    {
      FindTargetView(game);
      return;
    }

    var playerView = _targetView.transform;
    transform.position = new Vector3(playerView.transform.position.x, transform.position.y, playerView.transform.position.z);
  }
}
