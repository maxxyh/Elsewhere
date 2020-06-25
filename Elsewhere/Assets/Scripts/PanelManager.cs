using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    private int panelCounter;
    public List<CutScenePanelInput> cutscenePanelList;
    public List<CutScenePanelInput> postCutscenePanelList;
    private Dictionary<int, DialogueDisplay> sceneDialogueDisplays = new Dictionary<int, DialogueDisplay>();
    private Dictionary<int, DialogueDisplay> postSceneDialogueDisplays = new Dictionary<int, DialogueDisplay>();

    public static Action OnAllCrystalsCollected;

    [System.Serializable]
    public class CutScenePanelInput
    {
        public GameObject cutSceneGO;
        public int index;
    }

    private void Start()
    {
        panelCounter = 0;

        for (int i = 0; i < cutscenePanelList.Count; i++)
        {
            DialogueDisplay dp = cutscenePanelList[i].cutSceneGO.GetComponent<DialogueDisplay>();
            sceneDialogueDisplays.Add(cutscenePanelList[i].index, dp);
        }

        for (int i = 0; i < postCutscenePanelList.Count; i++)
        {
            DialogueDisplay dp = postCutscenePanelList[i].cutSceneGO.GetComponent<DialogueDisplay>();
            postSceneDialogueDisplays.Add(postCutscenePanelList[i].index, dp);
        }

        Unit.OnCrystalCollected += PlayCrystalDialogue;
    }
    public void PlayCrystalDialogue()
    {
        //AudioManager.Instance.PlaySFX()
        StartCoroutine(CrystalDialogue());
    }

    private IEnumerator CrystalDialogue()
    {
        yield return new WaitForSeconds(1f);

        CutScenePanelInput cutsceneDialogue = cutscenePanelList.Find(x => x.index == panelCounter);
        if (cutsceneDialogue != null)
        {
            cutsceneDialogue.cutSceneGO.SetActive(true);
            yield return new WaitUntil(() => this.sceneDialogueDisplays[panelCounter].endConvo);
            cutsceneDialogue.cutSceneGO.SetActive(false);
        }

        CutScenePanelInput postCutsceneDialogue = postCutscenePanelList.Find(x => x.index == panelCounter);
        if (postCutsceneDialogue != null)
        {
            postCutsceneDialogue.cutSceneGO.SetActive(true);
            yield return new WaitUntil(() => this.postSceneDialogueDisplays[panelCounter].endConvo);
            postCutsceneDialogue.cutSceneGO.SetActive(false);
        }
        panelCounter++;
        
        if (panelCounter == Math.Max(sceneDialogueDisplays.Count, postSceneDialogueDisplays.Count))
        {
            OnAllCrystalsCollected();
        }
    }
}
