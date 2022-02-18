using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvaderEventHandler : MonoBehaviour
{
    public Invader invaderScrp;

    public float tapSpeed = 0.2f;
    private float lastTapTime;
    int clicks = 0;
    bool clickCoroutineIsProcessing = false;

    public float dragStartDelay = 0.15f;
    bool dragging = false;
    bool checkDragIsProcessing = false;


    public void init(Invader invaderScrpVal)
    {
        invaderScrp = invaderScrpVal;

        Transform invaderSpriteTransform = invaderScrp.transform.GetChild(0);
        transform.position = invaderSpriteTransform.position;
        transform.localScale = invaderSpriteTransform.localScale;

        SpriteRenderer tmpSprite = gameObject.AddComponent<SpriteRenderer>();
        tmpSprite.sprite = invaderSpriteTransform.GetComponent<SpriteRenderer>().sprite;
        Collider touchCollider = gameObject.AddComponent<BoxCollider>();

        Destroy(tmpSprite);
    }


    private void OnMouseDown()
    {
        ClickTapHandling();
    }

    private void OnMouseUp()
    {
        if (checkDragIsProcessing)
        {
            checkDragIsProcessing = false;
            dragging = false;
        }

        invaderScrp.stopAimingDrag();
    }

    private void OnMouseDrag()
    {
        StartCoroutine(checkIsDrag());
    }

    private void OnMouseEnter()
    {
        invaderScrp.onEnterInvaderSprite();
    }

    private void OnMouseExit()
    {
        invaderScrp.onExitInvaderSprite();
    }


    private void ClickTapHandling()
    {
        lastTapTime = Time.time;

        if ((Time.time - lastTapTime) < tapSpeed)
        {
            clicks++;

            if (!clickCoroutineIsProcessing)
                StartCoroutine(clickTapHandlingCoroutine());
        }
    }

    private IEnumerator clickTapHandlingCoroutine()
    {
        clickCoroutineIsProcessing = true;

        while (!dragging && (Time.time - lastTapTime) < tapSpeed && clicks < invaderScrp.numberOfWeaponTypes() + 1)
        {
            yield return null;
        }

        if (!dragging)
        {
            switch (clicks)
            {
                case 1:
                    invaderScrp.onSingleClick();
                    break;
                case 2:
                    invaderScrp.onDoubleClick();
                    break;
                case 3:
                    invaderScrp.onTrippleClick();
                    break;
                default:
                    invaderScrp.multipleClick();
                    break;
            }
        }

        clicks = 0;
        clickCoroutineIsProcessing = false;
    }

    private IEnumerator checkIsDrag()
    {
        checkDragIsProcessing = true;
        float passedTime = dragStartDelay;

        while (checkDragIsProcessing)
        {
            yield return null;
            passedTime -= Time.deltaTime;

            //if holding longer drag delay time => start dragging
            if (Input.GetMouseButton(0) && passedTime <= 0)
            {
                dragging = true;
                invaderScrp.startAimingDrag();
                clicks = 0;
                break;
            }
            else if (passedTime <= 0)
            {
                dragging = false;
                invaderScrp.stopAimingDrag();
                break;
            }
        }

        checkDragIsProcessing = false;
    }

    /*
    private void OnMouseUp()
    {
        isHolding = false;
        invaderScrp.stopAimingDrag();
    }  

    private IEnumerator clickHandlerCoroutine()
    {
        yield return null;

        float timeCounter = 0;

        while (true)
        {
            yield return new WaitForFixedUpdate();
            timeCounter += Time.fixedDeltaTime;

            if (!isHolding && clicks == 3 && timeCounter > doubleClickMaxDelay && timeCounter <= trippleClickMaxDelay)
            {
                invaderScrp.onTrippleClick();
                clicks = 0;
                break;
            }

            if (!isHolding && clicks == 2 && timeCounter <= doubleClickMaxDelay)
            {
                invaderScrp.onDoubleClick();
                clicks = 0;
                break;
            }

            if (!isHolding && clicks == 1 && timeCounter > doubleClickMaxDelay)
            {
                invaderScrp.onSingleClick();
                clicks = 0;
                break;
            }

            if (isHolding && timeCounter > detectDragAfterSec)
            {
                invaderScrp.startAimingDrag();
                break;
            }

            if (timeCounter > doubleClickMaxDelay && timeCounter > detectDragAfterSec)
                break;
        }

        clicks = 0;
        clickHandlerIsProcessing = false;
    }
    */
}