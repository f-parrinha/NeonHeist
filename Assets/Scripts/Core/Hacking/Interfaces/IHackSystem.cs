using Core.Common.Interfaces;
using Core.Hacking.Events;
using System;

namespace Core.Hacking.Interfaces
{
    public interface IHackSystem : IOpenable
    {
        bool CheatMode { get; set; }

        void AddUponHackHandler(EventHandler<UponHackArgs> handler);
        void RemoveUponHackHandler(EventHandler<UponHackArgs> handler);
    }
}