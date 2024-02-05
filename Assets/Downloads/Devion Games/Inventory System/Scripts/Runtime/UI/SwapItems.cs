using UnityEngine;
using UnityEngine.InputSystem;

namespace DevionGames.InventorySystem
{
    public class SwapItems : MonoBehaviour
    {
        [SerializeField] private ItemSlot first;
        [SerializeField] private ItemSlot second;
        [SerializeField] private InputActionReference SwapReference;

        private void Start()
        {
            SwapReference.action.started +=
                _ => { first.Container.SwapItems(first, second); };
        }
    }
}