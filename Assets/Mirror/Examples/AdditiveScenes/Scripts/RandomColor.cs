using UnityEngine;

<<<<<<< Updated upstream
<<<<<<<< Updated upstream:Assets/Mirror/Examples/AdditiveScenes/Scripts/RandomColor.cs
namespace Mirror.Examples.Additive
=======
namespace Mirror.Examples.AdditiveScenes
>>>>>>> Stashed changes
{
    public class RandomColor : NetworkBehaviour
    {
        public override void OnStartServer()
        {
            base.OnStartServer();
            color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        }

<<<<<<< Updated upstream
========
namespace Mirror.Examples.AdditiveLevels
{
    public class RandomColor : NetworkBehaviour
    {
>>>>>>>> Stashed changes:Assets/Mirror/Examples/AdditiveLevels/Scripts/RandomColor.cs
=======
>>>>>>> Stashed changes
        // Color32 packs to 4 bytes
        [SyncVar(hook = nameof(SetColor))]
        public Color32 color = Color.black;

        // Unity clones the material when GetComponent<Renderer>().material is called
        // Cache it here and destroy it in OnDestroy to prevent a memory leak
        Material cachedMaterial;

        void SetColor(Color32 _, Color32 newColor)
        {
            if (cachedMaterial == null) cachedMaterial = GetComponentInChildren<Renderer>().material;
            cachedMaterial.color = newColor;
        }

        void OnDestroy()
        {
            Destroy(cachedMaterial);
        }
<<<<<<< Updated upstream

        public override void OnStartServer()
        {
            base.OnStartServer();

            // This script is on players that are respawned repeatedly
            // so once the color has been set, don't change it.
            if (color == Color.black)
                color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        }
=======
>>>>>>> Stashed changes
    }
}
