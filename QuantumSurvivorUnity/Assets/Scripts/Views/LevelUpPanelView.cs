using Quantum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelUpPanelView : QuantumCallbacks
{
  public GameObject PanelContent;
  public TextMeshProUGUI TimeLeftText;
  public TextMeshProUGUI[] OptionsLevels;
  private void Start()
  {
    QuantumEvent.Subscribe<EventOnLevelUp>(this, OnLevelUp);
    QuantumEvent.Subscribe<EventOnHideLevelUpPanel>(this, OnHidePanelEvent);
    PanelContent.SetActive(false);
  }

  public override void OnUpdateView(QuantumGame game)
  {
    Frame f = game.Frames.Verified;
    TeamProgression progression = f.GetSingleton<TeamProgression>();
    TimeLeftText.text = "Time Left: " + progression.TimeToSelect.ToString("#.00");
  }

  private void OnLevelUp(EventOnLevelUp e)
  {
    Frame f = e.Game.Frames.Verified;
    int localPlayer = e.Game.GetLocalPlayers()[0];
    PlayerLink link = f.Get<PlayerLink>(e.Character);
    if (link.Player == localPlayer)
    {
      Abilities abilities = f.Get<Abilities>(e.Character);
      for (int i = 0; i < e.Selection.Options.Length; i++)
      {
        if (abilities.Slot[i].IsEnabled == false)
        {
          OptionsLevels[i].text = "UNLOCK";
        }
        else
        {
          OptionsLevels[i].text = "Level: " + (abilities.Slot[i].Level);
        }
        //var spec = f.FindAsset<PowerUpSpec>(e.Selection.Options[i].Id) as AbilityPowerUpSpec;
        //if (spec != null)
        //{
        //  spec.AbilityProjectileSpec.
        //}
      }
      PanelContent.SetActive(true);
    }

  }

  private void OnHidePanelEvent(EventOnHideLevelUpPanel e)
  {
    PanelContent.SetActive(false);
  }

  public void OnMoreTimeButtonClicked()
  {
    CommandsManager.Instance.SendMoreTimeCommand();
  }

  public void OnPowerUpButtonClicked(int index)
  {
    CommandsManager.Instance.SendChoosePowerUpCommand(index);
  }
}
