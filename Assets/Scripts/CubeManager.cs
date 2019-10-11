using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    public GameObject cubePiecePref;

    GameObject cubeCenterPiece;
    Transform cubeTransf;

    List<GameObject> allCubePieces = new List<GameObject>();

    bool canRotate = true,
         canShuffle = true;

    List<GameObject> UpPieces
    {
        get
        {
            return allCubePieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == 0);
        }
    }
    List<GameObject> DownPieces
    {
        get
        {
            return allCubePieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == -2);
        }
    }
    List<GameObject> FrontPieces
    {
        get
        {
            return allCubePieces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == 0);
        }
    }
    List<GameObject> BackPieces
    {
        get
        {
            return allCubePieces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == -2);
        }
    }
    List<GameObject> LeftPieces
    {
        get
        {
            return allCubePieces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == 0);
        }
    }
    List<GameObject> RightPieces
    {
        get
        {
            return allCubePieces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == 2);
        }
    }
    List<GameObject> UpHorizontalPieces
    {
        get
        {
            return allCubePieces.FindAll(x => Mathf.Round(x.transform.localPosition.x) == -1);
        }
    }
    List<GameObject> UpVerticalPieces

    {
        get
        {
            return allCubePieces.FindAll(x => Mathf.Round(x.transform.localPosition.z) == 1);
        }
    }
    List<GameObject> FrontHorizontalPieces
    {
        get
        {
            return allCubePieces.FindAll(x => Mathf.Round(x.transform.localPosition.y) == -1);
        }
    }

    Vector3[] rotationVectors =
    {
        new Vector3(0, 1, 0), new Vector3(0, -1, 0),
        new Vector3(0, 0, -1), new Vector3(0, 0, 1),
        new Vector3(1, 0, 0), new Vector3(-1, 0, 0)
    };

    private void Start()
    {
        cubeTransf = transform;
        CreateCube();
    }

    private void Update()
    {
        if (canRotate)
            CheckInput();
    }

    void CreateCube()
    {
        foreach (GameObject go in allCubePieces)
            DestroyImmediate(go);

        allCubePieces.Clear();

        for (int x = 0; x < 3; x++)
            for (int y= 0; y < 3; y++)
                for (int z = 0; z < 3; z++)
                {
                    GameObject go = Instantiate(cubePiecePref, cubeTransf, false);
                    go.transform.localPosition = new Vector3(-x, -y, z);
                    go.GetComponent<CubePieceScr>().SetColor(-x, -y, z);
                    allCubePieces.Add(go);
                }
        cubeCenterPiece = allCubePieces[13];
    }

    void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
            StartCoroutine(Rotate(UpPieces, new Vector3(0, 1, 0)));
        else if (Input.GetKeyDown(KeyCode.E))
            StartCoroutine(Rotate(DownPieces, new Vector3(0, -1, 0)));
        else if (Input.GetKeyDown(KeyCode.S))
            StartCoroutine(Rotate(LeftPieces, new Vector3(0, 0, -1)));
        else if (Input.GetKeyDown(KeyCode.D))
            StartCoroutine(Rotate(RightPieces, new Vector3(0, 0, 1)));
        else if (Input.GetKeyDown(KeyCode.X))
            StartCoroutine(Rotate(FrontPieces, new Vector3(1, 0, 0)));
        else if (Input.GetKeyDown(KeyCode.C))
            StartCoroutine(Rotate(BackPieces, new Vector3(-1, 0, 0)));
        else if (Input.GetKeyDown(KeyCode.R) && canShuffle)
            StartCoroutine(Shuffle());
        else if (Input.GetKeyDown(KeyCode.T) && canShuffle)
            CreateCube();
    }

    IEnumerator Shuffle()
    {
        canShuffle = false;

        for (int moveCount = Random.Range(15, 30); moveCount >= 0; moveCount--)
        {
            int edge = Random.Range(0, 6);
            List<GameObject> edgePieces = new List<GameObject>();

            switch (edge)
            {
                case 0: edgePieces = UpPieces;
                    break;
                case 1: edgePieces = DownPieces;
                    break;
                case 2: edgePieces = LeftPieces;
                    break;
                case 3: edgePieces = RightPieces;
                    break;
                case 4: edgePieces = FrontPieces;
                    break;
                case 5: edgePieces = BackPieces;
                    break;
            }

            StartCoroutine(Rotate(edgePieces, rotationVectors[edge], 15));
            yield return new WaitForSeconds(.3f);
        }

        canShuffle = true;
    }

    IEnumerator Rotate(List<GameObject> pieces, Vector3 rotationVec, int speed = 5)
    {
        canRotate = false;
        int angle = 0;

        while (angle < 90)
        {
            foreach (GameObject go in pieces)
                go.transform.RotateAround(cubeCenterPiece.transform.position,rotationVec, speed);
            angle += speed;
            yield return null;
        }

        CheckComplete();
        canRotate = true;
    }

    public void DetectRotate(List<GameObject> pieces, List<GameObject> planes)
    {
        if (!canRotate || !canShuffle)
            return;

        if (UpVerticalPieces.Exists(x => x == pieces[0]) &&
            UpVerticalPieces.Exists(x => x == pieces[1]))
            StartCoroutine(Rotate(UpVerticalPieces, new Vector3(0, 0, 1 * DetectLeftMiddleRightSign(pieces))));

        else if (UpHorizontalPieces.Exists(x => x == pieces[0]) &&
                 UpHorizontalPieces.Exists(x => x == pieces[1]))
            StartCoroutine(Rotate(UpHorizontalPieces, new Vector3(1 * DetectFrontMiddleBackSign(pieces), 0, 0)));

        else if (FrontHorizontalPieces.Exists(x => x == pieces[0]) &&
                 FrontHorizontalPieces.Exists(x => x == pieces[1]))
            StartCoroutine(Rotate(FrontHorizontalPieces, new Vector3(0, 1 * DetectUpMiddleDownSign(pieces), 0)));

        else if (DetectSide(planes, new Vector3(1, 0, 0), new Vector3(0, 0, 1), UpPieces))
            StartCoroutine(Rotate(UpPieces, new Vector3(0, 1 * DetectUpMiddleDownSign(pieces), 0)));

        else if (DetectSide(planes, new Vector3(1, 0, 0), new Vector3(0, 0, 1), DownPieces))
            StartCoroutine(Rotate(DownPieces, new Vector3(0, 1 * DetectUpMiddleDownSign(pieces), 0)));

        else if (DetectSide(planes, new Vector3(0, 0, 1), new Vector3(0, 1, 0), FrontPieces))
            StartCoroutine(Rotate(FrontPieces, new Vector3(1 * DetectFrontMiddleBackSign(pieces), 0, 0)));

        else if (DetectSide(planes, new Vector3(0, 0, 1), new Vector3(0, 1, 0), BackPieces))
            StartCoroutine(Rotate(BackPieces, new Vector3(1 * DetectFrontMiddleBackSign(pieces), 0, 0)));

        else if (DetectSide(planes, new Vector3(1, 0, 0), new Vector3(0, 1, 0), LeftPieces))
            StartCoroutine(Rotate(LeftPieces, new Vector3(0, 0, 1 * DetectLeftMiddleRightSign(pieces))));

        else if (DetectSide(planes, new Vector3(1, 0, 0), new Vector3(0, 1, 0), RightPieces))
            StartCoroutine(Rotate(RightPieces, new Vector3(0, 0, 1 * DetectLeftMiddleRightSign(pieces))));
    }

    bool DetectSide(List<GameObject> planes, Vector3 fDirection, Vector3 sDirection, List<GameObject> side)
    {
        GameObject centerPiece = side.Find(x => x.GetComponent<CubePieceScr>().planes.FindAll(y => y.activeInHierarchy).Count == 1);

        List<RaycastHit> hit1 = new List<RaycastHit>(Physics.RaycastAll(planes[1].transform.position, fDirection)),
                         hit2 = new List<RaycastHit>(Physics.RaycastAll(planes[0].transform.position, fDirection)),
                         hit1_m = new List<RaycastHit>(Physics.RaycastAll(planes[1].transform.position, -fDirection)),
                         hit2_m = new List<RaycastHit>(Physics.RaycastAll(planes[0].transform.position, -fDirection)),

                         hit3 = new List<RaycastHit>(Physics.RaycastAll(planes[1].transform.position, sDirection)),
                         hit4 = new List<RaycastHit>(Physics.RaycastAll(planes[0].transform.position, sDirection)),
                         hit3_m = new List<RaycastHit>(Physics.RaycastAll(planes[1].transform.position, -sDirection)),
                         hit4_m = new List<RaycastHit>(Physics.RaycastAll(planes[0].transform.position, -sDirection));

        return hit1.Exists(x => x.collider.gameObject == centerPiece) ||
               hit2.Exists(x => x.collider.gameObject == centerPiece) ||
               hit1_m.Exists(x => x.collider.gameObject == centerPiece) ||
               hit2_m.Exists(x => x.collider.gameObject == centerPiece) ||

               hit3.Exists(x => x.collider.gameObject == centerPiece) ||
               hit4.Exists(x => x.collider.gameObject == centerPiece) ||
               hit3_m.Exists(x => x.collider.gameObject == centerPiece) ||
               hit4_m.Exists(x => x.collider.gameObject == centerPiece);

    }

    float DetectLeftMiddleRightSign(List<GameObject> pieces)
    {
        float sign = 0;

        if (Mathf.Round(pieces[1].transform.position.y) != Mathf.Round(pieces[0].transform.position.y))
        {
            if (Mathf.Round(pieces[0].transform.position.x) == -2)
                sign = Mathf.Round(pieces[0].transform.position.y) - Mathf.Round(pieces[1].transform.position.y);
            else
                sign = Mathf.Round(pieces[1].transform.position.y) - Mathf.Round(pieces[0].transform.position.y);
        }
        else
        {
            if (Mathf.Round(pieces[0].transform.position.y) == -2)
                sign = Mathf.Round(pieces[1].transform.position.x) - Mathf.Round(pieces[0].transform.position.x);
            else
                sign = Mathf.Round(pieces[0].transform.position.x) - Mathf.Round(pieces[1].transform.position.x);
        }

        return sign;
    }
    float DetectFrontMiddleBackSign(List<GameObject> pieces)
    {
        float sign = 0;

        if (Mathf.Round(pieces[1].transform.position.z) != Mathf.Round(pieces[0].transform.position.z))
        {
            if (Mathf.Round(pieces[0].transform.position.y) == 0)
                sign = Mathf.Round(pieces[1].transform.position.z) - Mathf.Round(pieces[0].transform.position.z);
            else
                sign = Mathf.Round(pieces[0].transform.position.z) - Mathf.Round(pieces[1].transform.position.z);
        }
        else
        {
            if (Mathf.Round(pieces[0].transform.position.z) == 0)
                sign = Mathf.Round(pieces[1].transform.position.y) - Mathf.Round(pieces[0].transform.position.y);
            else
                sign = Mathf.Round(pieces[0].transform.position.y) - Mathf.Round(pieces[1].transform.position.y);
        }

        return sign;
    }
    float DetectUpMiddleDownSign(List<GameObject> pieces)
    {
        float sign = 0;

        if (Mathf.Round(pieces[1].transform.position.z) != Mathf.Round(pieces[0].transform.position.z))
        {
            if (Mathf.Round(pieces[0].transform.position.x) == -2)
                sign = Mathf.Round(pieces[1].transform.position.z) - Mathf.Round(pieces[0].transform.position.z);
            else
                sign = Mathf.Round(pieces[0].transform.position.z) - Mathf.Round(pieces[1].transform.position.z);
        }
        else
        {
            if (Mathf.Round(pieces[0].transform.position.z) == 0)
                sign = Mathf.Round(pieces[0].transform.position.x) - Mathf.Round(pieces[1].transform.position.x);
            else
                sign = Mathf.Round(pieces[1].transform.position.x) - Mathf.Round(pieces[0].transform.position.x);
        }

        return sign;
    }

    void CheckComplete()
    {
        if (IsSideComplete(UpPieces) &&
            IsSideComplete(DownPieces) &&
            IsSideComplete(LeftPieces) &&
            IsSideComplete(RightPieces) &&
            IsSideComplete(FrontPieces) &&
            IsSideComplete(BackPieces))
            Debug.Log("COMPLETE!!!");
    }

    bool IsSideComplete(List<GameObject> pieces)
    {
        int mainPlaneIndex = pieces[4].GetComponent<CubePieceScr>().planes.FindIndex(x => x.activeInHierarchy);

        for (int i = 0; i < pieces.Count; i++)
        {
            if (!pieces[i].GetComponent<CubePieceScr>().planes[mainPlaneIndex].activeInHierarchy ||
                pieces[i].GetComponent<CubePieceScr>().planes[mainPlaneIndex].GetComponent<Renderer>().material.color !=
                pieces[i].GetComponent<CubePieceScr>().planes[mainPlaneIndex].GetComponent<Renderer>().material.color)

                return false;
        }

        return true;
    }
}
