﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsServiceSample
{
    public interface IPingSettings
    {
        string Target { get; set; }
        TimeSpan Frequency { get; set; }
        TimeSpan Timeout { get; set; }
    }
    public class PingSettings : IPingSettings
    {
        public string Target { get; set; }
        public TimeSpan Frequency { get; set; }
        public TimeSpan Timeout { get; set; }
    }
}
