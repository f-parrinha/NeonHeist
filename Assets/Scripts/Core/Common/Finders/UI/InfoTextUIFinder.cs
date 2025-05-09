using Core.Common.Interfaces.Info;
using UnityEngine;

namespace Core.Common.Finders.UI
{
    public class InfoTextUIFinder
    {
        public const string TAG = "InfoTextUI";

        private IInfoDisplayable infoText;

        public IInfoDisplayable Find()
        { 
            if (infoText != null) return infoText;

            return infoText = GameObject.FindGameObjectWithTag(TAG).GetComponent<IInfoDisplayable>();
        }
    }
}