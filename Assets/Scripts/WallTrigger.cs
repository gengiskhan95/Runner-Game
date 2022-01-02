using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTrigger : MonoBehaviour
{
    [Header("Bools")]
    [Tooltip("Duvarýn Týrmanýlabilir Olup Olmadýðý")]
    public bool isWallClimbeable;
    [Tooltip("Duvarýn Giriþ Noktasý(Duvar Týrmanýlamayansa)")]
    public bool isWallEnter;
    [Tooltip("Duvaron Çýkýþ Noktasý")]
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
