using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Quantum;

public class CharacterStatsView : QuantumCallbacks
{
  public Image HealthBar;

  private EntityView _entityView;

  // Start is called before the first frame update
  void Start()
  {
    _entityView = GetComponentInParent<EntityView>();
  }

  public override void OnUpdateView(QuantumGame game)
  {
    transform.rotation = Quaternion.Euler(90, 0, 0);

    Frame frame = game.Frames.Predicted;
    if (_entityView == null || frame.Exists(_entityView.EntityRef) == false)
    {
      return;
    }

    Frame f = game.Frames.Verified;
    CharacterHealth stats = f.Get<CharacterHealth>(_entityView.EntityRef);
    HealthBar.fillAmount = (float)(stats.Health / stats.MaxHealth);
  }
}