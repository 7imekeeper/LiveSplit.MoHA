using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LiveSplit.MoHA
{
	public class Level
	{
		public const string Menu = "shell";

		public const string Training = "Tra_Jmp_P";

		public const string HuskyBriefing = "Hus_M1_Briefing";
		public const string Husky = "Hus_M1_P";

		public const string AvalancheBriefing = "Ava_Pes_Briefing";
		public const string Avalanche = "Ava_Pes_P";

		public const string NeptuneBriefing = "Nep_Azv_Briefing";
		public const string Neptune = "Nep_Azv_P";

		public const string MarketGardenBriefing = "Mar_Nij_Briefing";
		public const string MarketGarden = "Mar_Nij_P";

		public const string VarsityBriefing = "Var_Fac_Briefing";
		public const string Varsity = "Var_Fac_P";

		public const string DerFlakturmBriefing = "Var_Flk_Briefing";
		public const string DerFlakturm = "Var_Flk_P";

		public static IReadOnlyDictionary<string, int> Order = new Dictionary<string, int>()
		{

			[Menu] = 0,
			[Training] = 1,
			[HuskyBriefing] = 2,
			[Husky] = 3,
			[AvalancheBriefing] = 4,
			[Avalanche] = 5,
			[NeptuneBriefing] = 6,
			[Neptune] = 7,
			[MarketGardenBriefing] = 8,
			[MarketGarden] = 9,
			[VarsityBriefing] = 10,
			[Varsity] = 11,
			[DerFlakturmBriefing] = 12,
			[DerFlakturm] = 13
		};

		protected static ReadOnlyCollection<string> Briefings = new List<string>()
		{
			HuskyBriefing,
			AvalancheBriefing,
			NeptuneBriefing,
			MarketGardenBriefing,
			VarsityBriefing,
			DerFlakturmBriefing
		}.AsReadOnly();

		protected static ReadOnlyCollection<string> Playable = new List<string>()
		{
			Training,
			Husky,
			Avalanche,
			Neptune,
			MarketGarden,
			Varsity,
			DerFlakturm
		}.AsReadOnly();

		public static bool IsBriefing(string level) =>
			Briefings.Contains(level);

		public static bool IsPlayable(string level) =>
			Playable.Contains(level);

		public static bool Advanced(string old, string current) =>
			Order[old] + 1 == Order[current];
	}
}
