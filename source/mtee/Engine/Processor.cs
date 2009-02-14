﻿// Copyright © Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System;
using System.Collections.Generic;
using fitnesse.mtee.model;
using fitnesse.mtee.operators;

namespace fitnesse.mtee.engine {

    public class Processor<U>: Copyable { //todo: add setup and teardown
        //todo: this is turning into a facade so push everything else out
        private readonly List<List<Operator>> operators = new List<List<Operator>>();

        private readonly Dictionary<Type, object> memoryBanks = new Dictionary<Type, object>();

        public ApplicationUnderTest ApplicationUnderTest { get; set; }

        public Processor(ApplicationUnderTest applicationUnderTest) {
            ApplicationUnderTest = applicationUnderTest;
            AddOperator(new DefaultParse<U>());
            AddOperator(new ParseType<U>());
            AddOperator(new DefaultRuntime<U>());
        }

        public Processor(): this(new ApplicationUnderTest()) {}

        public Processor(Processor<U> other): this(new ApplicationUnderTest(other.ApplicationUnderTest)) {
            operators.Clear();
            foreach (List<Operator> list in other.operators) {
                operators.Add(new List<Operator>(list));
            }
            memoryBanks = new Dictionary<Type, object>(other.memoryBanks);
        }

        public void AddOperator(string operatorName) {
            AddOperator((Operator)Create(operatorName));
        }

        public void AddOperator(Operator anOperator) { AddOperator(anOperator, 0); }

        public void AddOperator(Operator anOperator, int priority) {
            while (operators.Count <= priority) operators.Add(new List<Operator>());
            operators[priority].Add(anOperator);
        }

        public void RemoveOperator(string operatorName) {
            foreach (List<Operator> list in operators)
                foreach (Operator item in list)
                    if (item.GetType().FullName == operatorName) {
                        list.Remove(item);
                        return;
                    }
        }

        public void AddNamespace(string namespaceName) {
            ApplicationUnderTest.AddNamespace(namespaceName);
        }

        public bool Compare(Type type, object instance, Tree<U> parameters) {
            bool result = false;
            Do<CompareOperator<U>>(o => o.TryCompare(this, type, instance, parameters, ref result));
            return result;
        }

        public Tree<U> Compose(object instance) {
            return Compose(instance != null ? instance.GetType() : typeof (object), instance);
        }

        public Tree<U> Compose(Type type, object instance) {
            Tree<U> result = null;
            Do<ComposeOperator<U>>(o => o.TryCompose(this, type, instance, ref result));
            return result;
        }

        public object Execute(object instance, Tree<U> parameters) {
            object result = null;
            Do<ExecuteOperator<U>>(o => o.TryExecute(this, instance, parameters, ref result));
            return result;
        }

        public object Execute(Tree<U> parameters) {
            return Execute(null, parameters);
        }

        public object Parse(Type type, Tree<U> parameters) {
            object result = null;
            Do<ParseOperator<U>>(o => o.TryParse(this, type, parameters, ref result));
            return result;
        }

        public object Parse(Type type, U input) {
            return Parse(type, new TreeLeaf<U>(input));
        }

        public T ParseTree<T>(Tree<U> input) {
            return (T) Parse(typeof (T), input);
        }

        public T Parse<T>(U input) {
            return (T) Parse(typeof (T), input);
        }

        public object ParseString(Type type, string input) {
            return Parse(type, Compose(typeof(string), input));
        }

        public T ParseString<T>(string input) {
            return (T) ParseString(typeof (T), input);
        }

        public object Create(string membername) {
            return Create(membername, new TreeList<U>());
        }

        public object Create(string memberName, Tree<U> parameters) {
            object result = null;
            Do<RuntimeOperator<U>>(o => o.TryCreate(this, memberName, parameters, ref result));
            return result;
        }

        public TypedValue Invoke(object instance, string memberName, Tree<U> parameters) {
            return Invoke(instance.GetType(), instance, memberName, parameters);
        }

        public TypedValue Invoke(Type type, object instance, string memberName, Tree<U> parameters) {
            var result = new TypedValue();
            Do<RuntimeOperator<U>>(o => o.TryInvoke(this, type, instance, memberName, parameters, ref result));
            return result;
        }

        public delegate bool TryOperation<T>(T anOperator);

        public void Do<T>(TryOperation<T> tryOperation) where T: class, Operator {
            for (int priority = operators.Count - 1; priority >= 0; priority--) {
                for (int i = operators[priority].Count - 1; i >= 0; i--) {
                    var candidate = operators[priority][i] as T;
                    if (candidate != null && tryOperation(candidate)) {
                        return;
                    }
                }
            }
            throw new ApplicationException(string.Format("No default for {0}", typeof(T).Name));
        }

        Copyable Copyable.Copy() {
            return new Processor<U>(this);
        }

        public void AddMemory<T>() {
            memoryBanks[typeof (T)] = new List<T>();
        }

        public void Store<T>(T newItem) {
            List<T> memory = GetMemory<T>();
            foreach (T item in memory) {
                if (!newItem.Equals(item)) continue;
                memory.Remove(item);
                break;
            }
            memory.Add(newItem);
        }

        public T Load<T>(T matchItem) {
            foreach (T item in GetMemory<T>()) {
                if (matchItem.Equals(item)) return item;
            }
            throw new KeyNotFoundException();
        }

        public bool Contains<T>(T matchItem) {
            foreach (T item in GetMemory<T>()) {
                if (matchItem.Equals(item)) return true;
            }
            return false;
        }

        public void Clear<T>() {
            GetMemory<T>().Clear();
        }

        private List<T> GetMemory<T>() { return (List<T>) memoryBanks[typeof (T)];}
    }
}