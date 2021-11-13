using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Controller : MonoBehaviour
{
    [SerializeField] UI_Panel[] allPanels = null;
    [SerializeField] public StartMenuPanel startMenuPanel = null;
    [SerializeField] public BriefPanel briefPanel = null;
    [SerializeField] public UI_Panel characterMenuPanel = null;
    [SerializeField] public CombatPanel combatPanel = null;
    [SerializeField] public DebriefPanel debriefPanel = null;
    [SerializeField] public RewardPanel rewardPanel = null;
    [SerializeField] public AdvertPanel advertPanel = null;
    [SerializeField] public UpgradesPanel upgradesPanel = null;
    [SerializeField] public DebugPanel debugPanel = null;

    public enum Context {StartMenu, Brief, Overworld, CharacterMenu, Combat, 
        Debrief, Reward, Advert, Upgrades}

    GameController gc;

    void Start()
    {
        gc = Librarian.GetLibrarian().gameController;

        foreach (var panel in allPanels)
        {
            panel.gameObject.SetActive(true);
            panel.ShowHideElements(false);
        }
        debugPanel.gameObject.SetActive(true);
        debugPanel.ShowHideElements(false);

        startMenuPanel.ShowHideElements(true);
    }

    public void SetContext(Context newContext)
    {
        switch (newContext)
        {
            case Context.StartMenu:
                foreach (var panel in allPanels)
                {
                    panel.ShowHideElements(false);
                }
                startMenuPanel.ShowHideElements(true);
                gc.PauseGame();
                return;

            case Context.Brief:
                foreach (var panel in allPanels)
                {
                    panel.ShowHideElements(false);
                }
                briefPanel.ShowHideElements(true);
                gc.PauseGame();
                return;

            case Context.Overworld:
                foreach (var panel in allPanels)
                {
                    panel.ShowHideElements(false);
                }
                //could show any overworld-centric panels here, like a map?
                gc.ResumeGameSpeed(true);
                debugPanel.ShowHideDebugButton(gc.debug_ShowDebugMenuButton);
                return;

            case Context.CharacterMenu:
                foreach (var panel in allPanels)
                {
                    panel.ShowHideElements(false);
                }
                characterMenuPanel.ShowHideElements(true);
                gc.PauseGame();
                return;

            case Context.Combat:
                foreach (var panel in allPanels)
                {
                    panel.ShowHideElements(false);
                }
                combatPanel.ShowHideElements(true);
                gc.ResumeGameSpeed(true);
                debugPanel.ShowHideDebugButton(gc.debug_ShowDebugMenuButton);
                return;

            case Context.Debrief:
                foreach (var panel in allPanels)
                {
                    panel.ShowHideElements(false);
                }
                debriefPanel.ShowHideElements(true);
                gc.PauseGame();
                return;

            case Context.Reward:
                foreach (var panel in allPanels)
                {
                    panel.ShowHideElements(false);
                }
                rewardPanel.ShowHideElements(true);
                gc.PauseGame();
                return;

            case Context.Advert:
                foreach (var panel in allPanels)
                {
                    panel.ShowHideElements(false);
                }
                advertPanel.ShowHideElements(true);
                gc.PauseGame();
                return;

            case Context.Upgrades:
                foreach (var panel in allPanels)
                {
                    panel.ShowHideElements(false);
                }
                upgradesPanel.ShowHideElements(true);
                gc.PauseGame();
                debugPanel.ShowHideDebugButton(gc.debug_ShowDebugMenuButton);
                return;
        }
    }

    public void ShowHideDebugMenuButton(bool shouldBeShown)
    {
        debugPanel.gameObject.SetActive(shouldBeShown);
        debugPanel.GetComponent<DebugPanel>().ShowHideDebugButton(shouldBeShown);
    }
}
