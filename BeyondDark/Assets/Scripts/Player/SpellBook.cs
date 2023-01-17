using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpellBook : MonoBehaviour
{

    List<GameObject> pages = new List<GameObject>();


    public SpellBookInputData inputData;

    GameObject currentPage;

    int currentPageNumber;

    int numberOfPages;

    public int turn = 0;

    public Transform desiredInspectPosition;
    public Transform itemSlotPosition;

    public void UpdatePages(List<GameObject> items)
    {
       pages = items;
       numberOfPages = pages.Count;
    }

    public void OpenBook()
    {
        if(pages.Count == 0)
        {
            return;
        }
        DisablePages();
        //currentPageNumber = 0;
        pages[currentPageNumber].gameObject.SetActive(true);
    }

    private void Update()
    {
        if (pages.Count != 0)
        {
            inputData.PageTurnRight = Gamepad.current.dpad.right.wasPressedThisFrame;
            inputData.PageTurnLeft = Gamepad.current.dpad.left.wasPressedThisFrame;

            if (inputData.PageTurnRight)
            {
                Debug.Log("Page turn right");
                TurnPage("Right");
                turn = 0;
                inputData.PageTurnRight = false;
            }

            if (inputData.PageTurnLeft)
            {
                Debug.Log("Page turn left");
                TurnPage("Left");
                turn = 0;
                inputData.PageTurnLeft = false;
            }
        }

        inputData.ZoomBook = Gamepad.current.rightTrigger.isPressed;

        if (inputData.ZoomBook)
            InspectBook(desiredInspectPosition);
        else if(!inputData.ZoomBook)
            InspectBook(itemSlotPosition);

    }

    public void InspectBook(Transform newPosition)
    {
        this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, newPosition.position, 2f * Time.deltaTime);
        this.gameObject.transform.rotation = Quaternion.Lerp(this.gameObject.transform.rotation, newPosition.rotation, 2f * Time.deltaTime);
    }

    public void TurnPage(string direction)
    {
        switch(direction)
        {
            case "Left":
                if(currentPageNumber == 0)
                    return;
                DisablePages();
                currentPageNumber = currentPageNumber - 1;
                pages[currentPageNumber].gameObject.SetActive(true);

                break;
            case "Right":

                if (currentPageNumber == numberOfPages - 1)
                    return;

                DisablePages();
                currentPageNumber = currentPageNumber + 1;
                pages[currentPageNumber].gameObject.SetActive(true);
                break;
        }
    }

    public void DisablePages()
    {
        for(int i = 0; i < pages.Count; i++)
        {
            pages[i].SetActive(false);
        }
    }



}
