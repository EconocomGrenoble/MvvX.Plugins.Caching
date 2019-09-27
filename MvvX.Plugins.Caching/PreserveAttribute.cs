using System;

namespace MvvX.Plugins.Caching
{
    [AttributeUsage(AttributeTargets.Class)]
    class PreserveAttribute : Attribute
    {
        public PreserveAttribute() { }
        public bool AllMembers { get; set; }
        public bool Conditional { get; set; } 
    }
}
