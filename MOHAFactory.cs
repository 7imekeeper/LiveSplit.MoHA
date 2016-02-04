using System.Reflection;
using LiveSplit.MoHA;
using LiveSplit.UI.Components;
using System;
using LiveSplit.Model;

[assembly: ComponentFactory(typeof(MOHAFactory))]

namespace LiveSplit.MoHA
{
    public class MOHAFactory : IComponentFactory
    {
        public string ComponentName => "MOHA";
        public string Description => "Automates splitting and load removal for Medal of Honor: Airborne.";
        public ComponentCategory Category => ComponentCategory.Control;

        public IComponent Create(LiveSplitState state)
        {
            return new MOHAComponent(state);
        }

        public string UpdateName => this.ComponentName;
		public string UpdateURL => "";
        public Version Version => Assembly.GetExecutingAssembly().GetName().Version;
		public string XMLURL => this.UpdateURL;
    }
}
