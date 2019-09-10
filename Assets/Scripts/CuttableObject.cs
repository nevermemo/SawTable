using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttableObject : MonoBehaviour
{
    public CuttableObject otherHalf;
    public Rigidbody myBody;
    public Renderer myRenderer;
    public GameObject cutRegion;

    [SerializeField]
    private bool _isPrimary;
    
    public void GetCut(float cutRatio)
    {
        gameObject.tag = "Untagged";
        myBody.constraints = RigidbodyConstraints.None;

        if (_isPrimary)
        {
            cutRegion.SetActive(false);

            otherHalf.gameObject.SetActive(true);
            otherHalf.transform.SetParent(null);
            otherHalf.myBody.velocity = myBody.velocity;
            otherHalf.myBody.angularVelocity = myBody.angularVelocity;
            otherHalf.GetComponent<CuttableObject>().GetCut(cutRatio);
            GetCutToLeft(cutRatio);
        }
        else
        {
            GetCutToRight(cutRatio);
        }
    }

    public void GetCutToLeft(float cutRatio)
    {
        transform.localPosition += Vector3.left * transform.localScale.y * (1 - cutRatio);
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * cutRatio, transform.localScale.z);
        myRenderer.material.mainTextureScale = new Vector2(1f, cutRatio);
        myRenderer.material.mainTextureOffset = new Vector2(0f, 1f - cutRatio);
        myBody.AddForce(Vector3.left, ForceMode.Impulse);
    }
    public void GetCutToRight(float cutRatio)
    {
        transform.localPosition += Vector3.right * transform.localScale.y * cutRatio;
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * (1 - cutRatio), transform.localScale.z);
        myRenderer.material.mainTextureScale = new Vector2(1f, 1f - cutRatio);
        myRenderer.material.mainTextureOffset = new Vector2(0f, 0f);
        myBody.AddForce(Vector3.right, ForceMode.Impulse);
    }

    public void Reset()
    {
        if (_isPrimary)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            transform.localScale = Vector3.one;
            myBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            myBody.velocity = Vector3.zero;
            myBody.angularVelocity = Vector3.zero;
            myRenderer.material.mainTextureScale = Vector2.one;
            myRenderer.material.mainTextureOffset = Vector2.zero;
            tag = "Wood";

            cutRegion.SetActive(true);
            cutRegion.transform.localPosition = Vector3.zero;
            cutRegion.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            cutRegion.transform.localScale = new Vector3(1.01f, 0.2f, 1.01f);

            otherHalf.transform.SetParent(this.transform);
            otherHalf.transform.localPosition = Vector3.zero;
            otherHalf.transform.localRotation = Quaternion.Euler(0f,0f,0f);
            otherHalf.transform.localScale = Vector3.one;
            otherHalf.myBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            otherHalf.myBody.velocity = Vector3.zero;
            otherHalf.myBody.angularVelocity = Vector3.zero;
            otherHalf.myRenderer.material.mainTextureScale = Vector2.one;
            otherHalf.myRenderer.material.mainTextureOffset = Vector2.zero;
            otherHalf.tag = "Wood";
            otherHalf.gameObject.SetActive(false);

            GameManager.instance.InactiveWoods.Add(this);
            this.gameObject.SetActive(false);
        }
        else
        {
            otherHalf.Reset();
        }
    }

    //For debugging only
    public void GetCut()
    {
        GetCut(0.5f);
    }

}
