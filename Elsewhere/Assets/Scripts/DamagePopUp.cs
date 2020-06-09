using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Diagnostics;

public class DamagePopUp : MonoBehaviour
{

    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;

    private const float DISAPPEAR_TIMER_MAX = 1f;


    public static DamagePopUp Create(Vector3 position, int damageAmount, bool isCriticalHit = false)
    {
        Transform damagePopUpTransform = Instantiate(GameAssets.MyInstance.pfDamagePopUp, position, Quaternion.identity);
        DamagePopUp damagePopUp = damagePopUpTransform.GetComponent<DamagePopUp>();
        damagePopUp.SetUp(damageAmount, isCriticalHit);
        return damagePopUp;
    }
    
    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }
    public void SetUp(int damageAmount, bool isCriticalHit)
    {
        textMesh.SetText("- " + damageAmount.ToString() + " HP");
        if (!isCriticalHit)
        {
            textMesh.fontSize = 3;
            textColor = new Color(0.80392f, 0.36078f, 0.36078f);
        }
        else
        {
            textMesh.fontSize = 6;
            textColor = Color.red;
        }
        textMesh.color = textColor;
        disappearTimer = DISAPPEAR_TIMER_MAX;

        textMesh.sortingOrder = 4;

        moveVector = new Vector3(.7f, .5f) * 10f;
    }

    private void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 8f * Time.deltaTime;

        float increaseScaleAmount = 0.05f;
        float decreaseScaleAmount = 0.2f;
        if (disappearTimer > DISAPPEAR_TIMER_MAX *.5f)
        {
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        }
        else
        {
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
        }


        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            float disappearSpeed = 5f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }

}
