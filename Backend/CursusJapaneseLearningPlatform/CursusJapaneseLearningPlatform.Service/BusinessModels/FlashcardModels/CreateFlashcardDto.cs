﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.BusinessModels.FlashcardModels
{
    public class CreateFlashcardDto
    {
        public Guid CollectionId { get; set; }
        public string Word { get; set; }
    }
}
