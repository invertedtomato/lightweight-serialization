﻿using System;
using System.Collections.Generic;
using System.Text;

namespace InvertedTomato.LightWeightSerialization {
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class LightWeightPropertyAttribute : Attribute {
        // See the attribute guidelines at   http://go.microsoft.com/fwlink/?LinkId=85236
        
        public LightWeightPropertyAttribute(byte index) {
            Index = index;
        }

        public byte Index { get; private set; }
    }
}
