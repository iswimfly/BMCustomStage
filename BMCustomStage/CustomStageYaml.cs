using System;
using Flash2;
using YamlDotNet.Serialization;


namespace BMCustomStage
{

	public class CustomStageYaml
	{

		[YamlMember(Alias = "author_id", Description = "The numerical ID of the stage's author, from 1 to 32767.\nThis ID should be unique to this author and not be used by other authors. The author of this stage should use the same author ID for all of their stages.\nIf you are an author, please use the spreadsheet on Google Sheets to reserve an ID for yourself so that we can keep track of who is using which ID to avoid conflicts.")]
		public ushort AuthorId { get; set; }

		[YamlMember(Alias = "author_name", Description = "\nThe name of the stage's author.")]
		public string AuthorName { get; set; }

		[YamlMember(Alias = "stage_id", Description = "\nThe numerical ID of the stage, from 0 to 65535.\nThis ID should be unique amongst all the stages that this author has made and should not be used already by another one of their stages.\nHowever, stages created by different authors can have the same stage ID, since the author ID will be different.")]
		public ushort AuthorStageId { get; set; }

		[YamlMember(Alias = "stage_name", Description = "\nThe name of the stage.\nThis does not have to be unique, so if there is already another stage with the same name it doesn't matter.")]
		public string StageName { get; set; }

		[YamlMember(Alias = "background", Description = "\nThe stage background to use.\nAllowed values: Smb1_Jungle, Smb1_Ice, Smb1_Night, Smb1_Sands, Smb1_Storm, Smb1_Sunset, Smb1_Water, Smb1_Space, Smb1_Masters, Smb1_Bonus, Smb2_Jungle, Smb2_Lava, Smb2_Water, Smb2_Whale, Smb2_Park, Smb2_Pot, Smb2_Bubble, Smb2_Gear, Smb2_Space, Smb2_Electric, Smb2_Bonus")]
		public string Background { get; set; }
        
		[YamlMember(Alias = "custom_background", Description = "\nIs this a custom background? If it is, set to true.")]
		public bool CustomBackground { get; set; }

        [YamlMember(Alias = "custom_background_name", Description = "\nIs this a custom background? If it is, set to true.")]
        public string CustomBackgroundName { get; set; }
		
		[YamlMember(Alias = "time_limit_normal", Description = "\nThe time limit for the normal modes (story, challenge, and practice modes), in seconds.")]
		public int TimeLimitNormal { get; set; }

		[YamlMember(Alias = "time_limit_dark_banana", Description = "\nThe time limit for dark banana mode, in seconds.")]
		public int TimeLimitDarkBanana { get; set; }

		[YamlMember(Alias = "time_limit_golden_banana", Description = "\nThe time limit for golden banana mode, in seconds.")]
		public int TimeLimitGoldenBanana { get; set; }

		

        [YamlIgnore]
		public int StageId
		{
			get
			{
				return ((int)this.AuthorId << 16) + (int)this.AuthorStageId;
			}
		}

		[YamlIgnore]
		public string AssetBundleName
		{
			get
			{
				return string.Format("custom_{0}_{1}_stage", this.AuthorId, this.AuthorStageId);
			}
		}

		[YamlIgnore]
		public string AssetBundleFullPath { get; set; }

		[YamlIgnore]
		public string PackPath { get; set; }
        
    }
}
