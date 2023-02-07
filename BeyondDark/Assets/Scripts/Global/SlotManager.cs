using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Adpated from https://www.trickyfast.com/2017/10/09/building-an-attack-slot-system-in-unity/
public class SlotManager : MonoBehaviour
{
    public GameObject[] slotPositions;  // Assign positions in the editor ( but don't have to ), if this is left empty then can use default circle positions
    public float radius = 4.0f;         // If not assigning positions in editor then define size of circle positions 

    private Color[] slotColours = new Color[] { Color.green, Color.blue, Color.red, Color.yellow, Color.white, Color.magenta };
    //one slot contains the game object occpying the slot
    private List<GameObject> slotsAroundObject;
    private int maxNumOfSlots = 8;

    private void Awake()
    {
        if (slotPositions.Length != 0)
        {
            // slot positions are assigned in editor so be sure to use those
            maxNumOfSlots = slotPositions.Length;
        }
        slotsAroundObject = new List<GameObject>();
        for (int i = 0; i < maxNumOfSlots; i++)
        {
            slotsAroundObject.Add(null);
        }
    }
    // Use this for initialization
    void Start()
    {


    }

    //If slot positions are set in editor use those otherwise,
    //get the slot position  based on maxNumOfSlots to form a circle around object.
    public Vector3 GetSlotPosition(int index)
    {
        Vector3 slotPosition = new Vector3(0.0f, 0.0f, 0.0f);

        if (slotPositions.Length != 0 && index >= 0 && index < slotPositions.Length)
        {
            // Use slot positions assigned in editor
            if (index >= 0 && index < slotPositions.Length && (slotPositions[index] != null))
            {
                slotPosition = slotPositions[index].transform.position;
            }

            return slotPosition;
        }
        else
        {
            // if no positions are assigned in the editor then use positions in form of a circle with radius defined by "distance"
            float degreesPerIndex = 360f / maxNumOfSlots;
            Vector3 currPos = transform.position;  //center position: uses game object position to which this script is attached
            Quaternion rotationAboutAxis = Quaternion.AngleAxis(degreesPerIndex * index, new Vector3(0, 1, 0));
            slotPosition = currPos + (rotationAboutAxis * transform.forward * radius);
        }



        return slotPosition;
    }


    //Get(Reserve) the nearest slot to you(attacker) that is not already occupied
    //returns index of the available slot or -1 if there are no available slots
    public int ReserveSlotAroundObject(GameObject attacker)
    {
        if (attacker == null) return -1;

        int slotIndex = -1; // - 1 indicates that there are no slots available

        //get the object's current position
        Vector3 objectCurrentPos = attacker.transform.position;

        float bestDistance = Mathf.Infinity;
        //find the closest slot position to the bestPosition (where you want to go)
        for (int index = 0; index < slotsAroundObject.Count; index++)
        {
            //only use the slots that are not occupied -- they are considered free/available
            if (slotsAroundObject[index] == null)
            {
                Vector3 potentialSlotPosition = GetSlotPosition(index);
                //check to see if slot is not in obstacle

                //get distance of the bestPosition to the current slot
                float distance = Vector3.Distance(potentialSlotPosition, objectCurrentPos);
                if (distance < bestDistance)
                {
                    //found a new distance
                    bestDistance = distance;
                    slotIndex = index;
                }               
            }
        }
        if (slotIndex != -1)
        {
            slotsAroundObject[slotIndex] = attacker;
        }
        return slotIndex;
    }

    public int FindNumberOfEmptySlots()
    {
        int numSlots = 0;
        for (int index = 0; index < slotsAroundObject.Count; index++)
        {
            if (slotsAroundObject[index] != null)
            {
                numSlots++;
            }
        }
        return numSlots;
    }

    public void ReleaseSlot(int index, GameObject go)
    {
        if (index >= 0 && index < slotsAroundObject.Count)
        {
            if (go == slotsAroundObject[index])
            {
                slotsAroundObject[index] = null;
            }
        }

    }

    public void ClearSlots(GameObject go)
    {
        for (int i = 0; i < slotsAroundObject.Count; i++)
        {
            if (slotsAroundObject[i] == go)
            {
                slotsAroundObject[i] = null;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DebugDrawSlots()
    {
        if (slotsAroundObject == null) return;

        Color c = Color.green;
        for (int index = 0; index < slotsAroundObject.Count; index++)
        {
            if (slotsAroundObject[index] == null)
            {
                c = Color.black;
            }
            else
            {
                int colourIndex = index;
                if (index >= slotColours.Length)
                {
                    colourIndex = colourIndex % slotColours.Length;
                }
                c = slotColours[colourIndex];

            }

            Gizmos.color = c;
            Vector3 slotPos = GetSlotPosition(index);
            Gizmos.DrawWireSphere(slotPos, 0.5f);
            //UsefullFunctions.DebugRay(transform.position, slotPos - transform.position, c);
            //UsefullFunctions.DebugRay(transform.position, transform.forward * 5.0f, Color.cyan);
        }
    }

    void OnDrawGizmosSelected()
    {
        DebugDrawSlots();
    }
}
