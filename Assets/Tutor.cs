using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutor : MonoBehaviour
{
    TextMeshProUGUI tutorialTMP;
    [SerializeField] TutorialTag TutorialOkay = null;
    [SerializeField] TutorialTag OptionMenu = null;
    [SerializeField] TutorialTag PowersMenu = null;
    [SerializeField] TutorialTag FireWordButton = null;
    [SerializeField] TutorialTag EraseWordButton = null;
    [SerializeField] TutorialTag SecondLetter = null;
    [SerializeField] TutorialTag ThirdEnergyBar = null;

    [SerializeField] TutorialStep[] tutorialSteps = null;
    [SerializeField] GameObject arrowPrefab_UI = null;
    [SerializeField] GameObject arrowPrefab_Worldspace = null;

    UIDriver uid;
    public GameController gc;

    RectTransform arrow_UI;
    Transform arrow_worldSpace;
    Vector2 wayFar = new Vector2(2000f, 2000f);
    //state
    int currentStep = -1;
    TutorialStep currentTutorialStep;

    private void Start()
    {
        uid = FindObjectOfType<UIDriver>();
        uid.SetTutorRef(this);
        tutorialTMP = uid.GetTutorialTMP();
        gc = FindObjectOfType<GameController>();
        if (gc.isInTutorialMode == false)
        {
            uid.ToggleTutorialUIPanel(false);
            return;
        }
        else
        {
            uid.ToggleTutorialUIPanel(true);
        }

        currentTutorialStep = tutorialSteps[0];
        tutorialTMP.text = currentTutorialStep.instruction;
        arrow_UI = Instantiate(arrowPrefab_UI, tutorialTMP.transform).GetComponent<RectTransform>();
        arrow_worldSpace = Instantiate(arrowPrefab_Worldspace).transform;
        AdvanceToNextStepViaClick();

    }

    public void AdvanceToNextStepViaClick()
    {
        currentStep++;
        if (currentStep >= tutorialSteps.Length)
        {
            uid.ToggleTutorialUIPanel(false);
            arrow_UI.gameObject.SetActive(false);
            arrow_worldSpace.gameObject.SetActive(false);
            return;
        }
        currentTutorialStep = tutorialSteps[currentStep];
        tutorialTMP.text = currentTutorialStep.instruction;
        MoveArrowToCorrectPosition();
    }

    private void MoveArrowToCorrectPosition()
    {
        switch (currentTutorialStep.targetTutorialTag)
        {
                case TutorialTag.TagName.Nothing:
                arrow_UI.transform.parent = null;
                arrow_UI.localPosition = wayFar;
                arrow_worldSpace.transform.parent = null;
                arrow_worldSpace.position = wayFar;
                return;

            case TutorialTag.TagName.TutorialOkay:
                arrow_UI.transform.parent = TutorialOkay.transform;
                arrow_UI.localPosition = TutorialOkay.offsetForArrow;
                arrow_UI.rotation = Quaternion.Euler(0, 0, TutorialOkay.rotationForArrow);
                arrow_worldSpace.transform.parent = null;
                arrow_worldSpace.position = wayFar;
                return;

            case TutorialTag.TagName.EraseButton:
                arrow_UI.transform.parent = EraseWordButton.transform;
                arrow_UI.localPosition = EraseWordButton.offsetForArrow;
                arrow_UI.rotation = Quaternion.Euler(0, 0, EraseWordButton.rotationForArrow);
                arrow_worldSpace.transform.parent = null;
                arrow_worldSpace.position = wayFar;
                return;

            case TutorialTag.TagName.FireButton:
                arrow_UI.transform.parent = FireWordButton.transform;
                arrow_UI.localPosition = FireWordButton.offsetForArrow;
                arrow_UI.rotation = Quaternion.Euler(0, 0, FireWordButton.rotationForArrow);
                arrow_worldSpace.transform.parent = null;
                arrow_worldSpace.position = wayFar;
                return;

            case TutorialTag.TagName.OptionMenu:
                arrow_UI.transform.parent = OptionMenu.transform;
                arrow_UI.localPosition = OptionMenu.offsetForArrow;
                arrow_UI.rotation = Quaternion.Euler(0, 0, OptionMenu.rotationForArrow);
                arrow_worldSpace.transform.parent = null;
                arrow_worldSpace.position = wayFar;
                return;

            case TutorialTag.TagName.PowersMenu:
                arrow_UI.transform.parent = PowersMenu.transform;
                arrow_UI.localPosition = PowersMenu.offsetForArrow;
                arrow_UI.rotation = Quaternion.Euler(0, 0, PowersMenu.rotationForArrow);
                arrow_worldSpace.transform.parent = null;
                arrow_worldSpace.position = wayFar;
                return;

            case TutorialTag.TagName.SecondLetterTile:
                arrow_UI.transform.parent = SecondLetter.transform;
                arrow_UI.localPosition = SecondLetter.offsetForArrow;
                arrow_UI.rotation = Quaternion.Euler(0, 0, SecondLetter.rotationForArrow);
                arrow_worldSpace.transform.parent = null;
                arrow_worldSpace.position = wayFar;
                return;

            case TutorialTag.TagName.ThirdEnergyBar:
                arrow_UI.transform.parent = ThirdEnergyBar.transform;
                arrow_UI.localPosition = ThirdEnergyBar.offsetForArrow;
                arrow_UI.rotation = Quaternion.Euler(0, 0, ThirdEnergyBar.rotationForArrow);
                arrow_worldSpace.transform.parent = null;
                arrow_worldSpace.position = wayFar;
                return;

            case TutorialTag.TagName.WorldSpacePos:
                arrow_UI.transform.parent = null;
                arrow_worldSpace.transform.parent = null;
                arrow_worldSpace.position = currentTutorialStep.worldSpacePosition;
                arrow_worldSpace.rotation = Quaternion.Euler(0, 0, currentTutorialStep.arrowRotation);
                return;

            case TutorialTag.TagName.PlayerInWS:
                arrow_UI.transform.parent = null;
                if (!gc)
                {
                    gc = FindObjectOfType<GameController>();
                }
                arrow_worldSpace.transform.parent = gc.GetPlayer().transform;
                arrow_worldSpace.localPosition = currentTutorialStep.worldSpacePosition;
                arrow_UI.rotation = Quaternion.Euler(0, 0, currentTutorialStep.arrowRotation);
                return;

            default:
                arrow_UI.transform.parent = null;
                arrow_UI.localPosition = wayFar;
                arrow_worldSpace.transform.parent = null;
                arrow_worldSpace.position = wayFar;
                return;

        }

    }
}
