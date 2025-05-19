using Core.Common.Interfaces;
using Core.Guns.Data;
using Core.Guns.Events;
using Core.Guns.Interfaces;
using Core.Utilities;
using UnityEngine;

namespace UI
{
    public class CursorUI : MonoBehaviour, IInitializable, IRefreshable
    {
        private const int GUN_CURSOR_PART_COUNT = 4;

        [SerializeField] private RectTransform noGunCursor;
        [SerializeField] private RectTransform gunCursor;
        [SerializeField] private GameObject gunHolderObject;

        private IGunHolder gunHolder;

        public bool IsInitialized { get; private set; }


        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (IsInitialized) return;

            string methodName = "Initialize";


            // Check for small errors
            if (!gunHolderObject.TryGetComponent<IGunHolder>(out gunHolder)) 
            {
                Log.Warning(this, methodName, "GunHolderObject is not IGunHolder");
                return;
            }


            // Setup cursor
            Refresh();
            gunHolder.AddOnGunChangeHandler(OnGunChangeHandler);
            IsInitialized = true;
        }

        private void OnGunChangeHandler(object sender, OnGunChangeArgs args)
        {
            Refresh();
        }


        public void Refresh()
        {
            if (gunHolder.Gun == null)
            {
                noGunCursor.gameObject.SetActive(true);
                gunCursor.gameObject.SetActive(false);
                return;
            }

            noGunCursor.gameObject.SetActive(false);
            gunCursor.gameObject.SetActive(true);

            GunData data = gunHolder.Gun.GunData;
            gunCursor.sizeDelta = new Vector2(data.Offset, data.Offset) * IShootable.OFFSET_RESIZER;
        }
    }
}