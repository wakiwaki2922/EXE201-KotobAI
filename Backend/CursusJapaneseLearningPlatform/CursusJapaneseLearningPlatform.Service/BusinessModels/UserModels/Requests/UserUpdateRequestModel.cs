﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.BusinessModels.UserModels.Requests
{
    public class UserUpdateRequestModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
    }
}

