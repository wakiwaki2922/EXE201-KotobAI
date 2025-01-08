using CursusJapaneseLearningPlatform.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.BusinessModels.VocabularyModels
{
    public class VocabularyResponseModel
    {
        public Guid Id { get; set; }
        public string Word { get; set; }
        public string Meaning { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public bool IsActive { get; set; }

        public VocabularyResponseModel(Vocabulary vocabulary)
        {
            Id = vocabulary.Id;
            Word = vocabulary.Word;
            Meaning = vocabulary.Meaning;
            CreatedTime = vocabulary.CreatedTime;
            IsActive = vocabulary.IsActive;
        }
    }
}
