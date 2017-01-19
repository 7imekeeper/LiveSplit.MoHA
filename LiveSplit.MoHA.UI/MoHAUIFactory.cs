using LiveSplit.Model;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.MoHA.UI
{
	public class MoHAUIFactory : IComponentFactory
	{
		public string ComponentName => "MoHA Death Counter";
		public string Description => "MoHA death counter.";
		public ComponentCategory Category => ComponentCategory.Information;

		public IComponent Create(LiveSplitState state)
		{
			return new MoHAUIComponent(state);
		}

		public string UpdateName => this.ComponentName;
		public string UpdateURL => "https://raw.githubusercontent.com/7imekeeper/LiveSplit.MoHA/master/";
		public Version Version => Assembly.GetExecutingAssembly().GetName().Version;
		public string XMLURL => this.UpdateURL + "LiveSplit.MoHA.UI/Components/update.LiveSplit.MoHA.UI.xml";
	}
}
