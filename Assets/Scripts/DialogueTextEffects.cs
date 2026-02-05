using UnityEngine;
using TMPro;

public class DialogueTextEffects : MonoBehaviour
{
    public TMP_Text textComponent;
    
    public enum EffectType { None, Eva_Shake, HUE_Glitch }
    public EffectType currentEffect = EffectType.None;

    [Header("Eva Settings")]
    public float shakeAmount = 2.5f; 

    [Header("HUE Settings")]
    public float glitchAmount = 25.0f; 
    public float glitchSpeed = 15.0f;  

    void Awake()
    {
        if (textComponent == null) textComponent = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (textComponent == null) return;

        textComponent.ForceMeshUpdate(); 
        var textInfo = textComponent.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            Vector3 evaOffset = Vector3.zero;
            if (currentEffect == EffectType.Eva_Shake)
            {
                evaOffset = Random.insideUnitCircle * shakeAmount;
            }

            for (int j = 0; j < 4; j++)
            {
                var orig = verts[charInfo.vertexIndex + j];
                
                // EVA (Shiver)
                if (currentEffect == EffectType.Eva_Shake)
                {
                    verts[charInfo.vertexIndex + j] = orig + evaOffset;
                }
                
                // HUE (Digital Tearing)
                else if (currentEffect == EffectType.HUE_Glitch)
                {
                    if (Random.value < 0.01f * glitchSpeed) 
                    {
                        float tearX = Random.Range(-1f, 1f) * glitchAmount;
                        float tearY = Random.Range(-0.1f, 0.1f) * glitchAmount; // Keep Y small to stay readable
                        
                        verts[charInfo.vertexIndex + j] = orig + new Vector3(tearX, tearY, 0);
                    }
                }
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textComponent.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}