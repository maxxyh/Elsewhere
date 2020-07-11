using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    private int panelCounter;
    public List<CutScenePanelInput> cutscenePanelList;
    public List<CutScenePanelInput> postCutscenePanelList;
    private Dictionary<int, List<DialogueDisplay>> sceneDialogueDisplays = new Dictionary<int, List<DialogueDisplay>>();
    private Dictionary<int, List<DialogueDisplay>> postSceneDialogueDisplays = new Dictionary<int, List<DialogueDisplay>>();
    private List<Crystal> _playerCapturedCrystals = new List<Crystal>();

    public static Action<bool> OnCrystalCaptureCutSceneDone;

    [System.Serializable]
    public class CutScenePanelInput
    {
        public List<GameObject> cutSceneGO;
        public int index;
    }

    private void Start()
    {

        panelCounter = 0;

        for (int i = 0; i < cutscenePanelList.Count; i++)
        {
            List<DialogueDisplay> dp = cutscenePanelList[i].cutSceneGO.Select( x => x.GetComponent<DialogueDisplay>() ).ToList();
            sceneDialogueDisplays.Add(cutscenePanelList[i].index, dp);
        }

        for (int i = 0; i < postCutscenePanelList.Count; i++)
        {
            List<DialogueDisplay> dp = postCutscenePanelList[i].cutSceneGO.Select( x => x.GetComponent<DialogueDisplay>() ).ToList();
            postSceneDialogueDisplays.Add(postCutscenePanelList[i].index, dp);
        }

        Crystal.OnPlayerCrystalCollected += PlayCrystalDialogue;
    }
    public void PlayCrystalDialogue(Crystal crystal)
    {
        //AudioManager.Instance.PlaySFX
        if (this != null)
        {
            StartCoroutine(CrystalDialogue(crystal));
        }
    }

    private IEnumerator CrystalDialogue(Crystal crystal)
    {
        if (CrystalDialogue(crystal) == null)
        {
            Debug.Log("crystal dialogue null");
        }

        if (this == null)
        {
            Debug.Log("panel manager null");
        }
        
        // only invoke dialogue if new capture
        if (_playerCapturedCrystals.Contains(crystal))
        {
            yield break;
        }
        else
        {
            _playerCapturedCrystals.Add(crystal);
        }

        yield return new WaitForSeconds(1f);

        CutScenePanelInput cutsceneDialogue;
        cutsceneDialogue = cutscenePanelList.Find(x => x.index == panelCounter);
        if (cutsceneDialogue != null)
        {
            for (int i = 0; i < cutsceneDialogue.cutSceneGO.Count ; i++)
            {
                cutsceneDialogue.cutSceneGO[i].SetActive(true);
                yield return new WaitUntil(() => this.sceneDialogueDisplays[panelCounter][i].endConvo);
                cutsceneDialogue.cutSceneGO[i].SetActive(false);    
            }
        }

        CutScenePanelInput postCutsceneDialogue = postCutscenePanelList.Find(x => x.index == panelCounter);
        if (postCutsceneDialogue != null)
        {
            for (int i = 0; i < postCutsceneDialogue.cutSceneGO.Count; i++)
            {
                postCutsceneDialogue.cutSceneGO[i].SetActive(true);
                yield return new WaitUntil(() => this.postSceneDialogueDisplays[panelCounter][i].endConvo); 
                postCutsceneDialogue.cutSceneGO[i].SetActive(false);
            }
        }
        panelCounter++;
        
        if (panelCounter == Math.Max(sceneDialogueDisplays.Count, postSceneDialogueDisplays.Count))
        {
            OnCrystalCaptureCutSceneDone(true);
        }
        else
        {
            OnCrystalCaptureCutSceneDone(false);
        }
    }

    private void OnDestroy()
    {
        OnCrystalCaptureCutSceneDone = null;
        if (Crystal.OnPlayerCrystalCollected != null) Crystal.OnPlayerCrystalCollected -= PlayCrystalDialogue;
    }
}
