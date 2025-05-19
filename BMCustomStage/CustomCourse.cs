using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BMCustomStage
{
    [System.Serializable]
    public class CustomCourse
    {
        [JsonProperty("course_name")]
        public string CourseName { get; set; }
        [JsonProperty("authorID")]
        public int AuthorId { get; set; }
        [JsonProperty("category_type")]
        public string CategoryType { get; set; }
        [JsonProperty("subcategories")]
        public List<Subcategory> Subcategories { get; set; }
        [JsonProperty("thumbnail_name")]
        public string ThumbnailName { get; set; }

        [JsonIgnore]
        public string ThumbnailPath { get; set; }

        [JsonIgnore]
        public string ThumbnailAssetBundle { get; set; }
    }

    [System.Serializable]
    public class Subcategory
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("courseID")]
        public int CourseID { get; set; }
        [JsonProperty("specialMode", Required = Required.Default)]
        public string SpecialMode { get; set; } = string.Empty;
        [JsonProperty("stages")]
        public List<int> Stages { get; set; }
        [JsonProperty("thumbnail_name")]
        public string ThumbnailName { get; set; }

        [JsonIgnore]
        public int CourseEnum { get; set; }

        [JsonIgnore]
        public string ThumbnailPath { get; set; }

    }
}
