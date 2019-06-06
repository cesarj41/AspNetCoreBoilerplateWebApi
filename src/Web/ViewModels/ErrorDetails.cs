using System;
using System.Collections.Generic;

namespace Web.ViewModels
{
    public class ErrorDetails
    {
        public IEnumerable<string> errors { get; private set; }
        public string instance { get; private set; } = $"urn:company:error:{Guid.NewGuid().ToString()}";

        public ErrorDetails(params string[] errors)
        {
            this.errors = errors;
        }

        public ErrorDetails(IEnumerable<string> errors)
        {
            this.errors = errors;
        }

    }
}