using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManagerStandalone : MonoBehaviour
{
    [SerializeField] private GameObject scenePanelsParent;
    [SerializeField] private ChangeScene changeSceneManager;
    [SerializeField] private GameObject flashbackScenePanelParent;
    private GameObject[] _flashbackScenePanels;
    private GameObject[] _scenePanels;
    private int _numScenes;

    private void Start()
    {
        changeSceneManager.gameObject.SetActive(true);
        _numScenes = scenePanelsParent.transform.childCount;
        _scenePanels = new GameObject[_numScenes];
        for (int i = 0; i < _numScenes; i++)
        {
            _scenePanels[i] = scenePanelsParent.transform.GetChild(i).gameObject;
            _scenePanels[i].SetActive(false);
        }

        if (flashbackScenePanelParent != null)
        {
            _flashbackScenePanels = new GameObject[flashbackScenePanelParent.transform.childCount];
            for (int j = 0; j < flashbackScenePanelParent.transform.childCount; j++)
            {
                _flashbackScenePanels[j] = flashbackScenePanelParent.transform.GetChild(j).gameObject;
                _flashbackScenePanels[j].SetActive(false);
            }
        }


        StartCoroutine(PlayScenes());
    }

    private IEnumerator PlayScenes()
    {
        if (_flashbackScenePanels != null)
        {
            yield return Flashbacks();
        }
        
        DialogueDisplay currentDisplay;

        for (int i = 0; i < _numScenes; i++)
        {
            _scenePanels[i].SetActive(true);
            currentDisplay = _scenePanels[i].GetComponent<DialogueDisplay>();
            yield return new WaitUntil(() => currentDisplay.endConvo);
            _scenePanels[i].SetActive(false);
            if (i+1 != _numScenes) yield return changeSceneManager.CrossFade();
        }
        changeSceneManager.OnlickChangeSceneButton("NavigationMap");
    }

    private IEnumerator Flashbacks()
    {
        for (int i = 0; i < _flashbackScenePanels.Length; i++)
        {
            _flashbackScenePanels[i].SetActive(true);
            yield return new WaitForSeconds(2.5f);
            _flashbackScenePanels[i].SetActive(false);
        }

        yield return null;
    }
}
