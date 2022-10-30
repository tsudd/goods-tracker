using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Configs
{
    public class AdapterConfig
    {
        public string AdapterName { get; init; } = String.Empty;
        public string Arguments { get; init; } = String.Empty;
        public string LocalPath { get; init; } = String.Empty;
    }
}