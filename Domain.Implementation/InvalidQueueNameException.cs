﻿using Mercurio.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercurio.Domain.Implementation
{
    public class InvalidQueueNameException : MercurioException
    {
        public InvalidQueueNameException(string message)
            : base(message)
        {
        }
    }
}
