using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLayout : MonoBehaviour
{
    private const int Columns = 4;
    private const float Space = 2.5f;
    private Transform[] children;

    private void Start()
    {
        children = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
        }
        ArrangeChildren(children);
    }

    private void ArrangeChildren(Transform[] children)
    {
        for (int i = 0; i < children.Length; ++i)
        {
            int row = i / Columns;
            int column = i % Columns;
            children[i].position = new Vector2(column * Space, row * Space);
        }
    }

    private void OnTransformChildrenChanged()
    {
        Transform[] children = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
        }
        ArrangeChildren(children);
    }
}
