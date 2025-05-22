using Core.Common.Interfaces;
using Core.Guns.Data;
using Core.Guns.Events;
using Core.Guns.Interfaces;
using Core.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD
{
    public class CursorUI : MonoBehaviour, IInitializable, IRefreshable
    {
        [SerializeField] private RectTransform noGunCursor;
        [SerializeField] private RectTransform gunCursor;
        [SerializeField] private GameObject gunHolderObject;

        private IGunHolder gunHolder;
        private List<Image> cursorsImages;
        private float currentZoomOpacity;

        public bool IsInitialized { get; private set; }


        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            RefreshSize();

            // Update current opacity for a fade effect
            currentZoomOpacity = FadeZoomOpacity(currentZoomOpacity, 2f);
            SetOpacity(currentZoomOpacity);
        }

        public void Initialize()
        {
            if (IsInitialized) return;

            string methodName = "Initialize";


            // Check for small errors
            if (!gunHolderObject.TryGetComponent(out gunHolder)) 
            {
                Log.Warning(this, methodName, "GunHolderObject is not IGunHolder");
                return;
            }


            // Set state
            cursorsImages = new List<Image>();
            gunHolder.AddOnGunChangeHandler(OnGunChangeHandler);

            // Setup cursor
            Refresh();
            LoadCursorsImages();
            IsInitialized = true;
        }

        /// <summary>
        /// Sets the opacity of all cursors to the given value
        /// </summary>
        /// <param name="opacity"> ranges between 0 and 1</param>
        public void SetOpacity(float opacity)
        {
            opacity = Mathf.Clamp01(opacity);

            foreach (var image in cursorsImages)
            {
                Color color = image.color;
                color.a = opacity;

                image.color = color;
            }
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
        }


        /** -------- AUX METHODS -------- */

        private float FadeZoomOpacity(float currentZoomOpacity, float fadeBoost = 1f)
        {
            if (gunHolder.Gun == null) return 1f;

            float target = gunHolder.IsZooming ? 0f : 1f;
            float speed = fadeBoost * gunHolder.Gun.GunData.ZoomSpeed;
            return MathUtils.Lerp(currentZoomOpacity, target, speed, Time.deltaTime);
        }

        private void RefreshSize()
        {
            if (gunHolder.Gun == null) return;

            float offset = gunHolder.Gun.ShotOffset;
            gunCursor.sizeDelta = new Vector2(offset, offset) * IShootable.OFFSET_RESIZER;
        }

        private void OnGunChangeHandler(object sender, OnGunChangeArgs args)
        {
            Refresh();
        }

        private void LoadCursorsImages()
        {
            if (gunCursor == null || noGunCursor == null)
            {
                Log.Warning(this, "LoadCursorImages", "No cursors were given");
                return;
            }

            // Get image from noGunCursor
            var noGunCursorImage = noGunCursor.GetComponent<Image>();
            if (noGunCursorImage == null)
            {
                Log.Warning(this, "LoadCursorImages", "NoGun cursor is not an image");
            }
            else
            {
                cursorsImages.Add(noGunCursorImage);
            }

            // Get images from gunCursor
            foreach(RectTransform child in gunCursor)
            {
                if (child == null || !child.TryGetComponent<Image>(out var image))
                {
                    Log.Warning(this, "LoadCursorImages", "GunCursor child is not an image");
                    continue;
                }

                cursorsImages.Add(image);
            }
        }
    }
}