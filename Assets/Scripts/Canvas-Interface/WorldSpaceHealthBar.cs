using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceHealthBar : MonoBehaviour
{
    public Image Fill;
    public Image BackGround;
    public float MaxValue { get; set; } = 0f;

    public Transform lookAt;
    public Vector3 Offset;

    public bool continiuslyUpdatePosition = true;

    Camera cam;
    Vector3 _wordlSpacePosition = Vector3.zero;
    RectTransform myPos;

#if UNITY_EDITOR
    [SerializeField] bool debug_Update = true; 
#endif

    void Awake()
    {
        cam = Camera.main;
        myPos = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        _wordlSpacePosition = cam.WorldToScreenPoint(lookAt.position + Offset);
        myPos.position = _wordlSpacePosition + Offset;
    }

    public void UpdateDisplay(float currentValue)
    {
        float value = currentValue / MaxValue;
        Fill.fillAmount = value;

#if UNITY_EDITOR
        if (debug_Update)
        {
            print("UpdateDisplay");
            print("Ammount = " + value + " max Value is: " + MaxValue + " \nFinal Value is: " + Fill.fillAmount);
        } 
#endif
    }
    public void SetApha(float alphaValue)
    {
        BackGround.canvasRenderer.SetAlpha(alphaValue);
        Fill.canvasRenderer.SetAlpha(alphaValue);
    }

    public void FadeIn(float duration = 1f)
    {
        Fill.CrossFadeAlpha(1f, duration, false);
        BackGround.CrossFadeAlpha(1f, duration, false);
    }

    public void FadeOut(float duration = 1f)
    {
        Fill.CrossFadeAlpha(0f, duration, false);
        BackGround.CrossFadeAlpha(0f, duration, false);
    }
}
