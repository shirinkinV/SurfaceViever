﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationUsingOpenGLAndWPF.Parsing
{
    public class ParserException : Exception
    {
        public ParserException(string message)
            : base(message)
        { }
    }
}