using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public IEnumerator Shake(float duration, float range)
    {
        Vector3 original = transform.localPosition;
        Vector3 shaken = original;

        float timePassed = 0f;

        int i = 1;
        while (timePassed < duration)
        {

            if(i % 2 == 0)
            {
                transform.localPosition = original;
            }
            else
            {
                shaken = original + Random.onUnitSphere * range;
                transform.localPosition = shaken;
            }

            i++;
            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.localPosition = original;

    }
}
