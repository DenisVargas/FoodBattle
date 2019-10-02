using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public bool back;
    public bool touchScreen = false;
    public bool stopAll = false;
    public bool comingBack = false;

    public Vector3 starPos;
    private Vector3 mOffset;

    public GameObject attack;

    public int UniqueID;
    private float mZCoord;

    public Animator anim;
    public Rigidbody rb;
    public BoxCollider col;

    public Transform tablePosition;
    public Transform targetPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
        anim = GetComponent<Animator>();
        
        attack.SetActive(false);
        starPos = transform.position;
        back = true;
    }

    private void Update()
    {
        if (!back && !stopAll)
        {
            anim.SetBool("Flip", true);
        }
        else if(!back && stopAll)
        {
            anim.SetBool("ToTable", true);
        }
        else if (back)
        {
            anim.SetBool("Flip", false);
        }

        if (stopAll)
        {
            var dist = Vector3.Distance(transform.position, tablePosition.position);
            if (dist >= 0.2f)
                transform.position = Vector3.Lerp(transform.position, targetPosition.position, Time.deltaTime * 3f);
        }
        if (comingBack)
        {
            transform.position = Vector3.Lerp(transform.position, starPos, Time.deltaTime * 6f);
        }
    }

    public void OnTable()
    {
        Debug.Log("ataque");
    }

    public void OnMouseDown()
    {
        if (!stopAll)
        {
            starPos = transform.position;
            comingBack = false;
            touchScreen = false;
            back = true;
            mZCoord = Camera.main.WorldToScreenPoint(transform.position).z;
            mOffset = transform.position - GetMouseAsWorldPoint();
        }
    }

    public void OnMouseDrag()
    {
        if (!stopAll)
        {
            transform.position = GetMouseAsWorldPoint() + mOffset;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.layer == 10)
                {
                    back = false;
                    if (!touchScreen)
                    {
                        touchScreen = true;
                    }
                }
                else
                {
                    back = true;
                    touchScreen = false;
                }
            }
        }
    }

    private void OnMouseUp()
    {
        if (!stopAll)
        {
            if (back)
            {
                comingBack = true;
            }
            else if (!back && touchScreen)
            {
                stopAll = true;
                GetChildTable();
                StartCoroutine(WipAttack());
            }

        }
    }

    public void GetChildTable()
    {
        List<Transform> childs = new List<Transform>();
        for (int i = 0; i < tablePosition.childCount; i++)
        {
            childs.Add(tablePosition.GetChild(i));
        }
        foreach (var item in childs)
        {
            if (!item.GetComponent<PositionTable>().inUse)
            {
                targetPosition = item.transform;
                item.GetComponent<PositionTable>().inUse = true;
                item.GetComponent<PositionTable>().cardInUse = this;
                break;
            }
        }
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
    IEnumerator WipAttack()
    {
        yield return new WaitForSeconds(1);
        attack.SetActive(true);
        yield return new WaitForSeconds(2);
        attack.SetActive(false);
    }
}