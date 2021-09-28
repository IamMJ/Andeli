using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutor : MonoBehaviour
{
    TextMeshProUGUI tutorialTMP;
    [SerializeField] TutorialStep[] tutorialSteps = null;
    [SerializeField] GameObject arrowPrefab_UI = null;
    [SerializeField] GameObject arrowPrefab_Worldspace = null;

    RectTransform arrow_UI;
    Transform arrow_worldSpace;
    Vector2 wayFar = new Vector2(2000f, 2000f);
    //state
    int currentStep = -1;
    TutorialStep currentTutorialStep;

    private void Start()
    {
        currentTutorialStep = tutorialSteps[0];
        UIDriver uid = FindObjectOfType<UIDriver>();
        uid.SetTutorRef(this);
        tutorialTMP = uid.GetTutorialTMP();
        tutorialTMP.text = currentTutorialStep.instruction;
        arrow_UI = Instantiate(arrowPrefab_UI, tutorialTMP.transform).GetComponent<RectTransform>();
        arrow_worldSpace = Instantiate(arrowPrefab_Worldspace).transform;
        AdvanceToNextStep();

    }

    public void AdvanceToNextStep()
    {
        currentStep++;
        if (currentStep >= tutorialSteps.Length)
        {
            tutorialTMP.transform.parent.gameObject.SetActive(false);
        }
        currentTutorialStep = tutorialSteps[currentStep];
        tutorialTMP.text = currentTutorialStep.instruction;
        MoveArrowToCorrectPosition();
    }

    private void MoveArrowToCorrectPosition()
    {
        if (currentTutorialStep.positionAnchor == TutorialStep.PositionAnchor.relativeToTMP)
        {
            arrow_UI.transform.parent = tutorialTMP.transform;
            arrow_UI.localPosition = currentTutorialStep.position;
            arrow_worldSpace.position = wayFar;
        }
        else
        {
            arrow_UI.position = wayFar;
            arrow_worldSpace.position = currentTutorialStep.position;
            
        }
        arrow_UI.rotation = Quaternion.Euler(0, 0, currentTutorialStep.arrowRotation);
    }
}
