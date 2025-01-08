using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.BusinessModels.UserModels.Requests
{
    public  class ResetPasswordRequestModel
    {
        public string Password { get; set; } 
        public string VerificationToken { get; set; }
    }
}
