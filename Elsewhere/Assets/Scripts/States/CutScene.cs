using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CutScene : State
{
    public CutScene(TurnScheduler turnScheduler) : base(turnScheduler)
    {
    }

    public override IEnumerator Execute()
    {
        Button[] buttons = turnScheduler.playerActionPanel.GetComponentsInChildren<Button>();
        foreach(Button btn in buttons)
        {
            btn.interactable = false;
        }
        turnScheduler.DuringCutScene();
        yield break;
    }

    public override IEnumerator DuringCutScene()
    {
        yield return new WaitUntil(() => turnScheduler.openingDialogue.endConvo);
        Button[] buttons = turnScheduler.playerActionPanel.GetComponentsInChildren<Button>();
        foreach (Button btn in buttons)
        {
            btn.interactable = true;
        }
        turnScheduler.tutorialPanel.SetActive(false);
        
        turnScheduler.SetState(new TutTransition(turnScheduler));
    }
}
