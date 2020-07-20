using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UIElements;

public class DamagePopUp : MonoBehaviour
{
    [SerializeField] TMP_FontAsset damageFont;
    [SerializeField] TMP_FontAsset levelUpFont;
    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;

    private const float DISAPPEAR_TIMER_MAX = 2f;


    public static DamagePopUp Create(Vector3 position, string popupText, PopupType popupType, bool isCriticalHit = false)
    {
        Transform damagePopUpTransform = Instantiate(GameAssets.MyInstance.pfDamagePopUp, position, Quaternion.identity);
        DamagePopUp damagePopUp = damagePopUpTransform.GetComponent<DamagePopUp>();
        damagePopUp.SetUp(popupText, popupType, isCriticalHit);
        return damagePopUp;
    }
    
    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }
    public void SetUp(string popupText, PopupType popupType, bool isCriticalHit)
    {
        textMesh.SetText(popupText);
        if (isCriticalHit)
        {
            textMesh.font = damageFont;
            textMesh.fontSize = 6;
            textColor = Color.red;
        }
        else if (popupType == PopupType.BUFF || popupType == PopupType.HEAL)
        {
            textMesh.font = damageFont;
            textMesh.fontSize = 3;
            //textColor = new Color(0.961f, 0.808f, 0.039f);
            textColor = Color.white;
        }
        else if (popupType == PopupType.DAMAGE || popupType == PopupType.DEBUFF)
        {
            textMesh.font = damageFont;
            textMesh.fontSize = 3;
            //textColor = new Color(0.80392f, 0.36078f, 0.36078f);
            textColor = Color.red;
        }
        else if (popupType == PopupType.LEVEL_UP)
        {
            textMesh.font = levelUpFont;
            textMesh.fontSize = 5;
            textColor = new Color(0, 0.19921f, 0.19921f);
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

public enum PopupType
{
    DAMAGE,
    BUFF,
    DEBUFF,
    HEAL,
    LEVEL_UP
}