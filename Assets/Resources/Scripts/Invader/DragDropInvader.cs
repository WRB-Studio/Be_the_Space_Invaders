using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDropInvader : MonoBehaviour
{
    public Invader invaderScrp { get; private set; }
    public FormationMenuSlotItem formationMenuSlotItem { get; private set; }
    public static bool isDragging { get; private set; }
    private float mountRadius = 0.3f;

    private Image invaderImage;
    private Shader originShader;
    private Color originImageColor;
    public Color cantDropImageColor;
    public GameObject dropSuccessEffectPrefab;
    public GameObject dropFailedEffectPrefab;




    public void initByDrag(Invader invaderScrpVal)
    {
        invaderScrp = invaderScrpVal;

        initInvaderImage();
        MyUtilities.fillImageWithSolidColor(invaderImage, cantDropImageColor);
        invaderImage.enabled = true;

        startDrag();
    }

    public void initInFormation(Invader invaderScrpVal, FormationMenuSlotItem formationMenuSlotItemVal)
    {
        invaderScrp = invaderScrpVal;
        formationMenuSlotItem = formationMenuSlotItemVal;

        initInvaderImage();
        invaderImage.enabled = true;

        setPosition(formationMenuSlotItem.transform.position);
    }

    private void initInvaderImage()
    {
        invaderImage = transform.GetChild(0).GetComponent<Image>();
        invaderImage.sprite = invaderScrp.invaderTexture.sprite;
        //invaderImage.color = invaderScrp.textureColor;
        invaderImage.SetNativeSize();

        originShader = invaderImage.material.shader;
        originImageColor = invaderImage.color;

        transform.GetChild(0).gameObject.AddComponent<PolygonCollider2D>();
        transform.GetChild(0).gameObject.AddComponent<DragDropInvaderPointerHandler>();

        //create and set a copy / instance of image material. Without it the image assets material will change all images
        Material newMaterial = Instantiate(invaderImage.material);
        invaderImage.material = newMaterial;
    }


    public void startDrag()
    {
        if (!isDragging)
            StartCoroutine(dragAndDropCoroutine());
    }

    public void stopDrag()
    {
        isDragging = false;
    }

    private IEnumerator dragAndDropCoroutine()
    {
        isDragging = true;
        yield return null;

        FormationMenue.instance.showRemoveLine(true);

        StaticAudioHandler.playSound(SoundChooser.instance.dragInvader);

        if (formationMenuSlotItem != null)
        {
            FormationHandler.instance.removeInvaderFromPlace(formationMenuSlotItem.GetComponent<FormationMenuSlotItem>().formationPosition);
            FormationMenue.instance.refreshPlaceCountText();
        }

        transform.SetSiblingIndex(transform.parent.childCount);

        while (isDragging)
        {
            yield return null;

            Vector3 mousePosUI = Input.mousePosition;
            Vector3 mousePosWorld = Input.mousePosition;
            mousePosWorld.z = 10;
            mousePosWorld = Camera.main.ScreenToWorldPoint(mousePosWorld);

            formationMenuSlotItem = isNearFormationSlot(mousePosWorld);

            if (formationMenuSlotItem != null)
            {
                setPosition(formationMenuSlotItem.transform.position);
            }
            else
            {
                formationMenuSlotItem = null;
                setPosition(mousePosUI);
            }

            if (formationMenuSlotItem != null && FormationHandler.instance.invaderFitsInFormation(invaderScrp, formationMenuSlotItem.formationPosition))
            {
                if (invaderImage.color == cantDropImageColor)
                    MyUtilities.fillImageWithShaderAndColor(invaderImage, originImageColor, originShader);
            }
            else
            {
                if (invaderImage.color == originImageColor)
                    MyUtilities.fillImageWithSolidColor(invaderImage, cantDropImageColor);
            }

            if (!Input.GetMouseButton(0))
                stopDrag();
        }

        FormationMenue.instance.showRemoveLine(false);

        if (formationMenuSlotItem != null && FormationHandler.instance.invaderFitsInFormation(invaderScrp, formationMenuSlotItem.formationPosition))
        {
            successfullyDrop();
        }
        else
        {
            failedDrop();
        }

        InvaderChooser.instance.refreshList();
    }

    private void successfullyDrop()
    {
        MyUtilities.fillImageWithShaderAndColor(invaderImage, originImageColor, originShader);
        setPosition(formationMenuSlotItem.transform.position);
        setInvaderOnFormationSlot(formationMenuSlotItem);

        MyUtilities.instantiateAndDestroy(dropSuccessEffectPrefab, Camera.main.ScreenToWorldPoint(transform.position), 3);

        StaticAudioHandler.playSound(SoundChooser.instance.successfullInvaderDrop);
    }

    private void failedDrop()
    {
        StaticAudioHandler.playSound(SoundChooser.instance.failedInvaderDrop);
        MyUtilities.instantiateAndDestroy(dropFailedEffectPrefab, Camera.main.ScreenToWorldPoint(transform.position), 3);
        destroyDragAndDropInvader();
    }

    private FormationMenuSlotItem isNearFormationSlot(Vector3 mousePosition)
    {
        List<FormationMenuSlotItem> allFormationMenuSlotItems = FormationMenue.instance.instantiatedFormationMenuSlotItems;

        float nearestObjectDistance = 0.5f;
        FormationMenuSlotItem nearestObject = null;
        for (int i = 0; i < allFormationMenuSlotItems.Count; i++)
        {
            Vector3 formationMenueSlotItemPosition = allFormationMenuSlotItems[i].transform.position;
            formationMenueSlotItemPosition.z = 10;
            formationMenueSlotItemPosition = Camera.main.ScreenToWorldPoint(formationMenueSlotItemPosition);

            float currentMouseToObjectDistance = Vector3.Distance(mousePosition, formationMenueSlotItemPosition);
            if (currentMouseToObjectDistance <= mountRadius && currentMouseToObjectDistance < nearestObjectDistance)
            {
                nearestObjectDistance = currentMouseToObjectDistance;
                nearestObject = allFormationMenuSlotItems[i];
            }
        }

        return nearestObject;
    }

    private void setPosition(Vector3 position)
    {
        transform.position = new Vector2(position.x, position.y);

        int invaderRowSlots = invaderScrp.formationSlotSize.formationSlotSize[0];
        int invaderColumnSlots = invaderScrp.formationSlotSize.formationSlotSize[1];
        transform.GetChild(0).localPosition = new Vector2((invaderColumnSlots - 1) * 50, (invaderRowSlots - 1) * -50);
    }

    private void setInvaderOnFormationSlot(FormationMenuSlotItem formationMenuSlotItemVal)
    {
        if (!FormationHandler.instance.setInvaderOnFormationSlot(invaderScrp.id, formationMenuSlotItem.GetComponent<FormationMenuSlotItem>().formationPosition))
        {
            Destroy(gameObject);
        }

        FormationMenue.instance.refreshPlaceCountText();
    }

    public void destroyDragAndDropInvader()
    {
        Destroy(gameObject);
    }
}

public class DragDropInvaderPointerHandler : EventTrigger
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        transform.parent.GetComponent<DragDropInvader>().startDrag();
    }

}