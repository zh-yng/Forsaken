using UnityEngine;

public class DialogueActivator : MonoBehaviour, IInteractable
{
    public Material outlineMaterial;
    public Color outlineColor = Color.white;
    public float outlineThickness = .2f;
    private SpriteRenderer outlineRenderer;
    [SerializeField] private DialogueObject dialogueObject;

    private bool isDialogueActive;

    private void Start()
    {
        // create outline renderer not on object but as a child
        GameObject outlineObject = new GameObject("Outline");
        outlineObject.transform.SetParent(transform);
        outlineObject.transform.localPosition = Vector3.zero;
        outlineObject.transform.localScale = Vector3.one * (1 + outlineThickness);
        outlineRenderer = outlineObject.AddComponent<SpriteRenderer>();
        outlineRenderer.sprite = GetComponent<SpriteRenderer>().sprite;
        outlineRenderer.material = outlineMaterial;
        outlineRenderer.color = outlineColor;
        outlineRenderer.sortingLayerID = GetComponent<SpriteRenderer>().sortingLayerID;
        outlineRenderer.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 1; //
        outlineRenderer.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerStateMachine player))
        {
            player.Interactable = this;
            outlineRenderer.enabled = true;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerStateMachine player))
        {
            if (player.Interactable is DialogueActivator dialogueActivator && dialogueActivator == this)
            {
                player.Interactable = null;
                outlineRenderer.enabled = false;
            }
        }
    }

    public bool CanInteract()
    {
        return !isDialogueActive;
    }
    public void Interact(PlayerStateMachine player)
    {
        player.DialogueUI.ShowDialogue(dialogueObject);
        isDialogueActive = true;

        // Optionally, you can disable the outline while the dialogue is active
        outlineRenderer.enabled = false;
    }
}
