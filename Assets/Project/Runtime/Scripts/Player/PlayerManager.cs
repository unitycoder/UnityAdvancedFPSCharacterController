using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerAudio))]
[RequireComponent(typeof(PlayerMovement))]
/// <summary>
/// The "parent" class of the essential scripts needed to make the whole thing work, makes referencing other components more flexible.
/// </summary>
public class PlayerManager : MonoBehaviour
{
    [Header("References")]
    /// <summary>
    /// Can be found in Assets\Project\Runtime\Prefabs\
    /// </summary>
    public GameObject camPrefab;
    /// <summary>
    /// Makes the PlayerAudio easier to access. Remove this if you don't have any audio to use or just don't want to use it, but it works really well.
    /// </summary>
    public PlayerAudio playerAudio;
    /// <summary>
    /// Makes the CameraManager component easier to access.
    /// </summary>
    public CameraManager cameraManager;
    /// <summary>
    /// Makes the CameraController component easier to access.
    /// </summary>
    public CameraController cameraController;
    /// <summary>
    /// Makes the PlayerMovement component easier to access.
    /// </summary>
    public PlayerMovement playerMovement;
    /// <summary>
    /// Makes the CharacterController component easier to access.
    /// </summary>
    public CharacterController controller;
    /// <summary>
    /// Used by PlayerMovement component. Makes the Orientation transform easier to access.
    /// </summary>
    public Transform orientation;
    /// <summary>
    /// Used by PlayerMovement and CameraManager components. Makes the Head transform easier to access.
    /// </summary>
    public Transform head;

    public IEnumerator Start() // Initialize Player Camera if there is none, no need to put it yourself.
    {
        if (cameraManager == null && cameraController == null)
        {
            cameraManager = Instantiate(camPrefab, Vector3.zero, Quaternion.identity).GetComponent<CameraManager>();
            cameraManager.playerManager = this;
            cameraController = cameraManager.cameraController;
        }
        yield return new WaitForEndOfFrame();
        UISettings.Instance.playerManager = this;
    }
}
