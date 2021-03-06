﻿// Copyright © Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using fitnesse.mtee.engine;
using fitnesse.mtee.model;

namespace fitnesse.slim.operators {
    public class ExecuteCallAndAssign: ExecuteBase {
        public ExecuteCallAndAssign(): base("callAndAssign") {}

        protected override Tree<string> ExecuteOperation(Processor<string> processor, Tree<string> parameters) {
            TypedValue result = InvokeMember(processor, parameters, 3);
            processor.Store(new Symbol(parameters.Branches[2].Value, result.Value)); //todo: should we store the composed result?
            return Result(parameters, processor.Compose(result));
        }
    }
}
