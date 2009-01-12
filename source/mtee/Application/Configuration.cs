﻿// Copyright © Syterra Software Inc. All rights reserved.
// The use and distribution terms for this software are covered by the Common Public License 1.0 (http://opensource.org/licenses/cpl.php)
// which can be found in the file license.txt at the root of this distribution. By using this software in any fashion, you are agreeing
// to be bound by the terms of this license. You must not remove this notice, or any other, from this software.

using System.IO;
using System.Xml;
using fitnesse.mtee.engine;
using fitnesse.mtee.model;

namespace fitnesse.mtee.application {
    public class Configuration {

        public void LoadXml(string configXml) {
            var document = new XmlDocument();
            document.LoadXml(configXml);
            if (document.DocumentElement == null) return;
            foreach (XmlNode typeNode in document.DocumentElement.ChildNodes) {
                foreach (XmlNode methodNode in typeNode.ChildNodes) {
                    LoadNode(typeNode.Name, methodNode);
                }
            }
        }

        public void LoadFile(string filePath) {
            var reader = new StreamReader(filePath);
            LoadXml(reader.ReadToEnd());
            reader.Close();
        }

        private static void LoadNode(string typeName, XmlNode methodNode) {
            new Processor().Invoke(Context.Instance.GetItem(typeName), methodNode.Name, NodeParameters(methodNode));
        }

        private static Tree<object> NodeParameters(XmlNode node) {
            return new TreeList<object>()
                .AddBranch(node.InnerText);
        }
    }
}