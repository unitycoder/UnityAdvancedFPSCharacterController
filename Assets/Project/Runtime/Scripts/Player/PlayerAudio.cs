using UnityEngine;

[RequireComponent(typeof(AudioSource))]
/// <summary>
/// Simple yet good way to handle player footsteps, jumping, and other sound effects.
/// </summary>
public class PlayerAudio : MonoBehaviour {
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] public AudioSource source;

    /**
    Material-based footstep sound effects. They're arrays because you're supposed to have more variants for each material
    to have that natural randomness.
    */
    [field : SerializeField] public AudioWrapper[] footsteps;
    [SerializeField] public AudioClip jumpSound;
    [SerializeField] public float footstepRate; // How frequent the footsteps are produced
    [SerializeField] public bool steppingOnWood, steppingOnStone, steppingOnGrass, steppingOnMetal;
    private void Update()
    {
        Footsteps();
    }

    float Distance;
    RaycastHit footHit;
    public int currentMaterial;
    private void Footsteps()
	{
        // Don't make footsteps when crouching, standard for multiplayer and stealth games.
		if (playerManager.playerMovement.moveData.isCrouching)
		{
			return;
		}
		if (playerManager.playerMovement.moveData.coyoteGrounded()) // Do footsteps only when grounded.
		{
			float speed = new Vector3(playerManager.playerMovement.moveData.velocity.x, 0, playerManager.playerMovement.moveData.velocity.z).magnitude;
			if (speed > 20f)
			{
				speed = 20f;
			}
            if (playerManager.playerMovement.moveData.isMoving) // Reset when not moving, good for multiplayer and tactical shooters.
            {
			    Distance += speed * footstepRate * Time.deltaTime * 50f;
            } else
            {
                Distance = 0;
            }
			if (Distance > 100f / 1f)
			{
                AudioClip[] currentFootsteps;
                // Makes a raycast at the bottom of the controller to check for the tag of the gameobject it is stepping on
                if(Physics.Raycast(transform.position, Vector3.down, out footHit, (playerManager.controller.height / 2) +
                (playerManager.playerMovement.moveData.groundDistance), playerManager.playerMovement.moveData.groundMask))
                {
                    // MAKE SURE THE STRING INDEXES IN THE MATERIALTAGS ARRAY MATCH THE INDEXES OF AUDIOCLIP ARRAYS IN THE FOOTSTEPS ARRAY
                    // EG - MaterialTag[0] = Stone, the first array of AudioClips in the Footsteps array should have stone footstep sounds.
                    for (int i = 0; i < MaterialManager.Instance.materialTags.Length; i++)
                    {
                        string tag = footHit.transform.gameObject.tag;
                        currentMaterial = System.Array.IndexOf(MaterialManager.Instance.materialTags, tag); // Array.IndexOf lets us find the specific index of a string in the array
                        // Checks if currentMaterial >= 0 because it becomes -1 which causes it to go out of bounds from the array.
                        currentFootsteps = currentMaterial >= 0 ? footsteps[currentMaterial].audioClip : footsteps[i + 1].audioClip;
                        source.clip = currentFootsteps[Random.Range(0, currentFootsteps.Length)];
                    }
                }

                source.pitch = Random.Range(0.8f, 1); // Randomize pitch to make it more realistic.
                source.PlayOneShot(source.clip); // PlayOneShot not Play otherwise it'll interrupt the current sound that is playing.
                
				Distance = 0f; // Reset distance so it calculates again.
			}
		}
	}
}

[System.Serializable]
public struct AudioWrapper
{
    public AudioClip[] audioClip;
}