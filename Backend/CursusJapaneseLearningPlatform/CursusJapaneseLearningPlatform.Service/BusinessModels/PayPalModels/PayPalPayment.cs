using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.BusinessModels.PayPalModels
{
    public class PayPalPayment
    {
        public string Id { get; set; }
        public List<PayPalLink> Links { get; set; }
    }

    public class PayPalLink
    {
        public string Href { get; set; }
        public string Rel { get; set; }
        public string Method { get; set; }
    }
}
