﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    /// <summary>
    /// A type of cryptographic software that, if installed, can provide cryptographic services (represented via an ICryptoManager)
    /// </summary>
    public interface ICryptographicServiceProvider
    {
        CryptoType GetProviderType();
        bool IsInstalled();
    }
}