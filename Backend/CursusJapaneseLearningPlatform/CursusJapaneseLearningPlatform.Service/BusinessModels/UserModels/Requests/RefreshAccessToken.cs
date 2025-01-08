using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.BusinessModels.UserModels.Requests
{
    public class RefreshAccessToken
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
