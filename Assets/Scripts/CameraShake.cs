using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 camNeutralPos = transform.position;

        float elapsed = 0f;

        while(elapsed < duration)
        {
            float xPos = transform.position.x + Random.Range(-1f, 1f) * magnitude;
            float yPos = transform.position.y + Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(xPos, yPos, transform.position.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = camNeutralPos;
    }
}
