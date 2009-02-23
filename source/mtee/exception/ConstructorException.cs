﻿// Copyright © Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;

namespace fitnesse.mtee.exception {
    public class ConstructorException: ApplicationException {
        public Type Type { get; private set; }
        public int ParameterCount { get; private set; }

        public ConstructorException(Type type, int parameterCount)
            : base(string.Format("Constructor with {1} parameter(s) not found for type '{0}'.", type.FullName, parameterCount)) {
            Type = type;
            ParameterCount = parameterCount;
        }
    }
}
