using JetBrains.Annotations;
using Mono.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UnitSelection : MonoBehaviour
{
    public GameObject[] unitList;
    // public static List<GameObject> selectedUnitList;
    public static List<SelectableUnitTest> selectedUnitListTest;
    // public GameObject unitCellPrefab;
    private int index = 0;
    public int limit = 0;
    public string nextScene;
    public Text limitText;
    public Text noOfUnits;


    private void Awake()
    {
        limitText.text = "             /" + limit.ToString() + " units";
        unitList = new GameObject[transform.childCount];
        selectedUnitListTest = new List<SelectableUnitTest>();

        for (int i = 0; i < transform.childCount; i++)
        {
            unitList[i] = transform.GetChild(i).gameObject;
        }
/*
        foreach (GameObject go in unitList)
        {
            go.SetActive(false);
        }*/

        // Toggle on the first index
        /*if (unitList[index])
        {
            unitList[index].SetActive(true);
        }*/
    }
    private void Update()
    {
        noOfUnits.text = selectedUnitListTest.Count.ToString();
    }

    public void OnBackButton()
    {
        // Toggle off the current unit
        unitList[index].SetActive(false);

        index--;
        if (index < 0)
        {
            index = unitList.Length - 1;
        }

        // Toggle on the new unit
        unitList[index].SetActive(true);
    }

    public void OnNextButton()
    {
        unitList[index].SetActive(false);

        index++;
        if (index == unitList.Length)
        {
            index = 0;
        }

        // Toggle on the new unit
        unitList[index].SetActive(true);
    }

    public void OnSelectButton()
    {
        // SelectableUnitTest currUnit = unitList[index].GetComponent<SelectableUnitTest>();
        /*if (currUnit.selected)
        {
            if (selectedUnitListTest.Count < limit)
            {
                selectedUnitListTest.Add(currUnit);
            }
            // currUnit.selected = true;
            else if (selectedUnitListTest.Count == limit)
            {
                Debug.Log("Limit reached");
            }
        }
        else
        {
            Debug.Log("Please select unit first");
        }
        Debug.Log("No of units selected: " + selectedUnitListTest.Count);*/

        foreach (GameObject unit in unitList)
        {
            SelectableUnitTest currUnit = unit.GetComponent<SelectableUnitTest>();
            if (currUnit.selected)
            {
                if (selectedUnitListTest.Count < limit)
                {
                    selectedUnitListTest.Add(currUnit);
                }
                else if (selectedUnitListTest.Count > limit)
                {
                    Debug.Log("Do not choose more than " + limit + " unit");
                }
            }
        }
    }

    public void OnStartGameButton()
    {
        if (selectedUnitListTest.Count < limit)
        {
            Debug.LogError("Cannot start because not enough unit");
        }
        else if (selectedUnitListTest.Count == limit)
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}
