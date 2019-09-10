using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutter : MonoBehaviour
{
    [SerializeField] private ParticleSystem dustThrower;
    [SerializeField] private CameraShake shaker;

    //Debugging Only
    [SerializeField] private bool _invulnerable = false;

    private Vector3 cachedPos = Vector3.zero;
    
    void Start()
    {
        Input.simulateMouseWithTouches = true;
        cachedPos = transform.position;
    }

    private void Update()
    {
        if (GameManager.instance.IsPaused() || !Input.GetMouseButton(0))
            return;
        cachedPos.x = GameManager.instance.GetTableWidth() * ( Mathf.Max(0f, Mathf.Min(1f, (Input.mousePosition.x / Screen.width) ) ) - 0.5f);
        transform.position = cachedPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.instance.IsGameOver())
            return;

        if (other.tag.Equals("Wood"))
        {
            CuttableObject wood = other.GetComponent<CuttableObject>();
            float cutRatio = 0.5f - (other.transform.position.x - transform.position.x) / (2 * other.transform.localScale.y);
            float cutRegionPosRatio = 0.5f - wood.cutRegion.transform.localPosition.y / (2 * wood.transform.localScale.y);
            if(Mathf.Abs(cutRatio - cutRegionPosRatio) > wood.cutRegion.transform.localScale.y / 2)
            {
                if(!_invulnerable)
                    GameManager.instance.GameOver();
            }
            else
            {
                dustThrower.Play();
                StartCoroutine(shaker.Shake(0.3f, 0.1f));
                GameManager.instance.IncrementScore();
                other.GetComponent<CuttableObject>().GetCut(cutRatio);
            }
        }
    }
}
