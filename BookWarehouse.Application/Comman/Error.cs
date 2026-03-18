using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Application.Comman
{
    public record Error(string Code, string Description)
    {

        public static readonly Error None = new(string.Empty, string.Empty);
    }
}
