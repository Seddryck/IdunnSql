﻿using Idunn.SqlServer.Core.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Reflection;

namespace Idunn.SqlServer.Core.Testing.Acceptance.Parser.XmlParser
{
    [TestFixture]
    public class ParserTest
    {
        [Test]
        public void Parse_ValidXml_CorrectlyParsedPrincipal()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("Idunn.SqlServer.Core.Testing.Unit.Parser.XmlParser.Resources.Sample.xml"))
            {
                var factory = new Core.Parser.XmlParser.ParserFactory();
                factory.Initialize();

                var parser = new Core.Parser.XmlParser.RootParser(factory);
                var principals = parser.Parse(stream);
                Assert.That(principals, Is.Not.Null);
                Assert.That(principals, Has.Count.EqualTo(1));
            }
        }

        [Test]
        public void Parse_ValidXml_CorrectlyParsedDatabases()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("Idunn.SqlServer.Core.Testing.Unit.Parser.XmlParser.Resources.Sample.xml"))
            {
                var factory = new Core.Parser.XmlParser.ParserFactory();
                factory.Initialize();

                var parser = new Core.Parser.XmlParser.RootParser(factory);
                var principal = parser.Parse(stream).ElementAt(0);

                Assert.That(principal.Databases, Is.Not.Null.And.Not.Empty);
                Assert.That(principal.Databases, Has.Count.EqualTo(2));
                Assert.That(principal.Databases.All(d => d.Server == "sql-001"));
                Assert.That(principal.Databases.Any(d => d.Name == "db-001"));
                Assert.That(principal.Databases.Any(d => d.Name == "db-002"));
            }
        }

        [Test]
        public void Parse_ValidXml_CorrectlyParsedDatabasePermissions()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("Idunn.SqlServer.Core.Testing.Unit.Parser.XmlParser.Resources.Sample.xml"))
            {
                var factory = new Core.Parser.XmlParser.ParserFactory();
                factory.Initialize();

                var parser = new Core.Parser.XmlParser.RootParser(factory);
                var principal = parser.Parse(stream).ElementAt(0);

                var db = principal.Databases.Single(d => d.Name == "db-001");
                Assert.That(db.Permissions, Is.Not.Null.And.Not.Empty);
                Assert.That(db.Permissions, Has.Count.EqualTo(1));
                Assert.That(db.Permissions.First().Name, Is.EqualTo("CONNECT"));
            }
        }

        [Test]
        public void Parse_ValidXml_CorrectlyParsedSecurables()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("Idunn.SqlServer.Core.Testing.Unit.Parser.XmlParser.Resources.Sample.xml"))
            {
                var factory = new Core.Parser.XmlParser.ParserFactory();
                factory.Initialize();

                var parser = new Core.Parser.XmlParser.RootParser(factory);
                var principal = parser.Parse(stream).ElementAt(0);

                var db = principal.Databases.Single(d => d.Name == "db-001");
                Assert.That(db.Securables, Is.Not.Null.And.Not.Empty);
                Assert.That(db.Securables, Has.Count.EqualTo(2));
                Assert.That(db.Securables.All(s => s.Type == "schema"));
                Assert.That(db.Securables.Any(s => s.Name == "dbo"));
                Assert.That(db.Securables.Any(s => s.Name == "admin"));
            }
        }

        [Test]
        public void Parse_ValidXml_CorrectlySecurablePermissions()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("Idunn.SqlServer.Core.Testing.Unit.Parser.XmlParser.Resources.Sample.xml"))
            {
                var factory = new Core.Parser.XmlParser.ParserFactory();
                factory.Initialize();

                var parser = new Core.Parser.XmlParser.RootParser(factory);
                var principal = parser.Parse(stream).ElementAt(0);

                var db = principal.Databases.Single(d => d.Name == "db-001");
                var securable = db.Securables.Single(s => s.Name == "dbo");
                Assert.That(securable.Permissions, Is.Not.Null.And.Not.Empty);
                Assert.That(securable.Permissions, Has.Count.EqualTo(2));
                Assert.That(securable.Permissions.Any(s => s.Name == "SELECT"));
                Assert.That(securable.Permissions.Any(s => s.Name == "UPDATE"));
            }
        }

        [Test]
        public void Parse_ValidXmlWithMultiplePrincipals_CorrectlyPrincipals()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("Idunn.SqlServer.Core.Testing.Unit.Parser.XmlParser.Resources.Multiple.xml"))
            {
                var factory = new Core.Parser.XmlParser.ParserFactory();
                factory.Initialize();

                var parser = new Core.Parser.XmlParser.RootParser(factory);
                var principals = parser.Parse(stream);

                Assert.That(principals, Is.Not.Null);
                Assert.That(principals, Has.Count.EqualTo(2));
                Assert.That(principals.Any(p => p.Name == "ExecuteDwh"));
                Assert.That(principals.Any(p => p.Name == "Logger"));
                Assert.That(principals.All(p => p.Databases.Count>=1));
            }
        }
    }
}