using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTrigger : MonoBehaviour
{
    [Header("Bools")]
    [Tooltip("Duvar�n T�rman�labilir Olup Olmad���")]
    public bool isWallClimbeable;
    [Tooltip("Duvar�n Giri� Noktas�(Duvar T�rman�lamayansa)")]
    public bool isWallEnter;
    [Tooltip("Duvaron ��k�� Noktas�")]
    public bool isWallExit;

    [SerializeField]
    BoxCollider myCollider;

    public float cubeHeight;
    [SerializeField]
    string tagDefault;
    [SerializeField]
    Transform attachedCube;
    public void ChangeAttachedCubeTag()
    {
        attachedCube.tag = tagDefault;
    }

    public void CloseCollider()
    {
        myCollider.enabled = false;
    }
}
