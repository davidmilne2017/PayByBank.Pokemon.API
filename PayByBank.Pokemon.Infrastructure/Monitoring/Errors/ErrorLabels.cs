using PayByBank.Pokemon.Common.ErrorEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PayByBank.Pokemon.Infrastructure.Monitoring.Errors
{
    public static class ErrorLabels
    {
        public static string[] LabelNames => new[] { "category", "level" };
        public static string[] Labels(ErrorCategory category, ErrorLevel level)
        {
            return new[] { category.Label(), level.Label() };
        }

    }
}
