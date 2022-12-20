using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerAudio))]
[RequireComponent(typeof(PlayerMovement))]
/*
The "parent" class of the essential scripts needed to make the whole thing work,
makes referencing other components more flexible.
*/
public class PlayerManager : MonoBehaviour
{
    [Header("References")]
    public GameObject camPrefab; // Can be found in Assets\Project\Runtime\Prefabs\
    // The following components are set to public to make them easier to access each other just by referencing this script.
    public PlayerAudio playerAudio; // Makes the PlayerAudio easier to access. Remove this if you don't have any audio to use or just don't want to use it, but it works really well.
    public CameraManager cameraManager; // Makes the CameraManager component easier to access.
    public CameraController cameraController; // Makes the CameraController component easier to access.
    public PlayerMovement playerMovement; // Makes the PlayerMovement component easier to access.
    public CharacterController controller; // Makes the CharacterController component easier to access.
    public Transform orientation; // Used by PlayerMovement component. Makes the Orientation transform easier to access.
    public Transform head; // Used by PlayerMovement and CameraManager components. Makes the Head transform easier to access.

    public void Start() // Initialize Player Camera if there is none, no need to put it yourself.
    {
        if(cameraManager == null && cameraController == null)
        {
            cameraManager = Instantiate(camPrefab, Vector3.zero, Quaternion.identity).GetComponent<CameraManager>();
            cameraManager.playerManager = this;
            cameraController = cameraManager.cameraController;
        }
    }
}
