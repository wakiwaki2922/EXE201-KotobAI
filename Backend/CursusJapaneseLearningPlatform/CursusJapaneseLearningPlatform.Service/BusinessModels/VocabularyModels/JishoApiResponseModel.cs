using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.BusinessModels.VocabularyModels
{
    public class JishoApiResponseModel
    {
        public List<JishoData> Data { get; set; }
    }

    public class JishoData
    {
        public string Slug { get; set; }
        public List<Sense> Senses { get; set; }
    }

    public class Sense
    {
        [JsonPropertyName("english_definitions")]
        public List<string> EnglishDefinitions { get; set; }
    }
}
