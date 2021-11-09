using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.MoHA
{
	public partial class MoHASettings : UserControl
	{
		public bool AutoSplitBriefings { get; set; }

		#region Husky
		public bool SplitOnLowerOfficerKilled { get; set; }
		public bool SplitOnMidOfficerKilled { get; set; }
		#endregion Husky

		public MoHASettings()
		{
			InitializeComponent();
			this.splitBriefingsCheckBox.DataBindings.Add("Checked", this, "AutoSplitBriefings", false, DataSourceUpdateMode.OnPropertyChanged);
			this.huskyLowerOfficerKilledCheckBox.DataBindings.Add("Checked", this, "SplitOnLowerOfficerKilled", false, DataSourceUpdateMode.OnPropertyChanged);
			this.huskyMidOfficerKilledCheckBox.DataBindings.Add("Checked", this, "SplitOnMidOfficerKilled", false, DataSourceUpdateMode.OnPropertyChanged);
		}

		static XmlElement ToElement<T>(XmlDocument doc, string name, T value)
		{
			XmlElement element = doc.CreateElement(name);
			element.InnerText = value.ToString();
			return element;
		}

		static bool ParseBool(XmlNode settings, string setting, bool default_ = false)
		{
			bool val;
			return Boolean.TryParse(settings[setting].InnerText, out val) ? val : default_;
		}

		public XmlNode GetSettings(XmlDocument doc)
		{
			XmlElement settingsNode = doc.CreateElement("Settings");
			settingsNode.AppendChild(ToElement(doc, "AutoSplitBriefings", this.AutoSplitBriefings));
			return settingsNode;
		}

		public void SetSettings(XmlNode settings)
		{
			this.AutoSplitBriefings = ParseBool(settings, "AutoSplitBriefings");
		}
	}
}
