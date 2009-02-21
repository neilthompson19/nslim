﻿// Copyright © Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using fitnesse.mtee.engine;
using fitnesse.mtee.exception;
using fitnesse.mtee.model;

namespace fitnesse.mtee.operators {
    public class DefaultRuntime<T>: RuntimeOperator<T> {
        public bool TryCreate(Processor<T> processor, string memberName, Tree<T> parameters, ref TypedValue result) {
            var runtimeType = processor.ParseString<RuntimeType>(memberName);
            if (parameters.Branches.Count == 0) {
                result =  runtimeType.CreateInstance();
            }
            else {
                RuntimeMember member = runtimeType.GetConstructor(parameters.Branches.Count);
                result = member.Invoke(GetParameterList(processor, parameters, member));
            }
            return true;
        }

        public bool TryInvoke(Processor<T> processor, TypedValue instance, string memberName, Tree<T> parameters, ref TypedValue result) {
            RuntimeMember member = RuntimeType.GetInstance(instance, memberName, parameters.Branches.Count);
            result = member.Invoke(GetParameterList(processor, parameters, member));
            return true;
        }

        private static object[] GetParameterList(Processor<T> processor, Tree<T> parameters, RuntimeMember member) {
            var parameterList = new List<object>();
            int i = 0;
            foreach (Tree<T> parameter in parameters.Branches) {
                TypedValue parameterValue;
                try {
                    parameterValue = processor.Parse(member.GetParameterType(i), parameter);
                }
                catch (Exception e) {
                    throw new ParseException<T>(string.Format("Parse parameter {0} for '{1}' failed.", i+1, member.Name), parameter.Value, e);
                }
                parameterList.Add(parameterValue.Value);
                i++;
            }
            return parameterList.ToArray();
        }
    }
}