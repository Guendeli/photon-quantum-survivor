using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Quantum;
using Photon.Deterministic;
using TMPro;

public class ExperienceBarView : QuantumCallbacks
{
  public Image ExperienceBar;
  public TextMeshProUGUI LevelText;
  private EntityRef _entityRef;
  // Start is called before the first frame update


  public override void OnUpdateView(QuantumGame game)
  {
    Frame f = game.Frames.Verified;
    if (f.Exists(_entityRef) == false)
    {
      FindTargetView(game);
    }
    else
    {
      TeamProgression progression = f.GetSingleton<TeamProgression>();
      LevelText.text = "Level "+ (int)progression.Level;
      var totalToNextLevel = LevelUpHelper.GetXPToLevel(progression.Level + 1);
      ExperienceBar.fillAmount = (float)(progression.XP / (totalToNextLevel));
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
          _entityRef = team.Characters[i];
        }
      }
    }
  }



}
