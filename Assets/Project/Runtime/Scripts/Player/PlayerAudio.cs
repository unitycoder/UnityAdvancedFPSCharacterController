using UnityEngine;

[RequireComponent(typeof(AudioSource))]
/*
Simple yet good way to handle player footsteps, jumping, and other sound effects.
*/
public class PlayerAudio : MonoBehaviour {
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] public AudioSource source;

    /*
    Material-based footstep sound effects. They're arrays because you're supposed to have more variants for each material
    to have that natural randomness.
    */
    [SerializeField] public AudioClip[] footsteps;
    [SerializeField] public AudioClip[] footstepsWood;
    [SerializeField] public AudioClip[] footstepsStone;
    [SerializeField] public AudioClip[] footstepsGrass;
    [SerializeField] public AudioClip[] footstepsMetal;
    [SerializeField] public AudioClip jumpSound;
    [SerializeField] public float footstepRate; // How frequent the footsteps are produced
    [SerializeField] public bool steppingOnWood, steppingOnStone, steppingOnGrass, steppingOnMetal;

    private void Update() {
        Footsteps();
    }

    float Distance;
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
            if(playerManager.playerMovement.moveData.isMoving) // Reset when not moving, good for multiplayer and tactical shooters.
            {
			    Distance += speed * footstepRate * Time.deltaTime * 50f;
            } else
            {
                Distance = 0;
            }
			if (Distance > 100f / 1f)
			{
                RaycastHit footHit;

                var material = (steppingOnWood, steppingOnGrass, steppingOnStone, steppingOnMetal);

                // Makes a raycast at the bottom of the controller to check for the tag of the gameobject it is stepping on
                if(Physics.Raycast(transform.position, Vector3.down, out footHit, (playerManager.controller.height / 2) + (playerManager.playerMovement.moveData.groundDistance), playerManager.playerMovement.moveData.groundMask))
                {
                    material = (footHit.transform.gameObject.tag == "Material/Wood", footHit.transform.gameObject.tag == "Material/Grass",
                    footHit.transform.gameObject.tag == "Material/Stone", footHit.transform.gameObject.tag == "Material/Metal");
                } else
                {
                    material = (false, false, false, false);
                }

                // for each case you add it is important to keep it in order according to the material variable.
                switch(material)
                {
                    // Play wood footsteps when stepping on wood.
                    case (true, false, false, false):
                        source.clip = footstepsWood[Random.Range(0, footstepsWood.Length)]; 
                    break;
                    // Play grass footsteps when stepping on grass.
                    case (false, true, false, false):
                        source.clip = footstepsGrass[Random.Range(0, footstepsGrass.Length)];
                    break;
                    // Play stone footsteps when stepping on stone.
                    case (false, false, true, false):
                        source.clip = footstepsStone[Random.Range(0, footstepsStone.Length)];
                    break;
                    // Play metal footsteps when stepping on metal.
                    case (false, false, false, true):
                        source.clip = footstepsMetal[Random.Range(0, footstepsMetal.Length)];
                    break;
                    // Play default footsteps when stepping on undefined objects.
                    default:
                        source.clip = footsteps[Random.Range(0, footsteps.Length)];
                    break;
                }

                source.pitch = Random.Range(0.8f, 1); // Randomize pitch to make it more realistic.
                source.PlayOneShot(source.clip); // PlayOneShot not Play otherwise it'll interrupt the current sound that is playing.
                
				Distance = 0f; // Reset distance so it calculates again.
			}
		}
	}
}