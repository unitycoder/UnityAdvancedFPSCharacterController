using UnityEngine;

public class MaterialManager : MonoBehaviour
{
    public static MaterialManager Instance;

    // Manages the tags that defines the object's material so that footsteps and raycast shoot effects/particles will vary
    [Header("Scene Object Tags")]
    [field : SerializeField] public string[] materialTags;

    private void Awake()
    {
        Instance = this;
    }
}