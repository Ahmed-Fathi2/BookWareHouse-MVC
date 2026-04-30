using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Application.ViewModels.Payment
{
    public class WebHookVM
    {
        public string Status { get; set; }
        public string TransactionId { get; set; }

        public string MerchantOrderId { get; set; }


    }
}
