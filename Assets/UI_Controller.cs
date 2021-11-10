using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Controller : MonoBehaviour
{
    [SerializeField] UI_Panel[] allPanels = null;
    [SerializeField] UI_Panel startMenuPanel = null;
    [SerializeField] public UI_Panel briefPanel = null;
    [SerializeField] UI_Panel characterMenuPanel = null;
    [SerializeField] UI_Panel combatPanel = null;
    [SerializeField] UI_Panel debriefPanel = null;
    [SerializeField] UI_Panel rewardPanel = null;
    [SerializeField] UI_Panel debugPanel = null;

    public enum Context {StartMenu, Brief, Overworld, CharacterMenu, Combat, Debrief, Reward}
    void Start()
    {
        foreach (var panel in allPanels)
        {
            panel.gameObject.SetActive(true);
            panel.ShowHideElements(false);
        }
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
                return;

            case Context.Brief:
                foreach (var panel in allPanels)
                {
                    panel.ShowHideElements(false);
                }
                briefPanel.ShowHideElements(true);
                return;

            case Context.Overworld:
                foreach (var panel in allPanels)
                {
                    panel.ShowHideElements(false);
                }
                //could show any overworld-centric panels here, like a map?
                return;

            case Context.CharacterMenu:
                foreach (var panel in allPanels)
                {
                    panel.ShowHideElements(false);
                }
                characterMenuPanel.ShowHideElements(true);
                return;

            case Context.Combat:
                foreach (var panel in allPanels)
                {
                    panel.ShowHideElements(false);
                }
                combatPanel.ShowHideElements(true);
                return;

            case Context.Debrief:
                foreach (var panel in allPanels)
                {
                    panel.ShowHideElements(false);
                }
                debriefPanel.ShowHideElements(true);
                return;

            case Context.Reward:
                foreach (var panel in allPanels)
                {
                    panel.ShowHideElements(false);
                }
                rewardPanel.ShowHideElements(true);
                return;
        }
    }

    public void ShowHideDebugMenuButton(bool shouldBeShown)
    {
        debugPanel.gameObject.SetActive(shouldBeShown);
        debugPanel.GetComponent<DebugPanel>().ShowHideDebugButton(shouldBeShown);
    }
}
