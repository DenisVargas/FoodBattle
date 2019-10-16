using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraShaker : MonoBehaviour
{
    //Temblor de la camara
     public IEnumerator Shake(float dur, float magn)
    {
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0.0f;
        while (elapsed < dur)
        {
            float x = Random.Range(-1f, 1f) * magn;
            float y = Random.Range(-1f, 1f) * magn;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }
        transform.localPosition = originalPos;
    }
}
