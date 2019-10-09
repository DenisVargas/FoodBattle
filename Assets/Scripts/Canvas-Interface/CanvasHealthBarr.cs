using UnityEngine;
using UnityEngine.UI;

public class CanvasHealthBarr : MonoBehaviour
{
    public Image Fill;
    public Image BackGround;
    public float MaxValue { get; set; } = 0f;

#if UNITY_EDITOR
    [SerializeField] bool debug_Messages = true;
#endif

    public void UpdateDisplay(float currentValue)
    {
        float value = currentValue / MaxValue;
        Fill.fillAmount = value;

#if UNITY_EDITOR
        if (debug_Messages)
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
