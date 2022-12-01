using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SpellBook : MonoBehaviour
{

    List<GameObject> pages = new List<GameObject>();


    public SpellBookInputData inputData;

    GameObject currentPage;

    int currentPageNumber;

    int numberOfPages;

    public int turn = 0;

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
        currentPageNumber = 0;
        pages[currentPageNumber].gameObject.SetActive(true);
    }

    private void Update()
    {
        if (pages.Count != 0)
        {
            inputData.PageTurn = Input.GetAxis("PageTurn");

            if (inputData.PageTurn == 1)
            {
                turn += 1;

                if (turn == 15)
                {
                    Debug.Log("Page turn right");
                    TurnPage("Right");
                    turn = 0;
                }
            }
            else if (inputData.PageTurn == -1)
            {
                turn += 1;

                if (turn == 15)
                {
                    Debug.Log("Page turn left");
                    TurnPage("Left");
                    turn = 0;
                }
            }
        }
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
