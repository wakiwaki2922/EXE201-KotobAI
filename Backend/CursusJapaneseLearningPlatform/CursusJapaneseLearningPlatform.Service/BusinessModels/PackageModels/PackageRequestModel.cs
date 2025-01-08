using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.BusinessModels.PackageModels
{
    public class PackageRequestModel
    {
        public string PlanType { get; set; }
        public string PlanName { get; set; }
        public int Period { get; set; }
        public decimal Price { get; set; }
        public bool Status { get; set; }
    }
}
