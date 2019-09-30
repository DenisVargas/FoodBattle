using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public bool back;
    public bool test = false;

    public Vector3 starPos;
    private Vector3 mOffset;

    private float mZCoord;

    public Animator anim;
    public Rigidbody rb;
    public BoxCollider col;

    public Transform tablePosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
        anim = GetComponent<Animator>();
        starPos = transform.position;
        back = true;
    }

    public void OnMouseDown()
    {
        starPos = transform.position;
        test = false;
        back = true;
        mZCoord = Camera.main.WorldToScreenPoint(transform.position).z;
        mOffset = transform.position - GetMouseAsWorldPoint();
    }

    public void OnMouseDrag()
    {
        transform.position = GetMouseAsWorldPoint() + mOffset;
    }

    private void OnMouseUp()
    {
        if (back)
        {
            transform.position = starPos;
            anim.SetBool("Flip", !back);
        }
        else if (!back && test)
        {
            anim.SetBool("ToTable", test);
        }
    }
    private Vector3 GetMouseAsWorldPoint()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            back = false;
            if (!test)
            {
                anim.SetBool("Flip", back);
                test = true;
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            back = true;
            anim.SetBool("Flip", back);
            test = false;
        }
    }

    public void OnTable()
    {
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    public void DisableCol()
    {
        col.enabled = false;
    }
    public void EnableCol()
    {
        col.enabled = true;
    }


}