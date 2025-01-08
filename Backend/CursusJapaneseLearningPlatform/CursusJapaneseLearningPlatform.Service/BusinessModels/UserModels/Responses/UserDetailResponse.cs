using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.BusinessModels.UserModels.Responses
{
    public class UserDetailResponse
    {
        public Guid Id { get; set; }
        public string ImagePath { get; set; }
        public string EmailAddress { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
    }
}
