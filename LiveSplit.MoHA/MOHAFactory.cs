﻿using System.Reflection;
using LiveSplit.MoHA;
using LiveSplit.UI.Components;
using System;
using LiveSplit.Model;

[assembly: ComponentFactory(typeof(MoHAFactory))]

namespace LiveSplit.MoHA
{
	public class MoHAFactory : IComponentFactory
	{
		public string ComponentName => "MoHA";
		public string Description => "Automates splitting and load removal for Medal of Honor: Airborne.";
		public ComponentCategory Category => ComponentCategory.Control;

		public IComponent Create(LiveSplitState state)
		{
			return new MoHAComponent(state);
		}

		public string UpdateName => this.ComponentName;
		public string UpdateURL => "https://raw.githubusercontent.com/7imekeeper/LiveSplit.MoHA/master/";
		public Version Version => Assembly.GetExecutingAssembly().GetName().Version;
		public string XMLURL => this.UpdateURL + "LiveSplit.MoHA/Components/update.LiveSplit.MoHA.xml";
	}
}
