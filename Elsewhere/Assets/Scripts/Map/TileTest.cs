using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DefaultNamespace.Map
{
    public class TileTest : MonoBehaviour//, IPointerClickHandler
    {
        public void OnMouseDown()
        {
            Debug.Log("MOUSE DOWN");
        }

        private void Update()
        {
            if (Input.GetKey("x"))
            {
                OnMouseDown();
            }
        }

        /*public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("MOUSE DIFFERENT CLICK");
        }*/
    }
}