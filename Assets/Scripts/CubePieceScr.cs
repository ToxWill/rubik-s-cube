using System.Collections.Generic;
using UnityEngine;

public class CubePieceScr : MonoBehaviour
{
    public List<GameObject> planes = new List<GameObject>();

    public void SetColor(int x, int y, int z)
    {
        if (y == 0)
            planes[0].SetActive(true);
        else if (y == -2)
            planes[1].SetActive(true);

        if (z == 0)
            planes[2].SetActive(true);
        else if (z == 2)
            planes[3].SetActive(true);

        if (x == 0)
            planes[4].SetActive(true);
        else if (x == -2)
            planes[5].SetActive(true);
    }
}
