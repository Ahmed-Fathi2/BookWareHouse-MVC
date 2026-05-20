using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Application.Comman.Settings
{
    public class EmailSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
    }
}
