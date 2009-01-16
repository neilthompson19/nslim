﻿// Copyright © Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitnesse.mtee.engine;
using fitnesse.mtee.model;
using fitnesse.mtee.Model;

namespace fitnesse.slim.operators {
    public class ExecuteCallAndAssign: ExecuteBase {
        public ExecuteCallAndAssign(): base("callAndAssign") {}

        protected override Tree<object> ExecuteOperation(Processor processor, State state) {
            TypedValue result = InvokeMember(processor, state, 3);
            processor.Store("$" + state.ParameterString(2), result.Value); //todo: should we store the composed result?
            return Result(state, processor.Compose(result.Value, result.Type));
        }
    }
}
