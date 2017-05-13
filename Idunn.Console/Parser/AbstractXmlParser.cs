﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Idunn.Console.Parser
{
    public abstract class AbstractXmlParser<T> : IParser<T>
    {
        protected readonly IParserContainer container;

        public AbstractXmlParser(IParserContainer container)
        {
            this.container = container;
        }
        public T Parse(object node)
        {
            if (!(node is XmlNode))
                throw new ArgumentException();
            return Parse((XmlNode)node);
        }

        public abstract T Parse(XmlNode node);

        protected List<S> ParseChildren<S>(XmlNode node, string name)
        {
            var children = new List<S>();
            foreach (XmlNode child in node.ChildNodes.Cast<XmlNode>().Where(n => n.Name == name))
            {
                var parser = container.Retrieve<S>();
                children.Add(parser.Parse(child));
            }
            return children;
        }
    }
}