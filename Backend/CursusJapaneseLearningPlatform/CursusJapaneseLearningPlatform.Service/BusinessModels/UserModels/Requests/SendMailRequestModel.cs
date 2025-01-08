using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.BusinessModels.UserModels.Requests
{
    public class SendMailRequestModel
    {
        public string AccessToken { get; set; }
        public string EmailAddress { get; set; }
        public string FullName { get; set; }
    }
}
