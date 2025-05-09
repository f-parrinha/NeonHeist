using Core.Character.Events;
using Core.Common.Interfaces;
using Core.Guns.Enums;
using Core.Guns.Interfaces;
using Core.Utilities;
using UI.Components;
using UnityEngine;

namespace UI
{
    public class AmmoTextUI : TextDisplayerUI, IInitializable
    {
        [SerializeField] private GameObject ammoHolderObject;
        [SerializeField] private GunType ammoType;
        
        private IAmmoHolder ammoHolder;

        public bool IsInitialized { get; private set; }

        protected override void Start()
        {
            base.Start();

            Initialize();
        }

        public void Initialize()
        {
            if (IsInitialized) return;

            if (!ammoHolderObject.TryGetComponent<IAmmoHolder>(out ammoHolder))
            {
                Log.Warning(this, "Initialize", "AmmoHolderObject is not an IAmmoHolder");
                return;
            } 


            SetText(ammoHolder.GetByAmmoType(ammoType).ToString("0"));

            ammoHolder.AddOnAmmoChangeHandler((object sender, OnAmmoChangeArgs args) => 
            {
                if (args.AmmoType != ammoType) return; // Not really needed but ok

                SetText(ammoHolder.GetByAmmoType(ammoType).ToString("0"));
            });
            
            IsInitialized = true;
        }
    }
}